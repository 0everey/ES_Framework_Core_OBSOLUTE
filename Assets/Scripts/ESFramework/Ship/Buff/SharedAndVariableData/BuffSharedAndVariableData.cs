using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [Serializable, TypeRegistryItem("实体共享数据")]
    public class ESBuffSharedData : ISharedData
    {
        [InfoBox("如果能缓冲请务必开启，不支持尽量关闭，对性能影响很大")]
        [LabelText("支持缓冲")] public bool EnableBuffer = false;
        [LabelText("启用间隔触发")] public bool EnableTimeDisTrigger = true;
        [FoldoutGroup("间隔触发相关",VisibleIf = "EnableTimeDisTrigger")]
        [FoldoutGroup("间隔触发相关")][LabelText("启动时间")] public float triggerTimeStart = 1;
        [FoldoutGroup("间隔触发相关")][LabelText("触发间隔")] public float triggerTimeDis = 1;
        [FoldoutGroup("间隔触发相关")][LabelText("触发操作"),SerializeReference] public IOutputOpeationBuff opeationForTimeDis;
        [LabelText("启用开关触发")] public bool EnableOnOffTrigger = false;
        [FoldoutGroup("开关触发相关", VisibleIf = "EnableOnOffTrigger")]
        [FoldoutGroup("开关触发相关")]
        [FoldoutGroup("开关触发相关")][LabelText("触发操作"), SerializeReference] public IOutputOpeationBuff opeationForOnOff;

        [FoldoutGroup("显示"), LabelText("Buff图标")] public Sprite icon;
        [FoldoutGroup("显示"), LabelText("Buff好坏")] public EnumCollect.BuffTagForGoodOrBad buffGoodOrBad;
    }


    [Serializable, TypeRegistryItem("实体变量数据")]
    public class ESBuffVariableData : IVariableData
    {
        [LabelText("等级")]public int level = 1;
        [LabelText("剩余时间")]public float timeRemain = 10;
        public void Init(params object[] ps)
        {
            
        }
    }

}

