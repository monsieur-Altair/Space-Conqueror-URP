using System.Globalization;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class PrefsManager
    {
        public static void SetInt(string key, int value, bool reactive = false)
        {
            PlayerPrefs.SetInt(key, value);
            if (reactive)
                PlayerPrefs.Save();
        }

        public static void SetString(string key, string value, bool reactive = false)
        {
            PlayerPrefs.SetString(key, value);
            if (reactive)
                PlayerPrefs.Save();
        }

        public static void SetFloat(string key, float value, bool reactive = false)
        {
            PlayerPrefs.SetFloat(key, value);
            if (reactive)
                PlayerPrefs.Save();
        }

        public static void SetDouble(string key, double value, bool reactive = false)
        {
            PlayerPrefs.SetString(key, value.ToString(CultureInfo.InvariantCulture));
            if (reactive)
                PlayerPrefs.Save();
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public static double GetDouble(string key, double defaultValue = 0)
        {
            if (double.TryParse(PlayerPrefs.GetString(key), NumberStyles.Any, CultureInfo.InvariantCulture,out double result))
            {
                return result;
            }

            return defaultValue;
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}