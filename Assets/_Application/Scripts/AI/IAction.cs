using System.Collections.Generic;
using UnityEngine;

namespace _Application.Scripts.AI
{
    public interface IAction
    {
        void Execute();
        public void InitAction(List<List<Buildings.BaseBuilding>> buildings, Vector3 mainPos);
    }
}