using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [CreateAssetMenu(fileName = "独立数据-ES物体生成机制表", menuName = "ES数据/ES物体生成机制表")]
    public class ESObjectTypeSpawnHandle : SerializedScriptableObject
    {
        [LabelText("默认生成选项")]
        public Dictionary<ESSpawnMaster.GameObjectType, ESSpawnMaster.SpawnOption> SpawnType = new Dictionary<ESSpawnMaster.GameObjectType, ESSpawnMaster.SpawnOption>();
        [LabelText("默认生成池化")]
        public Dictionary<ESSpawnMaster.GameObjectType, ESSpawnMaster.PoolOption> PoolType = new Dictionary<ESSpawnMaster.GameObjectType, ESSpawnMaster.PoolOption>();
    }
}

