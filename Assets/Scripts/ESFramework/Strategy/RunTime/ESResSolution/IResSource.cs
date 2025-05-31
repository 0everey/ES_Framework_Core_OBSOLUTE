using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{
    public enum ResSourceState
    {
        [InspectorName("等待中(未使用)")] Waiting = 0,
        [InspectorName("加载中")] Loading = 1,
        [InspectorName("完毕")] Ready = 2,
    }
    public interface IResSource : IWithEnumeratorTask
    {
        string AssetPath { get; }

        string OwnerAssetBundleName { get; }

        ResSourceState State { get; }

        UnityEngine.Object Asset { get; }

        float Progress { get; }
        Type AssetType { get; set; }

        void OnLoadOK_Submit(Action<bool, IResSource> listener);
        void OnLoadOK_WithDraw(Action<bool, IResSource> listener);

        bool UnloadImage(bool flag);

        bool LoadSync();

        void LoadAsync();

        string[] GetDependResSourceAllAssetBundles();

        bool IsDependResLoadFinish();

        bool ReleaseTheResSource();

        void TryAutoPushToPool();

    }
    public class ESResSource : IResSource, IPoolable, IPoolablebSelfControl
    {
        #region 字段被保护，使用属性获取

        protected string mAssetPath;
        private ResSourceState mResSourceState = ResSourceState.Waiting;
        protected UnityEngine.Object mAsset;
        private event Action<bool, IResSource> mOnResLoadDoneEvent;

        #endregion
        public string AssetPath
        {
            get { return mAssetPath; }
            set { mAssetPath = value; }
        }


        public ResSourceState State
        {
            get { return mResSourceState; }
            set
            {
                mResSourceState = value;
                if (mResSourceState == ResSourceState.Ready)
                {
                    Method_ResLoadOK(true);
                }
            }
        }

        public virtual string OwnerAssetBundleName { get; set; }

        public Type AssetType { get; set; }

        /// <summary>
        /// 弃用
        /// </summary>
        public float Progress
        {
            get
            {
                switch (mResSourceState)
                {
                    case ResSourceState.Loading:
                        return CalculateProgress();
                    case ResSourceState.Ready:
                        return 1;
                }

                return 0;
            }
        }

        protected virtual float CalculateProgress()
        {
            return 0;
        }

        public UnityEngine.Object Asset
        {
            get { return mAsset; }
        }

        public bool IsRecycled { get; set; }

        public void OnLoadOK_Submit(Action<bool, IResSource> listener)
        {
            if (listener == null)
            {
                return;
            }
            //如果已经结束了，那就立刻触发
            if (mResSourceState == ResSourceState.Ready)
            {
                listener(true, this);
                return;
            }
            //没结束就加入到队列
            mOnResLoadDoneEvent += listener;
        }

        public void OnLoadOK_WithDraw(Action<bool, IResSource> listener)
        {
            if (listener == null)
            {
                return;
            }

            if (mOnResLoadDoneEvent == null)
            {
                return;
            }

            mOnResLoadDoneEvent -= listener;
        }

        protected void OnResLoadFaild()
        {
            mResSourceState = ResSourceState.Waiting;
            Method_ResLoadOK(false);
        }

        private void Method_ResLoadOK(bool readOrFail)
        {
            if (mOnResLoadDoneEvent != null)
            {
                mOnResLoadDoneEvent(readOrFail, this);
                mOnResLoadDoneEvent = null;
            }

        }

        protected ESResSource(string assetName)
        {
            IsRecycled = false;
            mAssetPath = assetName;
        }

        public ESResSource()
        {
            IsRecycled = false;
        }

        protected bool CheckIsWaitingToLoad()
        {
            return mResSourceState == ResSourceState.Waiting;
        }

        protected void HoldDependRes()
        {
            var depends = GetDependResSourceAllAssetBundles();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ESResMaster.Instance.GetInPool_ResSourceSearchKey(depends[i], null, assetType: typeof(AssetBundle));
                var res = ESResMaster.Instance.GetResSourceBySearchKeys(resSearchRule, false);
                resSearchRule.TryAutoPushToPool();
            }
        }

        protected void UnHoldDependRes()
        {
            var depends = GetDependResSourceAllAssetBundles();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ESResMaster.Instance.GetInPool_ResSourceSearchKey(depends[i]);
                var res = ESResMaster.Instance.GetResSourceBySearchKeys(resSearchRule, false);
                resSearchRule.TryAutoPushToPool();
                res.TryAutoPushToPool();
            }
        }

        #region 子类实现

        public virtual bool LoadSync()
        {
            return false;
        }

        public void LoadAsync()
        {
            if (!CheckIsWaitingToLoad())
            {
                return;
            }

            if (string.IsNullOrEmpty(AssetPath))
            {
                return;
            }

            ESResMaster.Instance.PushResLoadTask(this);
        }

        public virtual string[] GetDependResSourceAllAssetBundles()
        {
            return null;
        }

        public bool IsDependResLoadFinish()
        {
            return true;
            var depends = GetDependResSourceAllAssetBundles();
            if (depends == null || depends.Length == 0)
            {
                return true;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ESResMaster.Instance.GetInPool_ResSourceSearchKey(depends[i]);
                var res = ESResMaster.Instance.GetResSourceBySearchKeys(resSearchRule, false);
                resSearchRule.TryAutoPushToPool();

                if (res == null || res.State != ResSourceState.Ready)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool UnloadImage(bool flag)
        {
            return false;
        }

        public bool ReleaseTheResSource()
        {
            if (mResSourceState == ResSourceState.Loading)
            {
                return false;
            }

            if (mResSourceState != ResSourceState.Ready)
            {
                return true;
            }

            //Log.I("Release ESResSource:" + mName);

            OnReleaseRes();

            mResSourceState = ResSourceState.Waiting;
            mOnResLoadDoneEvent = null;
            return true;
        }

        protected virtual void OnReleaseRes()
        {
            //如果Image 直接释放了，这里会直接变成NULL
            if (mAsset != null)
            {
                ESResMaster.UnloadRes(mAsset);

                mAsset = null;
            }
        }

       

        public virtual void TryAutoPushToPool()
        {
            
        }

        public virtual void OnBePushedToPool()
        {
            mAssetPath = null;
            mOnResLoadDoneEvent = null;
        }

        public virtual IEnumerator DoTaskAsync(System.Action finishCallback)
        {
            finishCallback();
            yield break;
        }

        public override string ToString()
        {
            return string.Format("This is ResSourceForAsset ,Name:{0}\t State:{1}", AssetPath, State);
        }

       

        #endregion
    }
    public enum ResSourceLoadType
    {
        [InspectorName("AB包")] AssetBundle = 0,
        [InspectorName("AB资源")] ABAsset = 1,
        [InspectorName("AB场景")] ABScene = 2,
        [InspectorName("内置的Res")] InternalResource = 3,
        [InspectorName("网络图片")] NetImageRes = 4,
        [InspectorName("本地图片")] LocalImageRes = 5,
    }
    public class ResSourceSearchKey : IPoolable, IPoolablebSelfControl
    {
        public string AssetPath { get; set; }

        public string OwnerAssetBundle { get; set; }

        public Type AssetType { get; set; }

        public string OriginalAssetName { get; set; }
        public ResSourceLoadType LoadType { get; set; }




        public void TryAutoPushToPool()
        {
            ESResMaster.Instance.PoolForResSourceSearchKey.PushToPool(this);
        }

        public bool Match(IResSource res)
        {
            if (res.AssetPath == AssetPath)
            {
                var isMatch = true;

                if (AssetType != null)
                {
                    isMatch = res.AssetType == AssetType;
                }

                if (OwnerAssetBundle != null)
                {
                    isMatch = isMatch && res.OwnerAssetBundleName == OwnerAssetBundle;
                }

                return isMatch;
            }


            return false;
        }

        public override string ToString()
        {
            return string.Format("AssetName:{0} BundleName:{1} TypeName:{2}", AssetPath, OwnerAssetBundle,
                AssetType);
        }

        public  void OnBePushedToPool()
        {
            AssetPath = null;

            OwnerAssetBundle = null;

            AssetType = null;
        }

       public bool IsRecycled { get; set; }
    }
    #region 资源类型:常规资源
    public class AssetResSource : ESResSource
    {
        protected string[] mAssetBundleArray;
        protected AssetBundleRequest mAssetBundleRequest;
        public string mOwnerBundleName;

        public override string OwnerAssetBundleName
        {
            get { return mOwnerBundleName; }
            set { mOwnerBundleName = value; }
        }

       

        public string AssetBundleName
        {
            get { return mAssetBundleArray == null ? null : mAssetBundleArray[0]; }
        }

        public AssetResSource(string assetName) : base(assetName)
        {

        }

        public AssetResSource()
        {

        }

        public  bool LoadSync2()
        {
            if (!CheckIsWaitingToLoad())
            {
                return false;
            }

            if (string.IsNullOrEmpty(AssetBundleName))
            {
                return false;
            }


            UnityEngine.Object obj = null;

           /* if (EditorMaster.Instance.abPackType == EditorMaster.ABPackType.Simulate && !string.Equals(mAssetPath, "assetbundlemanifest"))
            {
                var ResSourceSearchKey =ESResMaster.Instance.GetInPool_ResSourceSearchKey(AssetBundleName, null, typeof(Asset_Bundle));

                var abR = ESResMaster.Instance.GetResSourceBySearchKeys<LocalAssetBundleResSource>(ResSourceSearchKey);
                ResSourceSearchKey.TryAutoPushToPool();

                var assetPaths = AssetBundlePathHelper.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetPath, mAssetPath);
                if (assetPaths.Length == 0)
                {
                    Debug.LogError("Failed Load Asset:" + mAssetPath);
                    OnResLoadFaild();
                    return false;
                }

                HoldDependRes();

                State = ResSourceState.Loading;

                if (AssetType != null)
                {

                    obj = AssetBundlePathHelper.LoadAssetAtPath(assetPaths[0], AssetType);
                }
                else
                {
                    obj = AssetBundlePathHelper.LoadAssetAtPath<UnityEngine.Object>(assetPaths[0]);
                }
            }
            else
            {
                var ResSourceSearchKey = ESResMaster.Instance.GetInPool_ResSourceSearchKey(AssetBundleName, null, typeof(Asset_Bundle));
                var abR = ESResMaster.Instance.GetResSourceBySearchKeys<LocalAssetBundleResSource>(ResSourceSearchKey);
                ResSourceSearchKey.TryAutoPushToPool();


                if (abR == null || !abR.Asset_Bundle)
                {
                    Debug.LogError("Failed to Load Asset, Not Find AssetBundleImage:" + AssetBundleName);
                    return false;
                }

                HoldDependRes();

                State = ResSourceState.Loading;

                if (AssetType != null)
                {
                    obj = abR.Asset_Bundle.LoadAsset(mAssetPath, AssetType);
                }
                else
                {
                    obj = abR.Asset_Bundle.LoadAsset(mAssetPath);
                }
            }*/

            UnHoldDependRes();

            if (obj == null)
            {
                Debug.LogError("Failed Load Asset:" + mAssetPath + ":" + AssetType + ":" + AssetBundleName);
                OnResLoadFaild();
                return false;
            }

            mAsset = obj;

            State = ResSourceState.Ready;
            return true;
        }

        public  void LoadAsync2()
        {
            if (!CheckIsWaitingToLoad())
            {
                return;
            }

            if (string.IsNullOrEmpty(AssetBundleName))
            {
                return;
            }

            State = ResSourceState.Loading;

           
        }

        public override IEnumerator DoTaskAsync(System.Action finishCallback)
        {
           /* if (RefCountNow <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }
*/

            //Object obj = null;
            var ResSourceSearchKey = ESResMaster.Instance.GetInPool_ResSourceSearchKey(AssetBundleName, null,assetType: typeof(AssetBundle));
            var abR = ESResMaster.Instance.GetRes<LocalAssetBundleResSource>(ResSourceSearchKey);
            ResSourceSearchKey.TryAutoPushToPool();

            /*   if (AssetBundlePathHelper.SimulationMode && !string.Equals(mAssetPath, "assetbundlemanifest"))
               {
                   var assetPaths = AssetBundlePathHelper.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetPath, mAssetPath);
                   if (assetPaths.Length == 0)
                   {
                       Debug.LogError("Failed Load Asset:" + mAssetPath);
                       OnResLoadFaild();
                       finishCallback();
                       yield break;
                   }

                   //确保加载过程中依赖资源不被释放:目前只有AssetRes需要处理该情况
                   HoldDependRes();
                   State = ResSourceState.Loading;

                   // 模拟等一帧
                   yield return new WaitForEndOfFrame();

                   UnHoldDependRes();

                   if (AssetType != null)
                   {

                       mAsset = AssetBundlePathHelper.LoadAssetAtPath(assetPaths[0], AssetType);
                   }
                   else
                   {
                       mAsset = AssetBundlePathHelper.LoadAssetAtPath<UnityEngine.Object>(assetPaths[0]);
                   }

               }
               else
               {

                   if (abR == null || abR.Asset_Bundle == null)
                   {
                       Debug.LogError("Failed to Load Asset, Not Find AssetBundleImage:" + AssetBundleName);
                       OnResLoadFaild();
                       finishCallback();
                       yield break;
                   }


                   HoldDependRes();

                   State = ResSourceState.Loading;

                   AssetBundleRequest abQ = null;

                   if (AssetType != null)
                   {
                       abQ = abR.Asset_Bundle.LoadAssetAsync(mAssetPath, AssetType);
                       mAssetBundleRequest = abQ;

                       yield return abQ;
                   }
                   else
                   {
                       abQ = abR.Asset_Bundle.LoadAssetAsync(mAssetPath);
                       mAssetBundleRequest = abQ;

                       yield return abQ;
                   }

                   mAssetBundleRequest = null;

                   UnHoldDependRes();

                   if (!abQ.isDone)
                   {
                       Debug.LogError("Failed Load Asset:" + mAssetPath);
                       OnResLoadFaild();
                       finishCallback();
                       yield break;
                   }

                   mAsset = abQ.asset;
               }

               State = ResSourceState.Ready;

               finishCallback();*/
            yield return null;
        }

        public override string[] GetDependResSourceAllAssetBundles()
        {
            return mAssetBundleArray;
        }

        public override void OnBePushedToPool()
        {
            mAssetBundleArray = null;
        }

        public override void TryAutoPushToPool()
        {
            ESResMaster.Instance.PoolForAssetResSource.PushToPool(this);
        }

        protected override float CalculateProgress()
        {
            if (mAssetBundleRequest == null)
            {
                return 0;
            }

            return mAssetBundleRequest.progress;
        }

        public void InitAssetBundleName()
        {
           /* mAssetBundleArray = null;

            var ResSourceSearchKey = ESResMaster.Instance.GetInPool_ResSourceSearchKey(AssetBundleName, null, typeof(Asset_Bundle));

            var config = AssetBundleSettings.AssetBundleConfigFile.GetAssetData(ResSourceSearchKey);

            ResSourceSearchKey.TryAutoPushToPool();

            if (config == null)
            {
                Debug.LogError("Not Find AssetData For Asset:" + mAssetPath);
                return;
            }

            var assetBundleName = config.OwnerBundleName;

            if (string.IsNullOrEmpty(assetBundleName))
            {
                Debug.LogError("Not Find Asset_Bundle In Config:" + config.AssetBundleIndex + mOwnerBundleName);
                return;
            }

            mAssetBundleArray = new string[1];
            mAssetBundleArray[0] = assetBundleName;*/
        }

        public override string ToString()
        {
            return string.Format("Type:Asset\t {0}\t FromAssetBundle:{1}", base.ToString(), AssetBundleName);
        }
    }

    #endregion

    #region 资源类型:AB包体资源
   

    #endregion
}

