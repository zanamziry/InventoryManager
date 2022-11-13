using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class SoldOutside
    {
        public int ID { get; set; }    
        public int OutsideID { get; set; }
        public int Amount { get; set; }
        public string Date { get; set; }
        public string CCode { get; set; }
    }
}
