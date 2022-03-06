using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "new call resource", menuName = "Resources/Call resource")]
    public class Call : Skill
    {
        public float callPercent;
    }
}