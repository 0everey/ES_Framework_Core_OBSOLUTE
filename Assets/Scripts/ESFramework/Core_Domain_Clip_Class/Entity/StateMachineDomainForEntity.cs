using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [LabelText("初始化进入状态名")]
        [ValueDropdown("StatePackKeys", AppendNextDrawer =true)]
        public string DefaultStateName = "静止";
        private string[] StatePackKeys()
        {
            if (RegisterPack != null)
            {
                return RegisterPack.allInfo.Keys.ToArray();
            }
            return new string[] { "先绑定" };
        }
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
            //不判断了
            StateMachine.HardUpdate();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            StateMachine._TryInActiveAndDisable();
        }


        #endregion

        #endregion
        #region 剪影
        [NonSerialized] public ClipStateMachine_CrashDodge Module_CrashDodge;

        #endregion
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.StateMachineDomain = this;

            StateMachine.CreateRelationShip(core,this);//重建实体状态机
            KeyValueMatchingUtility.DataApply.ApplyStatePackToMachine(RegisterPack, StateMachine);
            StateMachine.defaultStateKey = DefaultStateName;
        }
    }
    [Serializable]
    public abstract class StateMachineClipForDomainForEntity : Clip<Entity, StateMachineDomainForEntity>
    {
        
    }
    [Serializable, TypeRegistryItem("技能注册器")]
    public class ClipStateMachine_SkillRegister : StateMachineClipForDomainForEntity
    {
        [LabelText("注册技能的Info")]
        public List<SkillRegisterMessage> skillDataInfos = new List<SkillRegisterMessage>();
        private Dictionary<string, EntityState_Skill> Skills = new Dictionary<string, EntityState_Skill>();
        [ShowInInspector, DisableInEditorMode]
        public string[] Keys => Skills.Keys.ToArray();
        protected override void OnSubmitHosting(StateMachineDomainForEntity hosting)
        {
            base.OnSubmitHosting(hosting);
            foreach (var i in skillDataInfos)
            {
                Debug.Log("t1");
                if (i == null || i.info == null) continue;
                ReleasableSkillsSequence skillsSequence = i.info.sequence;
                Debug.Log("t2");
                string Key = i.info.key.Key();
                var Create = KeyValueMatchingUtility.Creator.CreateStateRunTimeLogicComplete(skillsSequence.bindingStateInfo);
                if (Create is EntityState_Skill skill)
                {
                    Debug.Log("t3");
                    skill.Setup(skillsSequence);
                    string name = i.useRename ? i.rename : Key;
                    Domain.StateMachine.RegisterNewState(name, Create);
                    Skills.Add(name, skill);
                }



            }
        }
    }
    [Serializable, TypeRegistryItem("注册技能交付")]
    public class SkillRegisterMessage : ILink
    {
        [LabelText("启用重命名？")] public bool useRename = true;
        [LabelText("重命名"), ShowIf("useRename")] public string rename = "技能1";
        [LabelText("数据源")] public SkillDataInfo info;
    }
    [Serializable,TypeRegistryItem("3D闪身支持")]
    public class ClipStateMachine_CrashDodge : StateMachineClipForDomainForEntity
    {
        [LabelText("闪身注册源Info")] public StateDataInfo dataInfo;
        [NonSerialized] public EntityState_CrashDodge Refer_Crash;


        [LabelText("最大支持数量")] public int MaxContain = 5;
        [LabelText("最长单个储存时间")] public float MaxContainTimeNext = 1;
        private float lastHasGo = 0;
        [LabelText("实时冷却"),ReadOnly] public float CoolDownNext = 0;
        [LabelText("默认设置：起始前摇")] public float StartPreventApply = 0.15f;
        [LabelText("默认设置：最终距离容差")] public float EndDisSuit = 0.5f;
        [LabelText("默认设置：最大速率")] public float MaxSpeed = 25;
        [LabelText("射线：射线距离")] public float RayDistance = 2;
        [LabelText("射线：被阻止")] public LayerMask WhatPrevent;
        [LabelText("射线：附加效果")] public string HitAddition;

        [LabelText("待执行队列"), ShowInInspector] public Queue<Applyable_CrashDodge> dodgeQueue = new Queue<Applyable_CrashDodge>();
        [Serializable,TypeRegistryItem("可应用的目的位移功能")]
        public struct Applyable_CrashDodge
        {
            [LabelText("持续时间")]public float duration;
            [LabelText("向量")] public Vector3 vector;
            [LabelText("冷却")] public float CoolDownNext;
            [LabelText("向量处理模式")] public EnumCollect.ToDestinationVectorSpace vectorHandle;
            [LabelText("路径处理模式")] public EnumCollect.ToDestinationPath pathType;
            [LabelText("实现模式")] public EnumCollect.ToDestionationBaseOn baseOn;
        }
       
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_CrashDodge = this;
            if (dataInfo != null)
            {
                var Create = KeyValueMatchingUtility.Creator.CreateStateRunTimeLogicComplete(dataInfo);
                if (Create is EntityState_CrashDodge crash)
                {
                    Refer_Crash = crash;
                    crash.Setup(this);
                }
                Domain.StateMachine.RegisterNewState("闪身", Create);
            }
        }
        protected override void OnSubmitHosting(StateMachineDomainForEntity hosting)
        {
            base.OnSubmitHosting(hosting);

        }
        public override void FixedUpdate_MustSelfDelegate()
        {
            base.FixedUpdate_MustSelfDelegate();
            if (dodgeQueue.Count == 0) {
                CoolDownNext = StartPreventApply;
                lastHasGo = 0;
                return;
            }
            CoolDownNext -= Time.fixedDeltaTime;
            lastHasGo += Time.fixedDeltaTime;
            
            if (CoolDownNext<0)
            {
                lastHasGo = 0;
                var apply = dodgeQueue.Dequeue();
                ApplyThis(ref apply);
                
            }
            if (lastHasGo > MaxContainTimeNext)
            {
                lastHasGo = 0;
                dodgeQueue.Dequeue();//直接移除
            }
        }
        private void ApplyThis(ref Applyable_CrashDodge use)
        {
            if (Refer_Crash != null)
            {
                Refer_Crash.SetData(ref use);
                bool b = false;
                if (b =Domain.StateMachine.TryActiveState(Refer_Crash)) {
                    CoolDownNext = 10;
                };
            }

        }
       
        [LabelText("测试输入")]
        public Applyable_CrashDodge test_applyable_Crash = new Applyable_CrashDodge();


        [Button("测试输入")]
        public void _TesetAddByInspector()
        {
            TryAddCrashDodge(ref test_applyable_Crash);
        }
        public void TryAddCrashDodge(ref Applyable_CrashDodge applyable_Crash)
        {
            dodgeQueue.Enqueue(applyable_Crash);
            if (dodgeQueue.Count > MaxContain)
            {
                dodgeQueue.Dequeue();//不要你
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Domain.OnFixedUpdate += FixedUpdate_MustSelfDelegate;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            Domain.OnFixedUpdate -= FixedUpdate_MustSelfDelegate;
        }
    }
    [Serializable, TypeRegistryItem("慢动作支持")]
    public class ClipStateMachine_SlowAction : StateMachineClipForDomainForEntity
    {

    }
    [Serializable, TypeRegistryItem("Buff注册器")]
    public class ClipStateMachine_BuffRefister : StateMachineClipForDomainForEntity
    {
        /*[LabelText("注册技能的Info")]
        public List<SkillRegisterMessage> skillDataInfos = new List<SkillRegisterMessage>();
        private Dictionary<string, EntityState_Skill> Skills = new Dictionary<string, EntityState_Skill>();
        [ShowInInspector, DisableInEditorMode]
        public string[] Keys => Skills.Keys.ToArray();*/
        public BuffSoInfo info;
        protected override void OnSubmitHosting(StateMachineDomainForEntity hosting)
        {
            base.OnSubmitHosting(hosting);
            if (info == null) return;
            var Create = KeyValueMatchingUtility.Creator.CreateStateRunTimeLogicComplete(info.binding);
            if (Create is EntityState_Buff buff)
            {
                buff.soInfo = info;
                Domain.StateMachine.RegisterNewState(info.key.str_direc, Create);
            }
            /*foreach (var i in skillDataInfos)
            {
                Debug.Log("t1");
                if (i == null || i.info == null) continue;
                ReleasableSkillsSequence skillsSequence = i.info.sequence;
                Debug.Log("t2");
                string Key = i.info.key.Key();
                var Create = KeyValueMatchingUtility.Creator.CreateStateRunTimeLogicComplete(skillsSequence.bindingStateInfo);
                if (Create is EntityState_Skill skill)
                {
                    Debug.Log("t3");
                    skill.Setup(skillsSequence);
                    string name = i.useRename ? i.rename : Key;
                    Domain.StateMachine.RegisterNewState(name, Create);
                    Skills.Add(name, skill);
                }



            }*/
        }
    }
}
