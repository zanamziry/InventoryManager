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
        public int Result => (TotalReal + TotalGivenAway + TotalOutside) - System.CloseBalance;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalReal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
        }
    }
}
