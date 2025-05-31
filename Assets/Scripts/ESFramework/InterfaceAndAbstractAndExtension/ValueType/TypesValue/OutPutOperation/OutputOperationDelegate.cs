using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    [Serializable]
    public abstract class OutputOpeationDelegate<On, From, With, Dele>
    {
        public abstract bool TryOpeation(On on, From from, With with);
        public abstract bool TryCancel(On on, From from, With with);

    }
    [Serializable]
    public abstract class OutputOpeationDelegateOnEntityFromEntityWithBuff : OutputOpeationDelegate<Entity, Entity, EntityState_Buff, IOutputOpeationBuff>, IOutputOpeationBuff
    {

    }
    [Serializable/*Buff委托专用操作*/]
    public abstract class OutputOpeationBuffDelegate : OutputOpeationDelegateOnEntityFromEntityWithBuff
    {

    }
    [Serializable]/*扩展类型*/
    public abstract class OutputOpeationBuffDelegateAsExpand<MakeAction> : OutputOpeationBuffDelegate where MakeAction:Delegate
    {
        [SerializeReference, LabelText("总是重制委托")]
        public bool AlwaysRemakeAction = false;
        [SerializeReference, LabelText("事件触发")]
        public IOutputOpeationBuff WhenHanppen;
        [SerializeReference, LabelText("触发次数")]
        public int TriggerNum = 99;
        
        public sealed override bool TryOpeation(Entity on, Entity from, EntityState_Buff with)
        {
            OnEnable(on, from, with);
            return true;
        }
        public sealed override bool TryCancel(Entity on, Entity from, EntityState_Buff with)
        {
            if (with.CacheActions.TryGetValue(this, out var use))
            {
                OnDisable(on, from, with);
                return true;
            }
            return true;
        }
        public abstract void OnEnable(Entity on, Entity from, EntityState_Buff with);
        public abstract void OnDisable(Entity on, Entity from, EntityState_Buff with);
        public MakeAction GetEnableAction(Entity on, Entity from, EntityState_Buff with)
        {
            var use = MakeTheAction(on, from, with);
            with.CacheActions.Add(this,(use, TriggerNum));
            return use;
        }
        public MakeAction GetDisableAction(Entity on, Entity from, EntityState_Buff with)
        {
            if (with.CacheActions.TryGetValue(this, out var use))
            {
                with.CacheActions.Remove(this);
                return use.Item1 as MakeAction;
            }
            return default;
        }
        protected abstract MakeAction MakeTheAction(Entity on, Entity from, EntityState_Buff with);

        public void DefaultDelegateHappen(Entity on, Entity from, EntityState_Buff with)
        {

            WhenHanppen?.TryOpeation(on, from, with);
        }
    }
    [Serializable]/*自定义类型*/
    public abstract class OutputOpeationBuffDelegateSelfDefine : OutputOpeationBuffDelegate
    {
        public sealed override bool TryOpeation(Entity on, Entity from, EntityState_Buff with)
        {
            OnEnable(on, from, with);
            return true;
        }
        public sealed override bool TryCancel(Entity on, Entity from, EntityState_Buff with)
        {
            OnDisable(on, from, with);
            return true;
        }
        public abstract void OnEnable(Entity on, Entity from, EntityState_Buff with);
        public abstract void OnDisable(Entity on, Entity from, EntityState_Buff with);
    }

    [Serializable, TypeRegistryItem("Buff委托-测试-攻击时触发(扩展)")]
    public class OutputOpeationBuffDelegateAsExpand_WhenAttack : OutputOpeationBuffDelegateAsExpand<Action<Entity, Damage>>
    {


        public override void OnEnable(Entity on, Entity from, EntityState_Buff with)
        {
            on.OnTryAttack += GetEnableAction(on, from, with);
        }
        public override void OnDisable(Entity on, Entity from, EntityState_Buff with)
        {
            on.OnTryAttack -= GetDisableAction(on, from, with);
        }
        protected override Action<Entity, Damage> MakeTheAction(Entity on, Entity from, EntityState_Buff with)
        {
            return (a, b) => DefaultDelegateHappen(on, from, with);
        }
    }
}

