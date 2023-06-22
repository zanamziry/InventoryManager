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
        public static async void Update()
        {
            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/zanamziry/InventoryManager"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    if (updateInfo.ReleasesToApply != null && updateInfo.ReleasesToApply.Count > 0)
                    {
                        if (MessageBoxResult.Yes == MessageBox.Show($"New Update Is Available Do You Want To Update To {updateInfo.ReleasesToApply.First().Version}", "Update Availbable!", MessageBoxButton.YesNo, MessageBoxImage.Question))
                        {
                            var s = await manager.UpdateApp();
                            ///TODO: Make A popup to show the progress
                            /*
                            while (s.StagingPercentage != null || s.StagingPercentage < 100)
                            {

                            }
                            */
                            MessageBox.Show($"Update Complete", "Restart Required For the Update To Take Effect");
                            UpdateManager.RestartApp();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"This is the latest version", "No Updates Available");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was a problem while searching for updates\n{ex.Message}", "No Updates Founds");
                return;
            }
        }
    }
}
