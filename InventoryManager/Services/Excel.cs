using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ex = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace InventoryManager.Services
{
    public class Excel:IDisposable
    {

        readonly string filePath;
        public Workbook workBook;
        public Worksheet worksheet;
        Ex.Application xlApp;
        public Excel(string FilePath)
        {
            filePath = FilePath;
            xlApp = new();
            if (!checkExcelExist(xlApp))
                return;
            OpenIfNot();
        }

        bool checkExcelExist(Ex.Application application)
        {
            if (application == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return false;
            }
            return true;
        }
        public void WriteToCell(int r, int c, string value)
        {
            if(r == 0 || c == 0)
            {
                throw new Exception("Row and Columns cant be 0");
            }
            OpenIfNot();
            (worksheet.Cells[r, c] as Ex.Range).Value = value;
        }
        public void OpenIfNot()
        {
            if (workBook != null)
                return;
            if (!File.Exists(filePath))
                workBook = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            else
                workBook = xlApp.Workbooks.Open(filePath);
            worksheet = (Worksheet)workBook.Worksheets.get_Item(1);
            workBook.SaveCopyAs(filePath);
        }
        public string ReadCell(int r,int c)
        {
            OpenIfNot();
            string s = (worksheet.Cells[r, c] as Ex.Range).Value;
            return s;
        }
        public void SaveAndClose()
        {
            if (workBook == null)
                return;
            workBook.SaveCopyAs(filePath);
            workBook.Close();
            xlApp.Quit();
            worksheet = null;
            workBook = null;
        }

        public void Dispose()
        {
            SaveAndClose();
        }
    }
}
