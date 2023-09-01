using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class TicketCompanyEvent
    {
        public int EventId { get; set; }
        public int CompanyId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [ForeignKey("CompanyId")]
        public TicketCompany TicketCompany { get; set; }
    }
}
