using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class Entity : ESObject, IReceiveAnyLink, IWithSharedAndVariableData<ESEntitySharedData, ESEntityVariableData>


    {

        [FoldoutGroup("固有"), LabelText("原始动画器")] public Animator Anim;

        [FoldoutGroup("固有"), LabelText("ES超级标签")]
        public ESTagCollection ESTagsC = new ESTagCollection();
        public CharacterController CharacterController;
        public float YV = 0;
        public Transform GroundCheck;
        public bool IsGrounded = false;
        [FoldoutGroup("扩展域")][LabelText("基本域")] public BaseDomainForEntity BaseDomain;
        [FoldoutGroup("扩展域")][LabelText("标准状态机域")] public StateMachineDomainForEntity StateMachineDomain;
        [FoldoutGroup("扩展域")][LabelText("AI域")] public AIDomainForEntity AIDomain;
        [FoldoutGroup("扩展域")][LabelText("Buff域")] public BuffDomainForEntity BuffDomain;

        [FoldoutGroup("属性")]
        [LabelText("实体共享数据(等待引用)")]
        public ActorDataInfo dataInfo;
        [FoldoutGroup("属性")]
        [LabelText("实体共享数据(等待引用)"), SerializeReference]
        public ESEntitySharedData entitySharedData = null;
        [FoldoutGroup("属性")]
        [LabelText("实体变量数据"), SerializeReference]
        public ESEntityVariableData entityVariableData;

        [FoldoutGroup("委托"), LabelText("尝试被攻击时")] public Action<Entity, Damage> OnTryBeAttack = (a, b) => { };
        [FoldoutGroup("委托"), LabelText("尝试攻击时")] public Action<Entity, Damage> OnTryAttack = (a, b) => { };
        [FoldoutGroup("委托"), LabelText("真的被攻击时")] public Action<Entity, Damage> OnTruelyBeAttack = (a, b) => { };
        [FoldoutGroup("委托"), LabelText("真的攻击时")] public Action<Entity, Damage> OnTruelyAttack = (a, b) => { };

        public ESEntitySharedData SharedData { get => entitySharedData; set => entitySharedData = value; }
        public ESEntityVariableData VariableData { get => entityVariableData; set => entityVariableData = value; }
        protected override void Awake()
        {
            if (dataInfo != null)
                KeyValueMatchingUtility.DataApply.CopyToClassSameType_WithSharedAndVariableDataCopyTo(dataInfo, this);
            base.Awake();

        }
        public void OnLink(ILink link)
        {


        }
        public void Invoke_TryAttackEntityCalculate(Entity who, Damage damage)
        {
            OnTryAttack?.Invoke(who, damage);
        }
        public void Invoke_BeAttackByEntityCalculate(Entity who, Damage damage)
        {
            Debug.Log("共计");
            OnTryBeAttack?.Invoke(who, damage);
            this.CharacterController.Move(Vector3.up * 0.25f);

        }
        public void Invoke_TrulyBeAttack(Entity who, Damage damage)
        {
            OnTruelyBeAttack?.Invoke(who, damage);
            this.VariableData.Health -= damage.damage;
        }
        public void Invoke_TrulyAttack(Entity who, Damage damage)
        {
            OnTruelyAttack?.Invoke(who, damage);
        }
        protected override void BeforeAwakeBroadCastRegester()
        {
            base.BeforeAwakeBroadCastRegester();
            Anim = GetComponent<Animator>();
            CharacterController = GetComponent<CharacterController>();

        }
        protected override void Update()
        {
            Transform t = transform;

            if (GroundCheck != null)
            {
                t = GroundCheck;
            }
            if (YV <= 0)
            {
                var back = Physics.Raycast(t.position + Vector3.up * 0.25f, Vector3.down, 0.5f, EditorMaster.LayerMaskGround);
               
                if (back)
                {
                    if (YV > -0.1f) YV = 0;
                    else YV = Mathf.Lerp(YV, 0, Time.deltaTime*3);
                    IsGrounded = true;
                }
                else
                {
                    YV -= Time.deltaTime * 9;
                    IsGrounded = false;
                }
            }
            else
            {
                YV -= Time.deltaTime * 9;
            }


            base.Update();

        }

    }
}
