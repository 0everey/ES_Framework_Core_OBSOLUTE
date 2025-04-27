using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于动画器 Anamator部分
    #region 反射的直接Get和Set部分(IPointerNone)
        #region 反射的Get部分(接口在前)
    public interface IPointerReflectionGetter<T> : IPointerOnlyBack<T>
    {

    }
    //抽象父类 字段Get
    [TypeRegistryItem("抽象_反射_获得字段", "反射/获得")]
    public abstract class PointerReflectionFieldGetter<T> : IPointerReflectionGetter<T>
    {
        [LabelText("任意物体"), SerializeReference] public IPointer every;
        [LabelText("字段名")] public string fieldName;
        public T Pick(object by = null, object yarn = null, object on = null)
        {
            if (every != null)
            {
                var o = every.Pick();
                Debug.Log("尝试" + o);
                if (o != null)
                {
                    T t = KeyValueMatchingUtility.Reflection.EasyGetField<T>(o, fieldName);
                    Debug.Log("获得" + t);
                    return t;
                }
            }
            return default;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    [TypeRegistryItem("抽象_反射_获得属性", "反射/获得")]
    public abstract class PointerReflectionPropertyGetter<T> : IPointerReflectionGetter<T>
    {
        [LabelText("任意物体"), SerializeReference] public IPointer every;
        [LabelText("属性名")] public string propertyInfo;
        public T Pick(object by = null, object yarn = null, object on = null)
        {
            if (every != null)
            {
                var o = every.Pick();
                Debug.Log("尝试" + o);
                if (o != null)
                {
                    T t = KeyValueMatchingUtility.Reflection.EasyGetProperty<T>(o, propertyInfo);
                    Debug.Log("获得" + t);
                    return t;
                }
            }
            return default;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    [TypeRegistryItem("抽象_反射_获得方法", "反射/获得")]
    public abstract class PointerReflectionMethodGetter<T> : IPointerReflectionGetter<T> where T : Delegate
    {
        [LabelText("任意物体"), SerializeReference] public IPointer every;
        [LabelText("方法名")] public string fieldName;
        public T Pick(object by = null, object yarn = null, object on = null)
        {
            if (every != null)
            {
                var o = every.Pick();
                if (o != null)
                {
                    return KeyValueMatchingUtility.Reflection.EasyGetMethod<T>(o, fieldName);
                }
            }
            return default;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }

    //功能
    public class PointerForSystemObject_Field_Reflection : PointerReflectionFieldGetter<object>, IPointer
    {

    }
    [Serializable, TypeRegistryItem("反射_获得浮点数_字段", "反射/获得")]
    public class PointerForFloat_Field_Reflection : PointerReflectionFieldGetter<float>, IPointerForFloat_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得整数_字段", "反射/获得")]
    public class PointerForInt_Field_Reflection : PointerReflectionFieldGetter<int>, IPointerForInt_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得Vector3_字段", "反射/获得")]
    public class PointerForVector3_Field_Reflection : PointerReflectionFieldGetter<Vector3>, IPointerForVector3_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得Vector2_字段", "反射/获得")]
    public class PointerForVector2_Field_Reflection : PointerReflectionFieldGetter<Vector2>, IPointerForVector2_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得游戏物体_字段", "反射/获得")]
    public class PointerForGameobject_Field_Reflection : PointerReflectionFieldGetter<GameObject>, IPointerForGameObject_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得Unity物体_字段", "反射/获得")]
    public class PointerForUnityObject_Field_Reflection : PointerReflectionFieldGetter<UnityEngine.Object>, IPointerForUnityObject_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得布尔值_字段", "反射/获得")]
    public class PointerForBool_Field_Reflection : PointerReflectionFieldGetter<bool>, IPointerForBool_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得字符串_字段", "反射/获得")]
    public class PointerForString_Field_Reflection : PointerReflectionFieldGetter<string>, IPointerForString_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得变换_字段", "反射/获得")]
    public class PointerForTransform_Field_Reflection : PointerReflectionFieldGetter<Transform>, IPointerForTransform_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得四元数体_字段", "反射/获得")]
    public class PointerForQuaternion_Field_Reflection : PointerReflectionFieldGetter<Quaternion>, IPointerForQuaternion_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得脚本组件_字段", "反射/获得")]
    public class PointerForCompoent_Field_Reflection : PointerReflectionFieldGetter<Component>, IPointerForComponent_Only
    {

    }
    //属性支持
    [Serializable, TypeRegistryItem("反射_获得系统物体_属性", "反射/获得")]
    public class PointerForSystemObject_Property_Reflection : PointerReflectionPropertyGetter<object>, IPointer
    {

    }
    [Serializable, TypeRegistryItem("反射_获得浮点数_属性", "反射/获得")]
    public class PointerForFloat_Property_Reflection : PointerReflectionPropertyGetter<float>, IPointerForFloat_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得整数_属性", "反射/获得")]
    public class PointerForInt_Property_Reflection : PointerReflectionPropertyGetter<int>, IPointerForInt_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得Vector3_属性", "反射/获得")]
    public class PointerForVector3_Property_Reflection : PointerReflectionPropertyGetter<Vector3>, IPointerForVector3_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得Vector2_属性", "反射/获得")]
    public class PointerForVector2_Property_Reflection : PointerReflectionPropertyGetter<Vector2>, IPointerForVector2_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得游戏物体_属性", "反射/获得")]
    public class PointerForGameobject_Property_Reflection : PointerReflectionPropertyGetter<GameObject>, IPointerForGameObject_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得Unity物体_属性", "反射/获得")]
    public class PointerForUnityObject_Property_Reflection : PointerReflectionPropertyGetter<UnityEngine.Object>, IPointerForUnityObject_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得布尔值_属性", "反射/获得")]
    public class PointerForBool_Property_Reflection : PointerReflectionPropertyGetter<bool>, IPointerForBool_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得字符串_属性", "反射/获得")]
    public class PointerForString_Property_Reflection : PointerReflectionPropertyGetter<string>, IPointerForString_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得变换_属性", "反射/获得")]
    public class PointerForTransform_Property_Reflection : PointerReflectionPropertyGetter<Transform>, IPointerForTransform_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得四元数_属性", "反射/获得")]
    public class PointerForQuaternion_Property_Reflection : PointerReflectionPropertyGetter<Quaternion>, IPointerForQuaternion_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得脚本组件_属性", "反射/获得")]
    public class PointerForCompoent_Property_Reflection : PointerReflectionPropertyGetter<Component>, IPointerForComponent_Only
    {

    }
    [Serializable, TypeRegistryItem("反射_获得成员方法", "反射/获得")]
    public class PointerForDelegate_Reflection : PointerReflectionMethodGetter<Delegate>
    {

    }
    #endregion
        #region 反射的Set部分
    //SetField
    [TypeRegistryItem("抽象_反射_设置字段", "反射/设置")]
    public abstract class PointerReflectionFieldSetter<T> : IPointerNone
    {
        [LabelText("任意物体"), SerializeReference] public IPointer every;
        [LabelText("字段名")] public string fieldName;
        public virtual T getValue { get; }
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (every != null)
            {
                var o = every.Pick();
                if (o != null)
                {
                    Handle(o);
                }
            }
            return default;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
        public virtual void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasySetField<T>(o, fieldName, getValue);
        }
    }
    [TypeRegistryItem("抽象_反射_设置属性", "反射/设置")]
    public abstract class PointerReflectionPropertySetter<T> : IPointerNone
    {
        [LabelText("任意物体"), SerializeReference] public IPointer every;
        [LabelText("属性名")] public string propertyName;
        public virtual T getValue { get; }
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (every != null)
            {
                var o = every.Pick();
                if (o != null)
                {
                    Handle(o);
                }
            }
            return default;
        }
        public virtual void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasySetProperty<T>(o, propertyName, getValue);
        }
    }
    [TypeRegistryItem("抽象_反射_调用方法", "反射/设置")]
    public abstract class PointerReflectionMethodInvoke : IPointerNone
    {
        [LabelText("任意物体"), SerializeReference] public IPointer every;
        [LabelText("方法名")] public string fieldName;
        [LabelText("参数列表"), SerializeReference] public List<IPointer> objects;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (every != null)
            {
                var o = every.Pick();
                if (o != null)
                {
                    List<object> os = new List<object>();
                    if (objects != null)
                    {
                        foreach (var i in objects)
                        {
                            object oo;
                            os.Add(oo = i?.Pick() ?? null);
                            Debug.Log("预期参数" + oo);
                        }
                    }
                    Debug.Log("调用");
                    KeyValueMatchingUtility.Reflection.EasyInvokeMethod(o, fieldName, os.ToArray());
                }
            }
            return default;
        }
    }
    [Serializable, TypeRegistryItem("反射_设置游戏物体_字段", "反射/设置")]
    public class PointerSetSystemObject_Reflection : PointerReflectionFieldSetter<object>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointer systemObject;
        public override object getValue => systemObject?.Pick() ?? null;
    }
    [Serializable, TypeRegistryItem("反射_设置浮点数_字段", "反射/设置")]
    public class PointerSetFloat_Field_Reflection : PointerReflectionFieldSetter<float>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForFloat_Only float_;
        public override float getValue => float_?.Pick() ?? 1;
    }
    [Serializable, TypeRegistryItem("反射_特殊操作浮点数_字段", "反射/设置")]
    public class PointerHanldeFloat_Field_Reflection : PointerSetFloat_Field_Reflection
    {
        [LabelText("使用的处理")] public EnumCollect.HandleTwoFloatFunction hanlde;
        public override void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasyHandleField<float>(o, fieldName, getValue, hanlde);
        }
    }
    [Serializable, TypeRegistryItem("反射_设置整数_字段", "反射/设置")]
    public class PointerSetInt_Field_Reflection : PointerReflectionFieldSetter<int>, IPointerNone
    {

        [LabelText("设置的值"), SerializeReference] public IPointerForInt_Only int_;
        public override int getValue => int_?.Pick() ?? 1;
    }
    [Serializable, TypeRegistryItem("反射_特殊操作整数_字段", "反射/设置")]
    public class PointerHanldeInt_Field_Reflection : PointerSetInt_Field_Reflection
    {
        [LabelText("使用的处理")] public EnumCollect.HandleTwoFloatFunction hanlde;
        public override void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasyHandleField<int>(o, fieldName, getValue, hanlde);
        }
    }
    [Serializable, TypeRegistryItem("反射_特殊操作枚举(当整数处理)_字段", "反射/设置")]
    public class PointerHanldeEnum_Field_Reflection : PointerSetInt_Field_Reflection
    {

        [LabelText("使用的处理")] public EnumCollect.HandleTwoFloatFunction hanlde;

        public override void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasyHandleField<int>(o, fieldName, getValue, hanlde);
        }
    }
    [Serializable, TypeRegistryItem("反射_特殊操作LayerMask_字段", "反射/设置")]
    public class PointerHanldeLayerMask_Field_Reflection : PointerReflectionFieldSetter<int>, IPointerNone
    {
        [LabelText("设置的值")] public LayerMask layerMask_;
        public override int getValue => layerMask_;
        [LabelText("使用的处理")] public EnumCollect.HandleTwoFloatFunction hanlde;

        public override void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasyHandleField<int>(o, fieldName, getValue, hanlde);
        }
    }
    [Serializable, TypeRegistryItem("反射_设置Vector3_字段", "反射/设置")]
    public class PointerSetVector3_Field_Reflection : PointerReflectionFieldSetter<Vector3>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForVector3_Only v3_;
        public override Vector3 getValue => v3_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置Vector2_字段", "反射/设置")]
    public class PointerSetVector2_Field_Reflection : PointerReflectionFieldSetter<Vector2>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForVector2_Only v2_;
        public override Vector2 getValue => v2_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置游戏物体_字段", "反射/设置")]
    public class PointerSetGameObject_Field_Reflection : PointerReflectionFieldSetter<GameObject>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForGameObject_Only g_;
        public override GameObject getValue => g_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置Unity物体_字段", "反射/设置")]
    public class PointerSetUnityObject_Field_Reflection : PointerReflectionFieldSetter<UnityEngine.Object>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForUnityObject_Only uo_;
        public override UnityEngine.Object getValue => uo_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置布尔值_字段", "反射/设置")]
    public class PointerSetBool_Field_Reflection : PointerReflectionFieldSetter<bool>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForBool_Only b_;
        public override bool getValue => b_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置字符串_字段", "反射/设置")]
    public class PointerSetString_Field_Reflection : PointerReflectionFieldSetter<string>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForString_Only str_;
        public override string getValue => str_?.Pick() ?? default;
    }
    //SetPro
    [Serializable, TypeRegistryItem("反射_设置系统物体_属性", "反射/设置")]
    public class PointerSetSystemObject_Property_Reflection : PointerReflectionPropertySetter<object>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointer systemObject;
        public override object getValue => systemObject?.Pick() ?? null;
    }
    [Serializable, TypeRegistryItem("反射_设置浮点数_属性", "反射/设置")]
    public class PointerSetFloat_Property_Reflection : PointerReflectionPropertySetter<float>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForFloat_Only float_;
        public override float getValue => float_?.Pick() ?? 1;
    }
    [Serializable, TypeRegistryItem("反射_特殊操作浮点数_属性", "反射/设置")]
    public class PointerHandleFloat_Property_Reflection : PointerSetFloat_Property_Reflection
    {
        [LabelText("使用的处理")] public EnumCollect.HandleTwoFloatFunction hanlde;
        public override void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasyHandleProperty<float>(o, propertyName, getValue, hanlde);
        }
    }

    [Serializable, TypeRegistryItem("反射_设置整数_属性", "反射/设置")]
    public class PointerSetInt_Property_Reflection : PointerReflectionPropertySetter<int>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForInt_Only int_;
        public override int getValue => int_?.Pick() ?? 1;
    }
    [Serializable, TypeRegistryItem("反射_特殊操作整数_属性", "反射/设置")]
    public class PointerHandleInt_Property_Reflection : PointerSetInt_Property_Reflection
    {
        [LabelText("使用的处理")] public EnumCollect.HandleTwoFloatFunction hanlde;
        public override void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasyHandleProperty<int>(o, propertyName, getValue, hanlde);
        }
    }
    [Serializable, TypeRegistryItem("反射_特殊操作枚举(当Int处理)_属性", "反射/设置")]
    public class PointerHanldeEnum_Property_Reflection : PointerSetInt_Property_Reflection
    {

        [LabelText("使用的处理")] public EnumCollect.HandleTwoFloatFunction hanlde;

        public override void Handle(object o)
        {
            KeyValueMatchingUtility.Reflection.EasyHandleField<int>(o, propertyName, getValue, hanlde);
        }
    }
    [Serializable, TypeRegistryItem("反射_设置Vector3_属性", "反射/设置")]
    public class PointerSetVector3_Property_Reflection : PointerReflectionPropertySetter<Vector3>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForVector3_Only v3_;
        public override Vector3 getValue => v3_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置Vector2_属性", "反射/设置")]
    public class PointerSetVector2_Property_Reflection : PointerReflectionPropertySetter<Vector2>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForVector2_Only v2_;
        public override Vector2 getValue => v2_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置游戏物体_属性", "反射/设置")]
    public class PointerSetGameObject_Property_Reflection : PointerReflectionPropertySetter<GameObject>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForGameObject_Only g_;
        public override GameObject getValue => g_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置Unity物体_属性", "反射/设置")]
    public class PointerSetUnityObject_Property_Reflection : PointerReflectionPropertySetter<UnityEngine.Object>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForUnityObject_Only uo_;
        public override UnityEngine.Object getValue => uo_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置布尔值_属性", "反射/设置")]
    public class PointerSetBool_Property_Reflection : PointerReflectionPropertySetter<bool>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForBool_Only b_;
        public override bool getValue => b_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射_设置字符串_属性", "反射/设置")]
    public class PointerSetString_Property_Reflection : PointerReflectionPropertySetter<string>, IPointerNone
    {
        [LabelText("设置的值"), SerializeReference] public IPointerForString_Only str_;
        public override string getValue => str_?.Pick() ?? default;
    }
    
    [Serializable, TypeRegistryItem("反射_调用成员方法", "反射/设置")]
    public class PointerInvokeDelegate_Reflection : PointerReflectionMethodInvoke, IPointerNone
    {

    }
    #endregion
    #endregion


    #region 反射信息针(XXXInfo_目前不是主流推荐使用上面的)
        #region XXXInfo接口抽象和包
        public interface IPointerForMemberInfo<By, Yarn, On> : IPointer<MemberInfo, By, Yarn, On>
        {

        }
        public interface IPointerForMemberInfo_Only : IPointerForMemberInfo<object, object, object>, IPointerOnlyBack<MemberInfo>
        {

        }
        public interface IPointerForFieldInfo<By, Yarn, On> : IPointer<FieldInfo, By, Yarn, On>
        {

        }
        public interface IPointerForFieldInfo_Only : IPointerForFieldInfo<object, object, object>, IPointerOnlyBack<FieldInfo>
        {
            object IPointer.Pick(object a, object b, object c)
            {
                return Pick();
            }
        }
        public abstract class PointerSetFieldInfo_InfoAndValue<T> : IPointerNone
        {
            [LabelText("对象源"), SerializeReference]
            public IPointer ob;
            [LabelText("字段源"), SerializeReference]
            public IPointerForFieldInfo_Only info;

            public abstract T getValue { get; }

            public object Pick(object by = default, object yarn = default, object on = default)
            {
                if (ob != null && info != null)
                {
                    FieldInfo fieldInfo = info.Pick();
                    Debug.Log(fieldInfo);
                    object oo = ob.Pick();
                    T t = getValue;
                    if (fieldInfo != null && oo != null && t != null)
                    {
                        fieldInfo.SetValue(oo, t);
                    }
                }
                return default;
            }
        }
        #endregion
        #region XXXInfo功能
    [Serializable, TypeRegistryItem("反射信息_获得字段信息_来自类型和名字", "反射信息/获得")]
    public class PointerForFieldInfo_TypeAndName : IPointerForFieldInfo_Only
    {
        [LabelText("类型"), SerializeReference]
        public IPointerForType_Only type;
        [LabelText("字段名称"), SerializeReference]
        public IPointerForString_Only fieldName;
        public FieldInfo Pick(object by, object yarn, object on)
        {
            if (type != null && fieldName != null)
            {
                Type tt = type?.Pick();
                if (tt != null)
                {
                    return tt.GetField(fieldName?.Pick());
                }
            }
            return default;
        }
    }
    //设置字段值 ； 抽象
    [Serializable, TypeRegistryItem("反射信息_使用字段信息_设置任意字段", "反射信息/设置")]
    public class PointerSetFieldInfoSystemObject_InfoAndValue : PointerSetFieldInfo_InfoAndValue<object>
    {
        [LabelText("获得任意对象值"), SerializeReference]
        public IPointer systemObject;
        public override object getValue => systemObject?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射信息_使用字段信息_设置浮点数字段", "反射信息/设置")]
    public class PointerSetFieldInfoFloat_InfoAndValue : PointerSetFieldInfo_InfoAndValue<float>
    {
        [LabelText("获得任意浮点值"), SerializeReference]
        public IPointerForFloat_Only float_ = new PointerForFloat_Direct() { float_ = 1 };
        public override float getValue => float_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射信息_使用字段信息_设置整数字段", "反射信息/设置")]
    public class PointerSetFieldInfoInt_InfoAndValue : PointerSetFieldInfo_InfoAndValue<int>
    {
        [LabelText("任意整数值"), SerializeReference]
        public IPointerForInt_Only int_;
        public override int getValue => int_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射信息_使用字段信息_设置字符串字段", "反射信息/设置")]
    public class PointerSetFieldInfoString_InfoAndValue : PointerSetFieldInfo_InfoAndValue<string>
    {
        [LabelText("任意字符串值"), SerializeReference]
        public IPointerForString_Only string_;
        public override string getValue => string_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射信息_使用字段信息_设置Vector3字段", "反射信息/设置")]
    public class PointerSetFieldInfoVector3_InfoAndValue : PointerSetFieldInfo_InfoAndValue<Vector3>
    {
        [LabelText("任意Vector3值"), SerializeReference]
        public IPointerForVector3_Only v3_;
        public override Vector3 getValue => v3_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射信息_使用字段信息_设置Vector2字段", "反射信息/设置")]
    public class PointerSetFieldInfoVector2_InfoAndValue : PointerSetFieldInfo_InfoAndValue<Vector2>
    {
        [LabelText("任意Vector2值"), SerializeReference]
        public IPointerForVector2_Only v2_;
        public override Vector2 getValue => v2_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射信息_使用字段信息_设置布尔值字段", "反射信息/设置")]
    public class PointerSetFieldInfoBool_InfoAndValue : PointerSetFieldInfo_InfoAndValue<bool>
    {
        [LabelText("任意Vector3值"), SerializeReference]
        public IPointerForBool_Only bool_;
        public override bool getValue => bool_?.Pick() ?? default;
    }
    [Serializable, TypeRegistryItem("反射信息_使用字段信息_设置Unity物体字段", "反射信息/设置")]
    public class PointerSetFieldInfoUnityObject_InfoAndValue : PointerSetFieldInfo_InfoAndValue<UnityEngine.Object>
    {
        [LabelText("任意Vector2值"), SerializeReference]
        public IPointerForUnityObject_Only uo_;
        public override UnityEngine.Object getValue => uo_?.Pick() ?? default;
    }

    #endregion
    #endregion
}

