using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ES.EditorMaster;
using static UnityEngine.Rendering.DebugUI;


namespace ES
{
    [DefaultExecutionOrder(-3)]
    public class ESResMaster : SingletonAsMono<ESResMaster>
    {
        #region 全局设置

        #region AB工具设置

        [FoldoutGroup("AB包工具")][LabelText("AB打包模式")] public ABPackType abPackType;
        [FoldoutGroup("AB包工具")][LabelText("AB代码生成模式")] public ABForAutoCodeGen abFoeAutoCodeGen;
        [FoldoutGroup("AB包工具"), FolderPath, LabelText("生成路径"), ReadOnly]
        public string genarateFolder = "Assets/StreamingAssets/AssetBundles";
        [FoldoutGroup("AB包工具"), LabelText("AB包标记模式")]
        public ABMaskType abMaskType = ABMaskType.AsOrinal;
        [FoldoutGroup("AB包工具"), LabelText("AB包标记自定义名字")]
        public string ABName;


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
            [InspectorName("原名")] AsOrinal,
            [InspectorName("收集到文件夹名")] AsFolder,
            [InspectorName("自定义名")] SelfDefine,
        }

        #endregion

        #region 主包
        public AssetBundle MainBundle;
        public AssetBundleManifest MainManifest;
        #endregion

        #endregion

        #region 加载任务链

        private int mMaxCoroutineCount = 8; //最快协成大概在6到8之间
        private LinkedList<IWithEnumeratorTask> ResLoadTasks = new LinkedList<IWithEnumeratorTask>();
        private bool isLoading = false;
        private void TryStartLoadTask()
        {
            if (ResLoadTasks.Count == 0) return;
            if (isLoading) return;
            StartCoroutine(LoadResTask());
        }
        public void PushResLoadTask(IWithEnumeratorTask task)
        {
            if (task == null)
            {
                return;
            }
            ResLoadTasks.AddLast(task);
            TryStartLoadTask();
        }
        private void OnLoadTaskOK()
        {

        }
        private IEnumerator LoadResTask()
        {
            isLoading = true;
            while (ResLoadTasks.Count > 0)
            {
                var task = ResLoadTasks.First;
                ResLoadTasks.RemoveFirst();
                yield return StartCoroutine(task.Value.DoTaskAsync(OnLoadTaskOK));
                yield return null;
            }
            isLoading = false;
            yield return null;
        }
        #endregion

        #region 池化
        public ESSimpleObjectPool<ResSourceSearchKey> PoolForResSourceSearchKey = new ESSimpleObjectPool<ResSourceSearchKey>(() => new ResSourceSearchKey(),
            (f) =>
            {
                f.AssetPath = null;
                f.AssetType = null;
                f.OwnerAssetBundle = null;
            }
            , 30);

        public ESSimpleObjectPool<InternalResourceRes> PoolForInternalResSource = new ESSimpleObjectPool<InternalResourceRes>(() => new InternalResourceRes(),
          (f) =>
          {

          }
          , 30);

