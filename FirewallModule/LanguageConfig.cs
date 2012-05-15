using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;

namespace FM
{
    public static class LanguageConfig
    {
        /// <summary>
        /// Language enums
        /// </summary>
        public enum Language
        {
            ENGLISH,
            SPANISH,
            GERMAN,
            CHINESE,
            RUSSIAN,
            PORTUGUESE,
            NONE
        }

        static Language cLanguage = Language.NONE;

        /// <summary>
        /// Change the current language
        /// </summary>
        /// <param name="l"></param>
        public static void SetLanguage(Language l)
        {
            cLanguage = l;
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = folder + Path.DirectorySeparatorChar + "firebwall";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string file = folder + Path.DirectorySeparatorChar + "language.cfg";
                switch (cLanguage)
                {
                    case Language.ENGLISH:
                        File.WriteAllText(file, "en");
                        break;
                    case Language.SPANISH:
                        File.WriteAllText(file, "es");
                        break;
                    case Language.GERMAN:
                        File.WriteAllText(file, "de");
                        break;
                    case Language.CHINESE:
                        File.WriteAllText(file, "zh");
                        break;
                    case Language.RUSSIAN:
                        File.WriteAllText(file, "ru");
                        break;
                    case Language.PORTUGUESE:
                        File.WriteAllText(file, "pt");
                        break;
                }                
            }
            catch { }
        }

        public static string GetCurrentTwoLetter()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = folder + Path.DirectorySeparatorChar + "firebwall";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string file = folder + Path.DirectorySeparatorChar + "language.cfg";
            if (File.Exists(file))
            {
                string twoLetter = File.ReadAllText(file);
                switch (twoLetter)
                {
                    case "en":
                        cLanguage = Language.ENGLISH;
                        break;
                    case "es":
                        cLanguage = Language.SPANISH;
                        break;
                    case "de":
                        cLanguage = Language.GERMAN;
                        break;
                    case "zh":
                        cLanguage = Language.CHINESE;
                        break;
                    case "ru":
                        cLanguage = Language.RUSSIAN;
                        break;
                    case "pt":
                        cLanguage = Language.PORTUGUESE;
                        break;
                }
            }
            else if (cLanguage == Language.NONE)
            {
                switch (cLanguage)
                {
                    case Language.NONE:
                        return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                    case Language.ENGLISH:
                        return "en";
                    case Language.CHINESE:
                        return "zh";
                    case Language.GERMAN:
                        return "de";
                    case Language.PORTUGUESE:
                        return "pt";
                    case Language.RUSSIAN:
                        return "ru";
                    case Language.SPANISH:
                        return "es";
                } 
            }
            return "en";
        }

        /// <summary>
        /// Returns the current language, or sets it if it hasn't been.
        /// </summary>
        /// <returns></returns>
        public static Language GetCurrentLanguage()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = folder + Path.DirectorySeparatorChar + "firebwall";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string file = folder + Path.DirectorySeparatorChar + "language.cfg";
            if (File.Exists(file))
            {
                string twoLetter = File.ReadAllText(file);
                switch (twoLetter)
                {
                    case "en":
                        cLanguage = Language.ENGLISH;
                        break;
                    case "es":
                        cLanguage = Language.SPANISH;
                        break;
                    case "de":
                        cLanguage = Language.GERMAN;
                        break;
                    case "zh":
                        cLanguage = Language.CHINESE;
                        break;
                    case "ru":
                        cLanguage = Language.RUSSIAN;
                        break;
                    case "pt":
                        cLanguage = Language.PORTUGUESE;
                        break;
                }
            }
            else if (cLanguage == Language.NONE)
            {
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                {
                    case "en":
                        cLanguage = Language.ENGLISH;
                        break;
                    case "es":
                        cLanguage = Language.SPANISH;
                        break;
                    case "de":
                        cLanguage = Language.GERMAN;
                        break;
                    case "zh":
                        cLanguage = Language.CHINESE;
                        break;
                    case "ru":
                        cLanguage = Language.RUSSIAN;
                        break;
                    case "pt":
                        cLanguage = Language.PORTUGUESE;
                        break;
                }
            }
            return cLanguage;
        }
    }
}
