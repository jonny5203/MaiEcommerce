 A simple E-commercve site built using ASP.Net Core with Entity framework, MSSql and Core Identity.
 If you want to try it out for now, you have to create your own database server and add the connection string into appsettings.json as "DefaultConnection".
 Then you have to go to the packet manager console in visual studio and write update database, or alternatively you could just open a terminal
 session within the project folder and write dotnet ef database update, if you have Entity Framework CLI installed. Also you may have to update the migration, or create it
 with df ef migrations add ApplicationDBContext --project MaiCommerce.DataAccess --startup MaiCommerce

 NB! Remember to delete the migration folder before migrating

 Description:
 This project is not done, but idea is to create a simple solution for an eccomerce website, with authentication for the users
 a place where you can sell or resell products with integrated payment system. I am currently using Enity Framework and MS Sql server
 for database handling, and I will use Core Identity as a authentication method, but later also add ways for Single Sign On with 
 popular services like google. I am planning to leverage the power of Power BI and databricks to do reports on the actitvity of the
 page, and for the user and seller to analyze for themselves. The seller could be a company or it could be a private person. It should also
 have order details as well as status on order. You should be able to log in as customer, company(seller) or admin. I am planning on using
 stripe as my payment service.
