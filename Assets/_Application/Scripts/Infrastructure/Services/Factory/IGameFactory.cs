using System.Collections.Generic;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Managers;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services.Factory
{
    public interface IGameFactory : IService
    {
        Main CreateWorld();
        GameObject CreateAcid();
        GameObject CreateIndicator();
        GameObject CreateIce();
        Scriptables.Skill CreateSkillResource(string path);
        List<IProgressReader> ProgressReaders { get; }
        List<IProgressWriter> ProgressWriters { get; }
        void CleanUp();
    }
}