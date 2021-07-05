using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicketOfficeAdmin.Web.Models
{
    public class TicketsInOrder
    {
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public Guid OrderId { get; set; }
        public Order UserOrder { get; set; }

        public int Quantity { get; set; }
    }
}
