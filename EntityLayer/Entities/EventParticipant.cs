using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class EventParticipant
    {
        public int UserID { get; set; } 
        public AppUser AppUser { get; set; }

        public int EventId { get; set; } 
        public Event Event { get; set; }
    }
}
