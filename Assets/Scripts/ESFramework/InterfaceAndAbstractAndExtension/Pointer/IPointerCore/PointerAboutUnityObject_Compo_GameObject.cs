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
    //核心 Ev针支持 关于Unity.Object 资源 脚本等部分
    #region UnityObject部分
        #region Unity.Object 接口抽象和包
    public interface IPointerForUnityObject<By, Yarn, On> : IPointer<UnityEngine.Object, By, Yarn, On>
    {

    }
    public interface IPointerForUnityObject_Only : IPointerForUnityObject<object, object, object>, IPointerOnlyBack<UnityEngine.Object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForUnityObjectList : IPointerOnlyBackList<UnityEngine.Object>
    {

    }
    [Serializable, TypeRegistryItem("Unity物体针包_选中几个")]
    public class PointerForUnityObject_PackerSelectSome : PointerPackerForSelectSomeBack<UnityEngine.Object, IPointerForUnityObject_Only>, IPointerForUnityObjectList
    {

    }
    [Serializable, TypeRegistryItem("Unity物体针包_选中一个")]
    public class PointerForUnityObject_PackerSelect : PointerPackerForOnlySelectBackOne<UnityEngine.Object, IPointerForUnityObject_Only>, IPointerForUnityObject_Only
    {

    }
    [Serializable, TypeRegistryItem("Unity物体值包_选中几个")]
    public class PointerForUnityObjectValueListSelectSome : PointerForValueListSelectSomeBack<UnityEngine.Object>, IPointerForUnityObjectList
    {

    }
    [Serializable, TypeRegistryItem("Unity物体值包_选中一个")]
    public class PointerForUnityObjectValueListSelect : PointerForValueListSelectBackOne<UnityEngine.Object>, IPointerForUnityObject_Only
    {

    }
    #endregion
    #endregion
    #region 脚本部分
        #region 脚本接口抽象和包
    public interface IPointerForComponent<By, Yarn, On> : IPointer<Component, By, Yarn, On>
    {

    }
    public interface IPointerForComponent_Only : IPointerForComponent<object, object, object>, IPointerOnlyBack<Component>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForComponentList : IPointerOnlyBackList<Component>
    {

    }
    [Serializable, TypeRegistryItem("脚本针包_选中一个")]
    public class PointerForComponent_PackerSelect : PointerPackerForOnlySelectBackOne<Component, IPointerForComponent_Only>, IPointerForComponent_Only
    {

    }
    [Serializable, TypeRegistryItem("脚本针包_选中几个")]
    public class PointerForComponent_PackerSelectSome : PointerPackerForSelectSomeBack<Component, IPointerForComponent_Only>, IPointerForComponentList
    {

    }
    [Serializable, TypeRegistryItem("脚本值包_选中几个")]
    public class PointerForComponentValueListSelectSome : PointerForValueListSelectSomeBack<Component>, IPointerForComponentList
    {

    }
    [Serializable, TypeRegistryItem("脚本值包_选中一个")]
    public class PointerForComponentValueListSelect : PointerForValueListSelectBackOne<Component>, IPointerForComponent_Only
    {

    }
    #endregion
        #region 脚本功能
    [Serializable ,TypeRegistryItem("脚本_直接引用", "单值针/脚本针")]
    public class PointerForCompoent_Direc : IPointerForComponent_Only
    {
        [LabelText("直接引用脚本")]
        public Component component_direc;
        public Component Pick(object by = null, object yarn = null, object on = null)
        {
            return component_direc;
        }
    }
    #endregion
    #endregion
    #region 游戏物体部分
        #region 游戏物体接口抽象和包
    public interface IPointerForGameObject<By, Yarn, On> : IPointer<GameObject, object, object, object>
    {

    }
    public interface IPointerForGameObject_Only : IPointerOnlyBack<GameObject>, IPointerForGameObject<object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerOnlyBack<GameObject>)?.Pick();
        }

    }
    public interface IPointerForGameObjectList : IPointerOnlyBackList<GameObject>
    {

    }

    [Serializable, TypeRegistryItem("游戏物体针包_选中几个")]
    public class PointerForGameObject_PackerSelectSome : PointerPackerForSelectSomeBack<GameObject, IPointerForGameObject_Only>, IPointerForGameObjectList
    {

    }

    [Serializable, TypeRegistryItem("游戏物体针包_选中一个")]
    public class PointerForGameObject_PackerSelect : PointerPackerForOnlySelectBackOne<GameObject, IPointerForGameObject_Only>, IPointerForGameObject_Only
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerPackerForOnlySelectBackOne<GameObject, IPointerForGameObject_Only>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("游戏物体值包_选中几个")]
    public class PointerForGameObjectValueListSelectSome : PointerForValueListSelectSomeBack<GameObject>, IPointerForGameObjectList
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerForValueListSelectSomeBack<GameObject>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("游戏物体值包_选中一个")]
    public class PointerForGameObjectValueListSelect : PointerForValueListSelectBackOne<GameObject>, IPointerForGameObject_Only
    {

    }
    #endregion
        #region 游戏物体功能
    [Serializable, TypeRegistryItem("游戏物体_来自脚本", "单值针/游戏物体针")]
    public class PointerForGameObject_FromCompoent : IPointerForGameObject_Only
    {
        [LabelText("脚本源"), SerializeReference] public IPointerForComponent_Only cP;
        public GameObject Pick(object by = null, object yarn = null, object on = null)
        {
            return cP?.Pick()?.gameObject;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerForComponent_Only)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("游戏物体_找Tag", "单值针/游戏物体针")]
    public class PointerForGameObject_FromTag : IPointerForGameObject_Only
    {
        [LabelText("使用的标签")] public PointerForString_Tag tag = new PointerForString_Tag();
        public GameObject Pick(object by = null, object yarn = null, object on = null)
        {
            if (tag != null)
                return GameObject.FindGameObjectWithTag(tag?.Pick());
            return null;
        }

    }
    [Serializable, TypeRegistryItem("游戏物体_直接", "单值针/游戏物体针")]
    public class PointerForGameObject_Direct : IPointerForGameObject_Only
    {
        [LabelText("直接引用游戏物体")] public GameObject g;
        public GameObject Pick(object by = null, object yarn = null, object on = null)
        {
            return g;
        }

    }
    [Serializable, TypeRegistryItem("投射游戏物体_针转投射", "投射")]
    public class PointerForGameObject_Caster : IPointerForGameObject_Only, IPointerOnlyBackCaster<GameObject>
    {
        [LabelText("可抓获")] public GameObject g_;
        [LabelText("可抓取"), SerializeReference] public IPointerForGameObject_Only gP;
        public GameObject Cast()
        {
            return g_;
        }

        public GameObject Pick(object by = null, object yarn = null, object on = null)
        {
            return g_ = gP?.Pick() ?? default;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerForComponent_Only)?.Pick();
        }
    }
    #endregion
    #endregion
}
