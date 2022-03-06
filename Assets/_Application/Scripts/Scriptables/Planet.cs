﻿using UnityEngine;

namespace Scriptables
{    
    [CreateAssetMenu (fileName = "new planet resource",menuName = "Resources/Planet Resource")]
    public class Planet : ScriptableObject
    {
        public int maxCount;
        public float produceCount;
        public float produceTime;
        public float defense;
        public float reducingSpeed;
    }
}