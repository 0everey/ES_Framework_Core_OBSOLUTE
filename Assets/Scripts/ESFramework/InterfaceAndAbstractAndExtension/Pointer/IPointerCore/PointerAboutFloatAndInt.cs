using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于Float 和 Int 部分
    #region Float部分
        #region Float接口抽象与包
    public interface IPointerForFloat<On, From, With> : IPointer<float, On, From, With>
    {

    }
    public interface IPointerForFloat_Only : IPointerOnlyBackSingle<float>, IPointerForFloat<object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerForFloat_Only)?.Pick();
        }
    }
    public interface IPointerForFloatList : IPointerOnlyBackList<float>
    {

    }
    [Serializable, TypeRegistryItem("浮点数针包_选中几个")]
   
    public class PointerForFloat_PackerSelectSome : PointerPackerForSelectSomeBack<float, IPointerForFloat_Only>, IPointerForFloatList
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerPackerForSelectSomeBack<float, IPointerForFloat_Only>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("浮点数针包_选中一个")]
    public class PointerForFloat_PackerSelect : PointerPackerForOnlySelectBackOne<float, IPointerForFloat_Only>, IPointerForFloat_Only
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerPackerForOnlySelectBackOne<float, IPointerForFloat_Only>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("浮点值包_选中几个")]
    public class PointerForFloatValueListSelectSome : PointerForValueListSelectSomeBack<float>, IPointerForFloatList
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerForValueListSelectSomeBack<float>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("浮点值包_选中一个")]
    public class PointerForFloatValueListSelect : PointerForValueListSelectBackOne<float>, IPointerForFloat_Only
    {

    }
    #endregion
        #region Float功能
    [Serializable, TypeRegistryItem("浮点数_加", "单值针/浮点数针")]
    public class PointerForFloat_Add : IPointerForFloat_Only
    {
        [LabelText("加float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("加float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (float_Only_1?.Pick() ?? default) + (float_Only_2?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("浮点数_投射", "单值针/浮点数针")]
    public class PointerForFloat_Caster : IPointerForFloat_Only, IPointerOnlyBackCaster<float>
    {
        [LabelText("抓获")] public float f;
        [LabelText("抓取")] public IPointerForFloat_Only fP = new PointerForFloat_Direct() { float_ = 1 };
        public float Cast()
        {
            return f;
        }

        public float Pick(object on= null, object from = null, object with = null)
        {
            return f = fP?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("浮点数减", "单值针/浮点数针")]
    public class PointerForFloat_Sub : IPointerForFloat_Only
    {
        [LabelText("减float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("减float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (float_Only_1?.Pick() ?? default) - (float_Only_2?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("浮点数乘以", "单值针/浮点数针")]
    public class PointerForFloat_Muti : IPointerForFloat_Only
    {
        [LabelText("乘float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("乘float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (float_Only_1?.Pick() ?? 1) * (float_Only_2?.Pick() ?? 1);
        }
    }
    [Serializable, TypeRegistryItem("浮点数除以", "单值针/浮点数针")]
    public class PointerForFloat_Divide : IPointerForFloat_Only
    {
        [LabelText("除float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("除float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (float_Only_1?.Pick() ?? 0) / (float_Only_2?.Pick() ?? 1);
        }
    }
    [Serializable, TypeRegistryItem("浮点数取余", "单值针/浮点数针")]
    public class PointerForFloat_Model : IPointerForFloat_Only
    {
        [LabelText("余float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("余float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (float_Only_1?.Pick() ?? 0) % (float_Only_2?.Pick() ?? 1);
        }
    }
    [Serializable, TypeRegistryItem("浮点数Power幂", "单值针/浮点数针")]
    public class PointerForFloat_Power : IPointerForFloat_Only
    {
        [LabelText("底数float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("指数float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return Mathf.Pow((float_Only_1?.Pick() ?? 0), (float_Only_2?.Pick() ?? 1));
        }
    }
    [Serializable, TypeRegistryItem("浮点数取最小", "单值针/浮点数针")]
    public class PointerForFloat_Min : IPointerForFloat_Only
    {
        [LabelText("最小float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("最小float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return Mathf.Min((float_Only_1?.Pick() ?? 0), (float_Only_2?.Pick() ?? 1));
        }
    }
    [Serializable, TypeRegistryItem("浮点数取最大", "单值针/浮点数针")]
    public class PointerForFloat_Max : IPointerForFloat_Only
    {
        [LabelText("最大float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("最大float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return Mathf.Max((float_Only_1?.Pick() ?? 0), (float_Only_2?.Pick() ?? 1));
        }
    }
    [Serializable, TypeRegistryItem("浮点数绝对距离", "单值针/浮点数针")]
    public class PointerForFloat_AbsDistance : IPointerForFloat_Only
    {
        [LabelText("绝对距离float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("绝对距离float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            return Mathf.Abs((float_Only_1?.Pick() ?? 0) - (float_Only_2?.Pick() ?? 0));
        }
    }
    [Serializable, TypeRegistryItem("浮点数插值", "单值针/浮点数针")]
    public class PointerForFloat_Lerp : IPointerForFloat_Only
    {
        [LabelText("起始float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("终点float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("Lerp_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t;
        [LabelText("取消钳制")] public bool clampNot;

        public float Pick(object on= null, object from = null, object with = null)
        {
            float a = (float_Only_1?.Pick() ?? 0);
            float b = (float_Only_2?.Pick() ?? 1);
            float t = (float_Only_t?.Pick() ?? 0);
            if (!clampNot) return Mathf.Lerp(a, b, t);
            else return Mathf.LerpUnclamped(a, b, t);
        }
    }
    [Serializable, TypeRegistryItem("浮点数平滑Step", "单值针/浮点数针")]
    public class PointerForFloat_SmoothStep : IPointerForFloat_Only
    {
        [LabelText("起始float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("终点float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_2 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("SmoothStep_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            float a = (float_Only_1?.Pick() ?? 0);
            float b = (float_Only_2?.Pick() ?? 1);
            float t = (float_Only_t?.Pick() ?? 0);
            return Mathf.SmoothStep(a, b, t);
        }
    }
    [Serializable, TypeRegistryItem("浮点数标志Sign", "单值针/浮点数针")]
    public class PointerForFloat_Sign : IPointerForFloat_Only
    {

        [LabelText("Sign_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {

            float t = (float_Only_t?.Pick() ?? 0);
            return Mathf.Sign(t);
        }
    }
    [Serializable, TypeRegistryItem("浮点数Cos", "单值针/浮点数针")]
    public class PointerForFloat_Cos : IPointerForFloat_Only
    {

        [LabelText("Cos_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t = new PointerForFloat_Direct() { float_ = Mathf.PI };
        public float Pick(object on= null, object from = null, object with = null)
        {

            float t = (float_Only_t?.Pick() ?? 0);
            return Mathf.Cos(t);
        }
    }
    [Serializable, TypeRegistryItem("浮点数Sin", "单值针/浮点数针")]
    public class PointerForFloat_Sin : IPointerForFloat_Only
    {

        [LabelText("Sin_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t = new PointerForFloat_Direct() { float_ = Mathf.PI };
        public float Pick(object on= null, object from = null, object with = null)
        {

            float t = (float_Only_t?.Pick() ?? 0);
            return Mathf.Sin(t);
        }
    }
    [Serializable, TypeRegistryItem("浮点数Tan", "单值针/浮点数针")]
    public class PointerForFloat_Tan : IPointerForFloat_Only
    {

        [LabelText("Tan_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t = new PointerForFloat_Direct() { float_ = Mathf.PI / 4 };
        public float Pick(object on= null, object from = null, object with = null)
        {

            float t = (float_Only_t?.Pick() ?? 0);
            return Mathf.Tan(t);
        }
    }
    [Serializable, TypeRegistryItem("浮点数弧度转角度", "单值针/浮点数针")]
    public class PointerForFloat_RadToDeg : IPointerForFloat_Only
    {

        [LabelText("Rad源ToAngle"), SerializeReference] public IPointerForFloat_Only float_Only_t = new PointerForFloat_Direct() { float_ = Mathf.PI };
        public float Pick(object on= null, object from = null, object with = null)
        {

            float t = (float_Only_t?.Pick() ?? 0);
            return t * Mathf.Rad2Deg;
        }
    }
    [Serializable, TypeRegistryItem("浮点数角度转弧度", "单值针/浮点数针")]
    public class PointerForFloat_DegToRad : IPointerForFloat_Only
    {

        [LabelText("Angle源ToDeg"), SerializeReference] public IPointerForFloat_Only float_Only_t = new PointerForFloat_Direct() { float_ = 180 };
        public float Pick(object on= null, object from = null, object with = null)
        {

            float t = (float_Only_t?.Pick() ?? 0);
            return t * Mathf.Deg2Rad;
        }
    }
    [Serializable, TypeRegistryItem("浮点数弧度转角度", "单值针/浮点数针")]
    public class PointerForFloat_Time : IPointerForFloat_Only
    {
        public float Pick(object on= null, object from = null, object with = null)
        {
            return Time.time;
        }
    }
    [Serializable, TypeRegistryItem("浮点数已经过去的时间(可初始化)", "单值针/浮点数针")]
    public class PointerForFloat_TimeHasGo : IPointerForFloat_Only, IInittable
    {
        private bool hasInit = false;
        [LabelText("开始时间")] public float startTime;

        public void Init(params object[] ps)
        {
            startTime = Time.time;
            hasInit = true;
        }

        public float Pick(object on= null, object from = null, object with = null)
        {
            if (!hasInit) Init();
            return Time.time - startTime;
        }
    }
    [Serializable, TypeRegistryItem("浮点数重映射", "单值针/浮点数针")]
    public class PointerForFloat_Remap : IPointerForFloat_Only
    {
        [LabelText("预期值"), SerializeReference] public IPointerForFloat_Only float_Only_t = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("起点下界float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_startDown_1 = new PointerForFloat_Direct() { float_ = 0 };
        [LabelText("起点上届float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_startUp_2 = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("终点下界float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_endDown_1 = new PointerForFloat_Direct() { float_ = 0 };
        [LabelText("终点上届float_2源"), SerializeReference] public IPointerForFloat_Only float_Only_endUp_2 = new PointerForFloat_Direct() { float_ = 1 };
        public float Pick(object on= null, object from = null, object with = null)
        {
            float a1 = float_Only_startDown_1?.Pick() ?? 0;
            float a2 = float_Only_startUp_2?.Pick() ?? 1;
            float b1 = float_Only_endDown_1?.Pick() ?? 0;
            float b2 = float_Only_endUp_2?.Pick() ?? 1;
            float value = float_Only_t?.Pick() ?? 1;
            if (a1 == a2 || b1 == b2) { Debug.LogError("映射针--上下界一致导致除0"); return 0; }
            float t = (value - a1) / (a2 - a1);
            return b1 + (b2 - b1) * t;
        }
    }
    [Serializable, TypeRegistryItem("浮点数开平方根", "单值针/浮点数针")]
    public class PointerForFloat_Sqrt : IPointerForFloat_Only
    {
        [LabelText("平方根float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };

        public float Pick(object on= null, object from = null, object with = null)
        {
            return Mathf.Sqrt(float_Only_1?.Pick() ?? 1);
        }
    }
    [Serializable, TypeRegistryItem("浮点数倒数", "单值针/浮点数针")]
    public class PointerForFloat_Recip : IPointerForFloat_Only
    {
        [LabelText("倒数float_1源"), SerializeReference] public IPointerForFloat_Only float_Only_1 = new PointerForFloat_Direct() { float_ = 1 };

        public float Pick(object on= null, object from = null, object with = null)
        {
            return 1 / (float_Only_1?.Pick() ?? 1);
        }
    }
    [Serializable, TypeRegistryItem("浮点数_Vector3的长度", "单值针/浮点数针")]
    public class PointerForFloat_LengthOfVector3 : IPointerForFloat_Only
    {
        [LabelText("Vector3源"), SerializeReference] public IPointerForVector3_Only vector3_Only;
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (vector3_Only?.Pick().magnitude) ?? default;
        }
    }
    [Serializable, TypeRegistryItem("浮点数两个Vector3距离", "单值针/浮点数针")]
    public class PointerForFloat_DistanceOfVector3 : IPointerForFloat_Only
    {
        [LabelText("Vector3源_1"), SerializeReference] public IPointerForVector3_Only vector3_Only1;
        [LabelText("Vector3源_2"), SerializeReference] public IPointerForVector3_Only vector3_Only2;
        public float Pick(object on= null, object from = null, object with = null)
        {
            Vector3 v1 = (vector3_Only1?.Pick()) ?? default;
            Vector3 v2 = (vector3_Only2?.Pick()) ?? default;
            return Vector3.Distance(v1, v2);
        }
    }
    [Serializable, TypeRegistryItem("浮点数_Vector3夹角", "单值针/浮点数针")]
    public class PointerForFloat_AngleOfVector3 : IPointerForFloat_Only
    {
        [LabelText("Vector3源_1"), SerializeReference] public IPointerForVector3_Only vector3_Only1;
        [LabelText("Vector3源_2"), SerializeReference] public IPointerForVector3_Only vector3_Only2;
        public float Pick(object on= null, object from = null, object with = null)
        {
            Vector3 v1 = (vector3_Only1?.Pick()) ?? default;
            Vector3 v2 = (vector3_Only2?.Pick()) ?? default;
            return Vector3.Angle(v1, v2);
        }
    }
    [Serializable, TypeRegistryItem("浮点数_Vector3的点积", "单值针/浮点数针")]
    public class PointerForFloat_DotOfVector3 : IPointerForFloat_Only
    {
        [LabelText("Vector3源_1"), SerializeReference] public IPointerForVector3_Only vector3_Only1;
        [LabelText("Vector3源_2"), SerializeReference] public IPointerForVector3_Only vector3_Only2;
        public float Pick(object on= null, object from = null, object with = null)
        {
            Vector3 v1 = (vector3_Only1?.Pick()) ?? default;
            Vector3 v2 = (vector3_Only2?.Pick()) ?? default;
            return Vector3.Dot(v1, v2);
        }
    }
    [Serializable, TypeRegistryItem("浮点数Vector2的长度", "单值针/浮点数针")]
    public class PointerForFloat_LengthOfVector2 : IPointerForFloat_Only
    {
        [LabelText("Vector2源"), SerializeReference] public IPointerForVector2_Only vector2_Only;
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (vector2_Only?.Pick().magnitude) ?? default;
        }
    }
    [Serializable, TypeRegistryItem("浮点数Vector2的距离", "单值针/浮点数针")]
    public class PointerForFloat_DistanceOfVector2 : IPointerForFloat_Only
    {
        [LabelText("Vector2源_1"), SerializeReference] public IPointerForVector2_Only vector2_Only1;
        [LabelText("Vector2源_2"), SerializeReference] public IPointerForVector2_Only vector2_Only2;
        public float Pick(object on= null, object from = null, object with = null)
        {
            Vector2 v1 = (vector2_Only1?.Pick()) ?? default;
            Vector2 v2 = (vector2_Only2?.Pick()) ?? default;
            return Vector2.Distance(v1, v2);
        }
    }
    [Serializable, TypeRegistryItem("浮点数Vector2的夹角", "单值针/浮点数针")]
    public class PointerForFloat_AngleOfVector2 : IPointerForFloat_Only
    {
        [LabelText("Vector2源_1"), SerializeReference] public IPointerForVector2_Only vector2_Only1;
        [LabelText("Vector2源_2"), SerializeReference] public IPointerForVector2_Only vector2_Only2;
        public float Pick(object on= null, object from = null, object with = null)
        {
            Vector2 v1 = (vector2_Only1?.Pick()) ?? default;
            Vector2 v2 = (vector2_Only2?.Pick()) ?? default;
            return Vector2.Angle(v1, v2);
        }
    }
    [Serializable, TypeRegistryItem("浮点数Vector2的点积", "单值针/浮点数针")]
    public class PointerForFloat_DotOfVector2 : IPointerForFloat_Only
    {
        [LabelText("Vector2源_1"), SerializeReference] public IPointerForVector2_Only vector2_Only1;
        [LabelText("Vector2源_2"), SerializeReference] public IPointerForVector2_Only vector2_Only2;
        public float Pick(object on= null, object from = null, object with = null)
        {
            Vector2 v1 = (vector2_Only1?.Pick()) ?? default;
            Vector2 v2 = (vector2_Only2?.Pick()) ?? default;
            return Vector2.Dot(v1, v2);
        }
    }
    [Serializable, TypeRegistryItem("浮点数Vector3的X值", "单值针/浮点数针")]
    public class PointerForFloat_XOfVector3 : IPointerForFloat_Only
    {
        [LabelText("Vector3源-X"), SerializeReference] public IPointerForVector3_Only vector3_Only;
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (vector3_Only?.Pick().x) ?? default;
        }
    }
    [Serializable, TypeRegistryItem("浮点数Vector3的Y值", "单值针/浮点数针")]
    public class PointerForFloat_YOfVector3 : IPointerForFloat_Only
    {
        [LabelText("Vector3源-Y"), SerializeReference] public IPointerForVector3_Only vector3_Only;
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (vector3_Only?.Pick().y) ?? default;
        }
    }
    [Serializable, TypeRegistryItem("浮点数Vector3的Z值", "单值针/浮点数针")]
    public class PointerForFloat_ZOfVector3 : IPointerForFloat_Only
    {
        [LabelText("Vector3源-Z"), SerializeReference] public IPointerForVector3_Only vector3_Only;
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (vector3_Only?.Pick().z) ?? default;
        }
    }
    [Serializable, TypeRegistryItem("浮点数来自Int", "单值针/浮点数针")]
    public class PointerForFloat_FromInt : IPointerForFloat_Only
    {
        [LabelText("Int源"), SerializeReference] public IPointerForInt_Only int_only;
        public float Pick(object on= null, object from = null, object with = null)
        {
            return (int_only?.Pick()) ?? default;
        }
    }
    [Serializable, TypeRegistryItem("浮点值_直接输入", "单值针/浮点数")]
    public class PointerForFloat_Direct : IPointerForFloat_Only
    {
        [LabelText("直接输入")] public float float_;
        public float Pick(object on= null, object from = null, object with = null)
        {
            return float_;
        }
    }

    [Serializable, TypeRegistryItem("浮点值_0-1直接输入", "单值针/浮点数")]
    public class PointerForFloat_DirectClamp01 : IPointerForFloat_Only
    {
        [LabelText("随机范围"), Range(0, 1)] public float @float = 0f;
        public float Pick(object on= null, object from = null, object with = null)
        {
            return @float;
        }
    }
    [Serializable, TypeRegistryItem("浮点值_0-1随机范围", "单值针/浮点数")]
    public class PointerForFloat_RandomClamp01 : IPointerForFloat_Only
    {
        [LabelText("随机范围"), MinMaxSlider(0, 1)] public Vector2 float_range = new Vector2(0, 1);
        public float Pick(object on= null, object from = null, object with = null)
        {
            return Mathf.Clamp01(UnityEngine.Random.Range(float_range.x, float_range.y));
        }
    }
    [Serializable, TypeRegistryItem("浮点值_0-30随机范围", "单值针/浮点数")]
    public class PointerForFloat_Random30 : IPointerForFloat_Only
    {
        [LabelText("随机范围"), MinMaxSlider(0, 30)] public Vector2 float_range = new Vector2(2, 5);
        public float Pick(object on= null, object from = null, object with = null)
        {
            return UnityEngine.Random.Range(float_range.x, float_range.y);
        }
    }
    [Serializable, TypeRegistryItem("浮点值_随机范围取出", "单值针/浮点数")]
    public class PointerForFloat_Random : IPointerForFloat_Only
    {
        [LabelText("随机范围")] public Vector2 float_range = new Vector2(2, 5);
        public float Pick(object on= null, object from = null, object with = null)
        {
            return UnityEngine.Random.Range(float_range.x, float_range.y);
        }
    }
    #endregion
    #endregion

    #region Int部分
        #region Int接口抽象和包 
    public interface IPointerForInt<On, From, With> : IPointer<int, On, From, With>
    {

    }
    public interface IPointerForInt_Only : IPointerOnlyBackSingle<int>, IPointerForInt<object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerOnlyBackSingle<int>)?.Pick();
        }
    }
    public interface IPointerForIntList : IPointerOnlyBackList<int>
    {

    }
    [Serializable, TypeRegistryItem("整数针包_选中几个")]
    public class PointerForInt_PackerSelectSome : PointerPackerForSelectSomeBack<int, IPointerForInt_Only>, IPointerForIntList
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerPackerForSelectSomeBack<int, IPointerForInt_Only>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("整数针包_选中一个")]
    public class PointerForInt_PackerSelect : PointerPackerForOnlySelectBackOne<int, IPointerForInt_Only>, IPointerForInt_Only
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerPackerForOnlySelectBackOne<int, IPointerForInt_Only>)?.Pick();
        }


    }
    [Serializable, TypeRegistryItem("整数值包_选中几个")]
    public class PointerForIntValueListSelectSome : PointerForValueListSelectSomeBack<int>, IPointerForIntList
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerForValueListSelectSomeBack<int>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("整数值包_选中一个")]
    public class PointerForIntValueListSelect : PointerForValueListSelectBackOne<int>, IPointerForInt_Only
    {

    }
    #endregion
        #region Int功能
    [Serializable, TypeRegistryItem("直接输入整数", "单值针/整数针")]
    public class PointerForInt_Direct : IPointerForInt_Only
    {
        [LabelText("直接输入")] public int int_;
        public int Pick(object on= null, object from = null, object with = null)
        {
            return int_;
        }
    }
    [Serializable, TypeRegistryItem("加权返回Index", "单值针/整数针")]
    public class PointerForInt_BackIntIndexWithWeight : IPointerForInt_Only
    {
        [LabelText("权重列(过大的Index视为空概率)")] public List<float> weights = new List<float>();

        public int Pick(object on= null, object from = null, object with = null)
        {

            float target = weights.Sum() * UnityEngine.Random.value;
            for (int i = 0; i < weights.Count; i++)
            {
                if (weights[i] > target) return i;
                target -= weights[i];
            }
            return weights.Count - 1;
        }
    }
    [Serializable, TypeRegistryItem("30内随机整数", "单值针/整数针")]
    public class PointerForInt_Random30 : IPointerForInt_Only
    {
        [LabelText("随机范围"), MinMaxSlider(0, 30)] public Vector2Int int_range = new Vector2Int(2, 5);
        public int Pick(object on= null, object from = null, object with = null)
        {
            return UnityEngine.Random.Range(int_range.x, int_range.y + 1);
        }
    }
    [Serializable, TypeRegistryItem("Int针转投射", "单值针/整数针")]
    public class PointerForInt_Caster : IPointerForInt_Only, IPointerOnlyBackCaster<int>
    {
        [LabelText("抓获")] public int int_;
        [LabelText("抓取")] public IPointerForInt_Only intP;
        public int Cast()
        {
            return int_;
        }

        public int Pick(object on= null, object from = null, object with = null)
        {
            return int_ = intP?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("Int随机数", "单值针/整数针")]
    public class PointerForInt_Random : IPointerForInt_Only
    {
        [LabelText("随机范围")] public Vector2Int int_range = new Vector2Int(2, 5);
        public int Pick(object on= null, object from = null, object with = null)
        {
            return UnityEngine.Random.Range(int_range.x, int_range.y);
        }
    }
    #endregion
    #endregion
}
