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
    //核心 Ev针支持 关于动画器 Anamator部分
    #region 布尔值部分
        #region 布尔值接口抽象和包
        public interface IPointerForBool<in By, in Yarn, in On> : IPointer<bool, By, Yarn, On>
        {

        }
        public interface IPointerForBool_Only : IPointerForBool<object, object, object>, IPointerOnlyBack<bool>
        {
            object IPointer.Pick(object a, object b, object c)
            {
                return (this as IPointerOnlyBack<bool>).Pick();
            }
        }
        public interface IPointerForBoolList : IPointerOnlyBackList<bool>
        {

        }
        [Serializable, TypeRegistryItem("布尔针包_选中几个")]
        public class PointerForBool_PackerSelectSome : PointerPackerForSelectSomeBack<bool, IPointerForBool_Only>, IPointerForBoolList
        {
            object IPointer.Pick(object a, object b, object c)
            {
                return (this as PointerPackerForSelectSomeBack<bool, IPointerForBool_Only>)?.Pick();
            }
        }
        [Serializable, TypeRegistryItem("布尔针包_选中一个")]
        public class PointerForBool_PackerSelect : PointerPackerForOnlySelectBackOne<bool, IPointerForBool_Only>, IPointerForBool_Only
        {
            object IPointer.Pick(object a, object b, object c)
            {
                return (this as PointerPackerForOnlySelectBackOne<bool, IPointerForBool_Only>)?.Pick();
            }
        }
        [Serializable, TypeRegistryItem("布尔值包_选中几个")]
        public class PointerForBoolValueListSelectSome : PointerForValueListSelectSomeBack<bool>, IPointerForBoolList
        {
            object IPointer.Pick(object a, object b, object c)
            {
                return (this as PointerForValueListSelectSomeBack<bool>)?.Pick();
            }
        }
        [Serializable, TypeRegistryItem("布尔值包_选中一个")]
        public class PointerForBoolValueListSelect : PointerForValueListSelectBackOne<bool>, IPointerForBool_Only
        {
            object IPointer.Pick(object a, object b, object c)
            {
                return (this as PointerForValueListSelectBackOne<bool>)?.Pick();
            }
        }
        #endregion
        #region 布尔值功能
    [Serializable, TypeRegistryItem("布尔值_针转投射", "单值针/布尔针")]
    public class PointerForBool_Caster : IPointerForBool_Only, IPointerOnlyBackCaster<bool>
    {
        [LabelText("可抓获")] public bool b;
        [LabelText("可抓取")] public IPointerForBool_Only bP;
        public bool Cast()
        {
            return b;
        }

        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            return b = bP?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("布尔针_直接输入", "单值针/布尔值")]
    public class PointerForBool_Direc : IPointerForBool_Only
    {

        [LabelText("直接输入")] public bool bool_;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            return bool_;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerForBool_Only)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("布尔值_游戏物体的当前运行状态", "单值针/布尔针")]
    public class PointerForBool_GameObjectActiveInHier : IPointerForBool_Only
    {
        [LabelText("层级活动游戏物体")] public GameObject ob;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            return ob?.activeInHierarchy ?? false;
        }

    }
    [Serializable, TypeRegistryItem("布尔值_游戏物体的个人运行状状态(忽略父级)", "单值针/布尔针")]
    public class PointerForBool_GameObjectActiveSelf : IPointerForBool_Only
    {
        [LabelText("自己活动游戏物体")] public GameObject ob;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            return ob?.activeSelf ?? false;
        }
    }
    [Serializable, TypeRegistryItem("布尔值_脚本的开关状态", "单值针/布尔针")]
    public class PointerForBool_MonoEnable : IPointerForBool_Only
    {
        [LabelText("Mono可用脚本")] public MonoBehaviour mono;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            return mono?.enabled ?? false;
        }
    }
    [Serializable, TypeRegistryItem("布尔值_脚本在活动并且开关状态(说明在运行)", "单值针/布尔针")]
    public class PointerForBool_MonoActiveAndEnable : IPointerForBool_Only
    {
        [LabelText("Mono活动脚本")] public MonoBehaviour mono;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            return mono?.isActiveAndEnabled ?? false;
        }
    }
    [Serializable, TypeRegistryItem("布尔值_01概率")]
    public class PointerForBool_Random01 : IPointerForBool_Only
    {
        [LabelText("?Bool概率")] public float P = 0.5f;

        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            if (UnityEngine.Random.value < P)
            {
                return true;
            }
            return false;
        }
    }
    [Serializable, TypeRegistryItem("布尔值_任意浮点数来源概率(0-1)", "单值针/布尔针")]
    public class PointerForBool_RandomBy : IPointerForBool_Only
    {
        [LabelText("?概率来源"), SerializeReference] public IPointerForFloat_Only float_;

        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            if (UnityEngine.Random.value < float_?.Pick())
            {
                return true;
            }
            return false;
        }
    }
    [Serializable, TypeRegistryItem("布尔值_非 !运算", "单值针/布尔针")]
    public class PointerForBool_Not : IPointerForBool_Only
    {
        [LabelText("!运算默认值")] public bool default_;
        [LabelText("!运算Bool针"), SerializeReference] public IPointerForBool_Only bool_;

        public bool Pick(object by = null, object yarn = null, object on = null)
        {

            return (!bool_?.Pick()) ?? default_;
        }
    }
    [Serializable, TypeRegistryItem("布尔值_且 &&运算", "单值针/布尔针")]

    public class PointerForBool_And : IPointerForBool_Only
    {
        [LabelText("&&默认值")] public bool default_;
        [LabelText("&&Bool针1"), SerializeReference] public IPointerForBool_Only bool_1;
        [LabelText("&&Bool针2"), SerializeReference] public IPointerForBool_Only bool_2;

        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            if (bool_1 == null || bool_2 == null) return default_;
            return bool_1.Pick() && bool_2.Pick();
        }
    }
    [Serializable, TypeRegistryItem("布尔值_或者 ||运算", "单值针/布尔针")]
    public class PointerForBool_Or : IPointerForBool_Only
    {
        [LabelText("||运算默认值")] public bool default_;
        [LabelText("||运算Bool针1"), SerializeReference] public IPointerForBool_Only bool_1;
        [LabelText("||运算Bool针2"), SerializeReference] public IPointerForBool_Only bool_2;

        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            if (bool_1 == null || bool_2 == null) return default_;
            return bool_1.Pick() || bool_2.Pick();
        }
    }
    [Serializable, TypeRegistryItem("布尔值_两个浮点数比较", "单值针/布尔针")]
    public class PointerForBool_FloatCompare : IPointerForBool_Only
    {
        [LabelText("使用函数")] public EnumCollect.CompareTwoFunction useFunction;
        [LabelText("默认赋值")] public float default_ = 1;
        [LabelText("左值"), SerializeReference] public IPointerForFloat_Only float_Left;
        [LabelText("右值"), SerializeReference] public IPointerForFloat_Only float_Right;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            float left = float_Left?.Pick() ?? default_;
            float right = float_Right?.Pick() ?? default_;
            return KeyValueMatchingUtility.Function.FunctionForCompareTwoFloat(left, right, useFunction);

        }
    }
    [Serializable, TypeRegistryItem("布尔值_不是default", "单值针/布尔针")]
    public class PointerForBool_NotDefault : IPointerForBool_Only
    {
        [LabelText("任意物体不为default"), SerializeReference] public IPointer ob_;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = ob_?.Pick() ?? default;
            return oo != default;
        }
    }
    [Serializable, TypeRegistryItem("布尔值_任意物体不为空", "单值针/布尔针")]
    public class PointerForBool_NotNullOnly : IPointerForBool_Only
    {
        [LabelText("任意物体不为null"), SerializeReference] public IPointer ob_;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = ob_?.Pick();
            if (oo is UnityEngine.Object uo) return uo != null;
            return oo != null;
        }
    }
    [Serializable, TypeRegistryItem("布尔值_任意物体不为空且不默认", "单值针/布尔针")]
    public class PointerForBool_NotNullAndDestroyed : IPointerForBool_Only
    {
        [LabelText("任意物体不为null"), SerializeReference] public IPointer ob_;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = ob_?.Pick();
            if (oo is UnityEngine.Object uo) return uo != null;
            return oo != null&&oo!=default;
        }
    }
    /* [Serializable]
     public class PFB_intfloat : IPointerForBool<int, float, float>
     {
         public bool Pick(int by = 0, float yarn = 0, float on = 0)
         {
             throw new NotImplementedException();
         }
     }*/
    #endregion
    #endregion
}
