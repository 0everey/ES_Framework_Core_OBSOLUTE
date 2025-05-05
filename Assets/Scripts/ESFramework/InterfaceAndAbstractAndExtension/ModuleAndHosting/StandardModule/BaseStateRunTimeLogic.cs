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
    public interface IState:IESModule
    {
        IState AsThis { get; set; }//自身状态
        void SetKey(object key);
        object GetKey();
        IStateSharedData SharedData { get; set; }
        IStateStatus Status { get; set; }
        public bool CheckStateCanUpdating { get; }
        public bool HasPrepared { get; set; }
        IEnumerable<IState> AsCurrentChild { get; }//当前运行子
        void OnStatePrepare();
        void OnStateUpdate();
        void OnStateExit();
        
    }
    public interface IStateMachine : IESHosting
    {

    }
    public enum EnumStateStandCurrentRunStatus
    {
        [InspectorName("从未启动")] Never,
        [InspectorName("准备中")] StatePrepared,
        [InspectorName("运行时")] StateUpdate,
        [InspectorName("熄火中")] StatePowerOff,
        [InspectorName("已退出")] StateExit
    }
    public interface IStateSharedData: ISharedData
    {

    }
    public interface IStateStatus : IStatus
    {
        public EnumStateStandCurrentRunStatus StandStatus{ get; set; }
    }
    [Serializable,TypeRegistryItem("微型状态共享数据")]
    public struct MicroStateSharedData : IStateSharedData
    {
        [LabelText("优先级")]public int order;
        [LabelText("默认处于时间")] public float defaultStayState;
        [LabelText("是否能打断")] public bool canBeHit;
    }
   
    [Serializable, TypeRegistryItem("微型状态运行情况")]
    public struct MicroStateStatus : IStateStatus
    {
        [LabelText("状态开始时间")]public float hasEnterTime;
        [LabelText("标准状态")] public EnumStateStandCurrentRunStatus stateStandStatus;

        public EnumStateStandCurrentRunStatus StandStatus { get => stateStandStatus; set => stateStandStatus=value; }

        public  void Init(params object[] ps)
        {
            
        }
    }
    public abstract class BaseStateRunTimeLogic<Hosting, Key, SharedData, Status, This> : BaseRunTimeLogic<Hosting, Key, SharedData, Status>, IState
        where This : BaseStateRunTimeLogic<Hosting, Key, SharedData, Status, This>
         where Hosting : BaseStateMachine
        where SharedData : IStateSharedData
        where Status : IStateStatus
    {

        public IState AsThis { get => this; set => Debug.Log("对于基准运行时状态来说，就是自己"); }
        public IEnumerable<IState> AsCurrentChild { get { Debug.LogWarning("基准状态没有子状态"); return null; } set => Debug.LogWarning("基准状态没有子状态"); }
        [LabelText("辨识键", SdfIconType.Key), FoldoutGroup("固有")] public Key key;


        public override Key ThisKey => key;
        [NonSerialized] public List<IState> TargetMainRunningStates;

        public bool CheckStateCanUpdating { get {
                if (GetHost != null)
                {
                    if (GetHost is IState state && !state.CheckStateCanUpdating) return false;
                    return GetHost.AsCurrentChild?.Contains(this) ?? false;
                }
                return false;
            }
        }

        IStateSharedData IState.SharedData { get => sharedData; set { if (value is SharedData data) { sharedData = data; } } }

        IStateStatus IState.Status
            {
                get => status; set { if (value is Status st) { status = st; } }
            }

        public bool HasPrepared { get; set; }

        protected void OnStateEnter()
        {
            if (status.StandStatus == EnumStateStandCurrentRunStatus.StateUpdate) return;
            status.StandStatus = EnumStateStandCurrentRunStatus.StateUpdate;
            
            RunStateEnterLogic();
        }
        protected virtual void RunStateEnterLogic()
        {

        }
        public void OnStateExit()
        {
            if (!HasPrepared) return;
            if (host != null)
            {
                if (TargetMainRunningStates.Contains(this))
                {
                    TargetMainRunningStates.Remove(this);
                }
                host.SelfRunningStates.valuesToRemove.Add(this);
            }
            HasPrepared = false;
            status.StandStatus = EnumStateStandCurrentRunStatus.StateExit;
            RunStateExitLogic();
        }
        protected virtual void RunStateExitLogic()
        {

        }
        public void OnStatePrepare()
        {
            if (HasPrepared) return;
            if (host != null)
            {
                TargetMainRunningStates ??= host.Root.MainRunningStates;
                Debug.Log("测试host");
                if (!TargetMainRunningStates.Contains(this))
                {
                    TargetMainRunningStates.Add(this);
                }
                host.SelfRunningStates.valuesToAdd.Add(this);
            }
            //默认是直接开始的
            HasPrepared = true;
            status.StandStatus = EnumStateStandCurrentRunStatus.StatePrepared;
           
            RunStatePreparedLogic();
            
        }
        protected virtual void RunStatePreparedLogic()
        {
            
            OnStateEnter();//默认情况
        }
        public void OnStateUpdate()
        {
            RunStateUpdateLogic();
        }
        protected virtual void RunStateUpdateLogic()
        {
            
        }
        protected override void Update()
        {
            base.Update();
            
           /* if (CheckStateCanUpdating)
            {
                OnStateUpdate();
            }*/
        }
        
        public abstract void SetKey(object key);
        public abstract object GetKey();
    }
    public abstract class BaseStateMachine : BaseESHostingAndModule<IState, IStateMachine>,IStateMachine,IState,IESModule
    {
        public static IState NullState = new BaseMicroStateRunTimeLogic_StringKey() { key="空状态" };
        [DisplayAsString(FontSize =25),HideLabel,PropertyOrder(-1)]
        public string des => "状态机";
        public abstract IEnumerable<IState> AsCurrentChild { get; }
        //全部运行中的状态
        [SerializeReference,LabelText("总状态机运行中状态")]
        public List<IState> MainRunningStates;
        [LabelText("自己的运行中状态")]
        public SafeUpdateList<IState> SelfRunningStates= new SafeUpdateList<IState>();
        public bool CheckStateCanUpdating
        {
            get
            {
                if (GetHost != null)
                {
                    if (GetHost is IState state)
                    {
                        if (!state.CheckStateCanUpdating)
                        {
                            return false;
                        }
                        if (state.AsCurrentChild?.Contains(this) ?? false)
                        {
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }
        }
        public abstract IState AsThis { get; set; }

        [ReadOnly, SerializeReference, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03"), LabelText("当前状态")] public IState currentFirstState;
        [ReadOnly, SerializeReference, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04"), LabelText("初始状态")] public IState EnterWithState;
        IStateSharedData IState.SharedData { get => AsThis?.SharedData; set { if(AsThis!=null)AsThis.SharedData = value; } }

        IStateStatus IState.Status
        {
            get => AsThis?.Status; set { if (AsThis != null) AsThis.Status = value; }
        }
        public bool HasPrepared { get; set; }
        public virtual void OnStateEnter()
        {
            
        }

        public virtual void OnStateExit()
        {
            AsThis?.OnStateExit();
            HasPrepared = false;
            if (host is BaseStateMachine parent)
            {
                parent.SelfRunningStates.valuesToRemove.Add(this);
            }
        }

        public void OnStatePrepare()
        {
            
            MainRunningStates = Root.MainRunningStates;
            AsThis?.OnStatePrepare();
            HasPrepared = true;
            SelfRunningStates?.Update();
            if (host is BaseStateMachine parent)
            {
                parent.SelfRunningStates.valuesToAdd.Add(this);
            }
            OnStateEnter();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (GetHost is BaseStateMachine machine) { }
            else OnStatePrepare();
        }
        public BaseStateMachine Root { get { if (GetHost is BaseStateMachine machine) return machine.Root; else return this; } }
        protected override void OnDisable()
        {
            base.OnDisable();
            if (GetHost is BaseStateMachine machine) { }
            else OnStateExit();
        }
        protected override void Update()
        {
            base.Update();
            /*(GetHost is BaseStateMachine parent)
            {
                //子状态机
                if (parent.AsCurrentChild.Contains(AsThis))
                {
                    OnStateUpdate();
                }
            }*/
            if (GetHost is BaseStateMachine machine) { }
            else OnStateUpdate();
        }
        public void OnStateUpdate()
        {
            AsThis?.OnStateUpdate();
            SelfRunningStates?.Update();
            if (AsCurrentChild != null)
            {
                foreach (var i in AsCurrentChild)
                {
                    if (i != null)
                    {
                        if(i.Status!=null&&i.Status.StandStatus!= EnumStateStandCurrentRunStatus.StateUpdate)
                        {
                            continue;
                        }else if (!i.HasPrepared)
                        {
                            continue;
                        }

                        i.OnStateUpdate();
                    }
                }
            }
        }

        public void SetKey(object key)
        {
            AsThis?.SetKey(key);
        }
        public object GetKey()
        {
            return AsThis?.GetKey();
        }
        public abstract void RegesterNewState(object key, IState logic);
        [Button("初始化"), Tooltip("定义初始化的状态")]
        public void WithEnterState(IState state)
        {
            if (state != null)
                EnterWithState = state;
        }
        [Button("输出全部状态")]
        public void OutPutAllStateRunning(string befo="合并：")
        {
            string all = befo+"现在运行的有：";
            foreach(var i in MainRunningStates)
            {
                all += i.GetKey() + " , ";
            }
            Debug.LogWarning(all);
        }
    }
    

    public abstract class BaseWithableStateRunTimeLogic<Machine, Key, SharedData, Status, This> : BaseStateRunTimeLogic<Machine, Key, SharedData, Status, This>
       where This : BaseStateRunTimeLogic<Machine, Key, SharedData, Status, This>
         where Machine : BaseStateMachine
        where SharedData : IStateSharedData
        where Status : IStateStatus
    {
        [FoldoutGroup("默认委托")] private Action<This> Action_OnPrepared;
        [FoldoutGroup("默认委托")] private Action<This> Action_OnEnter;
        [FoldoutGroup("默认委托")] private Action<This> Action_OnExit;
        [FoldoutGroup("默认委托")] private Action<This> Action_Update;
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
        protected override void Update()
        {
            Action_Update?.Invoke(this as This);
            base.Update();
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
       
    }
    public abstract class BaseMicroStateRunTimeLogic<Key,Machine,This> : BaseWithableStateRunTimeLogic<Machine,Key, MicroStateSharedData, MicroStateStatus,This>
        where Machine:BaseStateMachine
        where This : BaseMicroStateRunTimeLogic<Key, Machine, This> 
    {
       

    }
   
    public abstract class BaseStateMachine<Key,Logic> : BaseStateMachine where Logic:BaseESModule, IState
    {
        [SerializeReference,LabelText("全部状态字典")]
        public Dictionary<Key, IState> states = new Dictionary<Key, IState>();
        
        /* [GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03"),LabelText("当前状态")]*/
        /* public IState CurrentState =>currentFirstState?? NullState;*/
        public override IEnumerable<IState> NormalBeHosted => states.Values;
        [LabelText("状态机自基准状态"),SerializeReference]public IState thisState;
        public override IState AsThis { get => thisState; 
            set { Debug.LogWarning("修改状态机自状态是危险的，但是允许"); thisState = value; } }
        public override IEnumerable<IState> AsCurrentChild { get {return new IState[] { currentFirstState }; } }
        
        [Button("测试切换状态"),Tooltip("该方法建议用于运行时，不然得话会立刻调用状态的Enter，请切换使用WithEnterState")]
        public void TryActiveStateByKey(Key key, IState ifNULL=null)
        {
            if (states == null)
            {
                states = new Dictionary<Key, IState>();
                if (ifNULL != null)
                {
                    RegesterNewState(key, ifNULL);
                    SwitchStateTo(key);
                    //新建 没啥
                }
                return;
            }
            if (states.ContainsKey(key))
            {
                TryActiveState(states[key],key);
            }
            else
            {
                if (ifNULL != null)
                {
                    RegesterNewState(key, ifNULL);
                    TryActiveState(ifNULL);
                }
            }
        }
        public virtual bool TryActiveState(IState use,Key cacheKey=default)
        {
            Debug.LogError("老版本的Active"+use.GetKey());
            /* if (TargetMainRunningStates == null)
             {
                 TargetMainRunningStates = new Dictionary<Key, IState>();
                 if (use != null)
                 {
                     RegesterNewState(use, use);
                     SwitchStateTo(key);
                 }
                 return;
             }*/
            if (states.Values.Contains(use))
            {
                 TrySwitchStateTo(use);
                return true;
            }
            else
            {
                Debug.LogError("暂时不支持活动为注册的状态");
                return false;
            }
        }
        
        [Button("初始化"), Tooltip("定义初始化的状态")]
        public void WithEnterState(Key key)
        {
            if (key != null && states.ContainsKey(key))
                EnterWithState = states[key];
            else Debug.LogError("状态机没注册这个状态");
        }
        public override void OnStateEnter()
        {
            IState state = GetStartWith();
            if (state != null)
            {
                bool b = TryActiveState(state);
                if (b == false) OnStateExit();
            }
            base.OnStateEnter();
        }
        public IState GetStartWith()
        {
            if (EnterWithState != null && EnterWithState != NullState)
            {
                return EnterWithState;
            }
            else
            {
                return states.Values.First();
            }
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            if(AsCurrentChild!=null)
            foreach(var i in AsCurrentChild)
            {
                if (i != null)
                {
                    i.OnStateExit();
                }
            }
            currentFirstState = NullState;
        }
        
        public void RegesterNewState(Key key, IState logic)
        {
            if (states.ContainsKey(key))
            {
                Debug.LogError("重复注册状态?键是" + key);
            }
            else
            {
                states.Add(key,logic);
                logic.SetKey(key);
                
                if(logic is IESModule logic1)
                {
                    logic1.TrySubmitHosting(this,false);
                    Debug.Log("注册成功？"+logic.GetKey());
                }
                /*else if(logic is BaseStateMachine machine)
                {
                    machine.TrySubmitHosting(this);
                }*/
                else
                {
                    Debug.Log("啥也不是？");
                }
                
            }
        }
        
        protected void TrySwitchStateTo(Key k)
        {
            SwitchStateTo(k);
        }
        protected void TrySwitchStateTo(IState s)
        {
            SwitchStateTo(s);
        }
        protected void SwitchStateTo(Key to,Key from=default)
        {
            if (states.ContainsKey(to))
            {
                IState willUse = states[to];
                if (currentFirstState == willUse) return;
                currentFirstState?.OnStateExit();
                currentFirstState = states[to];
                currentFirstState?.OnStatePrepare();
                if (currentFirstState == null)
                {
                    Debug.LogError("状态为空！键是" + to);
                }
            }
        }
        protected void SwitchStateTo(IState to, Key from = default)
        {
            
            if (states.Values.Contains(to))
            {
                IState willUse = to;
               
                if (currentFirstState == willUse) return;
                currentFirstState?.OnStateExit();
                currentFirstState = to;
                currentFirstState?.OnStatePrepare();
                if (currentFirstState == null)
                {
                    Debug.LogError("状态为空！键是" + to);
                }
            }
        }
        protected override bool OnSubmitHostingAsNormal(IStateMachine hosting)
        {
            host = hosting;
            return true;
        }
        public override void TryRemoveModuleAsNormal(IState use)
        {
            //
        }

    }
    [Serializable, TypeRegistryItem("字符串键微型状态RunTime")]
    public class BaseMicroStateRunTimeLogic_StringKey : BaseMicroStateRunTimeLogic<string, BaseMicroStateMachine_StringKey, BaseMicroStateRunTimeLogic_StringKey>
    {
        public override object GetKey()
        {
            return this.key;
        }

        public override void SetKey(object key)
        {
            this.key = key.ToString();
        }
    }
    [Serializable, TypeRegistryItem("字符串键微型状态机")]
    public class BaseMicroStateMachine_StringKey : BaseStateMachine<string, BaseMicroStateRunTimeLogic_StringKey>
    {
        public override void RegesterNewState(object key, IState logic)
        {
            RegesterNewState(key.ToString(), logic);
        }
    }
    [Serializable, TypeRegistryItem("字符串键标准并行状态机")]
    public class BaseStandardStateMachine : BaseMicroStateMachine_StringKey,IESModule
    {
        public override IEnumerable<IState> AsCurrentChild => SelfRunningStates.valuesNow_;
        public override bool TryActiveState(IState use, string cacheKey = null)
        {
            Debug.Log("use0");
            //空状态：直接使用
            if(MainRunningStates.Count == 0)return base.TryActiveState(use, cacheKey);
            //包含取消
            if (MainRunningStates.Contains(use))
            {
                Debug.LogWarning("尝试使用已经存在的状态，有何用意");
                return false;
            }
            /*if(use is BaseStateMachine sonMachine)
            {
                sonMachine.OnStatePrepare();
                return false;
            }*/
            Debug.Log("-----------《《《《合并测试开始------来自"+ use.GetKey().ToString());
            //单状态，简易判断
            if (MainRunningStates.Count == 1)
            {
                IState state = MainRunningStates.First();
                {
                    //state的共享数据有的不是标准的哈/
                    //标准情形
                    Debug.Log(use.SharedData);
                    if(state.SharedData is StandardStateSharedData left&&use.SharedData is StandardStateSharedData right)
                    {
                        string leftKey = state.GetKey().ToString();
                        string rightKey = use.GetKey().ToString();
                        var back = StandardStateSharedData.HandleMerge(left.MergePart_, right.MergePart_, leftKey, rightKey);
                        if(back== StandardStateSharedData.HandleMergeBack.HitAndReplace)
                        {
                            state.OnStateExit();
                            use.OnStatePrepare();
                            currentFirstState = use;
                            Debug.Log("单-合并--打断  原有的  "+ leftKey + " 被 新的  "+ rightKey + "  打断!");
                        }else if(back== StandardStateSharedData.HandleMergeBack.MergeComplete)
                        {
                            use.OnStatePrepare();
                            Debug.Log("单-合并--成功  原有的  " + leftKey + " 和  新的  " + rightKey + "  合并!");
                        }
                        else //合并失败
                        {
                            Debug.Log("单-合并--失败  原有的  " + leftKey + " 阻止了  新的  "  + rightKey + "  !");
                        }
                    }
                    else //有的不是标准状态
                    {
                        base.TryActiveState(use, cacheKey);
                    }
                }
            }
            else  //多项目
            {
                if(use.SharedData is StandardStateSharedData right)
                {
                    string rightKey = use.GetKey().ToString();
                    List<IState> hit = new List<IState>();
                    List<string> merge = new List<string>();
                    foreach(var i in MainRunningStates)
                    {
                        if(i.SharedData is StandardStateSharedData left)
                        {
                            string leftKey = i.GetKey().ToString();
                            var back = StandardStateSharedData.HandleMerge(left.MergePart_, right.MergePart_, leftKey, rightKey);
                            if (back == StandardStateSharedData.HandleMergeBack.HitAndReplace)
                            {
                                hit.Add(i);
                                
                                //打断一个捏
                            }
                            else if (back == StandardStateSharedData.HandleMergeBack.MergeComplete)
                            {
                                //正常的
                                merge.Add(leftKey);
                                
                            }
                            else //合并失败
                            {
                                Debug.LogWarning("多-合并--失败" + leftKey + " 阻止了 " + rightKey + "的本次合并测，无事发生试!");
                                return false;
                            }
                        }
                    }
                    //成功合并了
                    Debug.Log("---√多-合并--完全成功！来自"+rightKey+"以下是细则：");
                    use.OnStatePrepare();
                    foreach(var i in merge)
                    {
                        Debug.Log("     --合并细则  本次合并-合并了了" + i);
                    }
                    foreach (var i in hit)
                    {
                        Debug.Log("     --合并细则  本次合并-打断了"+i.GetKey());
                        i.OnStateExit();
                    }
                }
                else //不是标准状态滚
                {
                    base.TryActiveState(use, cacheKey);
                }
            }
            return false;
        }
    }

    [Serializable, TypeRegistryItem("标准状态共享数据")]
    public class StandardStateSharedData : IStateSharedData
    {
        [LabelText("冲突与合并")]
        public StateDataClip_StringKeyMergeAndConflict MergePart_ = new StateDataClip_StringKeyMergeAndConflict() { channel = StateDataClip_Index_StandardChannel.AllBodyActive };

        [Serializable,TypeRegistryItem("状态共享数据_冲突与合并")]
        public struct StateDataClip_StringKeyMergeAndConflict
        {
            //最高级别
            [Header("最高级别")]
            [LabelText("无条件被打断"), Tooltip("最高级别的优先级，必定可被打断")] public string[] BeHitWithoutCondition;
            [LabelText("无条件打断"), Tooltip("最高级别的优先级,必定可以打断")] public string[] HitWithoutCondition;
            //第二级别
            [Header("第三级别")]
            [LabelText("是否可被打断")] public HitOption CanBeHit;//不发生,碾压,仅测试同级别
            [LabelText("是否可打断")] public HitOption CanHit;//不发生,碾压,仅测试同级别
            [LabelText("逻辑层"), Tooltip("有重合时可同级别判断，无重合时直接碾压判断,Rub层就没啥打断能力了")] public StateDataClip_Index_LogicAtLayer logicLayer;
            [LabelText("占据通道"), Tooltip("占据相同通道后开始判断是否可以打断等")] public StateDataClip_Index_StandardChannel channel;
            
            //第三级别
            //可以相容吗
            [Header("第二级别")]
             [LabelText("可被打断的优先级(byte)")] public byte BeHitOrder;
            [LabelText("打断的优先级(byte)")] public byte HitOrder;
        }
        [TypeRegistryItem("状态共享数据_动画器集成")]
        public struct StateDataClip_Animator
        {
           
        }
        [Flags]
        public enum StateDataClip_Index_LogicAtLayer
        {
            [InspectorName("垃圾层")] Rubbish = 0,//---永远不依赖优先级，层级总是不交和，必定打断
            [InspectorName("低等级")] Low = 1,
            [InspectorName("中等级")] Middle = 2,
            [InspectorName("高等级")] High = 4,
            [InspectorName("超等级")] Super = 8,
        }
        public enum HitOption
        {
            [InspectorName("同级别测试")] SameLayTest,
            [InspectorName("只允许层级碾压,忽略同级别")] LayerCrush,
            [InspectorName("永远不发生")] Never,
        }
        [Flags]
        public enum StateDataClip_Index_StandardChannel
        {
            [InspectorName("右手")] RightHand = 1 << 0,
            [InspectorName("左手")] LeftHand = 1 << 1,
            [InspectorName("双手")] DoubleHand = RightHand | LeftHand,
            [InspectorName("右腿")] RightLeg = 1 << 2,
            [InspectorName("左腿")] LeftLeg = 1 << 3,
            [InspectorName("双腿")] DoubleLeg = RightLeg | LeftLeg,
            [InspectorName("四肢")] FourLimbs = DoubleHand | DoubleLeg,
            [InspectorName("头")] Head = 1 << 4,
            [InspectorName("身体骨架")] BodySpine = 1 << 5,
            [InspectorName("全身占据活动")] AllBodyActive = FourLimbs | Head | BodySpine,
            [InspectorName("心灵:思考")] Heart = 1 << 6,
            [InspectorName("眼睛")] Eye = 1 << 7,
            [InspectorName("耳朵")] Ear = 1 << 8,
            [InspectorName("全身心")] AllBodyAndHeartAndMore = AllBodyActive | Heart | Eye | Ear,
            [InspectorName("目标")] Target = 1 << 9,
        }
        public enum HandleMergeBack
        {
            [InspectorName("打断并替换")]HitAndReplace,
            [InspectorName("合并成功")] MergeComplete,
            [InspectorName("合并失败")] MergeFail
        }
        public static HandleMergeBack HandleMerge(StateDataClip_StringKeyMergeAndConflict left, StateDataClip_StringKeyMergeAndConflict right,string leftName=null,string rightName=null)
        {            
            //进行两边的合并冲突测试
            //冲突
            do  //第一波测试 --- 无条件打断 和 不参与打断
            {
                //第一层
                if (left.BeHitWithoutCondition?.Contains(rightName) ?? false)
                { return HandleMergeBack.HitAndReplace; }//左边无条件被打断
                if ((right.HitWithoutCondition?.Contains(leftName) ?? false))
                { return HandleMergeBack.HitAndReplace; }//右边无条件打断

                var channel = left.channel & right.channel;
                //第二层 //两者都不在意打断 
                if (left.CanBeHit == HitOption.Never || right.CanHit == HitOption.Never)
                {
                    if (channel == 0) return HandleMergeBack.MergeComplete; //无冲突就合并了
                    else return HandleMergeBack.MergeFail;
                }
                var layerAND = left.logicLayer & right.logicLayer;
                if (channel > 0)//有冲突要解决
                {
                    //必须解决冲突 -- 形成一方碾压
                    if (layerAND == 0)
                    {
                        if (left.logicLayer > right.logicLayer)
                        {
                            return HandleMergeBack.MergeFail;//左边碾压 解决不了
                        }
                        else
                        {
                            Debug.Log(4444);
                            return HandleMergeBack.HitAndReplace;//右边碾压 直接拿下
                        }
                    }
                    else //层级有重叠
                    {
                        if (left.CanBeHit == HitOption.LayerCrush||right.CanHit== HitOption.LayerCrush)
                        {
                            return HandleMergeBack.MergeFail;//没有达成条件 合并失败
                        }
                        
                        //优先级的问题
                        if (left.BeHitOrder <= right.HitOrder)
                        {
                            
                            return HandleMergeBack.HitAndReplace;
                        }
                        else
                        {
                            
                            return HandleMergeBack.MergeFail;
                        }
                    }
                }
                else
                {
                    return HandleMergeBack.MergeComplete;
                }
                
            } while (false);
            //左边在高层级哈
            //能否合并

            //层级碾压
            //相交  双方层级碾压-->只允许层级碾压m




            //如果有一方是可同级别的,左边要求层级碾压时，




        }
    }
    [Serializable,TypeRegistryItem("标准状态运行状态")]
    public class StandardStateStatus : IStateStatus
    {
        [LabelText("状态开始时间")] public float hasEnterTime;
        [LabelText("标准状态")] public EnumStateStandCurrentRunStatus stateStandStatus;

        public EnumStateStandCurrentRunStatus StandStatus { get => stateStandStatus; set => stateStandStatus = value; }

        public void Init(params object[] ps)
        {

        }
    }
    [Serializable,TypeRegistryItem("字符串键继承状态元")]
    public class BaseStandardStateRunTimeLogic_StringKey:BaseStateRunTimeLogic<BaseMicroStateMachine_StringKey,string,StandardStateSharedData, StandardStateStatus, BaseStandardStateRunTimeLogic_StringKey>
    {
        public override object GetKey()
        {
            return this.key;
        }

        public override void SetKey(object key)
        {
            this.key = key.ToString();
        }
    }
}
[Serializable, TypeRegistryItem("字符串键可委托可继承状态元")]
public class BaseWithableStandardStateRunTimeLogic : BaseWithableStateRunTimeLogic<BaseMicroStateMachine_StringKey, string, StandardStateSharedData, StandardStateStatus, BaseStandardStateRunTimeLogic_StringKey>
{
    public override object GetKey()
    {
        return this.key;
    }

    public override void SetKey(object key)
    {
        this.key = key.ToString();
    }
}


[Serializable]
public class test: BaseStandardStateRunTimeLogic_StringKey
{
    protected override void RunStateUpdateLogic()
    {
        base.RunStateUpdateLogic();
        Debug.Log("I,am"+GetKey());
    }
}

