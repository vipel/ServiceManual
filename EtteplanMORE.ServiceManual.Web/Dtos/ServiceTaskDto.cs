namespace EtteplanMORE.ServiceManual.ApplicationCore.Entities
{
    public class ServiceTaskDto
    {
        public int Id { get; set; }
        public FactoryDevice? FactoryDevice { get; set; }
        public DateTime Created { get; set; }
        public string? Description { get; set; }
        public Criticality Criticality { get; set; }
        public State State { get; set; }
    }
}
