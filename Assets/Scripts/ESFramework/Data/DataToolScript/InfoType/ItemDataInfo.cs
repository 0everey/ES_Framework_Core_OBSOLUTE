using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
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
