using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaiCommerce.Utility
{
    //provide a string for 
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        
        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_In_Process= "Processing";
        public const string Status_Shipped = "Shipped";
        public const string Status_Cancelled = "Cancelled";
        public const string Status_Refunded = "Refunded";
        
        public const string Payment_Status_Pending = "Pending";
        public const string Payment_Status_Approved = "Approved";
        public const string Payment_Status_Delayed_Payment = "ApprovedForDelayedPayment";
        public const string Payment_Status_Rejected = "Rejected";
    }
}