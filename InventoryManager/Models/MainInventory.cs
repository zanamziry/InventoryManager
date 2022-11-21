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
        public LocalInventory Local { get; set; }
        public IEnumerable<GivenAway> Given { get; set; }
        public IEnumerable<SentOutside> Outsides { get; set; }
        public int TotalGivenAway
        {
            get
            {
                int total = 0;
                foreach (var i in Given)
                    total += i.Amount;
                return total;
            }
        }
        public int TotalOutside
        {
            get
            {
                int total = 0;
                foreach (var i in Outsides)
                    total += i.Amount;
                return total;
            }
        }
        public int Result => System.CloseBalance - Real + TotalGivenAway + TotalOutside;
        public int Real => Local.Inventory + Local.Open;
    }
}
