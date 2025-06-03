using ES;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ES
{


    public class ESResWindow : ESWindowBase_Abstract<ESResWindow>
    {
        #region 数据滞留
        public PartPageClass_AssetBundle Page_Assetbundle;
        

        #endregion
        [MenuItem("Tools/ES工具/ES资源管理窗口")]
        public static void TryOpenWindow()
        {
            if (ESEditorRuntimePartMaster.Instance != null)
                OpenWindow();
            else Debug.LogError("确保场景中有EditorMaster");
        }
        
     
        protected override void ES_BuildMenuTree(OdinMenuTree tree)
        {
            PartPage_AssetBundle(tree);
        }
        void PartPage_AssetBundle(OdinMenuTree tree)
        {
            tree.Add("AB包管理", Page_Assetbundle ??= new PartPageClass_AssetBundle());
        }
        #region 持久化
        public override void ES_LoadData()
        {
            base.ES_LoadData();
        }

        public override void ES_SaveData()
        {
            base.ES_SaveData();
        }
        #endregion
        #region 可序列化的组分
        //为AssetBundle提供的页面
        [Serializable]
        public class PartPageClass_AssetBundle : ESWindowPageBase
        {
            [TitleGroup("全局设置",alignment:TitleAlignments.Centered),PropertyOrder(-3)]
            [LabelText("更改AB打包模式"),ShowInInspector]public ESResMaster.ABPackType packType { get => ESResMaster.Instance.abPackType; set => ESResMaster.Instance.abPackType = value; }
            [InfoBox("",Message = "@GetCodeInfo()",InfoMessageType = InfoMessageType.Warning)]
            [LabelText("打AB包时，自动辅助代码生成格式"), PropertyOrder(-3), ShowInInspector,InlineButton("Handle_CodeGenAssetConstName", "手动生成常量名代码")] public ESResMaster.ABForAutoCodeGen codeType { get => ESResMaster.Instance.abFoeAutoCodeGen; set => ESResMaster.Instance.abFoeAutoCodeGen = value; }
            string GetCodeInfo()
            {
                switch (codeType)
                {
                    case ESResMaster.ABForAutoCodeGen.NoneCode: return "不生成代码";
                    case ESResMaster.ABForAutoCodeGen.CodeAsOriginal: return "生成代码(完全按源文件名)";
                    case ESResMaster.ABForAutoCodeGen.CodeAsLower: return "生成代码(源文件名转小写)";
                    case ESResMaster.ABForAutoCodeGen.CodeAsUpper: return "生成代码(源文件名转大写)";
                    default:return "不生成代码";
                }
            }
            
            private void Handle_CodeGenAssetConstName()
            {

            }
            [TitleGroup("使用功能", Alignment = TitleAlignments.Centered),PropertyOrder(-2)]
            [FolderPath, LabelText("生成路径"), ShowInInspector,ReadOnly]
            public string genarateFolder { get => ESResMaster.Instance.genarateFolder; set => ESResMaster.Instance.genarateFolder = value; }
            [TitleGroup("使用功能")]
            public RuntimePlatform applyPlatform = Application.platform;
            [ButtonGroup("使用功能/AB按钮")]
            [Button("生成AB包")]

            private void Handle_BuildAB()
            {
                if (AssetDatabase.IsValidFolder(genarateFolder))
                {
                    if(applyPlatform== RuntimePlatform.WindowsPlayer||applyPlatform== RuntimePlatform.WindowsEditor)
                    {

                        BuildAB(genarateFolder, applyPlatform.ToString(), assetBundleOptions: BuildAssetBundleOptions.ForceRebuildAssetBundle, targetPlatform: BuildTarget.StandaloneWindows);
                        BuildAB(genarateFolder, RuntimePlatform.WindowsEditor.ToString(), assetBundleOptions: BuildAssetBundleOptions.ForceRebuildAssetBundle, targetPlatform: BuildTarget.StandaloneWindows);
                    }
                    else if(applyPlatform == RuntimePlatform.Android)
                    {
                        BuildAB(genarateFolder , applyPlatform.ToString(), assetBundleOptions: BuildAssetBundleOptions.ForceRebuildAssetBundle, targetPlatform: BuildTarget.Android);
                    }else if(applyPlatform== RuntimePlatform.IPhonePlayer)
                    {
                        BuildAB(genarateFolder,applyPlatform.ToString(), assetBundleOptions: BuildAssetBundleOptions.ForceRebuildAssetBundle, targetPlatform: BuildTarget.iOS);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("无效的文件夹路径", "这个文件夹路径是不合法的，请确定正确的文件夹再重试", "知道了");
                }
                
            }
            private void BuildAB(string parent,string theFolder, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
            {
                string path = parent + "/" + theFolder;
                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateFolder(parent,theFolder);
                }
                BuildPipeline.BuildAssetBundles(path, assetBundleOptions,targetPlatform);
            }
            [ButtonGroup("使用功能/AB按钮")]
            [Button("清空AB包(需要额外确定)")]

            private bool Handle_ClearAB()
            {
                if (AssetDatabase.IsValidFolder(genarateFolder))
                {
                    bool b = EditorUtility.DisplayDialog("是否清空AB包", "这会删除全部的已生成AB包(也包括目标文件夹下的全部内容)", "我确定", "放弃");
                    if (b)
                    {
                        var guids= AssetDatabase.FindAssets("",new string[]{ genarateFolder });
                        foreach(var i in guids)
                        {
                            
                            string path= AssetDatabase.GUIDToAssetPath(i);
                            AssetDatabase.DeleteAsset(path);
                            AssetDatabase.Refresh();
                        }
                        return true;
                    }
                    else
                    {
                       
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("无效的文件夹路径", "这个文件夹路径是不合法的，请确定正确的文件夹再重试", "知道了");
                }
                return false;
            }

            [TitleGroup("收集AB包标记数据",Alignment = TitleAlignments.Centered),ReadOnly,ListDrawerSettings(DefaultExpandedState =true)]
            public List<string> AllPath = new List<string>();
            [TitleGroup("收集AB包标记数据")]
            [ButtonGroup("收集AB包标记数据/AB标记")]
            [Button("刷新AB标记")]
            public void ReFreshBundleList()
            {
                AllPath = new List<string>();
                bundleNames = AssetDatabase.GetAllAssetBundleNames();
                foreach (var i in bundleNames)
                {
                    string[] path_ = AssetDatabase.GetAssetPathsFromAssetBundle(i);
                    AllPath.AddRange(path_);

                }

            }
            [ButtonGroup("收集AB包标记数据/AB标记")]
            [Button("清除AB标记(需要额外确认)")]
            public void ClearBundleList()
            {
                bool b = EditorUtility.DisplayDialog("是否清空AB包标记", "这会移除全部的AB包标记，意味着要重新标记", "我确定", "放弃");
                if (b)
                {
                    bundleNames = AssetDatabase.GetAllAssetBundleNames();
                    foreach (var i in bundleNames)
                    {
                        AssetDatabase.RemoveAssetBundleName(i, forceRemove: true);
                    }
                }
                else
                {

                }
               
            }



            private string[] bundleNames;
            [TitleGroup("AB包查询与标记", alignment: TitleAlignments.Centered)]
            [ValueDropdown("bundleNames", AppendNextDrawer = true), LabelText("AB包名(可用于标记)"),ShowInInspector]
            public string selectBundleName { get => ESResMaster.Instance.ABName; set => ESResMaster.Instance.ABName = value; }
            [TitleGroup("AB包查询与标记")]
            [LabelText("标记模式"), ShowInInspector]
            public ESResMaster.ABMaskType maskType { get => ESResMaster.Instance.abMaskType; set => ESResMaster.Instance.abMaskType = value; }

            [TitleGroup("AB包查询与标记")]
            [LabelText("AB包对应的资源"),ListDrawerSettings(DefaultExpandedState=true),ReadOnly,ShowInInspector]
            public string[] ShowAssets=>AssetDatabase.GetAssetPathsFromAssetBundle(selectBundleName);

          
        }


        #endregion

        #region 值
 


        #endregion
    }


}