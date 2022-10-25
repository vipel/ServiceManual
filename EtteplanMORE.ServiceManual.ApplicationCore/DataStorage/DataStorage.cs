using System;
using System.Collections.Immutable;
using System.Data;
using System.Linq;

using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using Microsoft.Data.SqlClient;

namespace EtteplanMORE.ServiceManual.ApplicationCore
{
    internal partial class DataStorage
    {
        private ImmutableList<FactoryDevice> _factoryDevices;
        private ImmutableList<ServiceTask> _serviceTasks;

        private SqlConnection _sqlConnection;
        private SqlCommand _sqlCommand;
        private SqlDataReader _sqlDataReader;

        public DataStorage()
        {
            _factoryDevices = ImmutableList.Create<FactoryDevice>();
            _serviceTasks = ImmutableList.Create<ServiceTask>();
        }

        public ImmutableList<FactoryDevice> GetFactoryDevices()
        {
            if(_factoryDevices.Count != 0)
            {
                return _factoryDevices;
            }

            using (_sqlConnection = new(ConnectionString))
            {
                _sqlConnection.Open();
                _sqlCommand = new (SelectTarget(FactoryDevices), _sqlConnection);
                _sqlDataReader = _sqlCommand.ExecuteReader();
                while (_sqlDataReader.Read())
                {
                    FactoryDevice factoryDevice = new()
                    {
                        Id = Convert.ToInt32(_sqlDataReader["Id"]),
                        Name = _sqlDataReader["Name"].ToString(),
                        Year = Convert.ToInt32(_sqlDataReader["Year"]),
                        Type = _sqlDataReader["Type"].ToString(),
                    };
                    _factoryDevices = _factoryDevices.Add(factoryDevice);
                }
            }
            return _factoryDevices;
        }

        public FactoryDevice GetFactoryDevice(int id)
        {
            if (_factoryDevices.Count != 0)
            {
                return _factoryDevices.FirstOrDefault(fd => fd.Id == id);
            }

            using (_sqlConnection = new(ConnectionString))
            {
                _sqlConnection.Open();
                _sqlCommand = new(SelectTarget(FactoryDevices, id), _sqlConnection);
                _sqlDataReader = _sqlCommand.ExecuteReader();
                if (_sqlDataReader.Read())
                {
                    return new FactoryDevice()
                    {
                        Id = Convert.ToInt32(_sqlDataReader["Id"]),
                        Name = _sqlDataReader["Name"].ToString(),
                        Year = Convert.ToInt32(_sqlDataReader["Year"]),
                        Type = _sqlDataReader["Type"].ToString(),
                    };
                }
            }
            return default;
        }

        public bool AddServiceTask(ServiceTask serviceTask) // Multiple service tasks can be added for same FactoryDeviceId
        {
            using (_sqlConnection = new(ConnectionString))
            {
                _sqlConnection.Open();

                _sqlCommand = new(InsertServiceTask, _sqlConnection);
                _sqlCommand.Parameters.Add("@Id", SqlDbType.Int);
                _sqlCommand.Parameters.Add("@FactoryDeviceId", SqlDbType.Int);
                _sqlCommand.Parameters.Add("@Created", SqlDbType.DateTime);
                _sqlCommand.Parameters.Add("@Description", SqlDbType.NVarChar);
                _sqlCommand.Parameters.Add("@Criticality", SqlDbType.Int);
                _sqlCommand.Parameters.Add("@State", SqlDbType.Int);

                _sqlCommand.Parameters["@Id"].Value = serviceTask.Id;
                _sqlCommand.Parameters["@FactoryDeviceId"].Value = serviceTask.FactoryDevice.Id;
                _sqlCommand.Parameters["@Created"].Value = serviceTask.Created;
                _sqlCommand.Parameters["@Description"].Value = serviceTask.Description;
                _sqlCommand.Parameters["@Criticality"].Value = serviceTask.Criticality;
                _sqlCommand.Parameters["@State"].Value = serviceTask.State;

                int affected = _sqlCommand.ExecuteNonQuery();
                if (affected != 1)
                {
                    return false;
                }

                _serviceTasks.Add(serviceTask);

                return true;
            }
        }

        public ServiceTask GetServiceTask(int id)
        {
            if (_serviceTasks.Count != 0)
            {
                return _serviceTasks.FirstOrDefault(st => st.Id == id);
            }

            ImmutableList<FactoryDevice> factoryDevices = GetFactoryDevices();

            using (_sqlConnection = new(ConnectionString))
            {
                _sqlConnection.Open();
                _sqlCommand = new(SelectTarget(ServiceTasks, id), _sqlConnection);
                _sqlDataReader = _sqlCommand.ExecuteReader();
                if (_sqlDataReader.Read())
                {
                    return new ServiceTask()
                    {
                        Id = Convert.ToInt32(_sqlDataReader["Id"]),
                        FactoryDevice = factoryDevices.FirstOrDefault(c => c.Id == Convert.ToInt32(_sqlDataReader["FactoryDeviceId"])),
                        Created = Convert.ToDateTime(_sqlDataReader["Created"]),
                        Description = _sqlDataReader["Description"].ToString(),
                        Criticality = (Criticality)_sqlDataReader["Criticality"],
                        State = (State)_sqlDataReader["State"]
                    };
                }
            }
            return default;
        }

