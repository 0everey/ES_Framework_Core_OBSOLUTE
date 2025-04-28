using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于 Pick调用_执行_测试 部分
    //基本实现自 IPointerNone -- PointerOnlyAction 不需要额外的接口
    #region 调用/测试者功能
    [Serializable, TypeRegistryItem("Pick调用_任意针")]
    public class PointerPickerEveryThing : PointerOnlyAction
    {
        [LabelText("要Pick(调用)的针"), SerializeReference] public IPointer pointer1;

        [Button("Pick该针-可测试")]
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            pointer1?.Pick();
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("Pick调用_如果 bool条件成立 触发针")]
    public class PointerPickerByBool : PointerOnlyAction
    {
        [LabelText("条件"), SerializeReference] public IPointerForBool_Only when;
        [LabelText("Pick针"), SerializeReference] public IPointer pointer1;

        [Button("Pick该针")]
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (when?.Pick() ?? true)
            {
                pointer1?.Pick();
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("Pick调用_三元表达式 触发针")]
    public class PointerPickerByBool_TrueForFalseFor_Ternary : PointerOnlyAction
    {
        [LabelText("条件"), SerializeReference] public IPointerForBool_Only when;
        [LabelText("为真时Pick的针"), SerializeReference] public IPointer pointerForTrue;
        [LabelText("为假时Pick的针"), SerializeReference] public IPointer pointerForFalse;
        [Button("测试Pick该针")]
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (when?.Pick() ?? true)
            {
                pointerForTrue?.Pick();
            }
            else
            {
                pointerForFalse?.Pick();
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("触发针_触发UnityEvent")]
    public class PointerForAction_InvokeUnityEvent : IPointerOnlyAction
    {
        [LabelText("触发事件")] public UnityEvent unityEvent;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (unityEvent != null)
            {
                unityEvent?.Invoke();
            }
            return null;
        }
    }
    [Serializable, TypeRegistryItem("触发针_触发一个PointerPlayer")]
    public class PointerForAction_InvokePointerPlayer : IPointerForAction_Only
    {
        [LabelText("触发的针Player")] public PointerPlayer aPlayer;
        public Action Pick(object by = null, object yarn = null, object on = null)
        {
            if (aPlayer != null)
            {
                aPlayer?.Pick_Invoke();
            }
            return null;
        }
    }


    //每次随机触发一个
    [Serializable, TypeRegistryItem("触发针包_每次触发一个")]
    public class PointerPackOnlyAction_RandomOne : PointerPackerForNone, IPointerOnlyAction
    {
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (pointers != null && pointers.Count > 0)
            {
                intHepler = UnityEngine.Random.Range(0, pointers.Count);
                pointers[intHepler]?.Pick();
            }
            return -1;
        }
    }
    //循环每次触发下一个")]
    [Serializable, TypeRegistryItem("触发针包_循环每次触发下一个")]
    public class PointerPackOnlyAction_LoopAndNext : PointerPackerForNone, IPointerOnlyAction
    {
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (pointers != null && pointers.Count > 0)
            {
                intHepler++;
                intHepler %= pointers.Count;
                pointers[intHepler]?.Pick();
            }
            return -1;
        }
    }
    //循环触发一遍")]
    [Serializable, TypeRegistryItem("触发针包_完整循环触发全部")]
    public class PointerPackOnlyAction_LoopOnce : PointerPackerForNone, IPointerOnlyAction
    {
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (pointers != null)
            {
                foreach (var i in pointers)
                {
                    if (i != null)
                    {
                        i.Pick();
                    }
                }
            }
            return -1;
        }
    }

    #region 依赖_设计遗留问题_后面可能淘汰
    //依赖针--设计遗留问题不用在意
    public class PointerOnlyAction_DependenceOnlyAction : PointerOnlyAction
    {
        [LabelText("依赖触发"), SerializeReference]
        public IPointerNone dependPointer;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            return dependPointer?.Pick();
        }
    }
    [Serializable]
    public class PointerOnlyAction_DependenceOnlyActionAndInitable : PointerOnlyAction, IInittable
    {
        [LabelText("依赖触发"), SerializeReference]
        public IPointerNone dependPointer;

        public virtual void Init(params object[] ps)
        {
            hasInit = true;
        }
        public bool hasInit = false;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            if (!hasInit) Init();
            return dependPointer?.Pick();
        }
    }
    [Serializable]
    public class PointerOnlyAction_DependenceOnlyActionAndInitableDependenToo : PointerOnlyAction_DependenceOnlyActionAndInitable
    {
        [LabelText("初始化触发"), SerializeReference]
        public IPointerNone dependPointer_init;
        public override void Init(params object[] ps)
        {
            base.Init(ps);
            dependPointer_init?.Pick();
        }
    }
    [Serializable]
    public class PointerOnlyAction_DependenceOnlyActionAndInitableDependenToo<Init_> : PointerOnlyAction_DependenceOnlyActionAndInitable where Init_ : PointerOnlyAction
    {
        [LabelText("初始化触发"), SerializeReference]
        public Init_ dependPointer_init;
        public override void Init(params object[] ps)
        {
            base.Init(ps);
            dependPointer_init?.Pick();
        }
    }
    #endregion
    #endregion

    #region 专题_Debug调试
    [Serializable, TypeRegistryItem("Debug字符串针", "调试")]
    public class PointerOnlyDebug_PoinerContent : PointerOnlyAction
    {
        [LabelText("类型")] public LogType type = LogType.Log;
        [LabelText("debug内容"), SerializeReference] public IPointerForString_Only debug = new PointerForString_Direc() { string_direc = "任意内容" };
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            switch (type)
            {
                case LogType.Warning: Debug.LogWarning(debug?.Pick()); break;
                case LogType.Error: Debug.LogError(debug?.Pick()); break;
                case LogType.Log: Debug.Log(debug?.Pick()); break;
                default: Debug.Log(type + "$$" + debug?.Pick()); break;
            }

            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("Debug直接输入字符串", "调试")]
    public class PointerOnlyDebug : PointerOnlyAction
    {
        [LabelText("类型")] public LogType type = LogType.Log;
        [LabelText("debug内容")] public string debug = "测试";
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            switch (type)
            {
                case LogType.Warning: Debug.LogWarning(debug); break;
                case LogType.Error: Debug.LogError(debug); break;
                case LogType.Log: Debug.Log(debug); break;
                default: Debug.Log(type + "$$" + debug); break;
            }

            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("Debug任意物体", "调试")]
    public class PointerOnlyDebug_IPointer : PointerOnlyAction
    {
        [LabelText("类型")] public LogType type = LogType.Log;
        [LabelText("debugr任意内容")] public IPointer everythingPointer = new PointerForString_Direc() { string_direc = "任意内容" };
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            string eve = everythingPointer?.Pick().ToString();
            switch (type)
            {
                case LogType.Warning: Debug.LogWarning(eve); break;
                case LogType.Error: Debug.LogError(eve); break;
                case LogType.Log: Debug.Log(eve); break;
                default: Debug.Log(type + "$$" + eve); break;
            }

            return base.Pick(by, yarn, on);
        }
    }

    #endregion

    #region 专题_协程
    [Serializable, TypeRegistryItem("触发针_协程延迟")]
    public class PoinerOnlyAction_Delay : PointerOnlyAction_DependenceOnlyAction, IPointerForCancellationTokenSourceCaster
    {
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
        [LabelText("延迟时间默认1"), SerializeReference]
        public IPointerForFloat_Only delayTime;
        [LabelText("投射取消源")]
        public bool applyCancellationSource = false;
        private CancellationTokenSource cancelSourceToken;
        public CancellationTokenSource Cast()
        {
            return cancelSourceToken;
        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            GameCenterManager.Instance.StartCoroutine(CoroutineMaker.DelayCoroutine(() => { dependPointer?.Pick(); }, delayTime?.Pick() ?? 1,
                applyCancellationSource ? cancelSourceToken = new CancellationTokenSource() : default));
            if (applyCancellationSource && usePlayerCaster)
            {
                playerCaster_.Recieve(cancelSourceToken);
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("触发针_协程重复")]
    public class PoinerOnlyAction_Repeat : PointerOnlyAction_DependenceOnlyAction, IPointerForCancellationTokenSourceCaster
    {
        [LabelText("延迟启动默认1"), SerializeReference]
        public IPointerForFloat_Only delayTime;
        [LabelText("触发间隔默认1"), SerializeReference]
        public IPointerForFloat_Only intervalTime;
        [LabelText("重复次数(默认5，-1无限)"), SerializeReference]
        public IPointerForInt_Only repeatTimes;
        [LabelText("投射取消源")]
        public bool applyCancellationSource = false;
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
        private CancellationTokenSource cancelSourceToken;
        public CancellationTokenSource Cast()
        {
            return cancelSourceToken;
        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            GameCenterManager.Instance.StartCoroutine(CoroutineMaker.RepeatConroutine(
                () => { dependPointer?.Pick(); },
                delayTime?.Pick() ?? 1,
                intervalTime?.Pick() ?? 1,
                repeatTimes?.Pick() ?? 5,
                applyCancellationSource ? cancelSourceToken = new CancellationTokenSource() : default
                ));
            if (applyCancellationSource && usePlayerCaster) playerCaster_?.Recieve(cancelSourceToken);
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("触发针_协程帧运行一段时间")]
    public class PoinerOnlyAction_Running : PointerOnlyAction_DependenceOnlyAction, IPointerForCancellationTokenSourceCaster
    {
        [LabelText("持续时间默认1"), SerializeReference]
        public IPointerForFloat_Only runningTime;
        [LabelText("启动时可空"), SerializeReference]
        public IPointerNone dependPointer_start;
        [LabelText("结束时可空"), SerializeReference]
        public IPointerNone dependPointer_end;
        [LabelText("投射取消源")]
        public bool applyCancellationSource = false;
        private CancellationTokenSource cancelSourceToken;
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
        public CancellationTokenSource Cast()
        {
            return cancelSourceToken;
        }

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            GameCenterManager.Instance.StartCoroutine(CoroutineMaker.RunningConroutine(

               () => { dependPointer_start?.Pick(); },
               () => { dependPointer?.Pick(); },
               () => { dependPointer_end?.Pick(); },
               runningTime?.Pick() ?? 1,
                applyCancellationSource ? cancelSourceToken = new CancellationTokenSource() : default
                ));
            if (applyCancellationSource && usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(cancelSourceToken);
            }
            return -1;
        }
    }
    #endregion

    #region 专题_SendMessage(发送消息)

    public abstract class BasePointerForSendMessage : IPointerNone
    {
        public abstract Component com { get; }
        public abstract string methodName { get; }
        public abstract object pa { get; }
        public abstract bool needPa { get; }
        [LabelText("是否必需接受者")] public bool needRecieve = false;
        public EnumCollect.SendMessageType messageType;

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (com == null) return default;
            SendMessageOptions options = needRecieve ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver;
            switch (messageType)
            {
                case EnumCollect.SendMessageType.SendMessage:
                    if (needPa) com.SendMessage(methodName, pa, options);
                    else com.SendMessage(methodName, options);
                    break;
                case EnumCollect.SendMessageType.BroadCastMessage:
                    if (needPa) com.BroadcastMessage(methodName, pa, options);
                    else com.BroadcastMessage(methodName, options);
                    break;
                case EnumCollect.SendMessageType.SendMessageUpWards:
                    if (needPa) com.SendMessageUpwards(methodName, pa, options);
                    else com.SendMessageUpwards(methodName, options);
                    break;
                default: break;
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("发送消息<SendMessage>_简易无参")]
    public class PointerForSendMessage_EasyNoPa : BasePointerForSendMessage
    {

        public override Component com => component;

        public override string methodName => str;

        public override object pa => default;

        public override bool needPa => false;
        [LabelText("直接引用脚本")] public Component component;
        [LabelText("直接输入方法名")] public string str = "methodName";

    }
    [Serializable, TypeRegistryItem("发送消息<SendMessage>_简易含参")]
    public class PointerForSendMessage_EasyWithPa : BasePointerForSendMessage
    {

        public override Component com => component;

        public override string methodName => str;

        public override object pa => systemObject_Only?.Pick();

        public override bool needPa => true;
        [LabelText("直接引用脚本")] public Component component;
        [LabelText("直接输入方法名")] public string str = "methodName";
        [LabelText("参数"), SerializeReference] public IPointer systemObject_Only;
    }
    [Serializable, TypeRegistryItem("发送消息<SendMessage>_自定义无参")]
    public class PointerForSendMessage_SelfDefineNoPa : BasePointerForSendMessage
    {

        public override Component com => component?.Pick();

        public override string methodName => str?.Pick();

        public override object pa => default;

        public override bool needPa => false;
        [LabelText("引用脚本"), SerializeReference] public IPointerForComponent_Only component;
        [LabelText("输入方法名"), SerializeReference] public IPointerForString_Only str;

    }
    [Serializable, TypeRegistryItem("发送消息<SendMessage>_自定义有参")]
    public class PointerForSendMessage_SelfDefineWithPa : BasePointerForSendMessage
    {
        public override Component com => component?.Pick();
        public override string methodName => str?.Pick();
        public override object pa => systemObject_Only?.Pick();
        public override bool needPa => true;
        [LabelText("引用脚本"), SerializeReference] public IPointerForComponent_Only component;
        [LabelText("输入方法名"), SerializeReference] public IPointerForString_Only str;
        [LabelText("参数"), SerializeReference] public IPointer systemObject_Only;
    }
    [Serializable, TypeRegistryItem("发送消息<SendMessage>_完全自定义")]
    public class PointerForSendMessage_SelfDefineAll : BasePointerForSendMessage
    {
        public override Component com => component?.Pick();
        public override string methodName => str?.Pick();
        public override object pa => usePa ? systemObject_Only?.Pick() : default;
        public override bool needPa => usePa;
        [LabelText("引用脚本"), SerializeReference] public IPointerForComponent_Only component;
        [LabelText("输入方法名"), SerializeReference] public IPointerForString_Only str;
        [LabelText("使用参数")] public bool usePa;
        [LabelText("参数"), ShowIf("usePa"), SerializeReference] public IPointer systemObject_Only;
    }
    #endregion

}
