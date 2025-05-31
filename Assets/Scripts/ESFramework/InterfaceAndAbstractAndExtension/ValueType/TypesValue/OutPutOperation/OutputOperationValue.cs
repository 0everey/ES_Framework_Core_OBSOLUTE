using Sirenix.OdinInspector;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ES
{
    [Serializable]
    public abstract class OutputOpeationValue<On, From, With, ValueType, WithSelector> 
    {

        /*[LabelText("操作类型")]*/
        public abstract string desThis { get; }
        public abstract WithSelector SelectType { get; }
        /*[LabelText("最终操作")]*/
        public abstract ValueType DefaultOperation { get; }
        public virtual bool TryOpeation(On on, From from, With with)
        {
           return OnOpeation(on,from,with, DefaultOperation, SelectType);
        }
        public virtual bool TryCancel(On on, From from, With with) { return OnCancel(on, from, with, DefaultOperation, SelectType);}
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public abstract bool OnOpeation(On on, From from, With with,ValueType Opeation_,WithSelector SelectType_);
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public abstract bool OnCancel(On on, From from, With with, ValueType Opeation_, WithSelector SelectType_);
    }
    public abstract class OutputOpeationValueOnEntityFromEntityWithBuff<ValueType, WithSelector> : OutputOpeationValue<Entity, Entity, EntityState_Buff, ValueType, WithSelector>
    {
        
    }
    [Serializable]
    public abstract class OutputOpeationValueOnEntityFromEntityWithBuffFloatValue : OutputOpeationValueOnEntityFromEntityWithBuff<float, OperationHandleTypeForFloat>, IOutputOpeationBuff
    {
        public override OperationHandleTypeForFloat SelectType => selectType;
        
        [LabelText("操作类型")] public OperationHandleTypeForFloat selectType = OperationHandleTypeForFloat.Add;
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public float EasyOperation(ref float or,float Opeation)
        {
            return or=KeyValueMatchingUtility.Function.OpearationFloat_Inline(or, Opeation,selectType);
        }
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public float EasyCancel(ref float or, float Opeation)
        {
            return or = KeyValueMatchingUtility.Function.OpearationFloat_Inline(or, Opeation, selectType);
        }
    }

    [Serializable,TypeRegistryItem("Buff操作值-使用数值导向")]
    public abstract class OutputOpeationValueBuff_Target : OutputOpeationValueOnEntityFromEntityWithBuffFloatValue
    {
        [LabelText("导向目标值"), SerializeReference]
        public TargetOpeationValueOnEntityFromEntityWithBuffFloatValue target = null;
        public override string desThis => "通过导向值来完成值修改-而不需要再自定义";
        public abstract float GetOperationValue(Entity on, Entity from, EntityState_Buff with);
        
        public sealed override bool TryOpeation(Entity on, Entity from, EntityState_Buff with)
        {
            if (target != null) return target.OnOpeation(on,from,with, GetOperationValue(on, from, with), selectType);
            return OnOpeation(on, from, with, GetOperationValue(on, from, with), selectType);
        }
        public sealed override bool TryCancel(Entity on, Entity from, EntityState_Buff with)
        {
            if (target != null) return target.OnCancel(on, from, with, GetOperationValue(on, from, with), selectType);
            return OnCancel(on, from, with, GetOperationValue(on, from, with), selectType);
        }
        public sealed override float DefaultOperation => GetOperationValue(null,null,null);
        public override bool OnOpeation(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            //啥也做不了
            return false;
        }

        public override bool OnCancel(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            //啥也做不了
            return false;
        }
        
    }

    [Serializable, TypeRegistryItem("Buff操作值-使用数值导向-直接确定值")]
    public class OutputOpeationValueBuff_Target_Direct : OutputOpeationValueBuff_Target
    {
        [LabelText("作用值")] public float operation = 1;
        public override float GetOperationValue(Entity on, Entity from, EntityState_Buff with)
        {
            return operation;
        }
    }
    [Serializable, TypeRegistryItem("Buff操作值-使用数值导向-升级偏移值")]
    public class OutputOpeationValueBuff_Target_LevelUp : OutputOpeationValueBuff_Target
    {
        [LabelText("基值")] public float floor = 1;
        [LabelText("每级偏移值")] public float offsetPerLevel = 0.1f;
        public override float GetOperationValue(Entity on, Entity from, EntityState_Buff with)
        {
            return floor+(with.Level-1)*offsetPerLevel;
        }
    }
    [Serializable, TypeRegistryItem("Buff操作值-使用数值导向-自定义函数(不支持)")]
    public class OutputOpeationValueBuff_Target_SelfDefine : OutputOpeationValueBuff_Target
    {
        [LabelText("作用值")] public float operation = 1;
        public override float GetOperationValue(Entity on, Entity from, EntityState_Buff with)
        {
            return operation;
        }
    }


    [Serializable]//直接应用值
    public abstract class OutputOpeationValueOnEntityFromEntityWithBuffFloatValue_Direc : OutputOpeationValueOnEntityFromEntityWithBuffFloatValue
    {
        public override float DefaultOperation => operation;
        [LabelText("最终操作")] public float operation = 0;
    }
    [Serializable]//按升级的(常用)
    public abstract class OutputOpeationValueOnEntityFromEntityWithBuffFloatValue_LevelUp : OutputOpeationValueOnEntityFromEntityWithBuffFloatValue
    {
        public override float DefaultOperation => floor;
        [LabelText("基准")] public float floor = 0;
        [LabelText("每级偏移")] public float offsetPerLevel = 0;
        public sealed override bool TryOpeation(Entity on_, Entity from, EntityState_Buff with)
        {
            return OnOpeation(on_, from, with, floor + with.Level * offsetPerLevel, selectType);
        }
        public sealed override bool TryCancel(Entity on_, Entity from, EntityState_Buff with)
        {
            return OnCancel(on_, from, with, floor + with.Level * offsetPerLevel, selectType);
        }
    }

    [Serializable/*Buff更新专用操作*/]
    public abstract class OutputOpeationBuffDo : OutputOpeationValueOnEntityFromEntityWithBuffFloatValue_LevelUp
    {

    }
    [Serializable, TypeRegistryItem("Buff更新-(老版)测试-血量变动")]
    public class OutputOpeationBuffUpdate_Test_HelathSub : OutputOpeationBuffDo
    {
        [ShowInInspector,LabelText("描述"), PropertyOrder(-1)]
        public override string desThis => "测试Buff更新效果，用于扣除血量";
        public override bool OnOpeation(Entity on, Entity from, EntityState_Buff with,float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            if (on == null) return false;
            EasyOperation(ref on.VariableData.Health, Opeation_);
            return true;
        }
        public override bool OnCancel(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            return true;
            //血量还退还？想得美！！
            /* if (on == null) return false;

             EasyCancel(ref on.VariableData.Health, Opeation_);
             return true;*/
        }
    }
    [Serializable, TypeRegistryItem("Buff更新-(老版)测试-生成")]
    public class OutputOpeationBuffUpdate_Test_ : OutputOpeationBuffDo
    {
        [ShowInInspector, LabelText("描述"),PropertyOrder(-1)]
        public override string desThis => "测试Buff更新效果，用于生成物体,数值可能是概率或者生成的偏移，也可能wuyiyiw";

        [LabelText("生成的预制件")]
        public GameObject prefab;
        
        public override bool OnOpeation(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            if (on == null) return false;
            ESSpawnMaster.Instance.Ins(prefab,on.transform.position+Vector3.up,null);
            return true;
        }
        public override bool OnCancel(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            //生成预制件退还？也不是不行---自己找去吧
            return true;
        }
    }
}

