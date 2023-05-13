using InventoryManager.Contracts.Services;
using InventoryManager.Models;
using Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace InventoryManager.Services
{
    public class LanguageSelectorService : ILanguageSelectorService
    {
        private const string SettingsKey = "AppLanguagePreferences";

        public IEnumerable<Language> Languages { get; } = new List<Language>
        {
            new Language { Tag="en-US",Header="English" },
            new Language { Tag = "ar-IQ", Header = "عربي" },
        };
        public Language PreferedLang { get; set; }
        public FlowDirection Flow => PreferedLang?.Tag == Languages.First().Tag ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;

        public void InitializeLanguage()
        {
            PreferedLang = LoadLanguagePreferences();
            SetLanguagePreferences(PreferedLang);
        }

        public void SetLanguagePreferences(Language Language)
        {
            if (Language != PreferedLang)
                SaveLanguagePreferences(Language);
            var cult = new CultureInfo(Language.Tag);
            Dispatcher.CurrentDispatcher.Thread.CurrentCulture = cult;
            Dispatcher.CurrentDispatcher.Thread.CurrentUICulture = cult;
            App.Current.Dispatcher.Thread.CurrentCulture = cult;
            App.Current.Dispatcher.Thread.CurrentUICulture = cult;
            CultureInfo.CurrentCulture = cult;
            CultureInfo.CurrentUICulture = cult;
            CultureInfo.DefaultThreadCurrentUICulture = cult;
            CultureInfo.DefaultThreadCurrentCulture = cult;
            if (App.Current.MainWindow != null)
            {
                App.Current.MainWindow.Language = XmlLanguage.GetLanguage(Language.Tag);
            }
        }

        private Language LoadLanguagePreferences()
        {
            if (!App.Current.Properties.Contains(SettingsKey))
                return Languages.First();
            string lang = App.Current.Properties[SettingsKey].ToString();
            return Languages.FirstOrDefault(o => o.Tag == lang, Languages.First());
        }

        private void SaveLanguagePreferences(Language Language)
        {
            PreferedLang = Language;
            App.Current.Properties[SettingsKey] = Language.Tag;
        }
    }
}
