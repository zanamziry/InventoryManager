using System;
using InventoryManager.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InventoryManager.Models
{
    public class MainInventory : INotifyPropertyChanged
    {
        public Product Product { get; set; }
        public SystemProduct System { get; set; }
        public ObservableCollection<LocalInventory> Locals { get; set; } = new ObservableCollection<LocalInventory>();
        public ObservableCollection<SentOutside> SentOutsides { get; set; } = new ObservableCollection<SentOutside>();
        public ObservableCollection<GivenAway> GivenAways { get; set; } = new ObservableCollection<GivenAway>();

        public int TotalGivenAway => GivenAways.Sum(s => s.Amount);
        public int TotalOutside => SentOutsides.Sum(s => s.AmountSent);
        public int TotalSoldOutside => SentOutsides.Sum(s => s.AmountSold);
        public int TotalReal => Locals.Sum(o => o.Total);
        public int TotalOpen => Locals.Sum(o => o.Open);
        public int TotalInv => Locals.Sum(o => o.Inventory);
        public DateTime NearestExp
        {
            get
            {
                long nowTicks = DateTime.Now.Ticks;
                DateTime nearest = DateTime.MaxValue;
                foreach (var item in Locals)
                {
                    if ((item.ExpireDate.Ticks - nowTicks) < nearest.Ticks)
                        nearest = item.ExpireDate;
                }
                return nearest;
            }
        }
        public int Result => (TotalReal + TotalGivenAway + (TotalOutside - TotalSoldOutside)) - System.CloseBalance;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalReal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalGivenAway)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalOutside)));
        }
    }
}
