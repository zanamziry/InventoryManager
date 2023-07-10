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
        public int RemainingOutside => TotalOutside - TotalSoldOutside;
        public int TotalReal => Locals.Sum(o => o.Total);
        public int TotalOpen => Locals.Sum(o => o.Open);
        public int TotalInv => Locals.Sum(o => o.Inventory);
        public string Note 
        {
            get
            {
                string outsideNote = string.Empty;
                foreach(var i in SentOutsides)
                {
                    if (i.Remaining == 0)
                        continue;
                    outsideNote += $"({i.Remaining} في {i.Location})";
                }
                string giftNote = string.Empty;
                foreach (var i in GivenAways)
                {
                    if (i.Amount == 0)
                        continue;
                    giftNote += $"({i.Amount} عرض {i.Event})";
                }
                if (outsideNote.Length < 1 && giftNote.Length < 1)
                    return string.Empty;
                return $"{outsideNote} {giftNote}";
            }
        }
        public DateTime? NearestExp
        {
            get
            {
                DateTime? NearestDate = null;
                var nearestByDays = DateTime.MaxValue.Subtract(DateTime.Now);
                foreach (var item in Locals)
                {
                    if (item.ExpireDate == null)
                        continue;
                    var i = item.ExpireDate?.Subtract(DateTime.Now);
                    if (i < nearestByDays)
                    {
                        NearestDate = item.ExpireDate;
                        nearestByDays = (TimeSpan)i;
                    }
                }
                return NearestDate;
            }
        }
        public int Result => (TotalReal + TotalGivenAway + (TotalOutside - TotalSoldOutside)) - System.CloseBalance;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalGivenAway)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalOutside)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalSoldOutside)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingOutside)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalReal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalOpen)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalInv)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
        }
    }
}
