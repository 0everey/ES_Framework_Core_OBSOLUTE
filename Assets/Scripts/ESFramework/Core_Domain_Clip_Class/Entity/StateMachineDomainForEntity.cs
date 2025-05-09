using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ES
{
    public class StateMachineDomainForEntity : BaseDomain<Entity, StateMachineClipForDomainForEntity>,IWithESMachine
    {
        #region 固有
        [LabelText("标准角色总状态机")]public EntityStateMachine StateMachine=new EntityStateMachine();

        #region 为了性能节约，让状态机自己携带着转

        #region 初始化
        [LabelText("初始化注册状态数据")]
        public StateDataPack RegisterPack;


        #endregion
        #region 特殊属性
        public BaseOriginalStateMachine TheMachine => StateMachine;

        #endregion
        protected override void OnEnable()
        {
            StateMachine._TryActiveAndEnable();
            base.OnEnable();
        }
        protected override void Update()
        {
            base.Update();
            StateMachine.TryUpdate();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            StateMachine._TryInActiveAndDisable();
        }


        #endregion

        #endregion
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.StateMachineDomain = this;

            StateMachine.CreateRelationShip(core,this);//重建实体状态机
            KeyValueMatchingUtility.DataApply.ApplyStatePackToMachine(RegisterPack, StateMachine);
        }
    }
    [Serializable]
    public abstract class StateMachineClipForDomainForEntity : Clip<Entity, StateMachineDomainForEntity>
    {
        
    }
    [Serializable,TypeRegistryItem("技能注册器")]
    public class ClipStateMachine_SkillRegister : StateMachineClipForDomainForEntity
    {
        [LabelText("注册技能的Info")]
        public List<SkillDataInfo> skillDataInfos = new List<SkillDataInfo>();
        protected override void OnSubmitHosting(StateMachineDomainForEntity hosting)
        {
            base.OnSubmitHosting(hosting);
            foreach(var i in skillDataInfos)
            {
                if (i == null) continue;
                ReleasableSkillsSequence skillsSequence = i.sequence;
                string Key = i.key.Key();
                var Create = KeyValueMatchingUtility.Creator.CreateStateRunTimeLogicComplete(skillsSequence.bindingStateInfo);
                if(Create is EntityState_Skill skill)
                {
                    skill.Setup(skillsSequence);
                }
                Domain.StateMachine.RegisterNewState(Key, Create);
                
            }
        }
    }


}
