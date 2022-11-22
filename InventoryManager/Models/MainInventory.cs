using InventoryManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Models
{
    public class MainInventory
    {
        public Product Product { get; set; }
        public SystemProduct System { get; set; }
        public ObservableCollection<LocalInventory> Locals { get; set; } = new ObservableCollection<LocalInventory>();

        public int TotalGivenAway { get; set; }
        public int TotalOutside { get; set; }
        public int TotalReal {
            get
            {
                int r = 0;
                foreach (var item in Locals)
                {
                    r += item.Real;
                }
                return r;
            }
        }
        public int Result => System.CloseBalance - TotalReal + TotalGivenAway + TotalOutside;

    }
}
