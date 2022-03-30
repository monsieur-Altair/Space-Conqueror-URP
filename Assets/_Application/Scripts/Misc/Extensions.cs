using _Application.Scripts.SavedData;
using UnityEngine;

namespace _Application.Scripts.Misc
{
    public static class Extensions
    {
        public static T ConvertFromJson<T>(this string json) => 
            JsonUtility.FromJson<T>(json);
        
        public static string ConvertToJson(this object obj) => 
            JsonUtility.ToJson(obj);

        
    }
}