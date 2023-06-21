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
    public class UpdatingService : IUpdatingService
    {
        string githubRepo = @"https://github.com/zanamziry/InventoryManager";
        private UpdateManager manager;
        public async void Initialize()
        {
            if (manager == null)
                try
                {
                    manager = await UpdateManager.GitHubUpdateManager(githubRepo);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Error Initializing The Update Module\n{e.Message}");
                }
        }
        public async Task Update()
        {
            if (manager == null)
            {
                MessageBox.Show("Update Manager Not Initialized!");
                return;
            }
            try
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was a problem while searching for updates\n{ex.Message}", "No Updates Founds");
                return;
            }
        }
    }
}
