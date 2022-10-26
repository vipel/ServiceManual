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
        private bool _serviceTasksLoaded;

        private SqlConnection _sqlConnection;
        private SqlCommand _sqlCommand;
        private SqlDataReader _sqlDataReader;

        public DataStorage()
        {
            _factoryDevices = ImmutableList.Create<FactoryDevice>();
            _serviceTasks = ImmutableList.Create<ServiceTask>();
            _serviceTasksLoaded = false;
        }

        public ImmutableList<FactoryDevice> GetFactoryDevices()
        {
            try
            {
                if (_factoryDevices.Count != 0)
                {
                    return _factoryDevices;
                }

                using (_sqlConnection = new(ConnectionString))
                {
                    _sqlConnection.Open();
                    _sqlCommand = new(SelectTarget(FactoryDevices), _sqlConnection);
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
            catch (Exception ex)
            {
                throw new Exception("Failed to get factory devices", ex);
            }
        }

        public FactoryDevice GetFactoryDevice(int id)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to get factory device, device Id: {0}", id), ex);
            }
        }

        public bool AddServiceTask(ServiceTask serviceTask) // Multiple service tasks can be added for same FactoryDeviceId
        {
            try
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
                }

                if (_serviceTasksLoaded)
                {
                    _serviceTasks.Add(serviceTask);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to add service task, service task Id: {0}", serviceTask.Id), ex);
            }
        }

        public ServiceTask GetServiceTask(int id)
        {
            try
            {
                if (_serviceTasksLoaded)
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
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to get service task, service task Id: {0}", id), ex);
            }
        }

        public bool EditServiceTask(ServiceTask serviceTask) // Allows Description, Criticality and State updating
        {
            try
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
                }

                if (_serviceTasksLoaded)
                {
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
                        _serviceTasksLoaded = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to edit service task, service task Id: {0}", serviceTask.Id), ex);
            }
        }

        public bool RemoveServiceTask(int id)
        {
            try
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
                }

                if (_serviceTasksLoaded)
                {
                    ServiceTask serviceTask = _serviceTasks.Find((ServiceTask st) => st.Id == id);
                    if (serviceTask != null)
                    {
                        _serviceTasks.Remove(serviceTask);
                    }
                    else
                    {
                        _serviceTasks.Clear(); // Service Tasks list is corrupted, clear list
                        _serviceTasksLoaded = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to remove service task, service task Id: {0}", id), ex);
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
            try
            {
                if (_serviceTasksLoaded)
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

                if (deviceId == 0)
                {
                    _serviceTasksLoaded = true;
                }
                return _serviceTasks;
            }
            catch (Exception ex)
            {
                string message = deviceId != 0 ? string.Format("Failed to get service task, device Id: {0}", deviceId) : "Failed to get service tasks";
                throw new Exception(message, ex);
            }
        }

        private static string SelectTarget(string target, int id = 0)
        {
            return id == 0 ? target : string.Format("{0} {1} {2}", target, WhereId, id);
        }
    }
}
