using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class SystemProduct
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int OpenBalance { get; set; }
        public int Sold { get; set; }
        public int CloseBalance { get; set; }
    }
}
