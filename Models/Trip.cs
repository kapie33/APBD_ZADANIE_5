using System;
using System.Collections.Generic;

namespace YourNamespace.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<ClientTrip> ClientTrips { get; set; }
    }
}
