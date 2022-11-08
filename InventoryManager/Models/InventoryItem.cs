using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace InventoryManager.Models
{
    public class InventoryItem
    {
        public int System { get; set; }
        public int Real
        { 
            get 
            {
                int result = 0;
                foreach(var i in InventoryList)
                {
                    result += i.Open + i.Inventory;
                }
                return result;
            }
        }
        public int Result
        {
            get
            {
                int result = 0;
                foreach (var i in InventoryList)
                {
                    result += i.Open + i.Inventory + i.Outside + i.GivenAway;
                }
                return result - System;
            }
        }
        public IList<InventoryListItem> InventoryList { get; set; } = new List<InventoryListItem>();
    }
}
