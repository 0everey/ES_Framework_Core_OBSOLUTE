using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [ESDisplayNameKeyToType("数据单元", "物品数据单元")]
    public class ItemDataInfo : SoDataInfo, IWithSharedAndVariableData<ESItemSharedData, ESItemVariableData>
    {
        [SerializeReference]
        public ESItemSharedData sharedData;
        [SerializeReference]
        public ESItemVariableData defaultData;

        public ESItemSharedData SharedData { get => sharedData; set => sharedData = value; }
        public ESItemVariableData VariableData { get => defaultData; set => defaultData = value; }
    }
}
