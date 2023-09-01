using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class TicketCompany
    {
        [Key]
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Url { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? Role { get; set; }
        public List<TicketCompanyEvent> TicketCompanyEvents { get; set; } = new List<TicketCompanyEvent>();
    }
}
