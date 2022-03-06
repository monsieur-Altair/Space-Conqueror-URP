using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "new ice resource", menuName = "Resources/Ice resource")]
    public class Ice : Skill
    {
        public float duration;
    }
}