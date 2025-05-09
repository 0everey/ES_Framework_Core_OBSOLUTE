using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//接口--可更新
namespace ES
{
    public interface IESOriginalModule
    {

    }
    public interface IESOriginalModule<in Host> where Host:IESOringinHosting
    {
        bool OnSubmitHosting(Host host);
    }
    //生命周期接口--纯Host和模块和Host且模块都有
    public interface IESWithLife
    {
        #region 生命周期接口
        bool IsActiveAndEnable { get; set; }
        bool CanUpdating { get;}
        public void TryDisableSelf();
        public void TryEnableSelf();
        public void TryUpdate();
        #endregion
    }
    //可更新的
    public interface IESModule : IESOriginalModule,IESWithLife
    {
        //这个是模块专属哈
        #region 模块专属功能区
        bool enabledSelf { get; set; }
        public void _TryActiveAndEnable();
        public void _TryInActiveAndDisable();
        bool HasSubmit { get; set; }
        bool TrySubmitHosting(IESOringinHosting hosting, bool asVirtual);
        bool TryWithDrawHosting(IESOringinHosting hosting, bool asVirtual);
        #endregion 
    }

    public interface IESModule<Host> : IESModule where Host : class, IESOringinHosting
    {
        #region 托管声明
        Host GetHost { get; }
        bool IESModule.TrySubmitHosting(IESOringinHosting hosting, bool asVirtual)
        {
            return TrySubmitHosting(hosting as Host, asVirtual);
        }
        bool IESModule.TryWithDrawHosting(IESOringinHosting hosting, bool asVirtual)
        {
            return TryWithDrawHosting(hosting as Host, asVirtual);
        }
        public bool TrySubmitHosting(Host hosting, bool asVirtual);
        public bool TryWithDrawHosting(Host hosting, bool asVirtual);
        #endregion
    }

    public abstract class BaseESModule : IESModule
    {
        #region 显示控制状态
        [ShowInInspector,LabelText("控制自身启用状态"),PropertyOrder(-1)] public bool EnabledSelfControl { get => enabledSelf; set { if (value) TryEnableSelf(); else TryDisableSelf();  } }
        [ShowInInspector, LabelText("显示活动状态"),GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")]
        public bool IsActiveAndEnableShow { get => IsActiveAndEnable; }
        #endregion

        #region 重写逻辑
        //启用时逻辑
        public virtual bool CanUpdating => true;
        protected virtual void OnEnable() { IsActiveAndEnable = true; }
        //禁用时逻辑
        protected virtual void OnDisable() { IsActiveAndEnable = false; }
        //更新时逻辑
        protected virtual void Update() { }
        #endregion

        #region 关于开关逻辑与运行状态
        public bool IsActiveAndEnable { get; set; } = false;
        public bool enabledSelf { get; set; } = true;

        public void TryEnableSelf()
        {
            if (enabledSelf) return;
            enabledSelf = true;
        }
        public void TryDisableSelf()
        {
            if (enabledSelf)
            {
                enabledSelf = false;
            }
        }
        public void _TryActiveAndEnable()
        {
            
            if (IsActiveAndEnable || !enabledSelf) return;//不要你
            OnEnable();
            
        }
        public void _TryInActiveAndDisable()
        {
            if (IsActiveAndEnable) {
                OnDisable();
            }
        }
        public void TryUpdate()
        {
            
            if (CanUpdating&&IsActiveAndEnable)
            {
                
                Update();
            }
        }
        #endregion

        #region 关于提交SubMit
        
        public bool HasSubmit { get; set; }
        public bool TrySubmitHosting(IESOringinHosting hosting, bool asVirtual)
        {
            if (HasSubmit) return true;
            if (asVirtual&&hosting is IESHosting hosting1)
            {
                hosting1.VirtualBeHosted.TryAdd(this);
                return HasSubmit = true;
            }
            return HasSubmit = _OnSubmitAsNormal(hosting);
        }
        public bool TryWithDrawHosting(IESOringinHosting hosting, bool asVirtual)
        {
            if (!HasSubmit) return false;
            if (asVirtual)
            {
                return HasSubmit = false;
            }
            return HasSubmit = _OnWithDrawAsNormal(hosting);
        }
        public void TryWithDrawHostingVirtual()
        {
            TryWithDrawHosting(null,true);
        }
        protected virtual bool _OnSubmitAsNormal(IESOringinHosting hosting) { return true; }
        protected virtual bool _OnWithDrawAsNormal(IESOringinHosting hosting) { return false; }
        #endregion
    }
    public abstract class BaseESModule<Host> : BaseESModule, IESModule<Host> where Host : class, IESOringinHosting
    {
        #region 与自己的Host关联
        public virtual Host GetHost { get; }
        public bool TrySubmitHosting(Host hosting, bool asVirtual)
        {
            if (HasSubmit) return true;
            if (asVirtual && hosting is IESHosting hosting1)
            {
                hosting1.VirtualBeHosted.TryAdd(this);
                return HasSubmit = true;
            }
            return HasSubmit = _OnSubmitAsNormal(hosting);
        }

        public bool TryWithDrawHosting(Host hosting, bool asVirtual)
        {
            if (!HasSubmit) return false;
            if (asVirtual)
            {
                return HasSubmit = false;
            }
            return HasSubmit = _OnWithDrawAsNormal(hosting);
        }
        protected sealed override bool _OnSubmitAsNormal(IESOringinHosting hosting)
        {
            OnSubmitHosting(hosting as Host);
            return true;
        }
        protected sealed override bool _OnWithDrawAsNormal(IESOringinHosting hosting)
        {
            OnWithDrawHosting(hosting as Host);
            return false;
        }
        protected virtual void OnSubmitHosting(Host host)
        {
            
        }
        protected virtual void OnWithDrawHosting(Host host)
        {
            
        }
        #endregion
    }
    [Serializable,TypeRegistryItem("ES模块_带委托的","模块")]
    public class ESModule_WithDelegate : BaseESModule
    {
        [FoldoutGroup("默认委托")] private Action<ESModule_WithDelegate> Action_Enable;
        [FoldoutGroup("默认委托")] private Action<ESModule_WithDelegate> Action_Disable;
        [FoldoutGroup("默认委托")] private Action<ESModule_WithDelegate> Action_OnUpdate;
         
        protected sealed override void OnEnable()
        {
            Action_Enable?.Invoke(this);
            base.OnEnable();
            
        }
        protected sealed override void OnDisable()
        {
            Action_Disable?.Invoke(this);
            base.OnDisable();
            
        }
        protected sealed override void Update()
        {
            Action_OnUpdate?.Invoke(this);
            base.Update();
        }
        [Tooltip("规定启用时的事件")]
        public ESModule_WithDelegate WithEnable(Action<ESModule_WithDelegate> func)
        {
            Action_Enable = func;
            return this;
        }
        [Tooltip("规定禁用时的事件")]
        public ESModule_WithDelegate WithDisable(Action<ESModule_WithDelegate> func)
        {
            Action_Disable = func;
            return this;
        }
        [Tooltip("规定帧运行时的事件")]
        public ESModule_WithDelegate WithUpdate(Action<ESModule_WithDelegate> func)
        {
            Action_OnUpdate = func;
            return this;
        }
    }
}
