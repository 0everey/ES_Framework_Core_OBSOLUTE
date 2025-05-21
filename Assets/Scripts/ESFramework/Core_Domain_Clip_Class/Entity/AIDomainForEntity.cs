using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [FoldoutGroup("基本")][LabelText("目标")] public Entity Target;
        [FoldoutGroup("基本")][LabelText("目标位置点")] public Vector3 nextWayPointPosition;
        [FoldoutGroup("基本")][LabelText("智能寻路路径")] public NavMeshPath navMeshPath;
        [FoldoutGroup("基本")][LabelText("最终路径")] public Vector3[] EndPathVS = { };
        [FoldoutGroup("基本")][LabelText("最终下一个点")] public Vector3 NextPoint = default;
        [FoldoutGroup("基本"), LabelText("路径Index"), ReadOnly] public int nextWayPointIndex = 0;
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


            if (Target != null)
            {
                nextWayPointPosition = Target.transform.position;
            }
            //输入到移动了
            if (NextPoint == default) NextPoint = nextWayPointPosition;

            Vector3 to = Core.transform.InverseTransformDirection((NextPoint - Core.transform.position).NoY()).normalized;

            Core.BaseDomain.Module_AB_Motion.CurrentSpeedMutiplerZ = to.z;
            Core.BaseDomain.Module_AB_Motion.CurrentSpeedMutiplerX = to.x;

            Core.BaseDomain.Module_AB_Motion.CurrentRotationY = to.x * 100;
            timerForNextPathUpdate -= Time.deltaTime;
            if (timerForNextPathUpdate < 0)
            {
                UpdateAgentDestination();
                timerForNextPathUpdate = 0.3f;
            }

        }
        public void UpdateAgentDestination()
        {

            NavMesh.CalculatePath(Core.transform.position, nextWayPointPosition, 1, navMeshPath);
            EndPathVS = navMeshPath.corners;
            if (EndPathVS.Length == 0) return;
            int Index = 0;
            float dis = 100;
            for (int i = 0; i < EndPathVS.Length; i++)
            {
                float newDis = 0;
                if ((newDis = Vector3.Distance(Core.transform.position, EndPathVS[i])) < dis)
                {
                    dis = newDis;
                    Index = i;
                }
            }
            NextPoint = EndPathVS.Length > Index + 1 ? EndPathVS[Index + 1] : EndPathVS[Index];
        }
    }
    [Serializable, TypeRegistryItem("怪物行为目标")]
    public class ClipAI_EnemyTarget : ClipAI_AB_Target
    {
        [FoldoutGroup("初始化数据"), LabelText("设置路径更新间隔")] public float nextMaxTargetTimeCount = 3;
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
        [NonSerialized, ShowInInspector, ReadOnly]
        public Attackable attackableNow = null;

        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_AB_AttackControl = this;
        }

    }
    [Serializable, TypeRegistryItem("普通怪物攻击调度")]
    public class ClipAI_BaseEnemyAttackControl : ClipAI_AB_EnemyAttackControl
    {
        [FoldoutGroup("技能")][LabelText("技能列表")] public List<string> SkillNames = new List<string>();
        [FoldoutGroup("技能")][LabelText("触发最小间隔")] public float minTimeDis = 6;
        private float timeHas = 5;
        protected override void Update()
        {
            timeHas -= Time.deltaTime;
            if (timeHas < 0)
            {
                timeHas = minTimeDis;
                if (Domain.Module_AB_AITarget.Target != null)
                {
                    string name = SkillNames[UnityEngine.Random.Range(0, SkillNames.Count)];
                    Core.StateMachineDomain.StateMachine.TryActiveStateByKey(name);
                }
            }
            base.Update();

        }
    }
}
