using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class GivenAway
    {
        public int ID { get; set; }    
        public int InventoryID { get; set; }
        public int Amount { get; set; }
        public string Event { get; set; }
        public DateTime Date { get; set; }
    }
}
