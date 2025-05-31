using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public enum OperationHandleTypeForFloat
    {
        [InspectorName("加上")] Add,
        [InspectorName("减去")] Sub,
        [InspectorName("百分乘1+X")] PerUp,
        [InspectorName("限制最大")] Max,
        [InspectorName("限制最小")] Min,
        [InspectorName("震荡")] Wave,
    }
    public enum OpeationHanldeTypeForBuffer
    {
        [InspectorName("常规曲线缓冲")]NormalCurve,
        
    }

    [Serializable,TypeRegistryItem("缓冲参数")]
    public class BufferDataFloat
    {
        [LabelText("缓冲曲线")] public AnimationCurve curve = AnimationCurve.Constant(0, 1, 1);
        [LabelText("乘数")] public float mutipler = 1;
        [LabelText("总时间")] public float needTime = 1;
    }
    
}

