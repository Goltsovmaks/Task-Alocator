using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task_Alocator.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd{ get; set; }
        public string Description { get; set; }


        // внешний ключ
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
