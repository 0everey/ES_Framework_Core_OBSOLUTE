using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于四元数 Quaterntion部分
    #region 四元数部分
        #region 四元数接口抽象和包
    public interface IPointerForQuaternion<On, From, With> : IPointer<Quaternion, On, From, With> { }
    public interface IPointerForQuaternion_Only : IPointerOnlyBack<Quaternion>, IPointerForQuaternion<object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForQuaternionList : IPointerOnlyBackList<Quaternion> { }
    [Serializable, TypeRegistryItem("四元数针包_选中几个")]
    public class PointerForQuaternion_PackerSelectSome : PointerPackerForSelectSomeBack<Quaternion, IPointerForQuaternion_Only>, IPointerForQuaternionList
    {

    }
    [Serializable, TypeRegistryItem("四元数针包_选中一个")]
    public class PointerForQuaternion_PackerSelect : PointerPackerForOnlySelectBackOne<Quaternion, IPointerForQuaternion_Only>, IPointerForQuaternion_Only
    {

    }
    [Serializable, TypeRegistryItem("四元数值包_选中几个")]
    public class PointerForQuaternionValueListSelectSome : PointerForValueListSelectSomeBack<Quaternion>, IPointerForQuaternionList
    {

    }
    [Serializable, TypeRegistryItem("四元数值包_选中一个")]
    public class PointerForQuaternionValueListSelect : PointerForValueListSelectBackOne<Quaternion>, IPointerForQuaternion_Only
    {

    }
    #endregion
        #region 四元数功能
        [Serializable, TypeRegistryItem("四元数_直接")]
        public class PointerForQuaternion_Direc : IPointerForQuaternion_Only
        {
            [LabelText("四元数")]
            public Quaternion quaternion;
            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                return quaternion;
            }
        }
        [Serializable, TypeRegistryItem("四元数_来自欧拉角")]
        public class PointerForQuaternionFromVector3 : IPointerForQuaternion_Only
        {
            [LabelText("欧拉角"), SerializeReference]
            public IPointerForVector3_Only v3Source;
            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                return Quaternion.Euler(v3Source?.Pick() ?? default);
            }
        }
        [Serializable, TypeRegistryItem("四元数_来自变换的世界旋转")]
        public class PointerForQuaternionOfTransformRotation : IPointerForQuaternion_Only
        {
            [LabelText("变换源"), SerializeReference]
            public IPointerForTransform_Only trans;
            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                return trans?.Pick()?.rotation ?? default;
            }
        }
        [Serializable, TypeRegistryItem("四元数_来自变换的世界旋转_只初始化变化")]
        public class PointerForQuaternionOfTransformRotation_Init : IPointerForQuaternion_Only, IInittable
        {
            [LabelText("变换源"), SerializeReference]
            public IPointerForTransform_Only trans;
            private bool hasInit = false;
            private Quaternion q;
            public void Init(params object[] ps)
            {
                q = trans?.Pick()?.rotation ?? Quaternion.identity;
                hasInit = true;
            }

            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                if (!hasInit) Init();
                return q;
            }
        }
        [Serializable, TypeRegistryItem("四元数_来自变换的局部旋转")]
        public class PointerForQuaternionOfTransformLocalRotation : IPointerForQuaternion_Only
        {
            [LabelText("变换源"), SerializeReference]
            public IPointerForTransform_Only trans;
            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                return trans?.Pick()?.localRotation ?? default;
            }
        }
        [Serializable, TypeRegistryItem("四元数_来自Vector3方向")]
        public class PointerForQuaternion : IPointerForQuaternion_Only
        {
            [LabelText("方向"), SerializeReference]
            public IPointerForVector3_Only v3Direc;
            [LabelText("上轴(默认UP)"), SerializeReference]
            public IPointerForVector3_Only upWards;
            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                Vector3 vv = v3Direc?.Pick() ?? default;
                if (vv == default) return Quaternion.identity;
                Vector3 up = upWards?.Pick() ?? default;
                if (up == default) return Quaternion.LookRotation(vv);
                return Quaternion.LookRotation(vv, up);
            }
        }
        [Serializable, TypeRegistryItem("四元数_来自从一个方向到另外一个")]
        public class PointerForQuaternionFromTo : IPointerForQuaternion_Only
        {
            [LabelText("方向前"), SerializeReference]
            public IPointerForVector3_Only v3DirecFrom;
            [LabelText("方向后"), SerializeReference]
            public IPointerForVector3_Only v3DirecTo;
            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                Vector3 from_ = v3DirecFrom?.Pick() ?? default;
                if (from_ == default) return Quaternion.identity;
                Vector3 to = v3DirecTo?.Pick() ?? default;
                if (to == default) return Quaternion.identity;
                return Quaternion.FromToRotation(from_, to);
            }
        }
        [Serializable, TypeRegistryItem("四元数_归一化")]
        public class PointerForQuaternionNormalize : IPointerForQuaternion_Only
        {
            [LabelText("源四元数"), SerializeReference]
            public IPointerForQuaternion_Only quaternion_Only;

            public Quaternion Pick(object on= null, object from = null, object with = null)
            {

                Quaternion q = quaternion_Only?.Pick() ?? Quaternion.identity;
                return Quaternion.Normalize(q);
            }
        }
        [Serializable, TypeRegistryItem("四元数_相乘(左乘世界转右乘局部转)")]
        public class PointerForQuaternionMuti : IPointerForQuaternion_Only
        {
            [LabelText("源四元数左"), SerializeReference]
            public IPointerForQuaternion_Only quaternion_Only_left;
            [LabelText("源四元数右"), SerializeReference]
            public IPointerForQuaternion_Only quaternion_Only_right;

            public Quaternion Pick(object on= null, object from = null, object with = null)
            {

                Quaternion q1 = quaternion_Only_left?.Pick() ?? Quaternion.identity;
                Quaternion q2 = quaternion_Only_right?.Pick() ?? Quaternion.identity;
                return q1 * q2;
            }
        }

        [Serializable, TypeRegistryItem("四元数普通插值")]
        public class PointerForQuaternion_Lerp : IPointerForQuaternion_Only
        {
            [LabelText("起始四元数_1源"), SerializeReference] public IPointerForQuaternion_Only q_Only_1;
            [LabelText("终点四元数_2源"), SerializeReference] public IPointerForQuaternion_Only q_Only_2;
            [LabelText("Lerp_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t;
            [LabelText("取消钳制")] public bool clampNot;

            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                Quaternion a = (q_Only_1?.Pick() ?? default);
                Quaternion b = (q_Only_2?.Pick() ?? default);
                float t = (float_Only_t?.Pick() ?? 0);
                if (!clampNot) return Quaternion.Lerp(a, b, t);
                else return Quaternion.LerpUnclamped(a, b, t);
            }
        }
        [Serializable, TypeRegistryItem("四元数高精度插值")]
        public class PointerForQuaternion_SLerp : IPointerForQuaternion_Only
        {
            [LabelText("起始四元数_1源"), SerializeReference] public IPointerForQuaternion_Only q_Only_1;
            [LabelText("终点四元数_2源"), SerializeReference] public IPointerForQuaternion_Only q_Only_2;
            [LabelText("Lerp_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t;
            [LabelText("取消钳制")] public bool clampNot;

            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                Quaternion a = (q_Only_1?.Pick() ?? default);
                Quaternion b = (q_Only_2?.Pick() ?? default);
                float t = (float_Only_t?.Pick() ?? 0);
                if (!clampNot) return Quaternion.Slerp(a, b, t);
                else return Quaternion.SlerpUnclamped(a, b, t);
            }
        }
        [Serializable, TypeRegistryItem("四元数_度数插值(Towards)")]
        public class PointerForQuaternion_Towards : IPointerForQuaternion_Only
        {
            [LabelText("起始四元数_1源"), SerializeReference] public IPointerForQuaternion_Only q_Only_1;
            [LabelText("终点四元数_2源"), SerializeReference] public IPointerForQuaternion_Only q_Only_2;
            [LabelText("度数"), SerializeReference] public IPointerForFloat_Only float_Only_t;


            public Quaternion Pick(object on= null, object from = null, object with = null)
            {
                Quaternion a = (q_Only_1?.Pick() ?? default);
                Quaternion b = (q_Only_2?.Pick() ?? default);
                float t = (float_Only_t?.Pick() ?? 0);
                return Quaternion.RotateTowards(a, b, t);
            }
        }
        #endregion
    #endregion
}
