using InventoryManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Models
{
    public class MainInventory
    {
        public Product Product { get; set; }
        public SystemProduct System { get; set; } 
        public int TotalGivenAway { get; set; }
        public int TotalOutside { get; set; }
        public int TotalLocal { get; set; }
        public int Result => System.CloseBalance - TotalLocal + TotalGivenAway + TotalOutside;
    }
}
