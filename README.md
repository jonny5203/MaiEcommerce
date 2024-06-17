 A simple E-commercve site built using ASP.Net Core with Entity framework, MSSql and Core Identity.
 If you want to try it out for now, you have to create your own database server and add the connection string into appsettings.json as "DefaultConnection".
 Then you have to go to the packet manager console in visual studio and write update database, or alternatively you could just open a terminal
 session within the project folder and write dotnet ef database update, if you have Entity Framework CLI installed. Also you may have to update the migration, or create it
 with df ef migrations add ApplicationDBContext --project MaiCommerce.DataAccess --startup MaiCommerce

 NB! Remember to delete the migration folder before migrating
