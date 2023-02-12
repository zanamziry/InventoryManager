﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlzEx.Standard;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using InventoryManager.Helpers;
using InventoryManager.Models;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace InventoryManager.Views;

public partial class SentOutsidePage : Page, INotifyPropertyChanged, INavigationAware
{

    public SentOutsidePage(IDBSetup dBSetup)
    {
        DataContext = this;
        _dBSetup = dBSetup;
        outsideORM = _dBSetup.GetTable<SentOutsideORM>();
    }
    public ObservableCollection<string> Locations = new ObservableCollection<string>();
    public ObservableCollection<SentOutside> Source = new ObservableCollection<SentOutside>();
    private readonly SentOutsideORM outsideORM;
    private readonly IDBSetup _dBSetup;

    public event PropertyChangedEventHandler PropertyChanged;

    void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return;
        }

        storage = value;
        OnPropertyChanged(propertyName);
    }
    void INavigationAware.OnNavigatedTo(object parameter)
    {
        outsideORM.
    }


    void INavigationAware.OnNavigatedFrom()
    {
    }

    void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}