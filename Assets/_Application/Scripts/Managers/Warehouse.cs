using System.Collections.Generic;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class Warehouse : MonoBehaviour
    {
        public List<Texture> scientificTextures;
        public List<Texture> attackerTextures;
        public List<Texture> spawnerTextures;

        public List<Texture> rocketsTextures;

        public Material buffedPlanetMaterial;
        public Material buffedRocketMaterial;

        public Material basePlanetMaterial;
        public Material baseRocketMaterial;

        public Material glassMaterial;
        
        public List<Color> counterBackground;
        public List<Color> counterForeground;
    }
}