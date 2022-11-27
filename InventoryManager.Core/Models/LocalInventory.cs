using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class LocalInventory
    {
        public int ID { get; set; }
        public string ProductID { get; set; }
        public int Inventory { get; set; }
        public int Open { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Total => Inventory + Open;
    }
}
