using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Models
{
    public class InventoryListItem
    {
        public int ID { get; set; }
        public int Inventory { get; set; }
        public int Open { get; set; }
        public int Outside { get; set; }
        public int GivenAway { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Note { get; set; }
    }
}
