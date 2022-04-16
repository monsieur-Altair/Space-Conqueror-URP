using System.Collections.Generic;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class Warehouse : MonoBehaviour
    {
        public List<Texture> crystalBaseTextures;
        public List<Texture> crystalEmissionTextures;
        public List<Texture> flagTextures;
        public List<Texture> roofTextures;

        public List<Texture> warriorsTextures;

        public Material buffedBuildingMaterial;
        public Material buffedWarriorMaterial;

        public Material baseBuildingMaterial;
        public Material baseCrystalMaterial;
        public Material baseFlagMaterial;
        public Material baseRoofMaterial;
        
        public Material baseWarriorMaterial;

        public List<Color> counterBackground;
        public List<Color> counterForeground;
    }
}