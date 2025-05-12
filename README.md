AgriEnergy_ConnectApp
Overview
AgriEnergy_ConnectApp is a C# .NET MVC application designed to facilitate efficient management of farmers and their agricultural products. The system allows employees to register farmers, while farmers can add,
update, and manage their own products. Built using Microsoft SQL Server Management, the application ensures scalability, security, and efficient data processing.

Technologies Used
* Frontend: ASP.NET MVC
* Backend: C# (.NET Core)
* Database: Microsoft SQL Server
* ORM: Entity Framework Core

Authentication & Security: Identity Management
* Hosting: IIS / Azure Cloud Deployment

Key Features
* User Management: Employees can register and manage farmer profiles. 
* Product Management: Farmers can add and update products they sell.
* Search by Product: Employees and farmers can search for products based on name, category, and production date for efficient navigation.
* Data Persistence: Uses Entity Framework Core for efficient database interaction.
* Cascade Delete: When a farmer is removed, associated products are deleted automatically. 
* Price Validation: Ensures product pricing follows a decimal(18,2) SQL format.
* Automated Timestamps: Farmers’ profiles include a UTC-based creation timestamp for tracking.
* Security Measures: Unique email indexing for user authentication and verification.

Installation & Setup
Prerequisites
Ensure you have the following installed:
* Visual Studio (latest version)
* .NET SDK
* Microsoft SQL Server
* Entity Framework Core \9.0.4\
* Microsoft.EntityFrameworkcore.SqlServer\9.0.3\
* Bcrypt.net-next\4.0.3\
* Microsoft.EntityFrameworkCore.tools\9.0.4\
* Microsoft.Extensions.Logging\9.0.4\

Steps to Run Locally
Clone the repository:
* sh
* git clone https://github.com/your-repo/agri-energy-connectapp.git
* cd agri-energy-connectapp
* Configure the database connection in appsettings.json.

Apply migrations and seed data using:
* sh
* dotnet ef migrations add InitialCreate  
* dotnet ef database update  
* Run the application:

sh
* dotnet run  
* Access the system in a browser at:
* http://localhost:5000
  
Entity Relationships
The application follows a structured relational model:
* Employee (Users) → Farmer → Product
* One Employee can add multiple Farmers
* One Farmer can have multiple Products
* Products reference Farmers via a Foreign Key (FarmerId)

Future Enhancements
* Implement role-based authentication for employees and farmers 
* Add product categorization and filtering 
* Enhance real-time inventory tracking 
* Integrate cloud hosting for scalability

Contributors
* Developed by Thobani Antony Zondi 
* Contact: thobanizondi69@gmail.com 
* LinkedIn: linkedin.com/in/thobani-zondi
