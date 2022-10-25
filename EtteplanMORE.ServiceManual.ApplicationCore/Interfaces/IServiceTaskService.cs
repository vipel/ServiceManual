using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Interfaces
{
    public interface IServiceTaskService
    {
        Task<bool> Add(ServiceTask serviceTask);
        Task<ServiceTask> Get(int id);
        Task<bool> Edit(ServiceTask serviceTask);
        Task<bool> Remove(int id);
        Task<IEnumerable<ServiceTask>> GetAll();
        Task<IEnumerable<ServiceTask>> GetByDevice(int deviceId);
    }
}
