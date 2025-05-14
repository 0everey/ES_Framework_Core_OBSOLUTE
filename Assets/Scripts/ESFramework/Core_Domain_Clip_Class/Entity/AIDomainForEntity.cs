using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

namespace ES
{
    public class AIDomainForEntity : BaseDomain<Entity, BaseAIClipForDomainForEntity>
    {
        [NonSerialized] public ClipAI_AB_Target Module_AB_AITarget;//以抽象声明

        [NonSerialized] public ClipAI_AB_EnemyAttackControl Module_AB_AttackControl;//以抽象声明
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.AIDomain = this;
        }
    }
    [Serializable]
    public abstract class BaseAIClipForDomainForEntity : Clip<Entity, AIDomainForEntity>
    {
    }
    [Serializable]
    public abstract class ClipAI_AB_Target : BaseAIClipForDomainForEntity
    {
        [FoldoutGroup("基本")][LabelText("目标")]public Entity Target;
        [FoldoutGroup("基本")][LabelText("目标位置点")] public Vector3 nextWayPointPosition;
        [FoldoutGroup("基本")][LabelText("智能寻路体")]public NavMeshAgent Agent;
        [FoldoutGroup("基本")][LabelText("智能寻路路径")] public NavMeshPath navMeshPath;
        [FoldoutGroup("基本"),LabelText("路径Index"),ReadOnly]public int nextWayPointIndex = 0;
        [FoldoutGroup("更新")][LabelText("路径更新间隔")] public float PathUpdateTimeDis = 1;

        #region 私有
        private float timerForNextPathUpdate = 0.5f;

        #endregion
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_AB_AITarget = this;
            navMeshPath = new NavMeshPath();
        }
        protected override void Update()
        {
            base.Update();
            if (Agent != null)
            {
                Agent.speed = Core.BaseDomain.Module_AB_Motion.StandardSpeed.magnitude;
                if (Target != null)
                {
                    nextWayPointPosition = Vector3.Lerp(Core.transform.position,Target.transform.position,0.5f);
                }
                timerForNextPathUpdate -= Time.deltaTime;
                if (timerForNextPathUpdate < 0)
                {
                    UpdateAgentDestination();
                    timerForNextPathUpdate = 0.3f;
                }
            }
        }
        public void UpdateAgentDestination()
        {
            Agent.destination = nextWayPointPosition;
        }
    }
    [Serializable,TypeRegistryItem("怪物行为目标")]
    public class ClipAI_EnemyTarget : ClipAI_AB_Target
    {
        [FoldoutGroup("初始化数据"),LabelText("设置路径更新间隔")]public float nextMaxTargetTimeCount = 3;
        [FoldoutGroup("初始化数据"), LabelText("设置程序化生成半径")] public float procedurallyNormalR = 10;
        [FoldoutGroup("初始化数据"), LabelText("设置游荡路径类型")] public EnumCollect.TargetSelectType targetSelectType = EnumCollect.TargetSelectType.ProcedurallyWaypoints;
        [NonSerialized] public EntityState_AIPatrol state_AIPatrol;
        [NonSerialized] public EntityState_AIChase state_AIChase;
        [NonSerialized] public EntityState_AIAlert state_AIAlert;
        [NonSerialized] public EntityState_AIAttack state_AIAttack;

        [FoldoutGroup("更新数据")]
        protected override void Update()
        {
            base.Update();
        }
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            state_AIPatrol = Core.StateMachineDomain.StateMachine.GetStateByKey("游荡状态") as EntityState_AIPatrol;
            state_AIChase = Core.StateMachineDomain.StateMachine.GetStateByKey("追击状态") as EntityState_AIChase;
            state_AIAlert = Core.StateMachineDomain.StateMachine.GetStateByKey("警告状态") as EntityState_AIAlert;
            state_AIAttack = Core.StateMachineDomain.StateMachine.GetStateByKey("攻击状态") as EntityState_AIAttack;
            if (state_AIPatrol != null)
            {
                state_AIPatrol.nextMaxTargetTimeCount = nextMaxTargetTimeCount;
                state_AIPatrol.procedurallyNormalR = procedurallyNormalR;
                state_AIPatrol.targetSelectType = targetSelectType;
            }
        }
    }

    [Serializable]
    public abstract class ClipAI_AB_EnemyAttackControl : BaseAIClipForDomainForEntity
    {
        [NonSerialized,ShowInInspector,ReadOnly]
        public Attackable attackableNow = null;

        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_AB_AttackControl = this;
        }
      
    }
    [Serializable,TypeRegistryItem("普通怪物攻击调度")]
    public class ClipAI_BaseEnemyAttackControl : ClipAI_AB_EnemyAttackControl
    {
        
    }
}
