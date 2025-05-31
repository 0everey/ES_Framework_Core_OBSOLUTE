using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;

namespace ES
{
    //纳米状态，一定单层，一定单运行，不一定是RunTimeLogic
    public interface IESNanoState :IESOriginalModule
    {
        public bool HasPrepared { get; set; }//因为OnEnable不意味着状态真的进入了捏

        #region 设计层面 准备-更新-退出 (忽略Enter和PowerOff，因为他们通常不会以接口名义调用)
        void OnStatePrepare();
        void OnStateUpdate();
        void OnStateExit();
        #endregion

        #region 键
        void SetKey(object key);
        object GetKey();
        string QuickKey();
        #endregion
    }
    //微型状态，可能多层，但是不准并行,只能活动在当前层级，通常手写代码，一定是RunTimeLogic
    public interface IESMicroState : IESNanoState
    {
        IESMicroState AsThis { get; set; }//自身状态--如果是状态机的话，这个有用的
        
     

        #region 自身状态--以Prepared和Exit为界限
        public bool CheckThisStateCanUpdating { get; }//可以自定义是否要更新

        #endregion

        #region 固有
        IStateSharedData SharedData { get; set; } //共享数据
        IStateVariableData VariableData { get; set; } //变化数据
        #endregion

        [LabelText("标准状态")] public EnumStateRunningStatus RunningStatus { get; }//微型就有了
    }
    //标准状态，一般多层，完全支持并行，会参与总状态机活动，一定是RunTimeLogic,可以随时注册和注销，可以和Buff系统联动,可以通过数据工具配置信息和一键生成
    public interface IESStandardState : IESMicroState
    {
        
    }

    //状态机可被认为是一个标准状态,来允许无论任何情况下都能作为子状态- 
    //纳米状态机-只能同时有一个状态
    public interface IESNanoStateMachine :IESStandardState,IESOringinHosting
    {
       IESNanoState SelfRunningState { get; set; }
       
    }
    //微型状态机-能有多个状态，但是只能发生在当前层
    public interface IESMicroStateMachine : IESNanoStateMachine
    {
        IEnumerable<IESMicroState> SelfRunningStates { get; }
        /*void _TryAddInSelfRunningStates(IESMicroState ESMicro);
        void _TryRemoveInSelfRunningStates(IESMicroState ESMicro);*/
    }
    //标准状态机-全局计算合并与冲突，并且向上提交到Main
    public interface IESStandardStateMachine : IESMicroStateMachine
    {
        public HashSet<IESStandardState> RootMainRunningStates { get; }//根部的运行
        /*void _TryAddInRootMainRunningStates(IESStandardState ESMicro);
        void _TryRemoveInRootMainRunningStates(IESStandardState ESMicro);*/
    }

 


    //状态自主生命周期--微型数据开始才有
    public enum EnumStateRunningStatus
    {
        [InspectorName("从未启动")] Never,
        [InspectorName("准备中")] StatePrepared, //OnStatePrepared=>触发=》一般不重要---自动再Enter
        [InspectorName("运行时")] StateUpdate,  //OnStateEnter=>触发
        [InspectorName("熄火中")] StatePowerOff, //OnStatePowerOff=>触发=》一般不重要---自动再Exit
        [InspectorName("已退出")] StateExit //OnStateExit=>触发
    }
    //状态专属的共享数据--微型数据开始才有
    public interface IStateSharedData : ISharedData
    {
        int Order { get; }
        bool CanBeHit { get; }
        bool CanHit { get; }
        string[] BeHitWithoutCondition { get; }
        Enum Channel {get;}
        
    }
    //状态专属的变化数据--微型数据开始才有
    public interface IStateVariableData : IVariableData
    {

    }
}
