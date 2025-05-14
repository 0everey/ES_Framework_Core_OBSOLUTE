using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class ActorDataInfo : SoDataInfo,IWithSharedAndVariableData<ESEntitySharedData, ESEntityVariableData>
    {
        [LabelText("实体共享数据")]
        public ESEntitySharedData entitySharedData;

        [LabelText("实体变量数据")]
        public ESEntityVariableData entityVariableData;

        public ESEntitySharedData SharedData { get => entitySharedData; set => entitySharedData=value; }
        public ESEntityVariableData VariableData { get => entityVariableData; set => entityVariableData=value; }
    }
}
