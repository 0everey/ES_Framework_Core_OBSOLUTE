using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ES.KeyValueMatchingUtility;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES
{
    //核心 Ev针支持 支持对ESValue针播放器的Get和Set 部分
    //基本实现IPointerNone或者IPointerOnlyAction
    #region 抽象定义
    //原初
    [TypeRegistryItem("引用ES值_从针播放器", "ES值")]
    public abstract class PointerXXXESValueFromPointerPlayer<WithPlayer> : IWithPointerPlayerWithESValue where WithPlayer : PointerPlayerWithESValue
    {
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerWithESValue,用来获得或者设置它的值", Message = @"@ ""【绑定ES值目标备注："" + (playerESValue != null ? playerESValue.des : ""！未绑定"") ")]
        [LabelText("引用ES值播放器"),GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForESValue")] public WithPlayer withPlayer;

        public PointerPlayerWithESValue playerESValue => withPlayer;
    }
    //获得
    [TypeRegistryItem("获得ES值_从针播放器","ES值")]
    public abstract class PointerGetESValueFromPointerPlayer<WithPlayer,T> : PointerXXXESValueFromPointerPlayer<WithPlayer> where WithPlayer:PointerPlayerWithESValue<T>
    {
       
        public T Pick(object a = default, object b = default, object c = default)
        {
            if (withPlayer == null) return default;
            return withPlayer.Get();
        }
    }
    //应用基类
    [TypeRegistryItem("基类应用ES值(任意)_到针播放器", "ES值")]
    public abstract class PointerToApplyValueToESValuePlayer<WithPlayer, T> : PointerXXXESValueFromPointerPlayer<WithPlayer>,IPointerNone where WithPlayer : PointerPlayerWithESValue<T>
    {
        public abstract T GetTheEndValue { get; }
        public object Pick(object a = default, object b = default, object c = default)
        {
            if (withPlayer == null) return default;
            return withPlayer.Set(GetTheEndValue);
        }
    }
    //四类应用
    [TypeRegistryItem("设置ES值(针)_到针播放器", "ES值")]
    public abstract class PointerSetPointerESValueFromPointerPlayer<WithPlayer,T> : PointerToApplyValueToESValuePlayer<WithPlayer, T> where WithPlayer : PointerPlayerWithESValue<T>
    {
        
       
    }
    [TypeRegistryItem("设置ES值(直接)_到针播放器", "ES值")]
    public abstract class PointerSetDirectESValueFromPointerPlayer<WithPlayer, T> : PointerToApplyValueToESValuePlayer<WithPlayer, T> where WithPlayer : PointerPlayerWithESValue<T>
    {
        [LabelText("直接输入设置值")] public T direct; 
        public override T GetTheEndValue => direct;
    }
    [TypeRegistryItem("特殊操作ES值(针)_到针播放器", "ES值")]
    public abstract class PointerHandlePointerESValueFromPointerPlayer<WithPlayer, T> : PointerToApplyValueToESValuePlayer<WithPlayer, T> where WithPlayer : PointerPlayerWithESValue<T>
    {
        public override T GetTheEndValue => GetHandleValue();
        public virtual T GetHandleValue()
        {
            return default;
        }
    }
    [TypeRegistryItem("特殊操作ES值(直接)_到针播放器", "ES值")]
    public abstract class PointerHandleDirectESValueFromPointerPlayer<WithPlayer, T> : PointerToApplyValueToESValuePlayer<WithPlayer, T> where WithPlayer : PointerPlayerWithESValue<T>
    {
        [LabelText("直接输入操作值")] public T direct;
        public override T GetTheEndValue => GetHandleValue();
        public virtual T GetHandleValue()
        {
            return direct;
        }
    }
    #endregion
    #region ES值的Get功能
    [Serializable,TypeRegistryItem("获得浮点数ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_Float: PointerGetESValueFromPointerPlayer<PointerPlayerWithFloat_ESValue,float>,IPointerForFloat_Only
    {

    }
    [Serializable, TypeRegistryItem("获得整数ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_Int : PointerGetESValueFromPointerPlayer<PointerPlayerWithInt_ESValue, int>, IPointerForInt_Only
    {

    }
    [Serializable, TypeRegistryItem("获得字符串ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_String : PointerGetESValueFromPointerPlayer<PointerPlayerWithString_ESValue, string>, IPointerForString_Only
    {

    }
    [Serializable, TypeRegistryItem("获得Vector3ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_Vector3 : PointerGetESValueFromPointerPlayer<PointerPlayerWithVector3_ESValue, Vector3>, IPointerForVector3_Only
    {

    }
    [Serializable, TypeRegistryItem("获得Vector2ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_Vector2 : PointerGetESValueFromPointerPlayer<PointerPlayerWithVector2_ESValue, Vector2>, IPointerForVector2_Only
    {

    }
    [Serializable, TypeRegistryItem("获得布尔ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_Bool : PointerGetESValueFromPointerPlayer<PointerPlayerWithBool_ESValue, bool>, IPointerForBool_Only
    {

    }
    [Serializable, TypeRegistryItem("获得系统物体ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_SystemObject : PointerGetESValueFromPointerPlayer<PointerPlayerWithSystemObject_ESValue, object>, IPointer
    {

    }
    [Serializable, TypeRegistryItem("获得Unity物体ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_UnityObject : PointerGetESValueFromPointerPlayer<PointerPlayerWithUnityObject_ESValue, UnityEngine.Object>, IPointerForUnityObject_Only
    {
        
    }
    [Serializable, TypeRegistryItem("获得类型ES值_从针播放器", "ES值")]
    public class PointerGetESValueFromPointerPlayer_Type : PointerGetESValueFromPointerPlayer<PointerPlayerWithType_ESValue,Type>, IPointerForType_Only
    {

    }
    #endregion
    #region ES值的直接Set功能
    [Serializable, TypeRegistryItem("直接设置浮点数ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_Float : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithFloat_ESValue, float>
    {
        
    }
    [Serializable, TypeRegistryItem("直接设置整数ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_Int : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithInt_ESValue, int>
    {

    }
    [Serializable, TypeRegistryItem("直接设置字符串ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_String : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithString_ESValue, string>
    {

    }
    [Serializable, TypeRegistryItem("直接设置Vector3ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_Vector3 : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithVector3_ESValue, Vector3>
    {

    }
    [Serializable, TypeRegistryItem("直接设置Vector2ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_Vector2 : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithVector2_ESValue, Vector2>
    {

    }
    [Serializable, TypeRegistryItem("直接设置布尔ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_Bool : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithBool_ESValue, bool>
    {

    }
    [Serializable, TypeRegistryItem("直接设置系统物体ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_SystemObject : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithSystemObject_ESValue, object>
    {

    }
    [Serializable, TypeRegistryItem("直接设置Unity物体ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_UnityObject : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithUnityObject_ESValue, UnityEngine.Object>
    {

    }
    [Serializable, TypeRegistryItem("直接设置类型ES值_从针播放器", "ES值")]
    public class PointerSetDirectESValueFromPointerPlayer_Type : PointerSetDirectESValueFromPointerPlayer<PointerPlayerWithType_ESValue, Type>
    {

    }
    #endregion

    #region ES值的直接Handle功能
    [Serializable, TypeRegistryItem("直接特殊操作浮点数ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_Float : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithFloat_ESValue, float>
    {
        [LabelText("选择的浮点数特殊操作")] public EnumCollect.HandleTwoFloatFunction function;
        public override float GetHandleValue()
        {
            return KeyValueMatchingUtility.Function.FunctionForTwoFloat(withPlayer.ESValue,direct, function);
        }
    }
    [Serializable, TypeRegistryItem("直接特殊操作整数ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_Int : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithInt_ESValue, int>
    {
        [LabelText("选择的整数特殊操作")] public EnumCollect.HandleTwoFloatFunction function;
        public override int GetHandleValue()
        {
            return (int)KeyValueMatchingUtility.Function.FunctionForTwoFloat(withPlayer.ESValue, direct, function);
        }
    }
    [Serializable, TypeRegistryItem("直接特殊操作字符串ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_String : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithString_ESValue, string>
    {

    }
    [Serializable, TypeRegistryItem("直接特殊操作Vector3ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_Vector3 : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithVector3_ESValue, Vector3>
    {

    }
    [Serializable, TypeRegistryItem("直接特殊操作Vector2ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_Vector2 : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithVector2_ESValue, Vector2>
    {

    }
    [Serializable, TypeRegistryItem("直接特殊操作布尔ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_Bool : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithBool_ESValue, bool>
    {
        
    }
    [Serializable, TypeRegistryItem("直接特殊操作系统物体ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_SystemObject : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithSystemObject_ESValue, object>
    {

    }
    [Serializable, TypeRegistryItem("直接特殊操作Unity物体ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_UnityObject : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithUnityObject_ESValue, UnityEngine.Object>
    {

    }
    [Serializable, TypeRegistryItem("直接特殊操作类型ES值_从针播放器", "ES值")]
    public class PointerHandleDirectESValueFromPointerPlayer_Type : PointerHandleDirectESValueFromPointerPlayer<PointerPlayerWithType_ESValue, Type>
    {

    }
    #endregion

    #region ES值的针Set功能
    [Serializable, TypeRegistryItem("针设置浮点数ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_Float : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithFloat_ESValue, float>
    {
        [LabelText("设置的浮点数来源"),SerializeReference] public IPointerForFloat_Only pointerFor = new PointerForFloat_Direct();
        public override float GetTheEndValue => pointerFor?.Pick()??default;
    }
    [Serializable, TypeRegistryItem("针设置整数ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_Int : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithInt_ESValue, int>
    {
        [LabelText("设置的整数来源"), SerializeReference] public IPointerForInt_Only pointerFor = new PointerForInt_Direct();
        public override int GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针设置字符串ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_String : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithString_ESValue, string>
    {
        [LabelText("设置的整数来源"), SerializeReference] public IPointerForString_Only pointerFor = new PointerForString_Direc();
        public override string GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针设置Vector3ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_Vector3 : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithVector3_ESValue, Vector3>
    {
        [LabelText("设置的Vector3来源"), SerializeReference] public IPointerForVector3_Only pointerFor = new PointerForVector3_Direct();
        public override Vector3 GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针设置Vector2ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_Vector2 : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithVector2_ESValue, Vector2>
    {
        [LabelText("设置的Vector2来源"), SerializeReference] public IPointerForVector2_Only pointerFor = new PointerForVector2_Direct();
        public override Vector2 GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针设置布尔ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_Bool : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithBool_ESValue, bool>
    {
        [LabelText("设置的布尔值来源"), SerializeReference] public IPointerForBool_Only pointerFor = new PointerForBool_Direc();
        public override bool GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针设置系统物体ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_SystemObject : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithSystemObject_ESValue, object>
    {
        [LabelText("设置的系统物体来源"), SerializeReference] public IPointer pointerFor = new PointerForInt_Direct();
        public override object GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针设置Unity物体ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_UnityObject : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithUnityObject_ESValue, UnityEngine.Object>
    {
        [LabelText("设置的Unity物体来源"), SerializeReference] public IPointerForUnityObject_Only pointerFor=new PointerForUnityObejct_Direct();
        public override UnityEngine.Object GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针设置类型ES值_从针播放器", "ES值")]
    public class PointerSetPointerESValueFromPointerPlayer_Type : PointerSetPointerESValueFromPointerPlayer<PointerPlayerWithType_ESValue, Type>
    {
        [LabelText("设置的类型来源"), SerializeReference] public IPointerForType_Only pointerFor;
        public override Type GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    #endregion
    #region ES值的针Handle功能
    [Serializable, TypeRegistryItem("针特殊操作浮点数ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_Float : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithFloat_ESValue, float>
    {
        [LabelText("选择的浮点数特殊操作")] public EnumCollect.HandleTwoFloatFunction function;
        [LabelText("特殊操作的浮点数来源"), SerializeReference] public IPointerForFloat_Only pointerFor = new PointerForFloat_Direct();
        
        
        public override float GetHandleValue()
        {
            return KeyValueMatchingUtility.Function.FunctionForTwoFloat(withPlayer.ESValue, pointerFor?.Pick() ?? default, function);
        }
    }
    [Serializable, TypeRegistryItem("针特殊操作整数ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_Int : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithInt_ESValue, int>
    {
        [LabelText("选择的整数特殊操作")] public EnumCollect.HandleTwoFloatFunction function;
        [LabelText("特殊操作的整数来源"), SerializeReference] public IPointerForInt_Only pointerFor = new PointerForInt_Direct();

        
        public override int GetHandleValue()
        {
            return (int)KeyValueMatchingUtility.Function.FunctionForTwoFloat(withPlayer.ESValue, pointerFor?.Pick() ?? default, function);
        }
    }
    [Serializable, TypeRegistryItem("针特殊操作字符串ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_String : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithString_ESValue, string>
    {
        [LabelText("特殊操作的整数来源"), SerializeReference] public IPointerForString_Only pointerFor = new PointerForString_Direc();
        public override string GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针特殊操作Vector3ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_Vector3 : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithVector3_ESValue, Vector3>
    {
        [LabelText("特殊操作的Vector3来源"), SerializeReference] public IPointerForVector3_Only pointerFor = new PointerForVector3_Direct();
        public override Vector3 GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针特殊操作Vector2ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_Vector2 : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithVector2_ESValue, Vector2>
    {
        [LabelText("特殊操作的Vector2来源"), SerializeReference] public IPointerForVector2_Only pointerFor = new PointerForVector2_Direct();
        public override Vector2 GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针特殊操作布尔ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_Bool : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithBool_ESValue, bool>
    {
        [LabelText("特殊操作的布尔值来源"), SerializeReference] public IPointerForBool_Only pointerFor = new PointerForBool_Direc();
        public override bool GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针特殊操作系统物体ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_SystemObject : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithSystemObject_ESValue, object>
    {
        [LabelText("特殊操作的系统物体来源"), SerializeReference] public IPointer pointerFor = new PointerForInt_Direct();
        public override object GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针特殊操作Unity物体ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_UnityObject : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithUnityObject_ESValue, UnityEngine.Object>
    {
        [LabelText("特殊操作的Unity物体来源"), SerializeReference] public IPointerForUnityObject_Only pointerFor = new PointerForUnityObejct_Direct();
        public override UnityEngine.Object GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("针特殊操作类型ES值_从针播放器", "ES值")]
    public class PointerHandlePointerESValueFromPointerPlayer_Type : PointerHandlePointerESValueFromPointerPlayer<PointerPlayerWithType_ESValue, Type>
    {
        [LabelText("特殊操作的类型来源"), SerializeReference] public IPointerForType_Only pointerFor;
        public override Type GetTheEndValue => pointerFor?.Pick() ?? default;
    }
    #endregion
}

