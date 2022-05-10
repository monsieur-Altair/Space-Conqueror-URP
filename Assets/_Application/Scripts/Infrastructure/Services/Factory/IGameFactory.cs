using System.Collections.Generic;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Managers;
using _Application.Scripts.Scriptables;
using _Application.Scripts.UI;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services.Factory
{
    public interface IGameFactory : IService
    {
        Main CreateWorld();
        GameObject CreateAcid();
        GameObject CreateIndicator();
        GameObject CreateIce();
        List<IProgressReader> ProgressReaders { get; }
        List<IProgressWriter> ProgressWriters { get; }
        void CleanUp();
        UISystem CreateUISystem();
        IObjectPool CreatePool();
        ICoroutineRunner CreateCoroutineRunner();
        Camera CreateCamera();
    }
}