using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace ES
{
    public interface IESOringinHosting
    {

    }
    //以Hosting声明
    public interface IESHosting : IESOringinHosting, IESWithEnableLife
    {
        #region 托管器专属
        //虚拟的
       /* SafeUpdateSet_EasyQueue_SeriNot_Dirty<IESModule> VirtualBeHosted { get; }*/
        void UpdateAsHosting();
        void EnableAsHosting();
        void DisableAsHosting();
        #endregion
    }
    public abstract class BaseESHosting : IESHosting
    {
        
        #region 实现自定义帧更新
        private short UpdateIntervalFrameCount = -1;
        private short SelfTargetModel = -1;
        public void ResetUpdateIntervalFrameCount(short interval =10)
        {
            UpdateIntervalFrameCount = interval;
            if (UpdateIntervalFrameCount > 0)
            {
                SelfTargetModel = (short)UnityEngine.Random.Range(0, UpdateIntervalFrameCount);
            }
        }
        #endregion

        #region 虚拟列表
       /* public SafeUpdateSet_EasyQueue_SeriNot_Dirty<IESModule> VirtualBeHosted => virtualBeHostedList;
        [LabelText("虚拟托管列表"),FoldoutGroup("虚拟托管")] public SafeUpdateSet_EasyQueue_SeriNot_Dirty<IESModule> virtualBeHostedList = new SafeUpdateSet_EasyQueue_SeriNot_Dirty<IESModule>();
       */ 
       #endregion

        #region 重写逻辑
        public virtual bool CanUpdating => true;

        protected virtual void Update()
        {
            if (UpdateIntervalFrameCount > 0)
            {
                if (SelfTargetModel < 0) ResetUpdateIntervalFrameCount(UpdateIntervalFrameCount);
                if (Time.frameCount % UpdateIntervalFrameCount != SelfTargetModel)
                {
                    return;
                }
            }
            UpdateAsHosting();
            /*virtualBeHostedList.Update();*/
        }
        protected virtual void OnEnable()
        {
            if (UpdateIntervalFrameCount > 0&&SelfTargetModel < 0)
            {
                ResetUpdateIntervalFrameCount(UpdateIntervalFrameCount);
            }
            IsActiveAndEnable = true;
            EnableAsHosting();
        }
        protected virtual void OnDisable()
        {
            IsActiveAndEnable = false;
            DisableAsHosting();
        }
        #endregion

        #region 关于开关逻辑与运行状态
        public bool IsActiveAndEnable { get; set; } = false;

        public virtual void TryEnableSelf()
        {
            if (IsActiveAndEnable) return;
            OnEnable();
        }
        public virtual void TryDisableSelf()
        {
            if (!IsActiveAndEnable)
            {
                OnDisable();
            }
        }
        public void TryUpdate()
        {
            if (CanUpdating && IsActiveAndEnable)
            {
                Update();
            }
        }
        #endregion

        #region 与对子的控制
        public virtual void UpdateAsHosting()
        {
           /* if (VirtualBeHosted != null)
            {
                foreach (var i in VirtualBeHosted.valuesNow_)
                {
                        if (!i._HasSubmit)
                        {
                            i._TryInActiveAndDisable();
                            //已经放弃
                            virtualBeHostedList.TryRemove(i);
                            continue;
                        }
                    i.TryUpdate();
                }
            }*/
        }
        public virtual void EnableAsHosting()
        {
          /*  if (VirtualBeHosted != null)
            {
                foreach (var i in VirtualBeHosted.valuesNow_)
                {
                    i._TryActiveAndEnable();
                }
            }*/
        }
        public virtual void DisableAsHosting()
        {
           
           /* if (VirtualBeHosted != null)
            {
                foreach (var i in VirtualBeHosted.valuesNow_)
                {
                    i._TryInActiveAndDisable();
                }
            }*/
        }
        #endregion
    }
    //以泛型声明
    public interface IESHosting<With> : IESHosting where With : class,IESModule
    {
         IEnumerable<With> NormalBeHosted { get; }
         public abstract void TryRemoveModuleAsNormal(With use);
    }
    public abstract class BaseESHosting<With> : BaseESHosting, IESHosting<With> where With : class, IESModule
    {
        #region 对特定类型的托管支持
        public abstract IEnumerable<With> NormalBeHosted { get; }
        public override void EnableAsHosting()
        {
            if (NormalBeHosted != null)
            {
                foreach (var i in NormalBeHosted)
                {
                    i._TryActiveAndEnable();
                }
            }
            base.EnableAsHosting();
        }
        public override void DisableAsHosting()
        {
            if (NormalBeHosted != null)
            {
                foreach (var i in NormalBeHosted)
                {
                    i._TryInActiveAndDisable();
                }
            }
            base.DisableAsHosting();
        }
        public override void UpdateAsHosting()
        {
            if (NormalBeHosted != null)
            {
                foreach (var i in NormalBeHosted)
                {
                   
                        if (!i._HasSubmit)
                        {
                            i._TryInActiveAndDisable();
                            //已经放弃

                            continue;
                        }
                    i.TryUpdate();
                }
            }
            base.UpdateAsHosting();
        }

        public abstract void TryRemoveModuleAsNormal(With use);
        #endregion
    }
    public abstract class BaseESHostingAndModule<USE, Host> : BaseESHosting<USE>, IESModule<Host> where Host : class, IESHosting where USE : class, IESModule
    {
        #region 与自己的Host关联
        public Host GetHost => host;
        [LabelText("托管核心", SdfIconType.Bullseye)]
        public Host host;
        #endregion

        //启用时逻辑

        #region 关于提交SubMit
        public bool _HasSubmit { get; set; }
        public bool TrySubmitHosting(IESHosting hosting, bool asVirtual = false)
        {
            if (_HasSubmit) return true;
            if (asVirtual)
            {
                /*hosting.VirtualBeHosted.TryAdd(this);*/
                return _HasSubmit = true;
            }
            return _HasSubmit = _OnSubmitAsNormal(hosting);
        }
        public bool TryWithDrawHosting(IESHosting hosting, bool asVirtual = false)
        {
            if (!_HasSubmit) return false;
            if (asVirtual)
            {
                return _HasSubmit = false;
            }
            return _HasSubmit = _OnWithDrawAsNormal(hosting);
        }
        public bool TrySubmitHosting(Host hosting, bool asVirtual = false)
        {
            if (_HasSubmit) return true;
            if (asVirtual)
            {
                /*hosting.VirtualBeHosted.TryAdd(this);*/
                return _HasSubmit = true;
            }
            return _HasSubmit = _OnSubmitAsNormal(hosting);
        }

        public bool TryWithDrawHosting(Host hosting, bool asVirtual = false)
        {
            if (!_HasSubmit) return false;
            if (asVirtual)
            {
                return _HasSubmit = false;
            }
            return _HasSubmit = _OnWithDrawAsNormal(hosting);
        }
        public void TryWithDrawHostingVirtual()
        {
            TryWithDrawHosting(null, true);
        }
        #endregion

        #region 重写提交

        protected virtual bool OnSubmitHostingAsNormal(Host host)
        {
            return true;
        }
        protected virtual bool OnWithDrawHostingAsNormal(Host host)
        {
            return false;
        }
        #endregion

        #region 检查器显示控制与信息
        [ShowInInspector, LabelText("控制自身启用状态"), PropertyOrder(-1)] public bool EnabledSelfControl { get => EnabledSelf; set { if (value) TryEnableSelf(); else TryDisableSelf(); } }
        [ShowInInspector, LabelText("显示活动状态"), PropertyOrder(-1), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForUpdating")]
        public bool IsActiveAndEnableShow { get => IsActiveAndEnable; }
        [ShowInInspector, LabelText("是否已经Submit"),PropertyOrder(-1)] public bool ShowHasSubmit => _HasSubmit;
        #endregion

        #region 开关逻辑
        
        public bool EnabledSelf { get; set; } = true;

        public override void TryEnableSelf()
        {
            if (EnabledSelf) return;
            EnabledSelf = true;
           
        }
        public override void TryDisableSelf()
        {
            if (EnabledSelf)
            {
                EnabledSelf = false;
              
            }
        }
        public void _TryActiveAndEnable()
        {
            if (IsActiveAndEnable || !EnabledSelf) return;//不要你
            OnEnable();

        }
        public void _TryInActiveAndDisable()
        {
            if (IsActiveAndEnable)
            {
                OnDisable();
            }
        }
        #endregion

        #region 实现隐蔽辅助方法
        protected bool _OnSubmitAsNormal(IESHosting hosting)
        {
            return OnSubmitHostingAsNormal(hosting as Host);
        }
        protected  bool _OnWithDrawAsNormal(IESHosting hosting)
        {
            return OnWithDrawHostingAsNormal(hosting as Host);
        }
       
        #endregion




    }
    [Serializable,TypeRegistryItem("不限制模块类型托管器")]
    public class ESHostingAndModule_NoNormalModule<Host> : BaseESHostingAndModule<BaseESModule, Host> where Host : class, IESHosting
    {
        public override IEnumerable<BaseESModule> NormalBeHosted => null;

        public override void TryRemoveModuleAsNormal(BaseESModule use)
        {
           //无事发生
        }
    }
    [Serializable, TypeRegistryItem("ES托管器＋模块_带委托的", "模块")]
    public class ESHostingAndModule_WithDelegate : BaseESHostingAndModule<BaseESModule, BaseESHosting> 
    {
        public override IEnumerable<BaseESModule> NormalBeHosted => null;
        [FoldoutGroup("默认委托")] private Action<ESHostingAndModule_WithDelegate> Action_Enable;
        [FoldoutGroup("默认委托")] private Action<ESHostingAndModule_WithDelegate> Action_Disable;
        [FoldoutGroup("默认委托")] private Action<ESHostingAndModule_WithDelegate> Action_OnUpdate;
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
        public ESHostingAndModule_WithDelegate WithEnable(Action<ESHostingAndModule_WithDelegate> func)
        {
            Action_Enable = func;
            return this;
        }
        [Tooltip("规定禁用时的事件")]
        public ESHostingAndModule_WithDelegate WithDisable(Action<ESHostingAndModule_WithDelegate> func)
        {
            Action_Disable = func;
            return this;
        }
        [Tooltip("规定帧运行时的事件")]
        public ESHostingAndModule_WithDelegate WithUpdate(Action<ESHostingAndModule_WithDelegate> func)
        {
            Action_OnUpdate = func;
            return this;
        }

        public override void TryRemoveModuleAsNormal(BaseESModule use)
        {
            //无事发生
        }
    }

    #region 原始托管器
    public abstract class BaseESOringinalHostingAndModule<Host>:BaseESModule<Host>,IESOringinHosting where Host:class, IESOringinHosting
    {
        #region 与自己的Host关联
        public override Host GetHost => host;
        [LabelText("托管核心", SdfIconType.Bullseye),NonSerialized,ShowInInspector]
        public Host host=null;
        #endregion
    }

    #endregion
    //初级托管器--IMoudle

    //完整托管器--IDelegatedUpdatable


    //下发委托托管器---无条件

    //可委托的初级托管器

    //可委托的完备托管器


    //下发更新托管器--无条件

}
