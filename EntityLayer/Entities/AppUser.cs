using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class AppUser: IdentityUser<int>
    {
        public string? NameSurname { get; set; }
        public string? ConfirmPassword { get; set; }
        public List<Event>? Events {  get; set; }
        public List<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
    }
}
