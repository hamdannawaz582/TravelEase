using System;
using System.Collections.Generic;

namespace DB_Project.Models
{
    public class TravellerProfile
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public int Age { get; set; }
        public int Budget { get; set; }
    }
    
    public class Trip
    {
        public int TripID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string CancellationPolicy { get; set; }
        public int GroupSize { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PriceRange { get; set; }
        public string OperatorUsername { get; set; }
        public string Destination { get; set; }
    }
    
    // Add other models
}