using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ES
{

    public abstract class OutputOperationBuffer<On, From, With,ValueType,WithSelector>
    {
        
        public abstract bool TryOpeation(On on, From from, With with);
        public abstract bool TryCancel(On on, From from, With with);

    }
    public abstract class OutputOpeationBufferOnEntityFromEntityWithBuff<ValueType,WithSelector> : OutputOperationBuffer<Entity, Entity, EntityState_Buff, ValueType, WithSelector>
    {

    }
    [Serializable]
    public abstract class OutputOpeationBufferOnEntityFromEntityWithBuffFloatValue : OutputOpeationBufferOnEntityFromEntityWithBuff<float, OpeationHanldeTypeForBuffer>, IOutputOpeationBuff
    {
        

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public float EasyOperation(ref float or, float Opeation)
        {
            return or = KeyValueMatchingUtility.Function.OpearationFloat_Inline(or, Opeation, OperationHandleTypeForFloat.Add);
        }
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public float EasyCancel(ref float or, float Opeation)
        {
            return or = KeyValueMatchingUtility.Function.OpearationFloat_Inline(or, Opeation, OperationHandleTypeForFloat.Add);
        }
    }
    [Serializable]
    public abstract class OutputOperationBuffBuffer : OutputOpeationBufferOnEntityFromEntityWithBuffFloatValue
    {
        [LabelText("缓冲参数")]
        public BufferDataFloat bufferData;
        public sealed override bool TryOpeation(Entity on, Entity from, EntityState_Buff with)
        {
            var use = MakeTheOpeation(on, from, with);
            with.CacheBuffers.AddElement(this, use);
            return true;
        }
        public sealed override bool TryCancel(Entity on, Entity from, EntityState_Buff with)
        {
            if (with.CacheBuffers.IOC.TryGetValue(this, out var use))
            {
                with.CacheBuffers.IOC.Remove(this);
                return true;
            }
            return true;
        }
        protected virtual ValueBufferOperationFloat MakeTheOpeation( Entity on, Entity from, EntityState_Buff with)
        {
            var use = ESValueMaster.Instance.floatBufferOpsPool.GetInPool();
            use.timeHasGo = 0;
            use.bufferData = bufferData;
            return use;
        }
        public abstract void TryUpdateTheBuffer(ValueBufferOperationFloat buffer,Entity on, Entity from, EntityState_Buff with);
        public void TryStopBuffer(ValueBufferOperationFloat buffer, EntityState_Buff with)
        {
            with.CacheBuffers.ToDelete.Enqueue((this,buffer));
        }
    }
    [Serializable, TypeRegistryItem("Buff缓冲值-使用数值导向")]
    public  class OutputOperationBufferBuff_Target : OutputOperationBuffBuffer
    {
        [LabelText("导向目标值"), SerializeReference]
        public TargetOpeationValueOnEntityFromEntityWithBuffFloatValue target = null;

        public override void TryUpdateTheBuffer(ValueBufferOperationFloat buffer, Entity on, Entity from, EntityState_Buff with)
        {
            float pre = buffer.timeHasGo / bufferData.needTime;
            buffer.timeHasGo += Time.deltaTime;
            float now=buffer.timeHasGo / bufferData.needTime;
            if (now > 1)
            {
                TryStopBuffer(buffer,with);
                return;
            }
            float agoValue = bufferData.curve.Evaluate(pre);
            float afterValue = bufferData.curve.Evaluate(now);
            float offset = (afterValue - agoValue)*bufferData.mutipler;
            target.OnOpeation(on,from,with,offset, OperationHandleTypeForFloat.Add);
            
        }
    }
}

