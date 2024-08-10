using System.Security.Claims;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;
using MaiCommerce.Models.ViewModels;
using MaiCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace dotnetecommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    
    [BindProperty]
    // bind property to client side
    public OrderVM orderVM { get; set; }

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Details(int orderId)
    {
        orderVM = new()
        {
            orderHeader = _unitOfWork.OrderHeader.Get(n => n.Id == orderId, includeProperties: "ApplicationUser"),
            orderDetails = _unitOfWork.OrderDetail.GetAll(n => n.OrderHeaderId == orderId, includeProperties: "Product"),
        };
        
        return View(orderVM);
    }

    [HttpPost]
    // Only give access to certain role, utilizes app.UseAuthorization() middleware in program.cs
    // to determine in the user claims, this one reject everything that isn't admin or employee type
    // and is defined by comma separated string
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult UpdateOrderDetail()
    {
        // retrieve orderheader from db, based on id from input field received in client side, and then update it
        // with the orderHeader object that is bound. 
        var orderHeaderFromDB = _unitOfWork.OrderHeader.Get(n => n.Id == orderVM.orderHeader.Id);
        orderHeaderFromDB.Name = orderVM.orderHeader.Name;
        orderHeaderFromDB.PhoneNumber = orderVM.orderHeader.PhoneNumber;
        orderHeaderFromDB.StreetAddress = orderVM.orderHeader.StreetAddress;
        orderHeaderFromDB.City = orderVM.orderHeader.City;
        orderHeaderFromDB.State = orderVM.orderHeader.State;
        orderHeaderFromDB.PostalCode = orderVM.orderHeader.PostalCode;

        if (!string.IsNullOrEmpty(orderVM.orderHeader.Carrier))
        {
            orderHeaderFromDB.Carrier = orderVM.orderHeader.Carrier;
        }

        if (!string.IsNullOrEmpty(orderVM.orderHeader.TrackingNumber))
        {
            orderHeaderFromDB.TrackingNumber = orderVM.orderHeader.TrackingNumber;
        }
        
        _unitOfWork.OrderHeader.Update(orderHeaderFromDB);
        _unitOfWork.Save();
        
        // add notification that will be picked up by the notification system, SweetAlert2
        TempData["Success"] = "Order Shipped Successfully.";
        
        // redirect to Details page and pass a new data object to the redirected page
        return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult StartProcessing()
    {
        _unitOfWork.OrderHeader.UpdateStatus(orderVM.orderHeader.Id, SD.Status_In_Process);
        _unitOfWork.Save();
        
        TempData["Success"] = "Order has been started Successfully.";
        return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
    }
    
    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult ShipOrder()
    {
        var orderHeaderFromDB = _unitOfWork.OrderHeader.Get(n => n.Id == orderVM.orderHeader.Id);
        orderHeaderFromDB.TrackingNumber = orderVM.orderHeader.TrackingNumber;
        orderHeaderFromDB.Carrier = orderVM.orderHeader.Carrier;
        orderHeaderFromDB.OrderStatus = SD.Status_Shipped;
        orderHeaderFromDB.ShippingDate = DateTime.Now;

        if (orderHeaderFromDB.PaymentStatus == SD.Payment_Status_Delayed_Payment)
        {
            orderHeaderFromDB.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        }
        
        _unitOfWork.OrderHeader.UpdateStatus(orderVM.orderHeader.Id, SD.Status_Shipped);
        _unitOfWork.Save();
        
        TempData["Success"] = "Order has been started Successfully.";
        return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult CancelOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(n => n.Id == orderVM.orderHeader.Id);

        if (orderHeader.PaymentStatus == SD.Payment_Status_Approved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
                
            };

            var service = new RefundService();
            Refund refund = service.Create(options);
            
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.Status_Cancelled, SD.Status_Refunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.Status_Cancelled, SD.Status_Cancelled);
        }
        
        _unitOfWork.Save();
        TempData["Success"] = "Order Cancelled Successfully";
        return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
    }

    // Define a post method for the details route
    [ActionName("Details")]
    [HttpPost]
    public IActionResult Details_PAY_NOW()
    {
        orderVM.orderHeader = _unitOfWork.OrderHeader
            .Get(n => n.Id == orderVM.orderHeader.Id, includeProperties: "ApplicationUser");
        orderVM.orderDetails = _unitOfWork.OrderDetail
            .GetAll(n => n.Id == orderVM.orderHeader.Id, includeProperties: "Product");
        
            // stripe logic
            var domain = "http://localhost:5243/";
            
            // Defining options(meta data) about the stripe session
            var options = new SessionCreateOptions
            {
                // User will be redirected to this URL, when payment is succeeded
                SuccessUrl = domain + $"admin/order/OrderConfirmation?orderHeaderId={orderVM.orderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.orderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            
            // Looping though the OrderDetails and creating a new
            // SessionLineItemOptions with information about the product
            // that will tell the stripe session what products it is and the price, as well
            // as the total price to be paid
            foreach (var item in orderVM.orderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    
                    Quantity = item.Count,
                };
                
                // Add to stripe session item list
                options.LineItems.Add(sessionLineItem);
            }
            
            // Creating a new Stripe session and updating the orderheader in the
            // database with the session id for later usage
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(orderVM.orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            
            // Tells the respons object the location url to be redirected to
            Response.Headers.Add("Location", session.Url);
            // Return redirect status code
            return new StatusCodeResult(303);
    }
    
    // Called onsuccess for the stripe session in Details_PAY_NOW, and will handle
    // updating the database status 
    public IActionResult PaymentConfirmation(int orderHeaderId)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(n => n.Id == orderHeaderId);
        if (orderHeader.PaymentStatus == SD.Payment_Status_Delayed_Payment)
        {
            // this is an order by a company
            
            // getting the session that was created in summaryPOST
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            
            // if status from stripe is paid, then update the db with the correct status code
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                // pass in the same status as the one that already exist
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.Payment_Status_Approved);
                _unitOfWork.Save();
            }
        }
        
        return View(orderHeaderId);
    }
    
    #region API CALLS
    
    //API endpoint for getting the all the orders as json obj
    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderHeadersObj = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        
        // Check if admin or an employee of a company to show the correct
        // information about, basically all the order, as a normal user can only see their own orders
        if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
        {
            orderHeadersObj = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        }
        else
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            orderHeadersObj = _unitOfWork.OrderHeader.GetAll(n => n.ApplicationUserId == userId, includeProperties: "ApplicationUser");
        }
        
        // Check for status and try to query the enumerable for items that matches the status
        switch (status)
        {
            case "pending":
                orderHeadersObj = orderHeadersObj.Where(n => n.PaymentStatus == SD.Payment_Status_Delayed_Payment);
                break;
            case "inprocess":
                orderHeadersObj = orderHeadersObj.Where(n => n.OrderStatus == SD.Status_In_Process);
                break;
            case "completed":
                orderHeadersObj = orderHeadersObj.Where(n => n.OrderStatus == SD.Status_Shipped);
                break;
            case "approved":
                orderHeadersObj = orderHeadersObj.Where(n => n.OrderStatus == SD.Status_Approved);
                break;
            default:
                break;
        }

        return Json(new { data = orderHeadersObj });
    }
    
    #endregion
}