using System.Collections.Generic;
using System.Threading.Tasks;

using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Services
{
    public class FactoryDeviceService : IFactoryDeviceService
    {
        private readonly DataStorage _dbConenction;
        public FactoryDeviceService()
        {
            _dbConenction = new DataStorage();
        }

        public async Task<IEnumerable<FactoryDevice>> GetAll()
        {
            return await Task.FromResult(_dbConenction.GetFactoryDevices());
        }

        public async Task<FactoryDevice> Get(int id)
        {
            return await Task.FromResult(_dbConenction.GetFactoryDevice(id));
        }
    }
}