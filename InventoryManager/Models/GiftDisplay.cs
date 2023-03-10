using ControlzEx.Standard;
using InventoryManager.Core.Models;

namespace InventoryManager.Models
{
    public class GiftDisplay
    {
        public Product Product { get; set; }
        public GivenAway GivenAway { get; set; }
        public void OnPropertyChanged()
        {
            GivenAway.OnPropertyChanged();
        }
    }
}
