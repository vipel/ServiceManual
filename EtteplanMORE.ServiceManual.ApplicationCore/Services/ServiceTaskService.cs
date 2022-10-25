using System.Collections.Generic;
using System.Threading.Tasks;

using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Services
{
    public class ServiceTaskService : IServiceTaskService
    {
        private readonly DataStorage _dataStorage;
        public ServiceTaskService()
        {
            _dataStorage = new DataStorage();
        }

        public async Task<bool> Add(ServiceTask serviceTask)
        {
            return await Task.FromResult(_dataStorage.AddServiceTask(serviceTask));
        }

        public async Task<ServiceTask> Get(int id)
        {
            return await Task.FromResult(_dataStorage.GetServiceTask(id)); ;
        }

        public async Task<bool> Edit(ServiceTask serviceTask)
        {
            return await Task.FromResult(_dataStorage.EditServiceTask(serviceTask));
        }

        public async Task<bool> Remove(int id)
        {
            return await Task.FromResult(_dataStorage.RemoveServiceTask(id));
        }

        public async Task<IEnumerable<ServiceTask>> GetAll()
        {
            return await Task.FromResult(_dataStorage.GetServiceTasks());
        }

        public async Task<IEnumerable<ServiceTask>> GetByDevice(int deviceId)
        {
            return await Task.FromResult(_dataStorage.GetServiceTasks(deviceId));
        }
    }
}
