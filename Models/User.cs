using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task_Alocator.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public virtual List<Models.Event> Events { get; set; }
    }
}