        public ESSimpleObjectPool<AssetResSource> PoolForAssetResSource = new ESSimpleObjectPool<AssetResSource>(() => new AssetResSource(),
           (f) =>
           {

           }
           , 30);
        public ESSimpleObjectPool<LocalAssetBundleResSource> PoolForAssetBundleResSource = new ESSimpleObjectPool<LocalAssetBundleResSource>(() => new LocalAssetBundleResSource(),
         (f) =>
         {

         }
         , 30);
        public ESSimpleObjectPool<LocalABAssetResSource> PoolForABAssetResSource = new ESSimpleObjectPool<LocalABAssetResSource>(() => new LocalABAssetResSource(),
         (f) =>
         {

         }
         , 30);
        public ESSimpleObjectPool<ESResLoader> PoolForESLoader = new ESSimpleObjectPool<ESResLoader>(() => new ESResLoader(),
       (f) =>
       {

       }
       , 30);
        #region 池操作在这里
        public ResSourceSearchKey GetInPool_ResSourceSearchKey(string assetName, string ownerBundleName = null, ResSourceLoadType loadType = ResSourceLoadType.ABAsset, Type assetType = null)
        {
            var resSearchRule = ESResMaster.Instance.PoolForResSourceSearchKey.GetInPool();
            resSearchRule.AssetPath = assetName.ToLower();
            resSearchRule.OwnerAssetBundle = ownerBundleName == null ? null : ownerBundleName.ToLower();
            resSearchRule.AssetType = assetType;
            resSearchRule.OriginalAssetName = assetName;
            resSearchRule.LoadType = loadType;
            return resSearchRule;
        }
        public InternalResourceRes GetInPool_InternalResSource(string name, string onwerBundleName, Type assetTypde)
        {
            var res = ESResMaster.Instance.PoolForInternalResSource.GetInPool();
            res.AssetPath = name;
            res.AssetType = typeof(AssetBundle);
            return res;
        }
        public AssetResSource GetOneInPool_AssetResSource(string name, string onwerBundleName, Type assetTypde)
        {
            var res = ESResMaster.Instance.PoolForAssetResSource.GetInPool();
            if (res != null)
            {
                res.AssetPath = name;
                res.mOwnerBundleName = onwerBundleName;
                res.AssetType = assetTypde;
                res.InitAssetBundleName();
            }

            return res;
        }
        public LocalAssetBundleResSource GetInPool_AssetBundleResSource(string name)
        {
            var res = ESResMaster.Instance.PoolForAssetBundleResSource.GetInPool();

            res.AssetPath = name;
            res.AssetType = typeof(AssetBundle);
            res.InitAssetBundleName();

            return res;
        }

        public ESResLoader GetInPool_ESLoader()
        {
            return PoolForESLoader.GetInPool();
        }
        #endregion

        #endregion

        #region 全局资源源加载和创建
        public class ResTable : BaseListIOC_Arch_KeyAndList<string, ESResSource>
        {
            internal ESResSource GetResBySearchKeys(ResSourceSearchKey resSearchKeys)
            {
                if (resSearchKeys == null) { Debug.LogError("空搜索键"); return null; }
                var assetName = resSearchKeys.AssetPath;
                if (!IOC.TryGetValue(assetName, out var theReses))
                {
                    return null;
                }
                IEnumerable<ESResSource> reses = theReses;

                if (resSearchKeys.AssetType != null)
                {
                    reses = reses.Where(res => res.AssetType == resSearchKeys.AssetType);
                }

                if (resSearchKeys.OwnerAssetBundle != null)
                {
                    reses = reses.Where(res => res.OwnerAssetBundleName == resSearchKeys.OwnerAssetBundle);
                }

                return reses.FirstOrDefault();
            }
        }
        public ResTable AllResSource = new ResTable();

        public ESResSource CreateNewResByKey(ResSourceSearchKey resSearchKeys)
        {
            var retRes = resSearchKeys.LoadType switch
            {
                ResSourceLoadType.AssetBundle => CreateResSource_AssetBundle(resSearchKeys),
                ResSourceLoadType.ABAsset => CreateResSource_ABResource(resSearchKeys),
                ResSourceLoadType.ABScene => null,
                ResSourceLoadType.InternalResource => CreateResSource_InternalResource(resSearchKeys),
                ResSourceLoadType.LocalImageRes => null,
                ResSourceLoadType.NetImageRes => null,
                _ => null
            }; ;
            /*.Where(creator => creator.Match(resSearchKeys))
            .Select(creator => creator.Create(resSearchKeys))
            .FirstOrDefault();*/

            if (retRes == null)
            {
                Debug.LogError("创建资源源失败了. 找不到这个查找键" + resSearchKeys);
                return null;
            }

            return retRes;
        }
        public ESResSource GetResSourceBySearchKeys(ResSourceSearchKey searchKey, bool ifNullCreateNew = true)
        {
            var res = AllResSource.GetResBySearchKeys(searchKey);

            if (res != null)
            {
                return res;
            }

            if (!ifNullCreateNew)
            {
                Debug.LogErrorFormat("没找到资源，并且这里未允许创建:{0}", searchKey);
                return null;
            }

            res = CreateNewResByKey(searchKey);

            if (res != null)
            {
                AllResSource.AddElement(res.AssetPath, res);
            }

            return res;
        }
        public AssetBundle HasLoadedAB(string name)
        {
            foreach(var i in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (i.name == name) return i;
            }
            return null;
        }
        public ESResLoader MainLoader = new ESResLoader();
        #endregion

