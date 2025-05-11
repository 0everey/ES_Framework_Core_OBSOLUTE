using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES {
 
    public class EditorMaster : SingletonAsMono<EditorMaster>
    {
        #region 加载到
        public static List<string> ESTags = new List<string>();
        public static List<string> BuffKeys = new List<string>();

        #endregion

        #region 配置源
        [LabelText("配置的ESTags包")]public ESLayerStringSO ESTagsSO_;

        [LabelText("配置的Buff键")] public BuffDataPack BuffDataPack;

        #endregion
        void Start()
        {
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
        private void LoadESTags(){
            EditorMaster.ESTags = new List<string>();
            if (ESTagsSO_ != null)
            {
                foreach(var i in ESTagsSO_.LayerStrings)
                {
                    foreach(var ii in i.Value)
                    {
                        EditorMaster.ESTags.Add(i.Key + "/" + ii);
                    }
                }
            }
        }
        private void LoadTypesInfoKeys(){

        }
        void Update()
        {

        }
    }
}