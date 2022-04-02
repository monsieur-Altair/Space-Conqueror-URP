using System.Collections.Generic;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class Warehouse : MonoBehaviour
    {
        public List<Texture> altarTextures;
        public List<Texture> attackerTextures;
        public List<Texture> spawnerTextures;

        public List<Texture> warriorsTextures;

        public Material buffedBuildingMaterial;
        public Material buffedWarriorMaterial;

        public Material baseBuildingMaterial;
        public Material baseWarriorMaterial;

        public List<Color> counterBackground;
        public List<Color> counterForeground;
    }
}