# ServiceManual project

## Instructions

### How to build and run ASP.NET Core Web API ‘ServiceManual’ project in Visual Studio

	Crete dbo.factory_device and dbo.service_task tables in Database.
	Scripts located in ./Documents/FactoryDeviceTable.sql and ./Documents/ServiceTaskTable.sql files can be utilized in the case of MS SQL Server database.
	Fill-up dbo.factory_device table with records. Data located in ./Documents/seeddata.csv file can be used for that.
	Open ‘ServiceManual’ project in Visual Studio.
	Open Definitions.cs file of EtteplanMORE.ServiceManual.ApplicationCore namespace and change ‘ConnectionString’ constant value if needed.
	Select ‘Build > Build Solution’ to build solution
	Set 'EtteplanMORE.ServiceManual.Web' as Startup Project
	Select ‘Debug > Start Without Debugging’ to start Web server.

### How to use Postman application to test ‘ServiceManual’ ASP.NET Core Web API

	Run Postman application.
	Select ‘Import’ > ‘Upload Files’ and open ./Documents/ServiceManual.Web.Test.postman_collection.json Postman collection JSON file.
	Select target request, change port number in request URL if needed and press ‘Send’ button.
	Check the status and response in the bottom part of Postman UI.
