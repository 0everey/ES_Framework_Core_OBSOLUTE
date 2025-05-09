using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    [Serializable, TypeRegistryItem("实体标准状态机")]
    public class EntityStateMachine : ESStandardStateMachine_StringKey
    {
        [HideInInspector] public Entity entity;
        [HideInInspector] public StateMachineDomainForEntity StateDomain;

        public void CreateRelationShip(Entity e, StateMachineDomainForEntity stateMachineDomain)
        {
            if (e != null)
            {
                entity = e;
                StateDomain = stateMachineDomain;
                defaultStateKey = "静止";
                //
            }
        }
        protected override void Update()
        {
            base.Update();

        }
        protected override void OnEnable()
        {
            base.OnEnable();
           
        }
        protected override void OnDisable()
        {
            base.OnDisable();
           
        }
        protected override void OnStateEnter()
        {
            base.OnStateEnter();
           
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
          
        }
    }

    [Serializable, TypeRegistryItem("实体标准常见状态")]
    public class EntityState : BaseESStandardStateRunTimeLogic_StringKey
    {
        [NonSerialized]
        public EntityStateMachine TheEntityStateMachine;
        [NonSerialized]public Entity Entity;
        protected override void RunStatePreparedLogic()
        {
            TheEntityStateMachine = host as EntityStateMachine;
            Entity = TheEntityStateMachine?.StateDomain.core;
            base.RunStatePreparedLogic();
        }
    }
    [Serializable, TypeRegistryItem("实体标准技能状态")]
    public class EntityState_Skill : EntityState
    {
        #region 技能数据
        [NonSerialized]
        public ReleasableSkillsSequence Sequence;
        [FoldoutGroup("技能专属")][LabelText("技能总时间")] public float skillAllTime = 0;
        [FoldoutGroup("技能专属")][LabelText("技能片段序列")] public Queue<ReleaseableSkillClip> SkillClips;
        [FoldoutGroup("技能专属")][LabelText("上一个片段获得的实体列表")] public List<Entity> LastClipSelectorEntites = new List<Entity>();
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲实体")] public List<Entity> SelfCacheSkills = new List<Entity>();
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲坐标")] public List<Vector3> SelfCacheVector3 = new List<Vector3>();
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲模块")] public List<ESModule_WithDelegate> SelfModule = new List<ESModule_WithDelegate>();
        #endregion

        public void Setup(ReleasableSkillsSequence releasable)
        {
            Sequence = releasable;
        }
        //进入逻辑
        protected override void RunStatePreparedLogic()
        {
            TheEntityStateMachine = host as EntityStateMachine;
            base.RunStatePreparedLogic();
            variableData.hasEnterTime = 0;
            if (Sequence == null) return;
            //开始准备
            
            PrivateMethod_PrepareSkillClips();

        }
        //更新逻辑
        protected override void RunStateUpdateLogic()
        {
            base.RunStateUpdateLogic();
            variableData.hasEnterTime += Time.deltaTime;
            if (SkillClips != null)
            {
                if (SkillClips.Count != 0) {
                    var next = SkillClips.Peek();
                    if (next == null) SkillClips.Dequeue();
                    if (variableData.hasEnterTime > next.triggerAtTime)
                    {
                        SkillClips.Dequeue();//出列！
                        PrivateMethod_TriggerSkillClip(next);
                    }
                }
               
            }
            if (variableData.hasEnterTime >= Sequence.skillDuration) OnStateExit();//截止了

        }
        //退出逻辑
        protected override void RunStateExitLogic()
        {
            base.RunStateExitLogic();
            PrivateMethod_ClearCache();
        }


        #region 私有方法
        //-------准备切片
        private void PrivateMethod_PrepareSkillClips()
        {
            //
            SkillClips = new Queue<ReleaseableSkillClip>();
            foreach (var i in Sequence.AllClips)
            {
                SkillClips.Enqueue(i);
            }
            LastClipSelectorEntites = new List<Entity>();//初始化
        }
        //清理缓存
        private void PrivateMethod_ClearCache()
        {
            SelfCacheSkills.Clear();
            SelfCacheVector3.Clear();
            foreach (var i in SelfModule)
            {
                if (i != null)
                {
                    i.TryWithDrawHostingVirtual();
                }
            }
            SelfModule.Clear();
        }
        //使用切片
        private void PrivateMethod_TriggerSkillClip(ReleaseableSkillClip clip)
        {
            //动画器相关

            //开始筛选
            List<Entity> MyEntites = new List<Entity>();
            var overrideOption = clip.optionForOverrideLast;
            //--------直接使用上次的
            if (overrideOption == ReleaseableSkillClip.SelectorOverrideOptions.DirectUse && LastClipSelectorEntites.Count > 0)
            {
                MyEntites = LastClipSelectorEntites;
            }
            //--------跳过头部，再次筛选
            else if (overrideOption == ReleaseableSkillClip.SelectorOverrideOptions.IgnoreHeadAndReSelect && LastClipSelectorEntites.Count > 0)
            {
                if (clip.Selector is SomeEntitySelectorFromSelf chainSelector)
                {
                    MyEntites = chainSelector.PickAfterHead(LastClipSelectorEntites, Entity);
                }
            }
            //--------完全更新
            else
            {
                if (clip.Selector is SomeEntitySelectorFromSelf chainSelector)
                {
                   
                    MyEntites = chainSelector.Pick(Entity, Entity);
                }
            }
          

            //造成效果
            foreach(var e in MyEntites)
            {
                if (e == null) continue;
                foreach(var handle in clip.Applier.handles)
                {
                    handle.Pick(e,Entity,this);
                }
                
            }
            
            //应用到下次
            var nextOption = clip.optionForNext;
            //-------更新
            if (nextOption == ReleaseableSkillClip.SelectorNextOptions.UpdateAll)
            {
                LastClipSelectorEntites = MyEntites;
            }
            //------不关心
            else if (nextOption == ReleaseableSkillClip.SelectorNextOptions.DontEffectNext)
            {
                //不影响
                MyEntites.Clear();
            }
            //----------------添加到
            else if(nextOption== ReleaseableSkillClip.SelectorNextOptions.AddTo)
            {
                LastClipSelectorEntites.AddRange(MyEntites);
                MyEntites.Clear();
            }
            //-------------从中移除
            else if(nextOption== ReleaseableSkillClip.SelectorNextOptions.RemoveFrom)
            {
                foreach(var i in MyEntites)
                {
                    LastClipSelectorEntites.Remove(i);
                }
                MyEntites.Clear();
            }
            //--------------全部清除
            else if(nextOption== ReleaseableSkillClip.SelectorNextOptions.ClearAll)
            {
                LastClipSelectorEntites.Clear();
                MyEntites.Clear();
            }
        }
        #endregion
    }
}

