using ControlzEx.Standard;
using InventoryManager.Core.Models;

namespace InventoryManager.Models
{
    public class SentOutDisplay
    {

        public Product Product { get; set; }
        public SentOutside Outside { get; set; }
        public void OnPropertyChanged()
        {
            Outside.OnPropertyChanged();
        }
    }
}
