using System.Collections.Generic;
using _Application.Scripts.Planets;
using UnityEngine;

namespace _Application.Scripts.AI
{
    public interface IAction
    {
        void Execute();
        public void InitAction(List<List<Base>> planets, Vector3 mainPos);
    }
}