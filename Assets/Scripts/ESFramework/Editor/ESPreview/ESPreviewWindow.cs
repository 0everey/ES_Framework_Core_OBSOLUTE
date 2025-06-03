using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ES
{

    public class ESPreviewWindow : ESWindowBase_Abstract<ESPreviewWindow>
    {
        #region 数据滞留
        public Page_ShowSystemICON systemICON;

        #endregion
        [MenuItem("Tools/ES工具/ES预览窗口")]
        public static void TryOpenWindow()
        {
            if (ESEditorRuntimePartMaster.Instance != null)
                OpenWindow();
            else Debug.LogError("确保场景中有ESEditorRuntimePartMaster");
        }
        protected override void ES_BuildMenuTree(OdinMenuTree tree)
        {
            base.ES_BuildMenuTree(tree);
            tree.Add("系统图标预览", systemICON ??= new Page_ShowSystemICON());
        }
        public override void ES_LoadData()
        {
            base.ES_LoadData();
            icons = new List<SystemICON>(100);
            foreach(var i in UnityEditorIcons.UnityEditorIconNames.AllChinese.Keys)
            {
                try
                {
                    string en = UnityEditorIcons.UnityEditorIconNames.AllChinese[i];
                    Texture tt = EditorGUIUtility.IconContent(en)?.image;
                    if (tt != null)
                    {

                        icons.Add(new SystemICON() { chi = i, eng = en, texture = tt });
                    }
                }
                catch (Exception e)
                {

                }
                
            }
        }
        public static List<SystemICON> icons = new List<SystemICON>();
        
    }
    [Serializable, TypeRegistryItem("系统ICON")]
    public class SystemICON
    {
        [LabelText("中文"),ReadOnly,VerticalGroup("a")]
        [TableColumnWidth(50)]
        public string chi;
        [LabelText("英文"), ReadOnly, VerticalGroup("a")]
        public string eng;
        [LabelText("图标"), ReadOnly, VerticalGroup("a")]
        public Texture texture;
    }
    #region 预览图表集
    [Serializable]
    public class Page_ShowSystemICON
    {
        public TrueGridExample ssss;
        [ShowInInspector, HideLabel, InlineProperty,TableList(ShowIndexLabels = true, HideToolbar = false, AlwaysExpanded = true)]        
        public List<SystemICON> show {get=>ESPreviewWindow.icons;set { } }
        public Page_ShowSystemICON()
        {
           
            
        }
    }

    #endregion
}

