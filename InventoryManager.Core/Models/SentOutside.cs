using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class SentOutside
    {
        public int ID { get; set; }    
        public int InventoryID { get; set; }
        public int AmountSent { get; set; }
        public int AmountSold { get; set; }
        public string Location { get; set; }
        public int Remaining => AmountSent - AmountSold; 
    }
}
