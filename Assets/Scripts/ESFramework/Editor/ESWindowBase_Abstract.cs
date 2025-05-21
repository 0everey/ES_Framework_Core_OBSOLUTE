using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace ES
{
    public abstract class ESWindowBase_Abstract<T> :  OdinMenuEditorWindow where T : ESWindowBase_Abstract<T>
    {
        public static T usingWindow;


        public virtual GUIContent ESWindow_GetWindowGUIContent()
        {
            Texture2D texture = Resources.Load<Texture2D>("Sprites/iv2");
            var content= new GUIContent("依薇尔工具窗口", texture, "使用依薇尔工具完成快速开发");
            return content;
        }
        public virtual void ESWindow_OpenHandle()
        {
            /*if (usingWindow.HasDelegate)
            {
                //已经注册委托
            }
            else
            {
                usingWindow.DelegateHandle();
            }*/
        }
        public static void OpenWindow()
        {
            usingWindow = GetWindow<T>();
            usingWindow.ESWindow_OpenHandle();
            usingWindow.titleContent = usingWindow.ESWindow_GetWindowGUIContent();
            usingWindow.minSize = new Vector2(400, 300);
            usingWindow.maxSize = new Vector2(2500, 1800);
            usingWindow.maximized = true;
            usingWindow.MenuWidth = 250;
            usingWindow.Show();
            
        }
        protected sealed override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();
            ES_BuildMenuTree(tree);
            ES_LoadData();
            return tree;
        }
        protected virtual void ES_BuildMenuTree(OdinMenuTree tree)
        {
            
        }
        protected override void OnImGUI()
        {
            if (usingWindow == null)
            {
                usingWindow = this as T;
            }
            base.OnImGUI();
        }
        public virtual void ESWindow_RefreshWindow()
        {
            ES_SaveData();
            this.ForceMenuTreeRebuild();
            ES_LoadData();
        }
        public virtual void ES_LoadData()
        {

        }
        public virtual void ES_SaveData()
        {

        }
    }
}