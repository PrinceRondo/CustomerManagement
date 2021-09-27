# CustomerManagement
Managing Customer
This application (backend) helps in managing customers profile, customer memo, invoice url generator and invoice upload

Application Installation

The project can be published with visual studio and deployed to iis. Migration can be run from visual studio to create the database(mssql)
 using entity framework. Default user (Super admin) will be created when running the application for the first time. The list of endpoints can be viewed
on swagger. Some endpoints are authorized and user needs to login before they can be accessed.

How to use the Application

The design pattern MVC, Repository, OOP, Solid Principle
programming Language: C#
Framework: Asp.Net Core
Database: MSSQL
ORM: Entity Framework
SMTP: Gmail
Pdf: iTextSharp library

The application contains 4 module presently
1. User Account Management
2. Customer Profile Management
3. Customer Record(Memo) Management
4. Invoice Management

1. User Account Management: This is for login, user creation and password reset.
2. Customer Profile Management: To manage customer profile.
3. Customer Record(Memo) Management: To manage customer record.
4. Invoice Management: To create invoice url and invoice upload. The windows service(separate application) installed on the system will be sending 
the url to individual customer.
