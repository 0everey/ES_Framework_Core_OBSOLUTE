using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace ES {
    public interface ITargetOperation<On, From, With, ValueType, WithSelector>
    {
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public abstract bool OnOpeation(On on, From from, With with, ValueType Opeation_, WithSelector SelectType_);
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public abstract bool OnCancel(On on, From from, With with, ValueType Opeation_, WithSelector SelectType_);
    }
    public abstract class OutputOpeationValueOnEntityFromEntityFloatValue : ITargetOperation<Entity, Entity, object, float, OperationHandleTypeForFloat>
    {
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public abstract bool OnOpeation(Entity on, Entity from, object with, float Opeation_, OperationHandleTypeForFloat SelectType_);
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public abstract bool OnCancel(Entity on, Entity from, object with, float Opeation_, OperationHandleTypeForFloat SelectType_);
        }
    public abstract class TargetOpeationValueOnEntityFromEntityWithBuffFloatValue : OutputOpeationValueOnEntityFromEntityFloatValue, ITargetOperation<Entity, Entity, EntityState_Buff, float, OperationHandleTypeForFloat>
    {
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public abstract bool OnOpeation(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_);
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public abstract bool OnCancel(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_);
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public void EasyOperation(ref float or, float Opeation, OperationHandleTypeForFloat SelectType_=OperationHandleTypeForFloat.Add)
        {
            or = KeyValueMatchingUtility.Function.OpearationFloat_Inline(or, Opeation, SelectType_);
        }
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public void EasyCancel(ref float or, float Opeation, OperationHandleTypeForFloat SelectType_=OperationHandleTypeForFloat.Add)
        {
            or = KeyValueMatchingUtility.Function.OpearationFloat_Inline(or, Opeation, SelectType_);
        }
        #region 同源
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public sealed override bool OnOpeation(Entity on, Entity from, object with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            return OnOpeation(on,from,with as EntityState_Buff,Opeation_,SelectType_);
        }
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public sealed override bool OnCancel(Entity on, Entity from, object with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            return OnCancel(on, from, with as EntityState_Buff, Opeation_, SelectType_);
        }
        #endregion
    }
    [Serializable, TypeRegistryItem("血量修改导向")]
    public class Target_Helath : TargetOpeationValueOnEntityFromEntityWithBuffFloatValue
    {
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public override bool OnOpeation(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            EasyOperation(ref on.VariableData.Health,Opeation_,SelectType_);
            return true;
        }
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public override bool OnCancel(Entity on, Entity from, EntityState_Buff with, float Opeation_, OperationHandleTypeForFloat SelectType_)
        {
            EasyCancel(ref on.VariableData.Health, Opeation_, SelectType_);
            return true;
        }
    }



}
