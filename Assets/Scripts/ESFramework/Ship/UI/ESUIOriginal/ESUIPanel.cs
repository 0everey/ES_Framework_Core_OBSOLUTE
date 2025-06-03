using ES;
using GameKit.Dependencies.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class ESUIPanel : ESUIElement
    {
        [LabelText("全部元素(持久)")]
        public Dictionary<string, ESUIElement> AllElements = new Dictionary<string, ESUIElement>();
#if UNITY_EDITOR
        [ValueDropdown("ElementKeys",AppendNextDrawer =true),LabelText("元素测试")]
        public string testElement = "测试";
#endif

        public ESUIRoot MyRoot { get { if (dirty) GetMyParentAndRegisteThis(); return _myRoot=_myRoot.EX_NotNullAndUse()??_myParentPanel.EX_NotNullAndUse().MyRoot; } set { _myRoot = value; } }
        [SerializeField, LabelText("所属根节点")] private ESUIRoot _myRoot;
        private void Awake()
        {
           
        }
        public List<string> ElementKeys()
        {
            List<string> strings = new List<string>(5);
            var es = transform.GetComponentsInChildren<ESUIElement>();
            foreach (var i in es)
            {
                if (i.register&&i!=this)
                {
                    AddKeyReleThis(i,strings);
                }
            }
            return strings;
        }
        public void AddKeyReleThis(ESUIElement element, List<string> toAdd,string post="")
        {
            var parent = element.EX_GetCompoentInParentExcludeSelf<ESUIPanel>();
            if (parent == this||parent==null) toAdd.Add(element.RegisterKey+post);
            else AddKeyReleThis(parent,toAdd,"/"+ element.RegisterKey + post); 
        }
        public string GetKeyReleThis(ESUIElement element,string post = "")
        {
            var parent = element.EX_GetCompoentInParentExcludeSelf<ESUIPanel>();
            if (parent == this || parent == null) return element.RegisterKey+post;
            else return GetKeyReleThis(parent, "/" + element.RegisterKey + post);
        }
        [Button("注册全部元素")]
        public void RegisterAllElements()
        {
            var es = transform.EX_GetCompoentsInChildExcludeSelf<ESUIElement>();
            foreach (var i in es)
            {
                i.GetMyParentAndRegisteThis();
             
 
            }
        }
        public void _UnRegisterElement(ESUIElement i)
        {
            if (AllElements.TryGetValue(i.RegisterKey, out var e))
            {
                if (e == i)
                {
                    AllElements.Remove(i.RegisterKey);
                }
            }
            else
            {

            }
        }
        public void _RegisterElement(ESUIElement i)
        {
            if (AllElements.TryGetValue(i.RegisterKey, out var e))
            {

            }
            else
            {
                AllElements.Add(i.RegisterKey, i);
            }

        }


        public override ESUIPanel GetMyParentAndRegisteThis()
        {
            var root = this.EX_GetCompoentInParentExcludeSelf<ESUIRoot>();
            if (root != null)
            {
                _myRoot = root;
                if (register)
                {
                    _myRoot._RegisterPanel(this);
                }
            }
            return base.GetMyParentAndRegisteThis();
        }
    }
}