        public bool EditServiceTask(ServiceTask serviceTask) // Allows Description, Criticality and State updating
        {
            using (_sqlConnection = new(ConnectionString))
            {
                _sqlConnection.Open();
                _sqlCommand = new(SelectTarget(UpdateServiceTask, serviceTask.Id), _sqlConnection);
                _sqlCommand.Parameters.Add("@Description", SqlDbType.NVarChar);
                _sqlCommand.Parameters.Add("@Criticality", SqlDbType.Int);
                _sqlCommand.Parameters.Add("@State", SqlDbType.Int);
                _sqlCommand.Parameters["@Description"].Value = serviceTask.Description;
                _sqlCommand.Parameters["@Criticality"].Value = serviceTask.Criticality;
                _sqlCommand.Parameters["@State"].Value = serviceTask.State;
                int affected = _sqlCommand.ExecuteNonQuery();
                if (affected != 1)
                {
                    return false;
                }

                ServiceTask stToChange = _serviceTasks.Find((ServiceTask st) => st.Id == serviceTask.Id);
                if (stToChange != null)
                {
                    stToChange.Description = serviceTask.Description;
                    stToChange.Criticality = serviceTask.Criticality;
                    stToChange.State = serviceTask.State;
                }
                else
                {
                    _serviceTasks.Clear(); // Service Tasks list is corrupted, clear list
                }
                return true;
            }
        }

        public bool RemoveServiceTask(int id)
        {
            using (_sqlConnection = new(ConnectionString))
            {
                _sqlConnection.Open();
                _sqlCommand = new(SelectTarget(DeleteServiceTask, id), _sqlConnection);
                _sqlCommand.Parameters.Add("@Id", SqlDbType.Int);
                _sqlCommand.Parameters["@Id"].Value = id;

                int affected = _sqlCommand.ExecuteNonQuery();
                if (affected != 1)
                {
                    return false;
                }

                ServiceTask serviceTask = _serviceTasks.Find((ServiceTask st) => st.Id == id);
                if (serviceTask != null)
                {
                    _serviceTasks.Remove(serviceTask);
                }
                else
                {
                    _serviceTasks.Clear(); // Service Tasks list is corrupted, clear list
                }

                return true;
            }
        }

        public ImmutableList<ServiceTask> GetServiceTasks()
        {
            return GetTasks();
        }

        public ImmutableList<ServiceTask> GetServiceTasks(int deviceId)
        {
            return GetTasks(deviceId);
        }

        private ImmutableList<ServiceTask> GetTasks(int deviceId = 0)
        {
            if (_serviceTasks.Count != 0)
            {
                ImmutableList<ServiceTask> serviceTasks = deviceId != 0 ? _serviceTasks.Where(st => st.FactoryDevice.Id == deviceId).ToImmutableList() : _serviceTasks;
                return serviceTasks.OrderBy(st => st.Criticality).ThenByDescending(st => st.Created).ToImmutableList();
            }

            ImmutableList<FactoryDevice> factoryDevices = GetFactoryDevices();

            using (_sqlConnection = new(ConnectionString))
            {
                _sqlConnection.Open();

                string selectServiceTasks = deviceId > 0 ? string.Format("{0} {1} {2}", ServiceTasks, WhereFactoryDeviceId, deviceId) : ServiceTasks;
                string command = SelectTarget(string.Format("{0} {1}", selectServiceTasks, ServiceTasksOrder));

                _sqlCommand = new(command, _sqlConnection);
                _sqlDataReader = _sqlCommand.ExecuteReader();
                while (_sqlDataReader.Read())
                {
                    ServiceTask serviceTask = new()
                    {
                        Id = Convert.ToInt32(_sqlDataReader["Id"]),
                        FactoryDevice = factoryDevices.FirstOrDefault(c => c.Id == Convert.ToInt32(_sqlDataReader["FactoryDeviceId"])),
                        Created = Convert.ToDateTime(_sqlDataReader["Created"]),
                        Description = _sqlDataReader["Description"].ToString(),
                        Criticality = (Criticality)_sqlDataReader["Criticality"],
                        State = (State)_sqlDataReader["State"]
                    };
                    _serviceTasks = _serviceTasks.Add(serviceTask);
                }
            }
            return _serviceTasks;
        }

        private static string SelectTarget(string target, int id = 0)
        {
            return id == 0 ? target : string.Format("{0} {1} {2}", target, WhereId, id);
        }
    }
}
