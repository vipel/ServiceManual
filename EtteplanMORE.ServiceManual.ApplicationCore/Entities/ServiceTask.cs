using System;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Entities
{
    public enum Criticality
    {
        Critical,
        Important,
        Medium
    }

    public enum State
    {
        Open,
        Serviced
    }

    public class ServiceTask
    {
        public int Id { get; set; }
        public FactoryDevice FactoryDevice { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public Criticality Criticality { get; set; }
        public State State { get; set; }
    }
}
