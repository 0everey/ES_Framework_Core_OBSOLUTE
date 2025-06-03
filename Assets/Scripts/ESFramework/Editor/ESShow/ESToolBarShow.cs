using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Toolbars;
using UnityEditor;
using UnityEngine;

using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;
using UnityEditor.SceneManagement;

namespace ES
{

    public class CustomToolBar
    {

        [InitializeOnLoad]
        public static class CustomToolbarMenu
        {
            static CustomToolbarMenu()
            {
                // 注册到主工具栏
                ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
            }

            static void OnToolbarGUI()
            {
                // 创建下拉菜单按钮
                if (EditorGUILayout.DropdownButton(
                    new GUIContent("场景跳转", EditorGUIUtility.IconContent("d__Popup").image),
                    FocusType.Passive,
                    EditorStyles.toolbarDropDown))
                {
                    var menu = new GenericMenu();
                    var ss = EditorBuildSettings.scenes;
                    foreach (var i in ss)
                    {
                        string use = i.path;
                        int indexXIE = i.path.LastIndexOf('/') + 1;
                        int indexLast = i.path.LastIndexOf(".unity");
                        if (indexXIE >= 0 && indexLast >= 0)
                        {
                            string display = i.path.Substring(indexXIE, indexLast - indexXIE);
                            menu.AddItem(new GUIContent("<场景>" + display), false, () =>
                        {
                            EditorSceneManager.OpenScene(use);
                        });
                            /* int indexXIE = i.path.LastIndexOf('/') + 1;
                             int indexLast = i.path.LastIndexOf(".unity");
                             if (indexXIE >= 0 && indexLast >= 0)
                             {
                                 string use = i.path.Substring(indexXIE, indexLast - indexXIE);
                                 menu.AddItem(new GUIContent("<场景>" + use), false, () =>
                                 {
                                     EditorSceneManager.OpenScene(use);
                                 });
                             }*/
                        }
                    }
                    // 添加菜单项

                    menu.AddSeparator("");

                    // 带图标的菜单项
                    /*  menu.AddItem(
                          new GUIContent("带图标的项", EditorGUIUtility.IconContent("d_SceneViewFx").image),
                          false,
                          () => Debug.Log("图标项点击")
                      );*/

                    // 显示菜单
                    menu.ShowAsContext();
                }
            }
        }
    }
}




