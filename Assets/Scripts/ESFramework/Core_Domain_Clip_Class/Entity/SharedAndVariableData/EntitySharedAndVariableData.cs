using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES {
    [Serializable,TypeRegistryItem("实体共享数据")]
    public class ESEntitySharedData : ISharedData
    {
        [LabelText("怪物名称")] public string enemyName = "";
        [LabelText("特性表")] public List<string> characters = new List<string>();
        [LabelText("绑定怪物的预制件"), Required] public GameObject bindingEnemyPrefab;
        [LabelText("加权计算实力值"), InlineButton("calculateStrength", "计算实力")] public float weightedStrenthValue = 100;
        private void calculateStrength()
        {
            weightedStrenthValue = KeyValueMatchingUtility.Matcher.CalculateActorPropertyStrength(this);
        }
        [Header("属性表")]
        [FoldoutGroup("属性值"), LabelText("生命值")] public float Health = 100;
        
        [FoldoutGroup("属性值"), LabelText("视觉角度范围")] public float VisionRangeAngle = 90;
        [FoldoutGroup("属性值"), LabelText("视觉范围")] public float VisionRangeDis = 20;
        [FoldoutGroup("属性值"), LabelText("游荡速度")] public float PatrolSpeed = 1;
        [FoldoutGroup("属性值"), LabelText("追击速度")] public float ChaseSpeed = 2;
        [FoldoutGroup("属性值"), LabelText("追击距离")] public float ChaseDistance = 100;
        [FoldoutGroup("特殊功能"), LabelText("警报支持-可警报")] public bool UseAlert = false;
        [FoldoutGroup("特殊功能"), LabelText("警报支持-警报时间")] public float AlertTime = 2;
        [FoldoutGroup("特殊功能"), LabelText("警报支持-警报范围")] public float AlertRange = 8;
        [FoldoutGroup("属性值"), LabelText("可攻击列表")]
        [SerializeReference]
        public List<Attackable> Attacks_ = new List<Attackable>() { new Attackable_SimpleEnemy() };
        

    }


    [Serializable, TypeRegistryItem("实体变量数据")]
    public class ESEntityVariableData : IVariableData
    {
        [FoldoutGroup("属性值"), LabelText("当前生命值")] public float Health = 100;
        [FoldoutGroup("属性值"), LabelText("伤害加成(ADD)")] public float DamageAdd = 0;
        [FoldoutGroup("属性值"), LabelText("伤害加成(1+%)")] public float DamagePerUp = 0;
        [FoldoutGroup("属性值"), LabelText("攻击速度加成(1+%)")] public float AttackSpeed = 0;
        [FoldoutGroup("属性值"), LabelText("速度加成(1+%)")] public float SpeedPerUp = 0;
        public void Init(params object[] ps)
        {
            throw new NotImplementedException();
        }
    }
}
