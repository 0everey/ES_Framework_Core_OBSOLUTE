using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于 投射<Caster> 和 抓取<Catcher>  部分
    #region 投射和抓取
    //Cast用于投射出一个东西 Catcher用于抓取一个东西同时 抓取器是一个单值针
        #region 接口支持(抽象在功能的最前面)
    public interface ICaster { }
        public interface ICaster<out T>
        {
            T Cast();
        }
        public interface IPointerCaster<out T> : ICaster<T>, IPointer, IWithPointerPlayerSystemObjectCaster
        {
        }
        public interface IPointerNoneCaster<out T> : ICaster<T>, IPointerNone
        {

        }
        public interface IPointerOnlyBackCaster<out T> : ICaster<T>, IPointerOnlyBack<T>
        {

        }
        public interface ICatcher
        {

        }
        public interface ICatcher<out T>
        {
            T Catch();
        }
        public interface IPointerCatcher<out T> : ICatcher<T>, IPointer
        {

        }
        public interface IPointerNoneCatcher<out T> : ICatcher<T>, IPointerNone
        {

        }
        public interface IPointerOnlybackCatcher<out T> : ICatcher<T>, IPointerOnlyBack<T>
        {

        }
        //投射接口
        public interface IPointerForGameObjectCaster : IPointerCaster<GameObject>
        {

        }
        public interface IPointerForSystemObjectCaster : IPointerCaster<object>
        {

        }
        public interface IPointerForFloatCaster : IPointerCaster<float>
        {

        }
        public interface IPointerForIntCaster : IPointerCaster<int>
        {

        }
        public interface IPointerForVector3Caster : IPointerCaster<Vector3>
        {

        }
        public interface IPointerForVector2Caster : IPointerCaster<Vector2>
        {

        }
        public interface IPointerForStringCaster : IPointerCaster<string>
        {

        }
        public interface IPointerForBoolCaster : IPointerCaster<bool>
        {

        }
        public interface IPointerForUnityObjectCaster : IPointerCaster<UnityEngine.Object>
        {

        }
        public interface IPointerForComponentCaster : IPointerCaster<Component>
        {

        }
        public interface IPointerForTypeCaster : IPointerCaster<Type>
        {

        }
        public interface IPointerForCollider3DCaster : IPointerCaster<Collider>
        {

        }
        public interface IPointerForCollider2DCaster : IPointerCaster<Collider2D>
        {

        }
        public interface IPointerForCancellationTokenSourceCaster : IPointerCaster<CancellationTokenSource>
        {

        }
        public interface IPointerForColorCaster : IPointerCaster<Color>
        {

        }
        public interface IPointerForQuaternionCaster : IPointerCaster<Quaternion>
        {

        }
        public interface IWithPointerPlayerSystemObjectCaster
        {
            public abstract PointerPlayerSystemObjectCaster playerCaster { get; }
            public void Send(object oo)
            {
                playerCaster?.Recieve(oo);
            }
        }
        public interface IWithPointerPlayerWithESValue
        {
            public abstract PointerPlayerWithESValue playerESValue { get; }
        }
        #endregion
        #region 投射功能
    [Serializable, TypeRegistryItem("<投射核心功能>发送任意系统针的结果到播放器", "投射")]
    public class PointerSendSystemObjectToPlayerCaster : IPointerNone, IWithPointerPlayerSystemObjectCaster
    {
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;
        [LabelText("发送的物体"), SerializeReference] public IPointer object_;
        public object Pick(object on= null, object from = null, object with = null)
        {
            object oo = object_?.Pick();
            if (usePlayerCaster) playerCaster_?.Recieve(oo);
            return oo;
        }

    }
    //投射抽象父类
    public abstract class PointerForXXCaster<T> : IPointerCaster<T>, IPointerNone
    {
        [LabelText("直接投射")] public T cast;
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
        public T Cast()
        {
            return cast;
        }

        public object Pick(object on= null, object from = null, object with = null)
        {
            if (usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(cast);
            }
            return cast;
        }
    }
    //直接实现
    [Serializable, TypeRegistryItem("游戏物体_直接投射", "投射")]
    public class PointerForGameObjectCaster : PointerForXXCaster<GameObject>, IPointerForGameObjectCaster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("系统物体_直接投射", "投射")]
    public class PointerForSystemObjectCaster : PointerForXXCaster<object>, IPointerForSystemObjectCaster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("浮点数_直接投射", "投射")]
    public class PointerForFloatCaster : PointerForXXCaster<float>, IPointerForFloatCaster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("整数_直接投射", "投射")]
    public class PointerForIntCaster : PointerForXXCaster<int>, IPointerForIntCaster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("Vector3_直接投射", "投射")]
    public class PointerForVector3Caster : PointerForXXCaster<Vector3>, IPointerForVector3Caster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("Vector2_直接投射", "投射")]
    public class PointerForVector2Caster : PointerForXXCaster<Vector2>, IPointerForVector2Caster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("字符串_直接投射", "投射")]
    public class PointerForStringCaster : PointerForXXCaster<string>, IPointerForStringCaster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("布尔值_直接投射", "投射")]
    public class PointerForBoolCaster : PointerForXXCaster<bool>, IPointerForBoolCaster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("Unity物体_直接投射", "投射")]
    public class PointerForUnityObjectCaster : PointerForXXCaster<UnityEngine.Object>, IPointerForUnityObjectCaster
    {

    }
    //直接实现
    [Serializable, TypeRegistryItem("Unity碰撞器3D_直接投射", "投射")]
    public class PointerForCollider3DCaster : PointerForXXCaster<Collider>, IPointerForCollider3DCaster, IPointerForComponentCaster
    {
        Component ICaster<Component>.Cast()
        {
            return (this as IPointerForCollider3DCaster).Cast();
        }
    }
    [Serializable, TypeRegistryItem("Unity碰撞器2D_直接投射", "投射")]
    public class PointerForCollider2DCaster : PointerForXXCaster<Collider2D>, IPointerForCollider2DCaster, IPointerForComponentCaster
    {
        Component ICaster<Component>.Cast()
        {
            return (this as IPointerForCollider2DCaster).Cast();
        }
    }
    [Serializable, TypeRegistryItem("颜色-直接投射", "投射")]
    public class PointerForColorCaster : PointerForXXCaster<Color>, IPointerForColorCaster
    {

    }
    [Serializable, TypeRegistryItem("四元数_直接投射", "投射")]
    public class PointerForQuaternionCaster : PointerForXXCaster<Quaternion>, IPointerForQuaternionCaster
    {

    }
    #endregion
        #region 抓取功能
    //抓取抽象
    public abstract class PointerCatcher<T> : IPointerOnlybackCatcher<T>, IWithPointerPlayerSystemObjectCaster
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
        public abstract IPointerCaster<T> caster { get; }

        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;

        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,抓取它持有的值来应用自身", Message = @"@ ""【绑定抓取目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@useCatcherPlayer")]
        [LabelText("使用Player抓取?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCatcher")] public bool useCatcherPlayer;
        [LabelText("抓取Player", SdfIconType.At), ShowIf("useCatcherPlayer"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCatcher")] public PointerPlayerSystemObjectCaster playerCaster_;
        public virtual T Catch()
        {
            if (useCatcherPlayer && playerCaster_ != null)
            {
                
                return KeyValueMatchingUtility.Matcher.SystemObjectToT<T>(playerCaster_.Cast());
            }
            if (caster == default) return default;

            return caster.Cast();
        }

        public T Pick(object on= null, object from = null, object with = null)
        {
            return Catch();
        }
    }
    [Serializable, TypeRegistryItem("游戏物体_抓取", "抓取")]
    public class PointerForGameobjectCatcher : PointerCatcher<GameObject>, IPointerForGameObject_Only
    {
        [SerializeReference, LabelText("抓取游戏物体")] public IPointerForGameObjectCaster pointerCaster;
        public override IPointerCaster<GameObject> caster => pointerCaster;
    }
    [Serializable, TypeRegistryItem("系统物体_抓取", "抓取")]
    public class PointerForSystemObjectCatcher : PointerCatcher<object>, IPointer
    {
        [SerializeReference, LabelText("抓取系统物体")] public IPointerForSystemObjectCaster pointerCaster;
        public override IPointerCaster<object> caster => pointerCaster;
    }
    [Serializable, TypeRegistryItem("浮点数_抓取", "抓取")]
    public class PointerForFloatCatcher : PointerCatcher<float>, IPointerForFloat_Only
    {
        [SerializeReference, LabelText("抓取Float")] public IPointerForFloatCaster pointerCaster;
        public override IPointerCaster<float> caster => pointerCaster;
    }
    [Serializable, TypeRegistryItem("整数_抓取", "抓取")]
    public class PointerForIntCatcher : PointerCatcher<int>, IPointerForInt_Only
    {
        [SerializeReference, LabelText("抓取Int")] public IPointerForIntCaster pointerCaster;
        public override IPointerCaster<int> caster => pointerCaster;
    }
    [Serializable, TypeRegistryItem("Vector3_抓取", "抓取")]
    public class PointerForVector3Catcher : PointerCatcher<Vector3>, IPointerForVector3_Only
    {
        [SerializeReference, LabelText("抓取Vector3")] public IPointerForVector3Caster pointerCaster;
        public override IPointerCaster<Vector3> caster => pointerCaster;
    }
    [Serializable, TypeRegistryItem("Vector2_抓取", "抓取")]
    public class PointerForVector2Catcher : PointerCatcher<Vector2>, IPointerForVector2_Only
    {
        [SerializeReference, LabelText("抓取Vector2")] public IPointerForVector2Caster pointerCaster;
        public override IPointerCaster<Vector2> caster => pointerCaster;
    }
    [Serializable, TypeRegistryItem("字符串_抓取", "抓取")]
    public class PointerForStringCatcher : PointerCatcher<string>, IPointerForString_Only
    {
        [SerializeReference, LabelText("抓取String")] public IPointerForStringCaster pointerCaster;
        public override IPointerCaster<string> caster => pointerCaster;

    }
    [Serializable, TypeRegistryItem("布尔值_抓取", "抓取")]
    public class PointerForBoolCatcher : PointerCatcher<bool>, IPointerForBool_Only
    {
        [SerializeReference, LabelText("抓取Bool")] public IPointerForBoolCaster pointerCaster;
        public override IPointerCaster<bool> caster => pointerCaster;

    }
    [Serializable, TypeRegistryItem("Unity物体抓取", "抓取")]
    public class PointerForUnityObjectCatcher : PointerCatcher<UnityEngine.Object>, IPointerForUnityObject_Only
    {
        [SerializeReference, LabelText("抓取UnityObject")] public IPointerForUnityObjectCaster pointerCaster;
        public override IPointerCaster<UnityEngine.Object> caster => pointerCaster;

    }
    [Serializable, TypeRegistryItem("组件脚本_抓取", "抓取")]
    public class PointerForComponentCatcher : PointerCatcher<Component>, IPointerForComponent_Only
    {
        [SerializeReference, LabelText("抓取脚本")] public IPointerForComponentCaster pointerCaster;
        public override IPointerCaster<Component> caster => pointerCaster;

    }
    [Serializable, TypeRegistryItem("取消源_抓取", "抓取")]
    public class PointerForCancellationTokenSourceCatcher : PointerCatcher<CancellationTokenSource>, IPointerForCancellationTokenSource_Only
    {
        [SerializeReference, LabelText("尝试抓取一个取消源")] public IPointerForCancellationTokenSourceCaster pointerCaster;
        public override IPointerCaster<CancellationTokenSource> caster => pointerCaster;


    }
    [Serializable, TypeRegistryItem("颜色_抓取", "抓取")]
    public class PointerForColorCatcher : PointerCatcher<Color>, IPointerForColor_Only
    {
        [SerializeReference, LabelText("尝试抓取一个颜色")] public IPointerForColorCaster pointerCaster;
        public override IPointerCaster<Color> caster => pointerCaster;
    }
    [Serializable, TypeRegistryItem("四元数_抓取", "抓取")]
    public class PointerForQuaternionCatcher : PointerCatcher<Quaternion>, IPointerForQuaternion_Only
    {
        [SerializeReference, LabelText("尝试抓取一个四元数")] public IPointerForQuaternionCaster pointerCaster;
        public override IPointerCaster<Quaternion> caster => pointerCaster;
    }
    #endregion
    #endregion
}
