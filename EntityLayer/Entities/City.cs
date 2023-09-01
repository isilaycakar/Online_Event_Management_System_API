using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class City
    {
        [Key]
        public int CityID { get; set; }
        public string CityName { get; set; }
        public List<Event>? Events { get; set; }
    }
}
