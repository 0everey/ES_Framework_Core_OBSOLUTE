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
    //核心 Ev针支持 关于类型 Type 支持部分
    //类型 unity 默认是 不支持的 所以不提供 Type 的 直接输入 而是使用 投射Type的针播放器==》PlayerTypeCaster

    #region 类型针
    #region 类型接口抽象和包
    public interface IPointerForType<On, From, With> : IPointer<Type, On, From, With>
    {
        
    }
    public interface IPointerForType_Only : IPointerForType<object, object, object>, IPointerOnlyBack<Type>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #endregion
    #region 类型功能
    [Serializable, TypeRegistryItem("来自类型投射播放器", "单值针/类型针")]
    public class PointerForType_FromPlayer : IPointerForType_Only
    {
        [LabelText("类型投射播放器")] public PointerPlayerTypeCaster typeCaster;
        public Type Pick(object on= null, object from = null, object with = null)
        {

            return (typeCaster as IPointerForType_Only)?.Pick();
        }
    }
    #endregion
    /* [Serializable]
     public class PointerForType_Direct_Overridable : IPointerForType_Only
     {
         [LabelText("类型"),NonSerialized,OdinSerialize,ShowInInspector,TypeSelectorSettings(FilterTypesFunction = "TypeFilterBoolMayMachine")]
         public Type type;

         public Type Pick(object on= null, object from = null, object with = null)
         {
             return type;
         }
         public virtual bool TypeFilterBoolMayMachine(Type type)
         {
             return type.IsSubclassOf(typeof(System.Object)) && !type.IsAbstract && !type.IsInterface;
         }
     }*/

    #endregion
}
