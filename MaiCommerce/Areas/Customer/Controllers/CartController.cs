using System.Security.Claims;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;
using MaiCommerce.Models.IdentityModels;
using MaiCommerce.Models.ViewModels;
using MaiCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace dotnetecommerce.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    // Binding this property with the one that is going to the client, so that
    // changes will reflect automatically
    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        // Checking claims for valid userID, basically claims is just a user object
        // for storing the attributes about the user, usually through session or token
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(n => n.ApplicationUserId == userId
                , includeProperties: "Product"),
            OrderHeader = new()
        };

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        return View(ShoppingCartVM);
    }
    
    // Creating a ShoppingCartVM(view model) and sending it to the view
    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(n => n.ApplicationUserId == userId
                , includeProperties: "Product"),
            OrderHeader = new()
        };
        
        // Populating the Orderheader that was created above with the information
        // from the userID retrived from claims, and used to search the database for that spesifict user
        ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(n => n.Id == userId);
        
        ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
        ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }
        
        return View(ShoppingCartVM);
    }
    
    [HttpPost]
    //name for the http post endpoint
    [ActionName("Summary")]
    public IActionResult SummaryPOST()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(n => n.ApplicationUserId == userId
                , includeProperties: "Product");

        ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
        
        // Never populate foreign object the object you are trying to save to DB
        // ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(n => n.Id == userId);
        
        // Rather create a new object if you have to use the values
        // the reason is that with the other method it will try to create a new row
        // in the db when you try to save
        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(n => n.Id == userId);
        
        // No need anymore as setting ApplicationUserId, will automatically populate the fields
        // ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
        // ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
        // ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
        // ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
        // ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
        // ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        // Check if customer account or company employee account
        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // Capture payment if, as regular customer is not eligible for delayed payment
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.Payment_Status_Pending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.Status_Pending;
        }
        else
        {
            // Allow delayde payment because of company account
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.Payment_Status_Delayed_Payment;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.Status_Approved;
        }
        
        _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        _unitOfWork.Save();

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = cart.ProductId,
                OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                Count = cart.Count,
                Price = cart.Price
            };
            
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }
        
        // Check to see if regular customer or company, if it is company then
        // they will just be redirected directly to confirmation page(action)
        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // regular customer
            // stripe logic
            var domain = "http://localhost:5243/";
            
            // Defining options(meta data) about the stripe session
            var options = new SessionCreateOptions
            {
                // User will be redirected to this URL, when payment is succeeded
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            
            // Looping though the shoppingcartlist and creating a new
            // SessionLineItemOptions with information about the product
            // that will tell the stripe session what products it is and the price, as well
            // as the total price to be paid
            foreach (var item in ShoppingCartVM.ShoppingCartList)
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
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            
            // Tells the respons object the location url to be redirected to
            Response.Headers.Add("Location", session.Url);
            // Return redirect status code
            return new StatusCodeResult(303);
        }
        
        // if not normal customer, redirect directly to OrderConfirmation
        return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
    }
    
    // Called when payment is done, or it's approved for later
    public IActionResult OrderConfirmation(int Id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(n => n.Id == Id, includeProperties: "ApplicationUser");
        if (orderHeader.PaymentStatus != SD.Payment_Status_Delayed_Payment)
        {
            // this is an order by customer
            
            // getting the session that was created in summaryPOST
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            
            // if status from stripe is paid, then update the db with the correct status code
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(Id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(Id, SD.Status_Approved, SD.Payment_Status_Approved);
                _unitOfWork.Save();
            }
        }
        
        // Getting a list of shopping cart that has the users id
        List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(
            n => n.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
        
        // Removes the whole shopping cart list payment status has been confirmed
        // either payed or approved for later pay
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.Save();
        
        return View(Id);
    }

    public IActionResult Plus(int cartId)
    {
        var cartFromDB = _unitOfWork.ShoppingCart.Get(n => n.Id == cartId);
        cartFromDB.Count += 1;
        _unitOfWork.ShoppingCart.Update(cartFromDB);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDB = _unitOfWork.ShoppingCart.Get(n => n.Id == cartId);

        if (cartFromDB.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Remove(cartFromDB);
        }
        else
        {
            cartFromDB.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDB);
        }

        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var cartFromDB = _unitOfWork.ShoppingCart.Get(n => n.Id == cartId);

        _unitOfWork.ShoppingCart.Remove(cartFromDB);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }

        if (shoppingCart.Count >= 100)
        {
            return shoppingCart.Product.Price50;
        }

        return shoppingCart.Product.Price100;
    }
}