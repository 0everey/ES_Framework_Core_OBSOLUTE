using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    
    public enum UILayer
    {
        [InspectorName("场景层(血条)-200")]SceneLayer=-200,
        [InspectorName("背景层-100")] Background=-100,
        [InspectorName("常规层(一级界面)0")] Normal=0,
        [InspectorName("信息层(二级界面)100")] Info=100,
        [InspectorName("提示层(状态)200")] Tip=200,
        [InspectorName("顶层(加载)300")] Top=300,
        [InspectorName("警告层400")] Warning=400,
    }

}

