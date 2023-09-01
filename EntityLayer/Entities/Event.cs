using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public AppUser AppUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime CloseDate { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public bool Ticket { get; set; }
        public int? Price { get; set; }
        public bool Status { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        public int CityID { get; set; }
        public City City { get; set; }
        public List<TicketCompanyEvent> TicketCompanyEvents { get; set; } = new List<TicketCompanyEvent>();
        public List<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
    }
}
