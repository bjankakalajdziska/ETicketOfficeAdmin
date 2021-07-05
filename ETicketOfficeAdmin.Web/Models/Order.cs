using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicketOfficeAdmin.Web.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ETicketOfficeUser User { get; set; }
        public ICollection<TicketsInOrder> Tickets { get; set; }
    }
}
