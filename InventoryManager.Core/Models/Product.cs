using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class Product
    {
        public string ID { get; set; }    
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal PV { get; set; }
    }
}
