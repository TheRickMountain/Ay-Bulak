using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public enum GameCulture
    {
        German,
        English,
        French,
        Portuguese,
        Russian
    }

    public static class LocalizationManager
    {
        private static Dictionary<GameCulture, CultureInfo> supportedCultures = new Dictionary<GameCulture, CultureInfo>()
            {
                { GameCulture.English, new CultureInfo("en-US") },
                { GameCulture.Russian, new CultureInfo("ru-RU") }
            };

        private static Dictionary<string, string> languageKeyValuePairs = new Dictionary<string, string>();

        public static void Initialize(GameCulture currentCulture)
        {
            CultureInfo currentCultureInfo = supportedCultures[currentCulture];

            string localizationFileName = $"{currentCultureInfo}_Localization.csv";
            string pathToLocalizationFile = Path.Combine(Engine.ContentDirectory, "Localization", localizationFileName);

            TextFieldParser parser = new TextFieldParser(pathToLocalizationFile);

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            // We skip the first line, as it contains information for ease of reading
            bool ignoreFirstLine = true;
            
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                if (ignoreFirstLine)
                {
                    ignoreFirstLine = false;
                    continue;
                }

                languageKeyValuePairs.Add(fields[0], fields[1]);
            }
        }

        public static string GetText(string key)
        {
            return languageKeyValuePairs.ContainsKey(key) ? languageKeyValuePairs[key] : key;
        }

        public static string GetText(string key, object arg0)
        {
            return languageKeyValuePairs.ContainsKey(key) ? string.Format(languageKeyValuePairs[key], arg0) : key;
        }

        public static string GetText(string key, object arg0, object arg1)
        {
            return languageKeyValuePairs.ContainsKey(key) ? string.Format(languageKeyValuePairs[key], arg0, arg1) : key;
        }

        public static IEnumerable<CultureInfo> GetSupportedLanguages()
        {
            foreach(var kvp in supportedCultures)
            {
                yield return kvp.Value;
            }
        }

    }
}
