using InventoryManager.Contracts.Views;
using InventoryManager.Properties;
using InventoryManager.Views;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InventoryManager.Services
{
    public static class UpdatingService
    {
        public static async void Update(IShellWindow shellWindow)
        {
            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/zanamziry/InventoryManager"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    if (updateInfo.ReleasesToApply != null && updateInfo.ReleasesToApply.Count > 0)
                    {
                        if (MessageBoxResult.Yes == MessageBox.Show($"{Resources.UpdateServiceNewUpdate} {updateInfo.ReleasesToApply.First().Version}", Resources.UpdateServiceNewUpdateTitle, MessageBoxButton.YesNo, MessageBoxImage.Question))
                        {
                            var progressDialog = await shellWindow.ShowProgress("Updating", "Please Wait For The Update To Finish");
                            var s = await manager.UpdateApp(o => progressDialog.SetProgress(o));
                            ///TODO: Make A popup to show the progress
                            MessageBox.Show(Resources.UpdateServiceRestartRequired, Resources.UpdateServiceUpdateComplete);
                            UpdateManager.RestartApp();
                        }
                    }
                    else
                    {
                        MessageBox.Show(Resources.UpdateServiceThisIsTheLatest, Resources.UpdateServiceNoUpdateFound);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Resources.UpdateServiceProblemOccurred}\n{ex.Message}", Resources.UpdateServiceNoUpdateFound);
                return;
            }
        }
    }
}
