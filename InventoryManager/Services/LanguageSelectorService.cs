using InventoryManager.Contracts.Services;
using InventoryManager.Models;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

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
        public FlowDirection Flow => PreferedLang.Tag == Languages.First().Tag ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnLanguageChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PreferedLang.Tag));
        public void InitializeLanguage()
        {
            PreferedLang = LoadLanguagePreferences();
            SetLanguagePreferences(PreferedLang);
        }

        public void SetLanguagePreferences(Language Language)
        {
            SaveLanguagePreferences(Language);
            OnLanguageChanged();
            var cult = new CultureInfo(Language.Tag);
            App.Current.Dispatcher.Thread.CurrentCulture = cult;
            App.Current.Dispatcher.Thread.CurrentUICulture = cult;
            if (App.Current.MainWindow != null)
            {
                App.Current.MainWindow.FlowDirection = Flow;
                App.Current.MainWindow.Language = System.Windows.Markup.XmlLanguage.GetLanguage(Language.Tag);
                App.Current.MainWindow.UpdateLayout();
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
