using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Application.Scripts.Misc
{
    public static class Extensions
    {
        public static T ConvertFromJson<T>(this string json) => 
            JsonUtility.FromJson<T>(json);
        
        public static string ConvertToJson(this object obj) => 
            JsonUtility.ToJson(obj);

        public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(
                x ?? v.x,
                y ?? v.y,
                z ?? v.z
            );
        }

        /// <summary>
        /// Method to set specific elements of vector
        /// </summary>
        public static Vector2 With(this Vector2 v, float? x = null, float? y = null)
        {
            return new Vector2(
                x ?? v.x,
                y ?? v.y
            );
        }

        /// <summary>
        /// Equals to new Vector2(v.x, v.z)
        /// </summary>
        public static Vector2 ToXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        /// <summary>
        /// Equals to new Vector3(v.x, y, v.y)
        /// </summary>
        public static Vector3 FromXZ(this Vector2 v, float y = 0)
        {
            return new Vector3(v.x, y, v.y);
        }

        /// <summary>
        /// Inverse lerp for vectors
        /// </summary>
        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 v)
        {
            Vector3 ab = b - a;
            Vector3 av = v - a;
            return Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
        }

        /// <summary>
        /// Lerps between v.x and v.y
        /// </summary>
        public static float LerpInside(this Vector2 v, float t)
        {
            return Mathf.Lerp(v.x, v.y, t);
        }
        public static float RandomInside(this Vector2 v)
        {
            return Random.Range(v.x, v.y);
        }
        
        public static int RandomInside(this Vector2Int v)
        {
            return Random.Range(v.x, v.y);
        }
        
 	    public static float Remap(this float value, float minA, float maxA, float minB, float maxB)
        {
            float t = Mathf.InverseLerp(minA, maxA, value);
            return Mathf.Lerp(minB, maxB, t);
        }

        public static Vector2 GetDirection(this float radianAngle)
        {
            return new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
        }
        
        public static float RemapUnclamped(this float value, float minA, float maxA, float minB, float maxB)
        {
            float t = (value - minA) * (maxA - minA);
            return Mathf.LerpUnclamped(minB, maxB, t);
        }
        
        public static float Remap(this float value, float minA, float maxA, float minB, float maxB, AnimationCurve curve)
        {
            float t = Mathf.InverseLerp(minA, maxA, value);
            t = curve.Evaluate(t);
            return Mathf.Lerp(minB, maxB, t);
        }

        public static double Truncate(this double value, int digits)
        {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Truncate(value / mult) * mult;
            return result;
        }

        public static float GetCycledDistance(float a, float b, float cycleLength)
        {
            if (a > cycleLength)
                a %= cycleLength;
            if (b > cycleLength)
                b %= cycleLength;
            if (a < b)
            {
                return Mathf.Min(b - a, a + (cycleLength - b));
            }
            else
            {
                return Mathf.Min(a - b, b + cycleLength - a);
            }
        }
    }
}