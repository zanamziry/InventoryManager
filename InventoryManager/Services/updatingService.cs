using InventoryManager.Contracts.Views;
using InventoryManager.Properties;
using InventoryManager.Views;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InventoryManager.Services
{
    public static class UpdatingService
    {
        public static async void Update(IShellWindow shellWindow, bool IsStartup = false)
        {
            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/zanamziry/InventoryManager"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    if (updateInfo.ReleasesToApply != null && updateInfo.ReleasesToApply.Count > 0)
                    {
                        var UpdateORNo = await shellWindow.ShowMessage(Resources.UpdateServiceNewUpdateTitle, $"{Resources.UpdateServiceNewUpdate} {updateInfo.ReleasesToApply.First().Version}", MessageDialogStyle.AffirmativeAndNegative);
                        if (MessageDialogResult.Affirmative == UpdateORNo)
                        {
                            var DialogController = await shellWindow.ShowProgress(Resources.UpdatingServiceUpdating, Resources.UpdatingServicePleaseWait);
                            DialogController.Maximum = 100;
                            DialogController.Minimum = 0;
                            var s = await manager.UpdateApp(o => DialogController.SetProgress(o));
                            await DialogController.CloseAsync();
                            await shellWindow.ShowMessage(Resources.UpdateServiceUpdateComplete, Resources.UpdateServiceRestartRequired);
                            UpdateManager.RestartApp();
                        }
                    }
                    else if (!IsStartup)
                        await shellWindow.ShowMessage(Resources.UpdateServiceNoUpdateFound, Resources.UpdateServiceThisIsTheLatest);
                }
            }
            catch (Exception ex)
            {
                await shellWindow.ShowMessage(Resources.UpdateServiceNoUpdateFound, $"{Resources.UpdateServiceProblemOccurred}\n{ex.Message}");
                return;
            }
        }
    }
}
