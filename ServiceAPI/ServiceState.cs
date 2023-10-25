namespace ServiceAPI
{
    public class ServiceState
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int OldStateId { get; set; }
        public int NewStateId { get; set; }
        public DateTime Time { get; set; }
    }
}
