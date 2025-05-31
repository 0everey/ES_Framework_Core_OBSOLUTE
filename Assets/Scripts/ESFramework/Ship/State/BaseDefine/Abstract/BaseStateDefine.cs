using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ES
{
    //写一个纳米的
    [Serializable,TypeRegistryItem("抽象泛型元-原始状态_纳米")]
    public abstract class BaseESNanoState<Key_> : BaseESModule<BaseOriginalStateMachine>,IESNanoState
    {
        public abstract string QuickKey();
        #region 托管至
        [NonSerialized]
        public BaseOriginalStateMachine host;
        public override BaseOriginalStateMachine GetHost => host;
        #endregion

        #region 状态生命周期
        public bool HasPrepared { get; set; }

        public void OnStatePrepare()
        {
            if (HasPrepared) return;
            HasPrepared = true;//直接进入准备

            host._SelfRunningState = this;
            RunStatePreparedLogic();
        }

        public void OnStateUpdate()
        {
            RunStateUpdateLogic();
        }

        public void OnStateExit()
        {
            if (!HasPrepared) return;
            HasPrepared = false;//直接退出准备

            if (host._SelfRunningState == this)
            {
                host._SelfRunningState = null;
            }
            RunStateExitLogic();
        }
        #endregion

        #region 键
        [LabelText("辨识键", SdfIconType.Key), FoldoutGroup("固有")] public Key_ key;
        public Key_ ThisKey => key;
        public void SetKey(object _key)
        {
            if(_key is Key_ key_)
            {
                this.key = key_;
            }else if (_key != null)
            {
                this.key = KeyValueMatchingUtility.Matcher.SystemObjectToT<Key_>(_key);
            }
        }

        public object GetKey()
        {
            return key;
        }
        #endregion

        #region 应用层重写逻辑
      

        protected virtual void RunStatePreparedLogic()
        {
            //默认的
        }
        protected virtual void RunStateExitLogic()
        {

        }

        protected virtual void RunStateUpdateLogic()
        {

        }
        #endregion
        protected override void OnSubmitHosting(BaseOriginalStateMachine host)
        {
            this.host = host;
             base.OnSubmitHosting(host);
        }
    }
    [Serializable, TypeRegistryItem("抽象泛型元-带委托状态_纳米")]
    public abstract class BaseESNanoStateWithDelegate<Key_,This> : BaseESNanoState<Key_> where This: BaseESNanoStateWithDelegate<Key_, This>
    {
        #region 委托
        [FoldoutGroup("默认委托")] private Action<This> Action_OnPrepared;
        [FoldoutGroup("默认委托")] private Action<This> Action_OnExit;
        [FoldoutGroup("默认委托")] private Action<This> Action_Update;
        [FoldoutGroup("默认委托")] private Action<This> Action_OnStateUpdate;
        #endregion

        #region 重写的逻辑
        protected override void RunStateExitLogic()
        {
            Action_OnExit?.Invoke(this as This);
            base.RunStateExitLogic();
        }
        protected override void RunStatePreparedLogic()
        {
            Action_OnPrepared?.Invoke(this as This);
            base.RunStatePreparedLogic();
        }
        protected override void RunStateUpdateLogic()
        {
            Action_OnStateUpdate?.Invoke(this as This);
            base.RunStateUpdateLogic();
        }
        protected override void Update()
        {
            Action_Update?.Invoke(this as This);
            base.Update();
        }
        #endregion

        #region 携带
        public This WithStateExit(Action<This> action)
        {
            Action_OnExit = action;
            return this as This;
        }
        public This WithStatePrepared(Action<This> action)
        {
            Action_OnPrepared = action;
            return this as This;
        }
        public This WithUpdate(Action<This> action)
        {
            Action_Update = action;
            return this as This;
        }
        public This WithStateUpdate(Action<This> action)
        {
            Action_OnStateUpdate = action;
            return this as This;
        }
        #endregion
    }
    [Serializable, TypeRegistryItem("泛型键纳米状态(String)")]
    public class ESNanoStateWithDelegate<Key_> : BaseESNanoStateWithDelegate<Key_, ESNanoStateWithDelegate<Key_>>
    {
        public override string QuickKey()
        {
            return key.ToString();
        }
    }
    [Serializable,TypeRegistryItem("纳米状态(String)")]
    public class ESNanoStateWithDelegate_StringKey: BaseESNanoStateWithDelegate<string, ESNanoStateWithDelegate_StringKey>
    {
        public override string QuickKey()
        {
            return key;
        }
    }



    //微型开始的
    #region 原始状态定义
    [Serializable, TypeRegistryItem("抽象元_原始状态_微型开始_纳米路边一条")]
    public abstract class BaseStateRunTimeLogic<Key, SharedData_, VariableData_, This> : BaseRunTimeLogic<BaseOriginalStateMachine, Key, SharedData_, VariableData_>, IESMicroState
       where This : BaseStateRunTimeLogic<Key, SharedData_, VariableData_, This>
       where SharedData_ : IStateSharedData
       where VariableData_ : IStateVariableData
    {
        #region 杂货
        //检查是否可更新状态，没啥大用也
        public bool CheckThisStateCanUpdating
        {
            get
            {
                if (host != null)
                {
                    if (host is IESMicroState state && !state.CheckThisStateCanUpdating) return false;
                    else if (host is ESMicroStateMachine_StringKey mS)return mS._SelfRunningStates?.TryContains(this) ?? false;
                }
                return false;
            }
        }
        //状态的AsThis就是This哈，没啥大用
        public IESMicroState AsThis { get => this; set => Debug.LogWarning("对于运行时状态来说,AsThis就是自己,赋值无效"); }
        public abstract void SetKey(object key);//设置键
        public abstract object GetKey();//获得键
        public abstract string QuickKey();
        #endregion

        #region 自己状态与信息的声明
        //键
        [LabelText("辨识键", SdfIconType.Key), FoldoutGroup("固有")] public Key key;
        public override Key ThisKey => key;
        //自运行状态
        public EnumStateRunningStatus RunningStatus => _RunningStatus;
        [LabelText("状态运行情况", SdfIconType.Stack), FoldoutGroup("固有")]
        public EnumStateRunningStatus _RunningStatus = EnumStateRunningStatus.Never;
        #endregion

        #region 父子关系与保持的运行

        //标准状态才有哈
        /*[NonSerialized] public HashSet<IESMicroState> RootMainRunningStates;//当场声明引用来优化性能*/


        #endregion

        #region 生命周期_状态专属的
        [ShowInInspector,LabelText("状态进入状态"),ReadOnly,PropertyOrder(-1), FoldoutGroup("<汇总>运行情况")]
        public bool HasPrepared { get; set; }
        IStateSharedData IESMicroState.SharedData { get => sharedData; set { if (value is SharedData_ sharedValue) sharedData = sharedValue; else Debug.LogWarning("不匹配"+value+typeof( SharedData_)); } }
        IStateVariableData IESMicroState.VariableData { get => variableData; set { if (value is VariableData_ variableValue) variableData = variableValue; else Debug.LogWarning("不匹配" + value); } }
        public IESMicroState WithSharedData(IStateSharedData sharedData)
        {
            if(sharedData is SharedData_ use)
            {
                this.SharedData = use;
            }
            return this;
        }
        public IESMicroState WithVariableData(IStateVariableData variableData)
        {
            if (variableData is VariableData_ use)
            {
                this.VariableData = use;
            }
            return this;
        }
        //准备哈
        public void OnStatePrepare()
        {
            if (HasPrepared) return;
            HasPrepared = true;//直接进入准备
            if (host != null)
            {
                /*RootMainRunningStates ??= host.Root.MainRunningStates;
                Debug.Log("测试host");
                if (!RootMainRunningStates.Contains(this))
                {
                    RootMainRunningStates.Add(this);
                }*/
                /*host._SelfRunningStates.TryAdd(this);*/
                _Expand_PreparedHappenDesign();
            }
            //默认是直接开始的

            _RunningStatus = EnumStateRunningStatus.StatePrepared;//情形更新

            RunStatePreparedLogic();//运行准备时的重写逻辑
        }
        //进入哈
        protected void OnStateEnter()
        {
            if (_RunningStatus == EnumStateRunningStatus.StateUpdate) return;

            _RunningStatus = EnumStateRunningStatus.StateUpdate;//情形更新

            RunStateEnterLogic();//运行进入时的重写逻辑
        }
        //退出哈
        public void OnStateExit()
        {
            if (!HasPrepared) return;
            HasPrepared = false;//直接退出准备

            if (host != null)
            {
                _Expand_ExitHappenDesign();
             /*   if (RootMainRunningStates.Contains(this))
                {
                    RootMainRunningStates.Remove(this);
                }
                host._SelfRunningStates.TryRemove(this);*/
            }

            _RunningStatus = EnumStateRunningStatus.StateExit;
            RunStateExitLogic();
        }
        //运行哈
        public void OnStateUpdate()
        {
            RunStateUpdateLogic();
        }
        #endregion

        #region 生命周期_作为模块的
       
        #endregion

        #region 设计层扩展重写逻辑
        protected virtual void _Expand_PreparedHappenDesign()
        {
            host._SelfRunningState = this;
            if(host is ESMicroStateMachine_StringKey machine)
            {
                machine._SelfRunningStates.TryAdd(this);
            }
        }
        protected virtual void _Expand_ExitHappenDesign()
        {
            if (host._SelfRunningState == this)
            {
                host._SelfRunningState = null;
            }
            if (host is ESMicroStateMachine_StringKey machine)
            {
                machine._SelfRunningStates.TryRemove(this);
            }
        }
        #endregion

        #region 应用层重写逻辑

        protected virtual void RunStateEnterLogic()
        {

        }

        protected virtual void RunStateExitLogic()
        {

        }

        protected virtual void RunStatePreparedLogic()
        {
            //默认的
            OnStateEnter();//默认情况
        }

        protected virtual void RunStateUpdateLogic()
        {

        }
        #endregion
    }

    #endregion

    #region 定义特殊类型：委托
    [Serializable, TypeRegistryItem("抽象元_带委托状态")]
    public abstract class BaseWithableDelegateStateRunTimeLogic<Machine, Key, SharedData, Status, This> : BaseStateRunTimeLogic<Key, SharedData, Status, This>
           where This : BaseWithableDelegateStateRunTimeLogic<Machine, Key, SharedData, Status, This>
             where Machine : BaseOriginalStateMachine
            where SharedData : IStateSharedData
            where Status : IStateVariableData
    {
        [FoldoutGroup("默认委托")] private Action<This> Action_OnPrepared;
        [FoldoutGroup("默认委托")] private Action<This> Action_OnEnter;
        [FoldoutGroup("默认委托")] private Action<This> Action_OnExit;
        [FoldoutGroup("默认委托")] private Action<This> Action_OnStateUpdate;
        protected override void RunStateEnterLogic()
        {
            Action_OnEnter?.Invoke(this as This);
            base.RunStateEnterLogic();
        }
        protected override void RunStateExitLogic()
        {
            Action_OnExit?.Invoke(this as This);
            base.RunStateExitLogic();
        }
        protected override void RunStatePreparedLogic()
        {
            Action_OnPrepared?.Invoke(this as This);
            base.RunStatePreparedLogic();
        }
        protected override void RunStateUpdateLogic()
        {
            Action_OnStateUpdate?.Invoke(this as This);
            base.RunStateUpdateLogic();
        }
       
        public This WithStateEnter(Action<This> action)
        {
            Action_OnEnter = action;
            return this as This;
        }
        public This WithStateExit(Action<This> action)
        {
            Action_OnExit = action;
            return this as This;
        }
        public This WithStatePrepared(Action<This> action)
        {
            Action_OnPrepared = action;
            return this as This;
        }
        
        public This WithStateUpdate(Action<This> action)
        {
            Action_OnStateUpdate = action;
            return this as This;
        }

    }
    #endregion

    #region 微型状态
    [Serializable, TypeRegistryItem("抽象泛型-微型继承状态")]
    public abstract class BaseESMicroStateOverrideRunTimeLogic<Key, This> : BaseStateRunTimeLogic<Key, ESMicroStateSharedData, ESMicroStateStatus, This>
            where This : BaseESMicroStateOverrideRunTimeLogic<Key, This>
    {

    }
    [Serializable, TypeRegistryItem("抽象泛型-微型可委托状态")]
    public abstract class BaseESMicroStateWithDelegateRunTimeLogic<Key, This> : BaseWithableDelegateStateRunTimeLogic<BaseOriginalStateMachine, Key, ESMicroStateSharedData, ESMicroStateStatus, This>
            where This : BaseESMicroStateWithDelegateRunTimeLogic<Key, This>
    {

    }

    [Serializable, TypeRegistryItem("微型继承状态(String)")]
    public class BaseESMicroStateOverrideRunTimeLogic_StringKey : BaseESMicroStateOverrideRunTimeLogic<string, BaseESMicroStateOverrideRunTimeLogic_StringKey>
    {
        public override string QuickKey()
        {
            return key;
        }
        public override object GetKey()
        {
            return this.key;
        }

        public override void SetKey(object key)
        {
            this.key = key.ToString();
        }
    }
    [Serializable, TypeRegistryItem("微型可委托状态(String)")]
    public class BaseESMicroStateWithDelegateRunTimeLogic_StringKey : BaseESMicroStateWithDelegateRunTimeLogic<string,BaseESMicroStateWithDelegateRunTimeLogic_StringKey>
    {
        public override string QuickKey()
        {
            return key;
        }
        public override object GetKey()
        {
            return this.key;
        }

        public override void SetKey(object key)
        {
            this.key = key.ToString();
        }
    }
    #endregion

    #region 标准状态
    [Serializable, TypeRegistryItem("抽象泛型-标准继承状态")]
    public abstract class BaseESStandardStateOverrideRunTimeLogic<Key, This> : BaseStateRunTimeLogic<Key, ESStandardStateSharedData, ESStandardStateVariableData, This>, IESStandardState
           
            where This : BaseESStandardStateOverrideRunTimeLogic<Key,This>
    {
        [NonSerialized] public HashSet<IESStandardState> RootMainRunningStates;
        protected sealed override void _Expand_PreparedHappenDesign()
        {
            base._Expand_PreparedHappenDesign();
            //标准状态机
            if (host is ESStandardStateMachine_StringKey && host.Root is ESStandardStateMachine_StringKey ESStandardRoot)
            {
                RootMainRunningStates ??= ESStandardRoot.MainRunningStates;
                if (!RootMainRunningStates.Contains(this))
                {
                    RootMainRunningStates.Add(this);
                }
            }
        }
        protected sealed override void _Expand_ExitHappenDesign()
        {
            base._Expand_ExitHappenDesign();
            //标准状态机
            if (host is ESStandardStateMachine_StringKey && host.Root is ESStandardStateMachine_StringKey ESStandardRoot)
            {
                RootMainRunningStates ??= ESStandardRoot.MainRunningStates;
                if (RootMainRunningStates.Contains(this))
                {
                    RootMainRunningStates.Remove(this);
                }
            }
        }
    }
    [Serializable, TypeRegistryItem("抽象泛型-标准可委托状态")]
    public abstract class BaseESStandardStateWithDelegateRunTimeLogic<Key, This> : BaseWithableDelegateStateRunTimeLogic<BaseOriginalStateMachine, Key, ESStandardStateSharedData, ESStandardStateVariableData, This>, IESStandardState
            
            where This : BaseESStandardStateWithDelegateRunTimeLogic<Key, This>
    {
        [NonSerialized] public HashSet<IESStandardState> RootMainRunningStates;
        protected sealed override void _Expand_PreparedHappenDesign()
        {
            base._Expand_PreparedHappenDesign();
            //标准状态机
            if (host is ESStandardStateMachine_StringKey && host.Root is ESStandardStateMachine_StringKey ESStandardRoot)
            {
                RootMainRunningStates ??= ESStandardRoot.MainRunningStates;
                if (!RootMainRunningStates.Contains(this))
                {
                    RootMainRunningStates.Add(this);
                }
            }
        }
        protected sealed override void _Expand_ExitHappenDesign()
        {
            base._Expand_ExitHappenDesign();
            //标准状态机
            if (host is ESStandardStateMachine_StringKey && host.Root is ESStandardStateMachine_StringKey ESStandardRoot)
            {
                RootMainRunningStates ??= ESStandardRoot.MainRunningStates;
                if (RootMainRunningStates.Contains(this))
                {
                    RootMainRunningStates.Remove(this);
                }
            }
        }
    }
    [Serializable, TypeRegistryItem("标准状态元(String)")]
    public class BaseESStandardStateRunTimeLogic_StringKey : BaseESStandardStateOverrideRunTimeLogic<string, BaseESStandardStateRunTimeLogic_StringKey>
    {
        public override string QuickKey()
        {
            return key;
        }
        public override object GetKey()
        {
            return this.key;
        }

        public override void SetKey(object key)
        {
            this.key = key.ToString();
        }
#if UNITY_EDITOR
        protected override void RunStateUpdateLogic()
        {
            base.RunStateUpdateLogic();
        }
#endif
    }

    [Serializable, TypeRegistryItem("标准可委托可继承状态元(String)")]
    public class BaseWithableESStandardStateRunTimeLogic : BaseESStandardStateWithDelegateRunTimeLogic<string, BaseWithableESStandardStateRunTimeLogic>
    {
        public override string QuickKey()
        {
            return key;
        }
        public override object GetKey()
        {
            return this.key;
        }
        protected override void RunStateUpdateLogic()
        {
            base.RunStateUpdateLogic();
           
        }
        public override void SetKey(object key)
        {
            this.key = key.ToString();
        }
    }

#endregion
}



