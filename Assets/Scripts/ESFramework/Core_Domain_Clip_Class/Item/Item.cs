using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
 public class  Item  : ESObject, IWithSharedAndVariableData<ESItemSharedData, ESItemVariableData>
    {
        
        //显性声明扩展域
        public HoldableDomainForItem HoldableDomain;

        public HurtableDomainForItem HurtableDomain;
        //public 02DomainForXXX StateMachineDomain;
        [InfoBox("共享数据和变量数据请尽量在游戏开始前去数据工具修改")]
        [FoldoutGroup("属性")]
        [LabelText("来源SO")]
        public ItemDataInfo dataInfo;
        [FoldoutGroup("属性")]
        [SerializeReference,LabelText("共享数据")]
        public ESItemSharedData sharedData;
        [FoldoutGroup("属性")]
        [SerializeReference, LabelText("变量数据")]
        public ESItemVariableData defaultData;
        [LabelText("启用数据赋予")]
        [FoldoutGroup("属性")] public bool useDataApply = true;
        public ESItemSharedData SharedData { get => sharedData; set => sharedData = value; }
        public ESItemVariableData VariableData { get => defaultData; set => defaultData = value; }
        //注册前的操作
        protected override void BeforeAwakeBroadCastRegester()
        {
            if (dataInfo != null&&useDataApply)
                KeyValueMatchingUtility.DataApply.CopyToClassSameType_WithSharedAndVariableDataCopyTo(dataInfo, this);

            base.BeforeAwakeBroadCastRegester();
        }
}
}
   

