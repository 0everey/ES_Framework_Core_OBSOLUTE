using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于 全局功能 部分
    //一般实现IPoinerNone
    #region 全局功能
    [Serializable, TypeRegistryItem("全局功能_重新加载当前场景")]
    public class PointerPicker_LoadCurrentScene : PointerOnlyAction
    {
       public override object Pick(object by = null, object yarn = null, object on = null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("全局功能_直接加载场景")]
    public class PointerPicker_LoadScene : PointerOnlyAction
    {
        [LabelText("字符串"), SerializeReference]
        public IPointerForString_Only string_Only = new PointerForString_Direc();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            String s = string_Only?.Pick();
            if (s == null || s == "") return -1;
            SceneManager.LoadScene(s);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("全局功能_异步加载场景")]
    public class PointerPicker_LoadSceneAsync : PointerOnlyAction
    {
        [LabelText("字符串"), SerializeReference]
        public IPointerForString_Only string_Only = new PointerForString_Direc();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            String s = string_Only?.Pick();
            if (s == null || s == "") return -1;
            SceneManager.LoadSceneAsync(s);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("全局功能_叠加加载场景")]
    public class PointerPicker_LoadSceneAdditive : PointerOnlyAction
    {
        [LabelText("字符串"), SerializeReference]
        public IPointerForString_Only string_Only = new PointerForString_Direc();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            String s = string_Only?.Pick();
            if (s == null || s == "") return -1;
            SceneManager.LoadScene(s, LoadSceneMode.Additive);
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("全局功能_叠加异步加载场景")]
    public class PointerPicker_LoadSceneAsyncAdditive : PointerOnlyAction
    {
        [LabelText("字符串"), SerializeReference]
        public IPointerForString_Only string_Only = new PointerForString_Direc();

        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            String s = string_Only?.Pick();
            if (s == null || s == "") return -1;
            SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
            return base.Pick(by, yarn, on);
        }
    }

    #endregion

}
