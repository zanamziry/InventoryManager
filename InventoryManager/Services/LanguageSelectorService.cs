using InventoryManager.Contracts.Services;
using InventoryManager.Models;
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

        public void InitializeLanguage()
        {
            PreferedLang = LoadLanguagePreferences();
            SetLanguagePreferences(PreferedLang);
        }

        public void SetLanguagePreferences(Language Language)
        {
            if (Language != PreferedLang)
                SaveLanguagePreferences(Language);
            App.Current.Dispatcher.Thread.CurrentCulture = new CultureInfo(Language.Tag);
            App.Current.Dispatcher.Thread.CurrentUICulture = new CultureInfo(Language.Tag);
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
