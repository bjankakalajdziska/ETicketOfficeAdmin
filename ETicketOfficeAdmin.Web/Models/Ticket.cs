using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicketOfficeAdmin.Web.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string MovieName { get; set; }

        public string Image { get; set; }

        public string MovieDescription { get; set; }

        public double TicketPrice { get; set; }

        public double MovieRating { get; set; }

        public string MovieGenre { get; set; }

        public DateTime Date { get; set; }
    }
}
