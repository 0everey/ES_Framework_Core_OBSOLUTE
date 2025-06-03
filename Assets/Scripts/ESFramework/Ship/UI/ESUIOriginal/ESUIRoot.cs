using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class ESUIRoot : SerializedMonoBehaviour
    {
        [LabelText("全部面板(持久)")]
        public Dictionary<string, ESUIElement> AllPanels = new Dictionary<string, ESUIElement>();
#if UNITY_EDITOR
        [ValueDropdown("_PanelKeys", AppendNextDrawer = true), LabelText("(一级)面板测试")]
        public string testPanel = "测试";
#endif
        public List<string> _PanelKeys()
        {
            List<string> strings = new List<string>(5);
            var es = transform.GetComponentsInChildren<ESUIPanel>();
            foreach (var i in es)
            {
                if (i.register && i != this)
                {
                    _AddKeyReleThis(i, strings);
                }
            }
            return strings;
        }
        public void _AddKeyReleThis(ESUIPanel element, List<string> toAdd)
        {
            var parent = element.EX_GetCompoentInParentExcludeSelf<ESUIPanel>();
            if (parent == null) toAdd.Add(element.RegisterKey);
            else {/*忽略*/ }/*GetKeyReleThis(parent, toAdd, "/" + element.RegisterKey + post);*/
        }
        [Button("注册全部面板")]
        public void RegisterAllPanels()
        {
            var es = transform.EX_GetCompoentsInChildExcludeSelf<ESUIPanel>();
            foreach (var i in es)
            {
                i.GetMyParentAndRegisteThis();
            }
        }

        [Button("注册全部元素(包括面板)")]
        public void RegisterAllElements()
        {
            var es = transform.EX_GetCompoentsInChildExcludeSelf<ESUIElement>();
            foreach (var i in es)
            {
                i.GetMyParentAndRegisteThis();
            }
        }


        public void _RegisterPanel(ESUIPanel i)
        {
            if (AllPanels.TryGetValue(i.RegisterKey, out var e))
            {

            }
            else
            {
                AllPanels.Add(i.RegisterKey, i);
            }

        }
        public void _UnRegisterPanel(ESUIPanel i)
        {
            if (AllPanels.TryGetValue(i.RegisterKey, out var e))
            {
                if (e == i)
                {
                    AllPanels.Remove(i.RegisterKey);
                }
            }
            else
            {

            }
        }
        
    }
}

