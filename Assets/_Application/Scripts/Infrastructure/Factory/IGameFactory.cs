using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Factory
{
    public interface IGameFactory : IService
    {
        void CreateWorld();
        GameObject CreateAcid();
        GameObject CreateIndicator();
        GameObject CreateIce();
        Scriptables.Skill CreateSkillResource(string path);
    }
}