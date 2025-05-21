using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ES
{

    public class EditorMaster : SingletonAsMono<EditorMaster>
    {
        [LabelText("提示"),PropertyOrder(-5)]
        public ESReadMeClass readme = new ESReadMeClass() { readMe= "请在尽量在窗口处进行调整,这里不推荐\n因为" };
        #region 工具设置
        
        [FoldoutGroup("AB包工具")][LabelText("AB打包模式")]public ABPackType abPackType;
        [FoldoutGroup("AB包工具")][LabelText("AB代码生成模式")] public ABForAutoCodeGen abFoeAutoCodeGen;
        [FoldoutGroup("AB包工具"),FolderPath, LabelText("生成路径")]
        public string genarateFolder = "Assets/StreamingAssets/AssetBundles";
        [FoldoutGroup("AB包工具"),LabelText("AB包标记模式")]
        public ABMaskType abMaskType = ABMaskType.AsOrinal;
        [FoldoutGroup("AB包工具"), LabelText("AB包标记自定义名字")]
        public string ABName;
        #endregion

        #region 加载到
        public static List<string> ESTags = new List<string>();
        public static List<string> BuffKeys = new List<string>();

        #endregion

        #region 配置源
        [LabelText("配置的ESTags包")] public ESLayerStringSO ESTagsSO_;

        [LabelText("配置的Buff键")] public BuffDataPack BuffDataPack;



        #endregion

        #region 静态支持LayerMask Int
        public static int LayerDefault = 0;
        public static int LayerTransparentFX = 1;
        public static int LayerIgnoreRaycast = 2;
        // public static int LayerDefault = 0;
        public static int LayerWater = 4;
        public static int LayerUI = 5;
        public static int LayerWall = 6;
        public static int LayerGround = 7;
        /*  public static int LayerDefault = 0;
          public static int LayerDefault = 0;
          public static int LayerDefault = 0;
          public static int LayerDefault = 0;
          public static int LayerDefault = 0;*/
        public static int LayerEntity = 11;
        public static int LayerDroppedItem = 12;
        public static int LayerDamagable = 13;
        public static int LayerInteractive = 14;
        public static int LayerOnlyShow = 15;
        /*public static int LayerDefault = 0;
        public static int LayerDefault = 0;
        public static int LayerDefault = 0;
        public static int LayerDefault = 0;
        public static int LayerDefault = 0;*/



        #endregion
        #region 静态支持LayerMask LayerMask
        public static int LayerMaskMaskDefault = 1;
        public static int LayerMaskTransparentFX = 1 << 1;
        public static int LayerMaskIgnoreRaycast = 1 << 2;
        // public static int LayerMaskDefault = 0;
        public static int LayerMaskWater = 1 << 4;
        public static int LayerMaskUI = 1 << 5;
        public static int LayerMaskWall = 1 << 6;
        public static int LayerMaskGround = 1 << 7;
        /*  public static int LayerMaskDefault = 0;
          public static int LayerMaskDefault = 0;
          public static int LayerMaskDefault = 0;
          public static int LayerMaskDefault = 0;
          public static int LayerMaskDefault = 0;*/
        public static int LayerMaskEntity = 1 << 11;
        public static int LayerMaskDroppedItem = 1 << 12;
        public static int LayerMaskDamagable = 1 << 13;
        public static int LayerMaskInteractive = 1 << 14;
        public static int LayerMaskOnlyShow = 1 << 15;
        /*public static int LayerMaskDefault = 0;
        public static int LayerMaskDefault = 0;
        public static int LayerMaskDefault = 0;
        public static int LayerMaskDefault = 0;
        public static int LayerMaskDefault = 0;*/



        #endregion
        #region 静态支持LayerMask 特殊
        public static int LayerMaskSpecialMaskDefault = 1;
        public static int LayerMaskSpecialTransparentFX = 1 << 1;
        public static int LayerMaskSpecialIgnoreRaycast = 1 << 2;
        // public static int LayerMaskSpecialDefault = 0;
        public static int LayerMaskSpecialWater = 1 << 4;
        public static int LayerMaskSpecialUI = 1 << 5;
        public static int LayerMaskSpecialWall = 1 << 6;
        /*  public static int LayerMaskSpecialDefault = 0;
          public static int LayerMaskSpecialDefault = 0;
          public static int LayerMaskSpecialDefault = 0;
          public static int LayerMaskSpecialDefault = 0;
          public static int LayerMaskSpecialDefault = 0;*/
        public static int LayerMaskSpecialEntity = 1 << 11;
        public static int LayerMaskSpecialDroppedItem = 1 << 12;
        public static int LayerMaskSpecialDamagable = 1 << 13;
        public static int LayerMaskSpecialInteractive = 1 << 14;
        public static int LayerMaskSpecialOnlyShow = 1 << 15;
        /*public static int LayerMaskDefault = 0;
        public static int LayerMaskDefault = 0;
        public static int LayerMaskDefault = 0;
        public static int LayerMaskDefault = 0;
        public static int LayerMaskDefault = 0;*/



        #endregion


        #region 分类
        protected override void Awake()
        {

            base.Awake();
            Load();
        }
        private void OnValidate()
        {
            Debug.Log("开始加载编辑器缓存数据");
            Load();
        }
        private void Load()
        {
            LoadESTags();
        }
        private void LoadESTags()
        {
            EditorMaster.ESTags = new List<string>();
            if (ESTagsSO_ != null)
            {
                foreach (var i in ESTagsSO_.LayerStrings)
                {
                    foreach (var ii in i.Value)
                    {
                        EditorMaster.ESTags.Add(i.Key + "/" + ii);
                    }
                }
            }
        }
        private void LoadTypesInfoKeys()
        {

        }
        #endregion

        #region 枚举和类
        public enum ABPackType
        {
            [InspectorName("模拟模式")] Simulate,
            [InspectorName("发布模式")] Release
        }
        public enum ABForAutoCodeGen
        {
            [InspectorName("不生成代码")] NoneCode,
            [InspectorName("生成同名代码")] CodeAsOriginal,
            [InspectorName("生成代码转大写")] CodeAsUpper,
            [InspectorName("生成代码转小写")] CodeAsLower
        }

        public enum ABMaskType
        {
            [InspectorName("原名")]AsOrinal,
            [InspectorName("收集到文件夹名")] AsFolder,
            [InspectorName("自定义名")] SelfDefine,
        }
        #endregion
    }
}