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
    //核心 Ev针支持 关于Vector2和3部分
    #region Vector3部分
    #region Vector3接口抽象和包 
    public interface IPointerForVector3<By, Yarn, On> : IPointer<Vector3, By, Yarn, On>
    {

    }
    public interface IPointerForVector3_Only : IPointerOnlyBackSingle<Vector3>, IPointerForVector3<object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerForVector3_Only)?.Pick();
        }
    }
    public interface IPointerForVector3List : IPointerOnlyBackList<Vector3>
    {

    }
    [Serializable, TypeRegistryItem("Vector3针包_选中几个")]
    public class PointerForVector3_PackerSelectSome : PointerPackerForSelectSomeBack<Vector3, IPointerForVector3_Only>, IPointerForVector3List
    {

    }
    [Serializable, TypeRegistryItem("Vector3针包_选中一个")]
    public class PointerForVector3_PackerSelect : PointerPackerForOnlySelectBackOne<Vector3, IPointerForVector3_Only>, IPointerForVector3_Only
    {

    }
    [Serializable, TypeRegistryItem("Vector3值包_选中几个")]
    public class PointerForVector3ValueListSelectSome : PointerForValueListSelectSomeBack<Vector3>, IPointerForVector3List
    {

    }
    [Serializable, TypeRegistryItem("Vector3值包_选中一个")]
    public class PointerForVector3ValueListSelect : PointerForValueListSelectBackOne<Vector3>, IPointerForVector3_Only
    {

    }
    #endregion
    #region Vector3功能
    [Serializable, TypeRegistryItem("Vector3针转投射","单值针/Vector3针")]
    public class PointerForVector3_Caster : IPointerForVector3_Only, IPointerOnlyBackCaster<Vector3>
    {
        [LabelText("抓获")] public Vector3 vector;
        [LabelText("抓取")] public IPointerForVector3_Only v3P;
        public Vector3 Cast()
        {
            return vector;
        }

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return vector = v3P?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("Vector3_加", "单值针/Vector3针")]
    public class PointerForVector3_Add : IPointerForVector3_Only
    {
        [LabelText("加+_1"), SerializeReference] public IPointerForVector3_Only v3_1;
        [LabelText("加+_2"), SerializeReference] public IPointerForVector3_Only v3_2;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return (v3_1?.Pick() ?? default) + (v3_2?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_乘上浮点数", "单值针/Vector3针")]
    public class PointerForVector3_MutiFloat : IPointerForVector3_Only
    {
        [LabelText("原Vector3"), SerializeReference] public IPointerForVector3_Only v3_1;
        [LabelText("乘数"), SerializeReference] public IPointerForFloat_Only float_;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return (v3_1?.Pick() ?? default) * (float_?.Pick() ?? 1);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_减", "单值针/Vector3针")]
    public class PointerForVector3_Sub : IPointerForVector3_Only
    {
        [LabelText("减-_1"), SerializeReference] public IPointerForVector3_Only v3_1;
        [LabelText("减-_2"), SerializeReference] public IPointerForVector3_Only v3_2;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return (v3_1?.Pick() ?? default) - (v3_2?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_叉乘Cross", "单值针/Vector3针")]
    public class PointerForVector3_Cross : IPointerForVector3_Only
    {
        [LabelText("叉X_1"), SerializeReference] public IPointerForVector3_Only v3_1;
        [LabelText("叉X_2"), SerializeReference] public IPointerForVector3_Only v3_2;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return Vector3.Cross((v3_1?.Pick() ?? default), (v3_2?.Pick() ?? default));
        }
    }
    [Serializable, TypeRegistryItem("Vector3_投影到线", "单值针/Vector3针")]
    public class PointerForVector3_Project : IPointerForVector3_Only
    {
        [LabelText("源向量"), SerializeReference] public IPointerForVector3_Only v3_1;
        [LabelText("投影到"), SerializeReference] public IPointerForVector3_Only v3_pro;

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return Vector3.Project(v3_1?.Pick() ?? default, v3_pro?.Pick() ?? Vector3.forward);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_投影到自动归一化线上", "单值针/Vector3针")]
    public class PointerForVector3_ProjectNormalized : IPointerForVector3_Only
    {
        [LabelText("源向量"), SerializeReference] public IPointerForVector3_Only v3_1;
        [LabelText("投影到"), SerializeReference] public IPointerForVector3_Only v3_pro;

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return Vector3.Project(v3_1?.Pick() ?? default, v3_pro?.Pick().normalized ?? Vector3.forward);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_投影到面", "单值针/Vector3针")]
    public class PointerForVector3_ProjectPlane : IPointerForVector3_Only
    {
        [LabelText("源向量"), SerializeReference] public IPointerForVector3_Only v3_1;
        [LabelText("投影法平面"), SerializeReference] public IPointerForVector3_Only v3_pro;

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return Vector3.ProjectOnPlane(v3_1?.Pick() ?? default, v3_pro?.Pick() ?? Vector3.forward);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_归一化", "单值针/Vector3针")]
    public class PointerForVector3_Normalized : IPointerForVector3_Only
    {
        [LabelText("归一化"), SerializeReference] public IPointerForVector3_Only v3_1;

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return Vector3.Normalize(v3_1?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_合并3个浮点数", "单值针/Vector3针")]
    public class PointerForVector3_CombinedOfFloat3 : IPointerForVector3_Only
    {
        [LabelText("X"), SerializeReference] public IPointerForFloat_Only v3_x;
        [LabelText("Y"), SerializeReference] public IPointerForFloat_Only v3_y;
        [LabelText("Z"), SerializeReference] public IPointerForFloat_Only v3_z;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return new Vector3(v3_x?.Pick() ?? default, v3_y?.Pick() ?? default, v3_z?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_合并2维向量和浮点数(浮点为Y)", "单值针/Vector3针")]
    public class PointerForVector3_CombinedOfVector2AndFloat : IPointerForVector3_Only
    {
        [LabelText("XZ"), SerializeReference] public IPointerForVector2_Only v3_xz;
        [LabelText("Y"), SerializeReference] public IPointerForFloat_Only v3_y;

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            Vector2 vv = v3_xz?.Pick() ?? default;
            return new Vector3(vv.x, v3_y?.Pick() ?? default, vv.y);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_累积自偏移(可初始化的)", "单值针/Vector3针")]
    public class PointerForVector3_SelfOffset : IPointerForVector3_Only, IInittable
    {
        [LabelText("当前值")] public Vector3 now;
        [LabelText("开始点"), SerializeReference] public IPointerForVector3_Only InitValue;
        [LabelText("渐变偏移"), SerializeReference] public IPointerForVector3_Only OffsetValue;
        private bool hasInit = false;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            if (!hasInit) Init();
            Vector3 use = now;
            now += OffsetValue?.Pick() ?? default;
            return now;
        }

        public void Init(params object[] ps)
        {
            hasInit = true;
            if (InitValue != null) now = InitValue.Pick();
        }
    }
    [Serializable, TypeRegistryItem("Vector3_来自四元数的欧拉角", "单值针/Vector3针")]
    public class PointerForVector3_EulerOfQuaternion : IPointerForVector3_Only
    {
        [LabelText("源四元数"), SerializeReference] public IPointerForQuaternion_Only quaternion_Only;

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return quaternion_Only?.Pick().eulerAngles ?? default;
        }
    }
    [Serializable, TypeRegistryItem("Vector3插值", "单值针/Vector3针")]
    public class PointerForVector3_Lerp : IPointerForVector3_Only
    {
        [LabelText("起始Vector3_1源"), SerializeReference] public IPointerForVector3_Only v3_Only_1;
        [LabelText("终点Vector3_2源"), SerializeReference] public IPointerForVector3_Only v3_Only_2;
        [LabelText("Lerp_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t;
        [LabelText("取消钳制")] public bool clampNot;

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            Vector3 a = (v3_Only_1?.Pick() ?? default);
            Vector3 b = (v3_Only_2?.Pick() ?? default);
            float t = (float_Only_t?.Pick() ?? 0);
            if (!clampNot) return Vector3.Lerp(a, b, t);
            else return Vector3.LerpUnclamped(a, b, t);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_按距离插值(MoveTowards)", "单值针/Vector3针")]
    public class PointerForVector3_Toward : IPointerForVector3_Only
    {
        [LabelText("起始Vector3_1源"), SerializeReference] public IPointerForVector3_Only v3_Only_1;
        [LabelText("终点Vector3_2源"), SerializeReference] public IPointerForVector3_Only v3_Only_2;
        [LabelText("距离"), SerializeReference] public IPointerForFloat_Only float_Only_t;


        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            Vector3 a = (v3_Only_1?.Pick() ?? default);
            Vector3 b = (v3_Only_2?.Pick() ?? default);
            float t = (float_Only_t?.Pick() ?? 0);
            return Vector3.MoveTowards(a, b, t);
        }
    }
    [Serializable, TypeRegistryItem("Vector3_直接输入", "单值针/Vector3针")]
    public class PointerForVector3_Direct : IPointerForVector3_Only
    {
        [LabelText("直接输入Vector3")] public Vector3 vector;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return vector;
        }
    }
   /* [Serializable, TypeRegistryItem("Vector3_曲线和", "单值针/Vector3针")]
    public class PointerForVector3_Curve : IPointerForVector3_Only
    {
        [LabelText("X曲线")] public AnimationCurve curveX=AnimationCurve.Constant(0,1,1);
        [LabelText("Y曲线")] public AnimationCurve curveY = AnimationCurve.Constant(0, 1, 1);
        [LabelText("Z曲线")] public AnimationCurve curveZ = AnimationCurve.Constant(0, 1, 1);
        
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            
            return vector;
        }
    }*/
    [Serializable, TypeRegistryItem("Vector3值_变换引用的坐标", "单值针/Vector3")]
    public class PointerForVector3_Transform : IPointerForVector3_Only
    {
        [LabelText("使用变换坐标")] public Transform transform;
        [LabelText("变换为空时")] public Vector3 vector3;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            if (transform != null) return transform.position;
            return vector3;
        }
    }

    [Serializable, TypeRegistryItem("Vector3值_引用变换的坐标_初始化锁定", "单值针/Vector3")]
    public class PointerForVector3_Transform_Init : IPointerForVector3_Only, IInittable
    {
        [LabelText("使用变换坐标")] public Transform transform;
        [LabelText("得到的坐标")] public Vector3 vector3;
        private bool hasInit = false;
        public void Init(params object[] ps)
        {
            if (transform == null)
            {

            }
            else
            {
                vector3 = transform.position;
            }

            hasInit = true;
        }

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            if (!hasInit) Init();
            return vector3;
        }
    }
    [Serializable, TypeRegistryItem("Vector3值_变换针的坐标", "单值针/Vector3")]
    public class PointerForVector3_TransformPointer : IPointerForVector3_Only
    {
        [LabelText("使用变换坐标"), SerializeReference] public IPointerForTransform_Only transform;
        [LabelText("得到的坐标")] public Vector3 vector3;

        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            Transform tt = transform?.Pick(0);
            if (tt != null) return tt.position;
            return default;
        }
    }
    [Serializable, TypeRegistryItem("Vector3_球体_中心和半径都是针的随机点", "单值针/Vector3针")]
    public class PointerForVector3_PointerCenterRandomSphere : IPointerForVector3_Only
    {
        [LabelText("针中心"), SerializeReference] public IPointerForVector3_Only center;
        [LabelText("半径范围")] public float r;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return (center?.Pick() ?? default) + UnityEngine.Random.insideUnitSphere * r;
        }
    }
    [Serializable, TypeRegistryItem("Vector3_球体_中心是变换和半径固定的随机点", "单值针/Vector3针")]
    public class PointerForVector3_TransCenterRandomCircle : IPointerForVector3_Only
    {
        [LabelText("变换中心")] public Transform center;
        [LabelText("半径范围")] public float r;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            if (center != null) return (center.position) + UnityEngine.Random.insideUnitSphere * r;
            else return UnityEngine.Random.insideUnitSphere * r;
        }
    }
    [Serializable, TypeRegistryItem("Vector3_球体_中心和半径固定的随机点", "单值针/Vector3针")]
    public class PointerForVector3_EasyRandomSphere : IPointerForVector3_Only
    {
        [LabelText("随机中心")] public Vector3 center;
        [LabelText("半径范围")] public float r;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return center + UnityEngine.Random.insideUnitSphere * r;
        }
    }

    [Serializable, TypeRegistryItem("Vector3值_引用动态变换的面向方向", "单值针/Vector3")]
    public class PointerForVector3_TransformFoward : IPointerForVector3_Only
    {
        [LabelText("使用变换坐标")] public Transform transform;
        [LabelText("变换为空时")] public Vector3 vector3;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            return transform?.forward ?? vector3;
        }
    }
    #endregion
    #endregion

    #region Vector2部分
    #region Vector2接口抽象和包
    public interface IPointerForVector2 : IPointer<Vector2, object, object, object>
    {

    }
    public interface IPointerForVector2_Only : IPointerOnlyBackSingle<Vector2>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForVector2List : IPointerOnlyBackList<Vector2>
    {

    }
    [Serializable, TypeRegistryItem("Vector2针包_选中几个")]
    public class PointerForVector2_PackerSelectSome : PointerPackerForSelectSomeBack<Vector2, IPointerForVector2_Only>, IPointerForVector2List
    {

    }
    [Serializable, TypeRegistryItem("Vector2针包_选中一个")]
    public class PointerForVector2_PackerSelect : PointerPackerForOnlySelectBackOne<Vector2, IPointerForVector2_Only>, IPointerForVector2_Only
    {

    }
    [Serializable, TypeRegistryItem("Vector2值包_选中几个")]
    public class PointerForVector2ValueListSelectSome : PointerForValueListSelectSomeBack<Vector2>, IPointerForVector2List
    {

    }
    [Serializable, TypeRegistryItem("Vector2值包_选中一个")]
    public class PointerForVector2ValueListSelect : PointerForValueListSelectBackOne<Vector2>, IPointerForVector2_Only
    {

    }
    #endregion
        #region Vector2功能
    [Serializable, TypeRegistryItem("Vector2针转投射", "单值针/Vector2针")]
    public class PointerForVector2_Caster : IPointerForVector2_Only, IPointerOnlyBackCaster<Vector2>
    {
        [LabelText("抓获")] public Vector2 vector;
        [LabelText("抓取")] public IPointerForVector2_Only v2P;
        public Vector2 Cast()
        {
            return vector;
        }

        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            return vector = v2P?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("Vector2_直接输入", "单值针/Vector2针")]
    public class PointerForVector2_Direct : IPointerForVector2_Only
    {
        [LabelText("直接输入Vector2")] public Vector2 vector;


        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            return vector;
        }
    }
    [Serializable, TypeRegistryItem("Vector2_xy作为上下界限制在01", "单值针/Vector2针")]
    public class PointerForVector2_DirectClamp01 : IPointerForVector2_Only
    {
        [LabelText("范围01的Vector2"), MinMaxSlider(0, 1, showFields: true)] public Vector2 vector;


        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            return vector;
        }
    }
    [Serializable, TypeRegistryItem("Vector2_用两个浮点数合并", "单值针/Vector2针")]
    public class PointerForVector2_Combine : IPointerForVector2_Only
    {
        [LabelText("X值"), SerializeReference] public IPointerForFloat_Only forX;
        [LabelText("Y值"), SerializeReference] public IPointerForFloat_Only forY;

        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            return new Vector2(forX?.Pick() ?? 0, forY?.Pick() ?? 0);
        }
    }
    [Serializable, TypeRegistryItem("Vector2_相乘", "单值针/Vector2针")]
    public class PointerForVector2_Muti : IPointerForVector2_Only
    {
        [LabelText("Vector2值"), SerializeReference] public IPointerForVector2_Only Vector2;
        [LabelText("乘数值"), SerializeReference] public IPointerForFloat_Only forY;

        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            return (Vector2?.Pick() ?? default) * (forY?.Pick() ?? 1);
        }
    }
    [Serializable, TypeRegistryItem("Vector2插值", "单值针/Vector2针")]
    public class PointerForVector2_Lerp : IPointerForVector2_Only
    {
        [LabelText("起始Vector2_1源"), SerializeReference] public IPointerForVector2_Only v2_Only_1;
        [LabelText("终点Vector2_2源"), SerializeReference] public IPointerForVector2_Only v2_Only_2;
        [LabelText("Lerp_T源"), SerializeReference] public IPointerForFloat_Only float_Only_t;
        [LabelText("取消钳制")] public bool clampNot;

        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            Vector2 a = (v2_Only_1?.Pick() ?? default);
            Vector2 b = (v2_Only_2?.Pick() ?? default);
            float t = (float_Only_t?.Pick() ?? 0);
            if (!clampNot) return Vector2.Lerp(a, b, t);
            else return Vector2.LerpUnclamped(a, b, t);
        }
    }
    [Serializable, TypeRegistryItem("Vector2_距离插值(Towards)", "单值针/Vector2针")]
    public class PointerForVector2_Toward : IPointerForVector2_Only
    {
        [LabelText("起始Vector2_1源"), SerializeReference] public IPointerForVector2_Only v2_Only_1;
        [LabelText("终点Vector2_2源"), SerializeReference] public IPointerForVector2_Only v2_Only_2;
        [LabelText("距离"), SerializeReference] public IPointerForFloat_Only float_Only_t;


        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            Vector2 a = (v2_Only_1?.Pick() ?? default);
            Vector2 b = (v2_Only_2?.Pick() ?? default);
            float t = (float_Only_t?.Pick() ?? 0);
            return Vector2.MoveTowards(a, b, t);
        }
    }
    [Serializable, TypeRegistryItem("Vector2_圆形_中心是针半径固定的随机点", "单值针/Vector2针")]
    public class PointerForVector2_PointerCenterRandomCircle : IPointerForVector2_Only
    {
        [LabelText("针中心"), SerializeReference] public IPointerForVector2_Only center;
        [LabelText("半径范围")] public float r;
        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            return (center?.Pick() ?? default) + UnityEngine.Random.insideUnitCircle * r;
        }
    }
    [Serializable, TypeRegistryItem("Vector2_圆形_中心是变换引用半径固定的随机点", "单值针/Vector2针")]
    public class PointerForVector2_TransCenterRandomCircle : IPointerForVector2_Only
    {
        [LabelText("变换中心")] public Transform center;
        [LabelText("半径范围")] public float r;
        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            return (Vector2)(center.position) + UnityEngine.Random.insideUnitCircle * r;
        }
    }
    [Serializable, TypeRegistryItem("Vector2_圆形_中心和半径固定的随机点", "单值针/Vector2针")]
    public class PointerForVector2_EasyRandomCircle : IPointerForVector2_Only
    {
        [LabelText("随机中心")] public Vector2 center;
        [LabelText("半径范围")] public float r;
        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            return center + UnityEngine.Random.insideUnitCircle * r;
        }
    }
    [Serializable, TypeRegistryItem("Vector2_来自Vector3的X和Z", "单值针/Vector2针")]
    public class PointerForVector2_Vector3XZToVector2 : IPointerForVector2_Only
    {
        [SerializeReference, LabelText("Vector3源")]
        public IPointerForVector3_Only v3Source;
        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {

            Vector3 vv = v3Source?.Pick() ?? default;
            return new Vector2(vv.x, vv.z);
        }
    }
    [Serializable, TypeRegistryItem("Vector2_来自Vector3直接转化", "单值针/Vector2针")]
    public class PointerForVector2_Vector3ToVector2 : IPointerForVector2_Only
    {
        [SerializeReference, LabelText("Vector3源")]
        public IPointerForVector3_Only v3Source;
        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {

            Vector3 vv = v3Source?.Pick() ?? default;
            return (Vector2)vv;
        }
    }
    #endregion
    #endregion
}
