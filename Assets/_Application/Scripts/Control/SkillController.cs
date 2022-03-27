﻿using System;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.SavedData;
using UnityEngine;

namespace _Application.Scripts.Control
{
    public enum SkillName
    {
        Buff=0,
        Acid,
        Ice,
        Call,
        None
    }
    public class SkillController
    {
        public Skills.Call Call { get; }
        public Skills.Buff Buff { get; }
        public Skills.Acid Acid { get; }
        public Skills.Ice Ice { get; }

        private readonly IProgressService _progressService;
        private readonly IScriptableService _scriptableService;
        private readonly ObjectPool _objectPool;

        public bool IsSkillNotSelected => 
            SelectedSkillName == SkillName.None;
        
        public SkillName SelectedSkillName { get; private set; }

        public SkillController(IProgressService progressService ,IGameFactory gameFactory, 
            IScriptableService scriptableService, ObjectPool objectPool)
        {
            ClearSelectedSkill();

            _progressService = progressService;
            _scriptableService = scriptableService;
            _objectPool = objectPool;
            
            
            Call = new Skills.Call(Planets.Team.Blue, Planets.Scientific.DecreaseScientificCount);
            Call.NeedObjectFromPool += SpawnUnit;
            Call.SetSkillObject(gameFactory.CreateIndicator());

            Buff = new Skills.Buff(Planets.Team.Blue, Planets.Scientific.DecreaseScientificCount);

            Acid = new Skills.Acid(Planets.Team.Blue, Planets.Scientific.DecreaseScientificCount);
            Acid.SetSkillObject(gameFactory.CreateAcid());

            Ice = new Skills.Ice();
            Ice.SetSkillObject(gameFactory.CreateIce());
            
            ReloadSkills();
        }
        
        private Units.Base SpawnUnit(PoolObjectType poolObjectType, Vector3 launchPos, Quaternion rotation) => 
            _objectPool.GetObject(poolObjectType, launchPos, rotation).GetComponent<Units.Base>();

        public void ApplySkill(Vector3 position)
        {
            if (SelectedSkillName!=SkillName.None)
            {
                Skills.ISkill skill= ChooseSkill();
                skill.ExecuteForPlayer(position);
                SelectedSkillName = SkillName.None;
            }
        }

        public void ReloadSkills()
        {
            float upgradeCoefficient = _progressService.PlayerProgress.GetAchievedUpgrade(UpgradeType.Rain).upgradeCoefficient;
            Acid.Reload(_scriptableService.PlayersAcid, upgradeCoefficient);
            Buff.Reload(_scriptableService.PlayersBuff, 1.0f);
            Call.Reload(_scriptableService.PlayersCall, 1.0f);
            Ice.Reload(_scriptableService.PlayersIce, 1.0f);
        }

        public void RefreshSkills()
        {
            Acid.Refresh();
            Buff.Refresh();
            Call.Refresh();
            Ice.Refresh();
        }

        public void SetSelectedSkill(int index) =>
            SelectedSkillName = (SkillName) index;

        public void ClearSelectedSkill() =>
            SelectedSkillName = SkillName.None;

        private Skills.ISkill ChooseSkill()
        {
            return SelectedSkillName switch
            {
                SkillName.Buff => Buff,
                SkillName.Acid => Acid,
                SkillName.Ice  => Ice,
                SkillName.Call => Call,
                SkillName.None => null,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}