        #region 创建各种资源源的方法
        public ESResSource CreateResSource_AssetBundle(ResSourceSearchKey resSearchKeys)
        {
            var use = PoolForAssetBundleResSource.GetInPool();
            use.AssetPath = resSearchKeys.AssetPath;
            use.AssetType = typeof(AssetBundle);
            use.InitAssetBundleName();
            return use;
        }
        public ESResSource CreateResSource_InternalResource(ResSourceSearchKey resSearchKeys)
        {
            var use = PoolForInternalResSource.GetInPool();
            use.AssetPath = resSearchKeys.AssetPath;
            use.AssetType = resSearchKeys.AssetType;
            return use;
        }

        public ESResSource CreateResSource_ABResource(ResSourceSearchKey resSearchKeys)
        {
            var use = PoolForABAssetResSource.GetInPool();
            use.AssetPath = resSearchKeys.AssetPath;
            use.OwnerAssetBundleName = resSearchKeys.OwnerAssetBundle;
            use.AssetType = resSearchKeys.AssetType;
            return use;
        }
        #endregion

        #region AB依赖与路径
        public class TheResDatas
        {
            public string[] GetAllDependenciesByUrl(string url)
            {
                Debug.Log("依赖测试");
                return null;
            }
            public string[] GetAllDependenciesByLocalAB(string bundleName)
            {
                return ESResMaster.Instance.MainManifest.GetAllDependencies(bundleName);
            }

            public void LoadFromFile(string outRes)
            {

            }
            public void Reset()
            {

            }
            public IEnumerator LoadFromFileAsync(string outRes)
            {
                yield break;
            }
            /*  AssetData GetAssetData(ResSearchKeys resSearchKeys);
              int AddAssetBundleName(string abName, string[] depends, out AssetDataGroup @group);
            */
            public string GetABHash(string assetName)
            {
                return "";
            }
        }

        public static class ABHelper
        {
            public static TheResDatas AssetBundleConfigFile { get; } = new TheResDatas();
            public static string StreamingAB = Application.streamingAssetsPath + "/" + "AssetBundles/" + Application.platform;
        }

        #endregion

        #region 杂碎
        public T GetRes<T>(ResSourceSearchKey ResSourceSearchKey) where T : class, IResSource
        {
            return GetResSourceBySearchKeys(ResSourceSearchKey) as T;
        }

        public static void UnloadRes(UnityEngine.Object asset)
        {
            if (asset is GameObject)
            {
            }
            else
            {
                Resources.UnloadAsset(asset);
            }
        }

        public static void DestroyAObject(UnityEngine.Object asset)
        {
            UnityEngine.Object.Destroy(asset);
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            MainBundle = AssetBundle.LoadFromFile(ABHelper.StreamingAB + "/" + Application.platform);
            Debug.Log("加载主包" + MainBundle);
            MainManifest = MainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            Debug.Log("加载主依赖"+MainManifest);
          /*  string[] allBundle = MainManifest.GetAllAssetBundles();
            
            foreach(var i in allBundle)
            {
                Debug.Log("AB包" + i);
                string[] allDepends = MainManifest.GetAllDependencies(i);
                foreach(var ii in allDepends)
                {
                    Debug.Log("---" + i + "依赖" + ii);
                }
            }*/
        }

    }
}