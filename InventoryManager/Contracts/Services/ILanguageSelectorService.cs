using InventoryManager.Models;
using System.ComponentModel;
using System.Windows;

namespace InventoryManager.Contracts.Services
{
    public interface ILanguageSelectorService 
    {
        IEnumerable<Language> Languages { get; }
        Language PreferedLang { get; set; }
        public FlowDirection Flow { get; }
        void InitializeLanguage();
        void SetLanguagePreferences(Language Language);
    }
}
