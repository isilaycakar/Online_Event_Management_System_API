﻿namespace OEMS_API.Models
{
    public class EditEventModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public int? CityID { get; set; }
        public int? Capacity { get; set; }
    }
}
