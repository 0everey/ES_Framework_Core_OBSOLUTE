using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public abstract class OutputOperationSettle<On, From, With, ValueType_>
    {
        public abstract bool TryOpeation(On on, From from, With with);
        public abstract bool TryCancel(On on, From from, With with);
    }
    [Serializable]
    public abstract class OutputOpeationSettleOnEntityFromEntityWithBuff_Float : OutputOperationSettle<Entity, Entity, EntityState_Buff, float>, IOutputOpeationBuff
    {

    }
    [Serializable/*Buff结算浮点专用操作*/]
    public abstract class OutputOpeationBuffSettle : OutputOpeationSettleOnEntityFromEntityWithBuff_Float
    {
        [LabelText("强制不渐变")] public bool ForceNormal = false;
        [LabelText("结算类型")] public OperationHandleTypeForFloat settleType = OperationHandleTypeForFloat.Add;
        [LabelText("效果类型")] public SettleSelfTypeForFloat selfType =  SettleSelfTypeForFloat.None;
        [LabelText("优先级")] public int Priority = 0;
        [LabelText("基值")] public float baseValue = 1;
        [LabelText("每级偏移")] public float offsetPerLevel = 0.1f;
        public sealed override bool TryOpeation(Entity on, Entity from, EntityState_Buff with)
        {
            var settle = GetSettlement(on,from,with);
            var operation = GetEnableSettle(on, from, with);
            operation.TryStartHandle(settle);
            return true;
        }
        public sealed override bool TryCancel(Entity on, Entity from, EntityState_Buff with)
        {
            var settle = GetSettlement(on, from, with);
            var operation = GetDisableSettle(on, from, with);
            operation.TryStopHandle(settle);
            return true;
        }
        public void MakeDirty_Output(Entity on, Entity from, EntityState_Buff with)
        {
            if( with.CacheSettles.TryGetValue(this,out var use))
            {
                use.Value = baseValue + with.Level * offsetPerLevel;
                var settle = GetSettlement(on, from, with);
                settle.MakeDirty();
            }
        }
        public abstract SettlementFloat GetSettlement(Entity on, Entity from, EntityState_Buff with);
        public SettleOperationFloat GetEnableSettle(Entity on, Entity from, EntityState_Buff with)
        {
            var use = MakeTheOpeation(on, from, with);
            with.CacheSettles.Add(this, use);
            return use;
        }
        public SettleOperationFloat GetDisableSettle(Entity on, Entity from, EntityState_Buff with)
        {
            if (with.CacheSettles.TryGetValue(this, out var use))
            {
                with.CacheSettles.Remove(this);
                return use;
            }
            return default;
        }
        protected virtual SettleOperationFloat MakeTheOpeation(Entity on, Entity from, EntityState_Buff with) {
           var use= ESValueMaster.Instance.floatSettleOpsPool.GetInPool();
            use.selfType = selfType;
            use.settleType = settleType;
            use.Priority = Priority;
            use.Value = baseValue + with.Level*offsetPerLevel;
            return use;
        }

    }

    [Serializable,TypeRegistryItem("Buff结算-测试-攻击力结算")]
    public class OutputOpeationBuffSettle_Test_Attack : OutputOpeationBuffSettle
    {
        public override SettlementFloat GetSettlement(Entity on, Entity from, EntityState_Buff with)
        {
            return on.VariableData.Attack;
        }
    }
}

