namespace OEMS_API.Models
{
    public class MemberEventListModel
    {
        public int EventID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime CloseDate { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public bool Ticket { get; set; }
        public string? Status { get; set; }
        public int? Price { get; set; }
        public int CategoryID { get; set; }
        public int CityID { get; set; }
    }
}
