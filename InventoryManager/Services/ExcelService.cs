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
    public static class ExcelService
    {
        static bool checkExcelExist(Ex.Application application)
        {
            if (application == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return false;
            }
            return true;
        }
        public static void MakeJard(string Filename)
        {
            Ex.Application xlApp = new Ex.Application();
            if (!checkExcelExist(xlApp))
                return;
            var s = xlApp.;
            Workbook wb = xlApp.Workbooks.Open(Filename);
            Worksheet ws = (Ex.Worksheet)wb.Worksheets.get_Item(1);
            ws.Cells[1, 1] = "ID";
            wb.Save();
            wb.Close();
        }
    }
}
