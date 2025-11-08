namespace SchoolEquipmentApi.DTOs
{
    public class Data
    {
        public List<User> Users { get; set; } = new();
        public Dictionary<string, Session> Sessions { get; set; } = new();
        public List<Equipment> Equipment { get; set; } = new();
        public List<BorrowRequest> Requests { get; set; } = new();
    }
}