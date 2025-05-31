#if UNITY_EDITOR
using UnityEditorInternal;
#endif

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
    //核心 Ev针支持 关于 字符串 部分
    #region 字符串部分
    #region 字符串接口抽象和包
    public interface IPointerForString_Only : IPointerOnlyBackSingle<string>, IPointerForString<object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForString<On, From, With> : IPointer<string, On, From, With>
    {

    }

    public interface IPointerForStringList : IPointerOnlyBackList<String>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }

    [Serializable, TypeRegistryItem("字符串针包_选中几个")]
    public class PointerForString_PackerSelectSome : PointerPackerForSelectSomeBack<String, IPointerForString_Only>, IPointerForStringList
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerPackerForSelectSomeBack<String, IPointerForString_Only>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("字符串针包_选中一个")]
    public class PointerForString_PackerSelect : PointerPackerForOnlySelectBackOne<String, IPointerForString_Only>, IPointerForString_Only
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerPackerForOnlySelectBackOne<String, IPointerForString_Only>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("字符串值包_选中几个")]
    public class PointerForStringValueListSelectSome : PointerForValueListSelectSomeBack<String>, IPointerForStringList
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerForValueListSelectSomeBack<String>)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("字符串值包_选中一个")]
    public class PointerForStringValueListSelect : PointerForValueListSelectBackOne<String>, IPointerForString_Only
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as PointerForValueListSelectBackOne<String>)?.Pick();
        }
    }
    #endregion
   
    [Serializable, TypeRegistryItem("万能组合字符串", "单值针/字符串针")]
    public class PointerForString_CombineAll : IPointerForString_Only, IPointerForStringCaster
    {
        [LabelText("链接列"), SerializeReference]
        public List<IPointer> pointers = new List<IPointer>();
        [LabelText("上次输出"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public string cast;
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;

        public string Cast()
        {
            return cast;
        }

        public string Pick(object on= null, object from = null, object with = null)
        {
            string s = "";
            foreach (var i in pointers)
            {
                if (i != null)
                {
                    s += i.PickToString();
                }
            }
            if (usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(s);
            }
            return cast = s;
        }
    }
    [Serializable, TypeRegistryItem("字符串资源文件路径", "单值针/字符串针")]
    public class PointerForString_ResourceDirec : IPointerForString_Only
    {
        [LabelText("直接拖入资源")]
        [FilePath(ParentFolder = "Assets/Resources/", RequireExistingPath = false, IncludeFileExtension = false)]
        public string path;
        public string Pick(object on= null, object from = null, object with = null)
        {
            return path;
        }
    }
   


    [Serializable]
    public class PointerForString_Direc : IPointerForString_Only
    {
        [LabelText("直接输入")] public string string_direc;
        public string Pick(object on= null, object from = null, object with = null)
        {
            return string_direc;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerForString_Only)?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("字符串_标签", "单值针/字符串")]
    public class PointerForString_Tag : IPointerForString_Only
    {
        [ValueDropdown("tags"), LabelText("选择标签")]
        public string tagName;
        public string Pick(object on= null, object from = null, object with = null)
        {
            return tagName;
        }

        public string[] tags()
        {
            string[] s = default;
#if UNITY_EDITOR

            s = InternalEditorUtility.tags;
#endif
            return s;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return (this as IPointerForString_Only)?.Pick();
        }
    }

    [Serializable, TypeRegistryItem("字符串列表_标签", "单值针/字符串")]
    public class PointerForStringList_Tag : IPointerForStringList
    {
        [ValueDropdown("tags"), LabelText("选择一些标签")]
        public List<string> tagNames = new List<string>();
        public List<string> Pick(object on= null, object from = null, object with = null)
        {
            
            return tagNames;
        }
        public string[] tags()
        {
            string[] s = default;
#if UNITY_EDITOR
            s = InternalEditorUtility.tags;
#endif
            return s;
        }
        
    }
    #endregion
}
