# A simple E-commercve site built using ASP.Net Core with Entity framework, MSSql and Core Identity.

### Frameworks and Libraries used:
  1. Asp.net core MVC
  2. Entity Framework core
  3. Microsoft Sql Server
  4. Asp.net core Identity
  5. TinyMCE
  6. CloudTable
  7. Bootstrap
  8. Jquery and Ajax
  9. SweetAlert2

### Requirements to run: 
  1. .Net 8.
  2. A database server(EF core handles most databases, so you can choose) NB! if you choose a database other than SQL Server you have to install the EF core nuget packages for that.
  3. Connection string to DB.
  4. Nuget packages should be installed automatically, but if not you have to install them manually.
  5. Update the DB with EF core, see below.

If you want to try it out for now, you have to create your own database server and add the connection string into `appsettings.json` as "DefaultConnection".
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server={servername};Database=ecommerse;User Id={username};Password={password};TrustServerCertificate=True(depends on your database setup)"
  }
}
```
 
 Then you have to go to the packet manager console in visual studio and write update database, or alternatively you could just open a terminal
 session within the project folder and write dotnet ef database update, if you have Entity Framework CLI installed. Also you may have to update the migration, or create it
 with `df ef migrations add ApplicationDBContext --project MaiCommerce.DataAccess --startup MaiCommerce`

 ### New: 
 seem like you can just do `dotnet ef database update` or `update-database` in packet manager console, and EF core will autormatically
 update or create the database with the tables and values, but it seems like either you have to do that with all migration in `MaiCommerce.DataAccess.Migrations`
 or you have to delete everything and then create a new migration based on the db context class `ApplicationDBContext`, project should be `MaiCommerce.DataAccess` startup is
 `MaiCommerce`. ### you may have to update each migrations sequentually, the migrations is located at `MaiCommerce.DataAccess/Migrations`

 ### NB!:
 I am using Linux as my development environment, so there is some places where I have to provide path for file saving, those path has to be replace with \\ instead of /. You find them in the productcontroller
 inside `MVC` or `MaiCommerce/Areas/Admin/Controllers/ProductController`

 ## Description:
 This project is not done, but idea is to create a simple solution for an eccomerce website, with authentication for the users
 a place where you can sell or resell products with integrated payment system. I am currently using Enity Framework and MS Sql server
 for database handling, and I will use Core Identity as a authentication method, but later also add ways for Single Sign On with 
 popular services like google. I am planning to leverage the power of Power BI and databricks to do reports on the actitvity of the
 page, and for the user and seller to analyze for themselves. The seller could be a company or it could be a private person. It should also
 have order details as well as status on order. You should be able to log in as customer, company(seller) or admin. I am planning on using
 stripe as my payment service.

 ## Full Description Coming Soon ...
