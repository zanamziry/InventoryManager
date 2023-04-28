using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using InventoryManager.Models;
using GemBox.Spreadsheet;
using Newtonsoft.Json;

using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace InventoryManager.Views;

public partial class MainPage : Page, INotifyPropertyChanged, INavigationAware
{
    public MainPage(IDBSetup dBSetup,INavigationService navigationService, ISystemDataGather dataGather)
    {
        InitializeComponent();
        DataContext = this;
        _navigationService = navigationService;
        _dBSetup = dBSetup;
        ProductsORM = _dBSetup.GetTable<ProductsORM>();
        LocalORM = _dBSetup.GetTable<LocalInventoryORM>();
        SystemORM = _dBSetup.GetTable<SystemProductsORM>();
        GivenORM = _dBSetup.GetTable<GivenAwayORM>();
        OutsideORM = _dBSetup.GetTable<SentOutsideORM>();
        _dataGather = dataGather;
    }
    
    private readonly INavigationService _navigationService;
    private readonly ISystemDataGather _dataGather;
    private readonly IDBSetup _dBSetup;
    private readonly SystemProductsORM SystemORM;
    private readonly ProductsORM ProductsORM;
    private readonly GivenAwayORM GivenORM;
    private readonly LocalInventoryORM LocalORM;
    private readonly SentOutsideORM OutsideORM;

    public bool SourceHasItems => Source.Count > 0;

    public decimal SoldMoney
    {
        get
        {
            decimal result = 0;
            foreach(var mainInventory in Source)
            {
                foreach (var outside in mainInventory.SentOutsides)
                    if (outside.Old)
                        result += outside.AmountSold * mainInventory.Product.Old_Price;
                    else
                        result += outside.AmountSold * mainInventory.Product.Price;
            }
            return result;
        }
    }

    public decimal GiftPoints => Source.Sum(o => o.TotalGivenAway * o.Product.PV);

    public decimal GiftMoney => Source.Sum(o => o.TotalGivenAway * o.Product.Price);

    public ObservableCollection<MainInventory> Source { get; } = new ObservableCollection<MainInventory>();
    public event PropertyChangedEventHandler PropertyChanged;

