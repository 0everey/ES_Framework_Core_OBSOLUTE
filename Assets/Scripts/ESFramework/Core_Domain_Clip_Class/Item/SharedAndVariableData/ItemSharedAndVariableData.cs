using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES {
    #region 原始定义
    [Serializable, TypeRegistryItem("物品共享数据")]
    public class ESItemSharedData : ISharedData
    {
        [LabelText("物品显示名称")] public string itemDisplayName = "";
        [LabelText("特性表")] public List<string> characters = new List<string>();
        [LabelText("绑定物体的预制件"), Required] public GameObject bindingEnemyPrefab;
        
        
    }


    [Serializable, TypeRegistryItem("物品变量数据")]
    public class ESItemVariableData : IVariableData
    {
        
        public void Init(params object[] ps)
        {
            
        }
    }

    #endregion

    #region 飞行物专属
    [Serializable, TypeRegistryItem("物品：飞行物共享数据")]
    public class ESItem_FlyingSharedData : ESItemSharedData
    {
        [LabelText("默认伤害")]public float damage=20;
        [LabelText("默认速度")] public float speed = 8;
        [LabelText("生命")]public float missileLife_=8;
        [LabelText("最大碰撞次数")] public int maxTimes = 1;
        [LabelText("可用触发效果")] public EntityHandleOfFlyingItem entityHandleOfItem;
    }


    #endregion
}
