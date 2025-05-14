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
        
        [FoldoutGroup("固有"),LabelText("原始动画器")] public Animator Anim;
        
        [FoldoutGroup("固有"), LabelText("ES超级标签")]
        public ESTagCollection ESTagsC = new ESTagCollection();

        
        [FoldoutGroup("扩展域")][LabelText("基本域")]public BaseDomainForEntity BaseDomain;
        [FoldoutGroup("扩展域")][LabelText("标准状态机域")] public StateMachineDomainForEntity StateMachineDomain;
        [FoldoutGroup("扩展域")][LabelText("AI域")] public AIDomainForEntity AIDomain;
        [FoldoutGroup("扩展域")][LabelText("Buff域")] public BuffDomainForEntity BuffDomain;

        [FoldoutGroup("属性")]
        [LabelText("实体共享数据(等待引用)")]
        public ActorDataInfo dataInfo;
        [FoldoutGroup("属性")]
        [LabelText("实体共享数据(等待引用)"),SerializeReference]
        public ESEntitySharedData entitySharedData=null;
        [FoldoutGroup("属性")]
        [LabelText("实体变量数据"), SerializeReference]
        public ESEntityVariableData entityVariableData;

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
            Debug.Log("Link"+link);
            
        }
        public void Invoke_BeAttackByEntity(Entity who,Damage damage)
        {
            this.VariableData.Health -= damage.damage;
        }
     
        protected override void BeforeAwakeBroadCastRegester()
        {
            base.BeforeAwakeBroadCastRegester();
            Anim = GetComponent<Animator>();
            

        }

      
    }
}
