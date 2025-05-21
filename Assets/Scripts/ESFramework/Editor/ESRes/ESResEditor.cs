using DG.Tweening.Plugins.Core.PathCore;
using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ES
{
    public class ESResEditor
    {
        #region Asset菜单部分
        [InitializeOnLoad]
        public class ESResEditorForAssetMenu
        {
            #region 常量表
            public const string _ConstString_MenuForMarkAssetBundle = "Assets/【ES】标记到AssetBundle";

            #endregion

            #region 改变选择对象——调整Check
            static ESResEditorForAssetMenu()
            {
                Selection.selectionChanged = OnSelectionChanged;
            }
            public static void OnSelectionChanged()
            {
                var paths = GetSelectedObjectsAllPaths();
                if (paths.Count == 0) return;
                string apply = paths.First((i) => i != null && !string.IsNullOrEmpty(i));
                if (!string.IsNullOrEmpty(apply))
                {
                    Menu.SetChecked(_ConstString_MenuForMarkAssetBundle, IsObjectPathMarked(apply));
                }
            }
            #endregion 
            [MenuItem(_ConstString_MenuForMarkAssetBundle)]
            public static void Handle_TryMarkToAssetBundle()
            {
                var ss = GetSelectedObjectsAllPaths();
                foreach (var i in ss)
                {
                    MarkToABOrCancelTruely(i);
                }

            }

            #region 辅助功能
            public static List<string> GetSelectedObjectsAllPaths()
            {
                var paths = new List<string>();
               
                foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
                {
                    
                    var path = AssetDatabase.GetAssetPath(obj);

                    if (!string.IsNullOrEmpty(path) && (File.Exists(path)||AssetDatabase.IsValidFolder(path)))
                    {
                        paths.Add(path);
                    }
                }
                return paths;
            }
            public static void MarkToABOrCancelTruely(string path)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    
                    //Asset设置
                    var ai = AssetImporter.GetAtPath(path);
                    //获得文件夹
                    var dir = new DirectoryInfo(path);

                    if (IsObjectPathMarked(path))
                    {
                        Menu.SetChecked(_ConstString_MenuForMarkAssetBundle, false);
                        ai.assetBundleName = null;
                    }
                    else
                    {
                        Menu.SetChecked(_ConstString_MenuForMarkAssetBundle, true);
                        if (EditorMaster.Instance.abMaskType== EditorMaster.ABMaskType.AsOrinal)
                        {
                            ai.assetBundleName = dir.Name.Replace(".", "_");
                        }else if(EditorMaster.Instance.abMaskType == EditorMaster.ABMaskType.AsFolder)
                        {
                            ai.assetBundleName = dir.Parent.Name.Replace(".", "_");
                        }
                        else
                        {
                            ai.assetBundleName = EditorMaster.Instance.ABName.Replace(".", "_");
                        }
                       
                    }

                    AssetDatabase.RemoveUnusedAssetBundleNames();
                }
            }

            public static bool IsObjectPathMarked(string path)
            {
                try
                {
                    var ai = AssetImporter.GetAtPath(path);
                    var dir = new DirectoryInfo(path);
                    return string.Equals(ai.assetBundleName, dir.Name.Replace(".", "_").ToLower());
                }
                catch (Exception _)
                {
                    Debug.LogError(_);
                    return false;
                }
            }
            #endregion
        }


        #endregion
    }
}