    private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return;
        }

        storage = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async void OnRemoveAllButtonClicked(object sender, RoutedEventArgs e)
    {
        await ProductsORM.DeleteAll();
        Source.Clear();
    }

    private async void OnGetLatestListClicked(object sender, RoutedEventArgs e)
    {
        string json = await _dataGather.GetProductsAsync();
        if(json != null)
        {
            try
            {
                var products = JsonConvert.DeserializeObject<List<Product>>(json);
                await ProductsORM.DeleteAll();
                Source.Clear();
                foreach (var item in products)
                {
                    await ProductsORM.Insert(item);
                    AddToView(item);
                }
            }
            catch
            {
                MessageBox.Show("Didnt Find Any Products!");
                return;
            }
        }
    }

    private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if(e.OriginalSource is FrameworkElement FE && FE.DataContext is MainInventory SelectedProduct)
        {
            _navigationService.NavigateTo(typeof(InventoryPage), new { SelectedProduct , Source });
        }
    }

    async void AddToView(Product p)
    {
        var localDB = await LocalORM.SelectProduct(p);
        var sys = await SystemORM.SelectProduct(p);
        Source.Add(new MainInventory
        {
            Product = p,
            System = sys,
            Locals = new ObservableCollection<LocalInventory>(localDB),
            GivenAways = new ObservableCollection<GivenAway>(await GivenORM.SelectByProduct(p)),
            SentOutsides = new ObservableCollection<SentOutside>(await OutsideORM.SelectByProduct(p)),
        });
    }
    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        Source.CollectionChanged += Source_CollectionChanged; 
        Source.Clear();
        foreach (var p in await ProductsORM.SelectAll())
            AddToView(p);
    }

    private void Source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SoldMoney));
        OnPropertyChanged(nameof(GiftMoney));
        OnPropertyChanged(nameof(GiftPoints));
        OnPropertyChanged(nameof(SourceHasItems));
    }

    void INavigationAware.OnNavigatedFrom()
    {
        Source.CollectionChanged -= Source_CollectionChanged;
    }
    
    #region Excel Exportation
    /// <summary>
    /// Export The Data To An Excel File
    /// </summary>
    private void OnExportAsExcelClicked(object sender, RoutedEventArgs e)
    {
        string date = DateTime.Now.ToString("(dd-MM)");
        SaveFileDialog sd = new SaveFileDialog();
        sd.DefaultExt = ".xlsx";
        sd.FileName = $"INV-AUTO-{@date}";
        if (sd.ShowDialog() != true || string.IsNullOrWhiteSpace(sd.FileName))
            return;
        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        ExcelFile xl = new ExcelFile();
        var ws = xl.Worksheets.Add("جرد");

        CellStyle headerStyle = new CellStyle();
        headerStyle.VerticalAlignment = VerticalAlignmentStyle.Center;
        headerStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
        headerStyle.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromArgb(-11645362), LineStyle.Thin);
        headerStyle.FillPattern.SetSolid(SpreadsheetColor.FromArgb(-8420218));
        
        CellStyle cellstyle = new CellStyle();
        cellstyle.VerticalAlignment = VerticalAlignmentStyle.Center;
        cellstyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
        cellstyle.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Accent3Lighter60Pct), LineStyle.Thin);
        cellstyle.WrapText = true;

        // Write the name of headers
        var i = 0;
        ws.Cells[i, 0].Value = "ID";
        ws.Cells[i, 1].Value = "Name";
        ws.Cells[i, 2].Value = "Real";
        ws.Cells[i, 3].Value = "System";
        ws.Cells[i, 4].Value = "Gifts";
        ws.Cells[i, 5].Value = "Outside";
        ws.Cells[i, 6].Value = "Result";
        ws.Cells[i, 7].Value = "Open";
        ws.Cells[i, 8].Value = "Price";
        ws.Cells[i, 9].Value = "PV"; 

        for (int c = 0; c <= 9; c++)
        {
            ws.Cells[i, c].Style = headerStyle;
            ws.Columns[c].SetWidth(70, LengthUnit.Pixel);
        }
        ws.Columns[0].SetWidth(1,LengthUnit.Inch);
        ws.Columns[1].SetWidth(2.5,LengthUnit.Inch);

        // Write the values to the cells
        foreach (var item in Source)
        {
            i++;
            ws.Cells[i, 0].Value = item.Product.ID;
            ws.Cells[i, 0].Style = cellstyle;
            ws.Cells[i, 1].Value = $"{item.Product.Name}\n{item.Product.Name_AR}";
            ws.Cells[i, 1].Style = cellstyle;
            ws.Cells[i, 2].Value = item.System.CloseBalance;
            ws.Cells[i, 2].Style = cellstyle;
            ws.Cells[i, 3].Value = item.TotalReal;
            ws.Cells[i, 3].Style = cellstyle;
            ws.Cells[i, 4].Value = item.TotalGivenAway;
            ws.Cells[i, 4].Style = cellstyle;
            ws.Cells[i, 5].Value = item.TotalOutside;
            ws.Cells[i, 5].Style = cellstyle;
            ws.Cells[i, 6].Value = item.Result;
            ws.Cells[i, 6].Style = cellstyle;
            ws.Cells[i, 7].Value = item.TotalOpen;
            ws.Cells[i, 7].Style = cellstyle;
            ws.Cells[i, 8].Value = item.Product.Price;
            ws.Cells[i, 8].Style = cellstyle;
            ws.Cells[i, 9].Value = item.Product.PV;
            ws.Cells[i, 9].Style = cellstyle;
        }

        // Create table and enable totals row.
        var table = ws.Tables.Add("Jard", $"A1:J{Source.Count+1}", true);
        ws.ProtectionSettings.SetPassword("ZANA99");
        xl.ProtectionSettings.SetPassword("ZANA99");
        ws.ProtectionSettings.AllowSelectingLockedCells = true;
        ws.ProtectionSettings.AllowSelectingUnlockedCells = true;
        ws.ProtectionSettings.AllowSorting = false;
        ws.ProtectionSettings.AllowDeletingColumns = false;
        ws.ProtectionSettings.AllowDeletingRows = false;
        ws.ProtectionSettings.AllowInsertingRows = false;
        ws.ProtectionSettings.AllowFormattingRows = true;
        table.HasTotalsRow = false;
        ws.Protected = true;
        xl.Protected = true;
        xl.Save(sd.FileName);
        #endregion
    }
}
