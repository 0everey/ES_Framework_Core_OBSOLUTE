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
    public class Entity : BaseCore,IReceiveAnyLink
    {
        [FoldoutGroup("固有"),LabelText("原始动画器")] public Animator Anim;
        [FoldoutGroup("固有"), LabelText("刚体")] public Rigidbody Rigid;
        [FoldoutGroup("固有"), LabelText("ES超级标签")]
        public ESTagCollection ESTagsC = new ESTagCollection();

        
        [FoldoutGroup("扩展域")][LabelText("基本域")]public BaseDomainForEntity BaseDomain;
        [FoldoutGroup("扩展域")][LabelText("标准状态机域")] public StateMachineDomainForEntity StateMachineDomain;
        [FoldoutGroup("扩展域")][LabelText("AI域")] public AIDomainForEntity AIDomain;
        [FoldoutGroup("扩展域")][LabelText("Buff域")] public BuffDomainForEntity BuffDomain;

        public void OnLink(ILink link)
        {
            Debug.Log("Link"+link);
        }

        #region 委托事件


        #endregion
        protected override void BeforeAwakeBroadCastRegester()
        {
            base.BeforeAwakeBroadCastRegester();
            Anim = GetComponent<Animator>();
            Rigid = GetComponent<Rigidbody>();

        }
    }
}
