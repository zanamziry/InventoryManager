using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class SystemAPI
    {
        public string agent { get; set; }
        public string date { get; set; }
        public IEnumerable<SystemProduct> list { get; set; }
    }
}
