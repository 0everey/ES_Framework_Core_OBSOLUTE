using DG.Tweening;
using DG.Tweening.Core.Easing;
using ES;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
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
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲实体")] public HashSet<Entity> SelfCacheEntites= new HashSet<Entity>();
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲坐标")] public HashSet<Vector3> SelfCacheVector3 = new HashSet<Vector3>();
        [FoldoutGroup("技能专属")][LabelText("自己的缓冲模块")] public List<ESModule_WithDelegate> SelfModule = new List<ESModule_WithDelegate>();
        [FoldoutGroup("技能专属")][LabelText("退出委托")] public Action<float> OnExit=(f)=> { };//float--已经开始的时间
        [FoldoutGroup("技能专属")][LabelText("运行时委托")] public Action<float> OnUpdate = (progress) => { };
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
                        Debug.Log("技能"+SkillClips.Count+TheEntityStateMachine.StateDomain.core.gameObject.name);
                        PrivateMethod_TriggerSkillClip(next);
                    }
                }
               
            }
            OnUpdate?.Invoke(variableData.hasEnterTime/ Sequence.skillDuration);
            if (variableData.hasEnterTime >= Sequence.skillDuration) OnStateExit();//截止了

        }
        //退出逻辑
        protected override void RunStateExitLogic()
        {
            base.RunStateExitLogic();
            PrivateMethod_ClearCache();
            OnExit?.Invoke(variableData.hasEnterTime);
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
            SelfCacheEntites.Clear();
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
                   
                    MyEntites = chainSelector.Pick(Entity, Entity,this);
                }
            }
            if(clip.sortType== EnumCollect.PathSortType.NoneSort)
            {

            }
            else
            {
                MyEntites = MyEntites.Where((w) =>w!=null).ToList();
                MyEntites = KeyValueMatchingUtility.Sorter.SortAny(MyEntites,(f) => f.transform.position,clip.sortType, Entity.transform.position, Entity.transform);
            }


            if (!clip.UseTimeDis)
            {
                //造成效果
                foreach (var e in MyEntites)
                {
                if (e == null) continue;
                
                    foreach (var handle in clip.Applier.handles)
                    {
                       /* Debug.Log(MyEntites.Count + "/" + clip.name + "/" + clip.Applier.handles_.Count + "/" + handle);*/
                        handle.Pick(e, Entity, this);
                    }
                }
            }else
            {
                
                var se = DOTween.Sequence();
                foreach (var e in MyEntites)
                {
                    if (e == null) continue;
                    se.AppendCallback(() => {
                        if(e!=null&&Entity!=null)
                        foreach (var handle in clip.Applier.handles)
                        {
                            Debug.Log(MyEntites.Count + "/" + clip.name + "/" + clip.Applier.handles.Count + "/" + handle);
                            handle.Pick(e, Entity, this);
                        }
                    });
                    se.AppendInterval(Mathf.Max( clip.TriggerTimeDis_?.Pick() ?? 0.2f,0.2f));
                    
                }
                OnExit += (f) => { se.Kill(); };
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
        private ClipBase_3DStandardMotion motion;
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
            motion.Set_TargetVX(0, null);
            motion.Set_TargetVZ(0, null);
            
        }
    }

    [Serializable, TypeRegistryItem("实体闪身状态")]
    public class EntityState_CrashDodge : EntityState
    {
        private ClipBase_3DStandardMotion motion;
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
                        Entity.Rigid.position = Vector3.MoveTowards(Entity.Rigid.position, data.vector, crashDodge.MaxSpeed*Time.deltaTime);
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

    #region AI状态
    [Serializable,TypeRegistryItem("实体AI游荡状态")]
    public class EntityState_AIPatrol : EntityState
    {
        public float nextMaxTargetTimeCount = 3;
        private float nextHasTargetPosTimeCount = 2;
        public float nextSeeTargetTimeCount = 0.5f;
        private float nextHasSeeTargetTimeCount = 2;
        [NonSerialized]public ClipAI_AB_Target ReferModule_Target;
        [NonSerialized] public ClipBase_AB_Vision Refer_Vision;
        public float procedurallyNormalR = 10;
        public EnumCollect.TargetSelectType targetSelectType= EnumCollect.TargetSelectType.ProcedurallyWaypoints;
        [NonSerialized] public ESEntitySharedData ESEntityShared;

        protected override void RunStatePreparedLogic()
        {
            base.RunStatePreparedLogic();
            ReferModule_Target = Entity.AIDomain.Module_AB_AITarget;
            Refer_Vision = Entity.BaseDomain.Module_AB_Vision;
            if (ReferModule_Target == null || Refer_Vision == null) { OnStateExit(); }
            ESEntityShared = Entity.SharedData ?? new ESEntitySharedData();
            Entity.BaseDomain.Module_AB_Motion.StandardSpeed.y = ESEntityShared.PatrolSpeed;
        }
        protected override void RunStateUpdateLogic()
        {
            base.RunStateUpdateLogic();
            if (ReferModule_Target.Target != null)
            {
                TheEntityStateMachine.TryActiveStateByKey("追击状态");
            }
            nextHasTargetPosTimeCount -= Time.deltaTime;
            nextHasSeeTargetTimeCount -= Time.deltaTime;
            if (nextHasSeeTargetTimeCount < 0)
            {
                nextHasSeeTargetTimeCount = nextSeeTargetTimeCount;
                Refer_Vision.TrySee();
                Refer_Vision.MakeSeeAsTarget();
            }
            if ((ReferModule_Target.Agent.remainingDistance <= ReferModule_Target.Agent.stoppingDistance || nextHasTargetPosTimeCount < 0) && !ReferModule_Target.Agent.pathPending)
            {
                nextHasTargetPosTimeCount = UnityEngine.Random.Range(nextMaxTargetTimeCount/2,nextMaxTargetTimeCount);
                if (targetSelectType == EnumCollect.TargetSelectType.Numerically)
                {
                   
                }
                else if (targetSelectType == EnumCollect.TargetSelectType.Random)
                {
                    
                }
                else if (targetSelectType == EnumCollect.TargetSelectType.ProcedurallyWaypoints)
                {
                    ReferModule_Target.nextWayPointPosition = RandomNavmeshLocation(procedurallyNormalR);
                }
            }
            if (targetSelectType == EnumCollect.TargetSelectType.PlayerTarget)
            {

                ReferModule_Target.nextWayPointPosition = GameCenterManager.Instance.transform.parent.position;
            }
        }
        public Vector3 RandomNavmeshLocation(float radius)
        {
            Vector3 randomDirection =UnityEngine.Random.insideUnitSphere * radius;
            randomDirection += Entity.transform.position;

            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;


            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }
    }
    [Serializable, TypeRegistryItem("实体AI追击状态")]
    public class EntityState_AIChase : EntityState
    {
        [NonSerialized] public ClipAI_AB_Target ReferModule_Target;
        [NonSerialized] public ClipAI_AB_EnemyAttackControl ReferModule_Attack;
        [NonSerialized] public ESEntitySharedData ESEntityShared;
        protected override void RunStatePreparedLogic()
        {
            base.RunStatePreparedLogic();
            ReferModule_Target = Entity.AIDomain.Module_AB_AITarget;
            ReferModule_Attack = Entity.AIDomain.Module_AB_AttackControl;
            ESEntityShared = Entity.SharedData ?? new ESEntitySharedData();
            Entity.BaseDomain.Module_AB_Motion.StandardSpeed.y = ESEntityShared.ChaseSpeed;

            if (ReferModule_Target == null) OnStateExit();
        }
        protected override void RunStateUpdateLogic()
        {
            var AG = ReferModule_Target.Agent;
            base.RunStateUpdateLogic();
            AG.isStopped = false;


            float remainDis = AG.remainingDistance;
            float ffNext = 1 / (Entity.entitySharedData.Attacks_.Count + 1);
            ReferModule_Attack.attackableNow = null;
            foreach (var i in Entity.entitySharedData.Attacks_)
            {
                
                if (remainDis < i.AttackRangeDis)
                {
                    ReferModule_Attack.attackableNow = i;
                    if (UnityEngine.Random.value < ffNext * 1.5f)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }else if (remainDis > Entity.entitySharedData.VisionRangeDis)
                {
                    ReferModule_Target.Target = null;
                    TheEntityStateMachine.TryActiveStateByKey("巡逻状态");
                }
                
            }
            if (ReferModule_Attack.attackableNow != null)
            {
                
               bool b=  TheEntityStateMachine.TryActiveStateByKey("攻击状态");
               
            }
        }
    }
    [Serializable, TypeRegistryItem("实体AI警报状态")]
    public class EntityState_AIAlert : EntityState
    {
        private float timerHasGo = 0;
        protected override void RunStateUpdateLogic()
        {
            base.RunStateUpdateLogic();
            timerHasGo += Time.deltaTime;
            //还没写特殊的
            if (timerHasGo >= Entity.entitySharedData.AlertTime)
            {
                timerHasGo = 0;
                //进入游荡
            }
        }
    }
    [Serializable, TypeRegistryItem("实体AI攻击状态")]
    public class EntityState_AIAttack : EntityState
    {
        private float preDelay = 0.25f;
        private float toExit = 1;
        public bool hasAttack = false;
        [NonSerialized] public ClipAI_AB_EnemyAttackControl attackControl;
        protected override void RunStateEnterLogic()
        {
            base.RunStateEnterLogic();
            Entity.BaseDomain.Module_AB_Motion.StandardSpeed.y = 0.2f;
            
            attackControl = Entity.AIDomain.Module_AB_AttackControl;
            hasAttack = false;
            if (attackControl == null||attackControl.attackableNow==null) OnStateExit();
            toExit = attackControl.attackableNow.attackExit_;
            preDelay = attackControl.attackableNow.preDelay;
        }
        protected override void RunStateUpdateLogic()
        {
            base.RunStateUpdateLogic();
            if (hasAttack) { }
            else
            {
                preDelay -= Time.deltaTime;
                if (preDelay < 0)
                {
                    hasAttack = true;
                    attackControl.attackableNow.TryAttackOn(Entity);
                }
            }
            toExit -= Time.deltaTime;
            if (toExit < 0)
            {
                OnStateExit();
            }
        }
        /*   void Attack(bool shot)
           {
               // Melle Attack
               if (shot == false)
               {
                   if (enemy == null) return;
                   enemy.chaseTarget.SendMessage("GetDamage", new DamageClass(enemy.ActualMelleAttack.meleeDamage, DamageType.Melle, enemy.GetComponent<Enemy>()), SendMessageOptions.DontRequireReceiver);
                   enemy.chaseTarget.SendMessage("DamageIndicatorFunction", enemy.transform, SendMessageOptions.DontRequireReceiver);
                   enemy.GetComponent<Enemy>().source.PlayOneShot(enemy.GetComponent<Enemy>().MelleAttackSound[Random.Range(0, enemy.GetComponent<Enemy>().MelleAttackSound.Length)]);
               }
               // Range Attack
               else if (shot == true)
               {
                   Vector3 SpawnPoint = enemy.vision.position;
                   GameObject missile = GameObject.Instantiate(enemy.ActualRangeAttack.missile, SpawnPoint, enemy.vision.rotation);

                   // AddInfo Colliders

                   missile.GetComponent<Missile>().EnemyColli = enemy.transform.GetComponentsInChildren<Collider>();
                   missile.GetComponent<Missile>().Guided = enemy.ActualRangeAttack.AutoAim;
                   missile.GetComponent<Missile>().speed = enemy.ActualRangeAttack.missileSpeed;
                   missile.GetComponent<Missile>().ApplyDamage = enemy.ActualRangeAttack.missileDamage;
                   missile.GetComponent<Missile>().source = enemy.transform;
                   enemy.GetComponent<Enemy>().source.PlayOneShot(enemy.GetComponent<Enemy>().RangeAttackSound[Random.Range(0, enemy.GetComponent<Enemy>().RangeAttackSound.Length)]);
               }
           }
           public void UpdateActions()
           {
               timer += Time.deltaTime;
               float distance = Vector3.Distance(enemy.chaseTarget.transform.position, enemy.transform.position);

               Watch();

               // Chase Detect
               if (enemy.attackType == EnemyAttackType.Melle && distance > enemy.ActualMelleAttack.attackRange)
               {
                   ToChaseState();

               }
               if (enemy.attackType == EnemyAttackType.Range && distance > enemy.ActualRangeAttack.shootRange)
               {
                   ToChaseState();
               }

               float biggerRange = enemy.ActualRangeAttack.shootRange;
               if (biggerRange < enemy.ActualMelleAttack.attackRange) { biggerRange = enemy.ActualMelleAttack.attackRange; }

               if (enemy.attackType == EnemyAttackType.MelleAndRange && distance > biggerRange)
               {
                   ToChaseState();
               }

               // Do Attack

               // When Melle and Range
               if (distance <= enemy.ActualRangeAttack.shootRange && distance > enemy.ActualMelleAttack.attackRange && timer >= enemy.ActualRangeAttack.Delay & enemy.attackType == EnemyAttackType.MelleAndRange)
               {
                   Attack(true);
                   timer = 0;

                   enemy.GetNextRangeAttack();
               }
               // When Only Range
               if (distance <= enemy.ActualRangeAttack.shootRange && timer >= enemy.ActualRangeAttack.Delay & enemy.attackType == EnemyAttackType.Range)
               {
                   Attack(true);
                   timer = 0;

                   enemy.GetNextRangeAttack();
               }
               // When Melle
               if (distance <= enemy.ActualMelleAttack.attackRange && timer >= enemy.ActualMelleAttack.Delay & enemy.attackType != EnemyAttackType.Range)
               {
                   Attack(false);
                   timer = 0;

                   enemy.GetNextMelleAttack();
               }
           }*/
    }

    #endregion
}

