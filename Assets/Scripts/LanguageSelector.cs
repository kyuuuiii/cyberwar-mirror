using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;

public class LanguageSelector : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    void Start()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        languageDropdown.ClearOptions();

        foreach (var locale in locales)
        {
            languageDropdown.options.Add(new TMP_Dropdown.OptionData(locale.LocaleName));
        }

        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        languageDropdown.value = LocalizationSettings.AvailableLocales.Locales.IndexOf(
            LocalizationSettings.SelectedLocale);
    }

    void OnLanguageChanged(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}