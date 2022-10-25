namespace EtteplanMORE.ServiceManual.ApplicationCore
{
    internal partial class DataStorage
    {
        private const string ConnectionString = "Data Source=localhost;Initial Catalog=ServiceDev;Integrated Security=True;TrustServerCertificate=True";

        private const string FactoryDevices = "SELECT [Id], [Name], [Year], [Type] FROM dbo.factory_device";

        private const string ServiceTasks = "SELECT [Id], [FactoryDeviceId], [Created], [Description], [Criticality], [State] FROM dbo.service_task";

        private const string InsertServiceTask =
            "INSERT INTO dbo.service_task ([Id], [FactoryDeviceId], [Created], [Description], [Criticality], [State]) VALUES (@Id, @FactoryDeviceId, @Created, @Description, @Criticality, @State)";
        private const string UpdateServiceTask = "UPDATE dbo.service_task SET [Description] = @Description, [Criticality] = @Criticality, [State] = @State";
        private const string DeleteServiceTask = "DELETE FROM dbo.service_task";

        private const string ServiceTasksOrder = "ORDER BY Criticality ASC, Created DESC";
        private const string WhereId = "WHERE [Id]=";
        private const string WhereFactoryDeviceId = "WHERE [FactoryDeviceId]=";
    }
}
