using System;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Managers;
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
        
        private readonly ObjectPool _objectPool;

        public bool IsSkillNotSelected => 
            SelectedSkillName == SkillName.None;
        
        public SkillName SelectedSkillName { get; private set; }

        public SkillController(IGameFactory gameFactory, ObjectPool objectPool)
        {
            ClearSelectedSkill();

            _objectPool = objectPool;
            
            Call = new Skills.Call();
            Call.NeedObjectFromPool += SpawnUnit;
            Call.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.CallResourcePath));
            Call.SetTeamConstraint(Team.Blue);
            Call.SetDecreasingFunction(Altar.DecreaseManaCount);

            Buff = new Skills.Buff();
            Buff.Construct(gameFactory,gameFactory.CreateSkillResource(AssetPaths.BuffResourcePath));
            Buff.SetTeamConstraint(Team.Blue);
            Buff.SetDecreasingFunction(Altar.DecreaseManaCount);

            Acid = new Skills.Acid();
            Acid.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.AcidResourcePath));
            Acid.SetTeamConstraint(Team.Blue);
            Acid.SetDecreasingFunction(Altar.DecreaseManaCount);

            Ice = new Skills.Ice();
            Ice.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.IceResourcePath));
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