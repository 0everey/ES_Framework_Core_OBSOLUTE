using DG.Tweening;
using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ES.ClipStateMachine_CrashDodge;
using static UnityEngine.EventSystems.EventTrigger;

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
        [FoldoutGroup("技能专属")][LabelText("技能片段序列")] public Queue<ReleaseableSkillClip> SkillClips;
        [FoldoutGroup("技能专属")][LabelText("上一个片段获得的实体列表")] public List<Entity> LastClipSelectorEntites = new List<Entity>();
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲实体")] public List<Entity> SelfCacheSkills = new List<Entity>();
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲坐标")] public List<Vector3> SelfCacheVector3 = new List<Vector3>();
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲模块")] public List<ESModule_WithDelegate> SelfModule = new List<ESModule_WithDelegate>();
        [FoldoutGroup("技能专属")][LabelText("退出委托")] public Action<float> OnExit=(f)=> { };//float--已经开始的时间
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
            Debug.Log("进入");
            OnExit = (f) => { };
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
                        next=SkillClips.Dequeue();//出列！
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
            Debug.Log("Heelpw"+Sequence.AllClips.Count);
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
                    Debug.Log(clip.name+clip.Applier.handles.Count+ handle);
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

    [Serializable,TypeRegistryItem("实体移动状态")]
    public class EntityState_Move : EntityState
    {
        private ClipBase_3DMotion motion;
        private float HasIn =0;
        protected override void RunStatePreparedLogic()
        {
            base.RunStatePreparedLogic();
            motion = Entity.BaseDomain.Module_3DMotion;
            HasIn = 0;
        }
        protected override void RunStateUpdateLogic()
        {
            base.RunStateUpdateLogic();
            HasIn += Time.deltaTime;
            if (HasIn > 0.1f)
            {
                if (Mathf.Abs(motion.CurrentSpeedMutiplerZ) < 0.05 && Mathf.Abs(motion.CurrentSpeedMutiplerX) < 0.05)
                {
                    OnStateExit();
                }
            }
        }
        protected override void RunStateExitLogic()
        {
            base.RunStateExitLogic();
            motion.TargetSpeedMutiplerX = motion.TargetSpeedMutiplerZ = 0;
        }
    }

    [Serializable, TypeRegistryItem("实体闪身状态")]
    public class EntityState_CrashDodge : EntityState
    {
        private ClipBase_3DMotion motion;
        [NonSerialized]public ClipStateMachine_CrashDodge crashDodge;
        [NonSerialized]public Applyable_CrashDodge data;
        private float HasIn = 0;
        private Vector3 startPos;
        private Quaternion startDirec;
        private Vector3 applyVector;
        private Tween withTween=null;
        
        public void Setup(ClipStateMachine_CrashDodge _CrashDodge)
        {
            crashDodge = _CrashDodge;
        }
        public void SetData(ref Applyable_CrashDodge data_)
        {
            data = data_;
        }
        protected override void RunStatePreparedLogic()
        {
            base.RunStatePreparedLogic();
            motion = Entity.BaseDomain.Module_3DMotion;
            HasIn = 0;
            startPos = Entity.transform.position;
            startDirec = Entity.transform.rotation;
            applyVector = Entity.transform.TransformDirection(data.vector);
            data.duration = Mathf.Max(0.1f, data.duration);
            if(data.baseOn== EnumCollect.ToDestionationBaseOn.DotweenDoMove)
            {
                if(data.pathType== EnumCollect.ToDestinationPath.Direct)
                {
                    if(data.vectorHandle== EnumCollect.ToDestinationVectorSpace.Target)
                    {
                        Debug.Log("aa"+data.vector+data.duration);
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration);
                    }else if(data.vectorHandle== EnumCollect.ToDestinationVectorSpace.WorldSpace)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration).SetRelative();
                    }else if(data.vectorHandle== EnumCollect.ToDestinationVectorSpace.SelfSpace)
                    {
                        withTween = Entity.Rigid.DOMove(applyVector, data.duration).SetRelative();
                    }
                }
                else if (data.pathType == EnumCollect.ToDestinationPath.JumpAndDown)
                {
                    if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.Target)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration);
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.WorldSpace)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration).SetRelative();
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.SelfSpace)
                    {
                        withTween = Entity.Rigid.DOMove(Entity.transform.TransformDirection(data.vector), data.duration);
                    }
                }
                else if (data.pathType == EnumCollect.ToDestinationPath.Rad)
                {
                    if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.Target)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration);
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.WorldSpace)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration).SetRelative();
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.SelfSpace)
                    {
                        withTween = Entity.Rigid.DOMove(Entity.transform.TransformDirection(data.vector), data.duration);
                    }
                }
                else if (data.pathType == EnumCollect.ToDestinationPath.AIPath)
                {
                    if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.Target)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration);
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.WorldSpace)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration).SetRelative();
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.SelfSpace)
                    {
                        withTween = Entity.Rigid.DOMove(Entity.transform.TransformDirection(data.vector), data.duration);
                    }
                }
            }
        }
        protected override void RunStateUpdateLogic()
        {
            base.RunStateUpdateLogic();
            //超时
            HasIn += Time.deltaTime;
            if (HasIn > data.duration||HasIn>1)
            {
                OnStateExit();
            }
            if (data.baseOn == EnumCollect.ToDestionationBaseOn.ESCurveModule)
            {
                if (data.pathType == EnumCollect.ToDestinationPath.Direct)
                {
                    if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.Target)
                    {
                        Entity.Rigid.position = Vector3.MoveTowards(Entity.Rigid.position, data.vector, crashDodge.MaxSpeed);
                        if (Vector3.Distance(Entity.Rigid.position, data.vector) < crashDodge.EndDisSuit)
                        {
                            OnStateExit();
                        }
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.WorldSpace)
                    {
                        Entity.Rigid.position += data.vector * Time.deltaTime / data.duration;
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.SelfSpace)
                    {
                        Entity.Rigid.position += (applyVector) * Time.deltaTime / data.duration;
                    }
                }
                else if (data.pathType == EnumCollect.ToDestinationPath.JumpAndDown)
                {
                    if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.Target)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration);
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.WorldSpace)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration).SetRelative();
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.SelfSpace)
                    {
                        withTween = Entity.Rigid.DOMove(Entity.transform.TransformDirection(data.vector), data.duration);
                    }
                }
                else if (data.pathType == EnumCollect.ToDestinationPath.Rad)
                {
                    if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.Target)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration);
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.WorldSpace)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration).SetRelative();
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.SelfSpace)
                    {
                        withTween = Entity.Rigid.DOMove(Entity.transform.TransformDirection(data.vector), data.duration);
                    }
                }
                else if (data.pathType == EnumCollect.ToDestinationPath.AIPath)
                {
                    if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.Target)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration);
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.WorldSpace)
                    {
                        withTween = Entity.Rigid.DOMove(data.vector, data.duration).SetRelative();
                    }
                    else if (data.vectorHandle == EnumCollect.ToDestinationVectorSpace.SelfSpace)
                    {
                        withTween = Entity.Rigid.DOMove(Entity.transform.TransformDirection(data.vector), data.duration);
                    }
                }
            }

        }

        protected override void RunStateExitLogic()
        {
            base.RunStateExitLogic();
            if (withTween != null)
            {
                withTween.Kill();
            }
            crashDodge.CoolDownNext = Mathf.Min(data.CoolDownNext, crashDodge.CoolDownNext);
        }
    }
}

