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
    public interface IResSource 
    {
        string AssetName { get; }

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

        string[] GetDependResSource();

        bool IsDependResLoadFinish();

        bool ReleaseTheResSource();

        void TryAutoPushToPool();

    }
    public class ESResSource : IResSource, IPoolable, IPoolablebSelfControl
    {
        #region 字段被保护，使用属性获取

        protected string mAssetName;
        private ResSourceState mResSourceState = ResSourceState.Waiting;
        protected UnityEngine.Object mAsset;
        private event Action<bool, IResSource> mOnResLoadDoneEvent;

        #endregion
        public string AssetName
        {
            get { return mAssetName; }
            set { mAssetName = value; }
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
            mAssetName = assetName;
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
            var depends = GetDependResSource();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ESResMaster.Instance.GetInPool_ResSourceSearchKey(depends[i], null, typeof(AssetBundle));
                var res = ESResMaster.Instance.GetRes(resSearchRule, false);
                resSearchRule.TryAutoPushToPool();
            }
        }

        protected void UnHoldDependRes()
        {
            var depends = GetDependResSource();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ESResMaster.Instance.GetInPool_ResSourceSearchKey(depends[i]);
                var res = ESResMaster.Instance.GetRes(resSearchRule, false);
                resSearchRule.TryAutoPushToPool();
                res.TryAutoPushToPool();
            }
        }

        #region 子类实现

        public virtual bool LoadSync()
        {
            return false;
        }

        public virtual void LoadAsync()
        {

        }

        public virtual string[] GetDependResSource()
        {
            return null;
        }

        public bool IsDependResLoadFinish()
        {
            var depends = GetDependResSource();
            if (depends == null || depends.Length == 0)
            {
                return true;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ESResMaster.Instance.GetInPool_ResSourceSearchKey(depends[i]);
                var res = ESResMaster.Instance.GetRes(resSearchRule, false);
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
            mAssetName = null;
            mOnResLoadDoneEvent = null;
        }

        public virtual IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            finishCallback();
            yield break;
        }

        public override string ToString()
        {
            return string.Format("This is ResSourceForAsset ,Name:{0}\t State:{1}/*\t RefCount:{2}*/", AssetName, State);
        }
   
        #endregion
    }
    public class ResSourceSearchKey : IPoolable, IPoolablebSelfControl
    {
        public string AssetName { get; set; }

        public string OwnerAssetBundle { get; set; }

        public Type AssetType { get; set; }

        public string OriginalAssetName { get; set; }


      

        public void TryAutoPushToPool()
        {
            ESResMaster.Instance.PoolForResSourceSearchKey.PushToPool(this);
        }

        public bool Match(IResSource res)
        {
            if (res.AssetName == AssetName)
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
            return string.Format("AssetName:{0} BundleName:{1} TypeName:{2}", AssetName, OwnerAssetBundle,
                AssetType);
        }

        public  void OnBePushedToPool()
        {
            AssetName = null;

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

        public override bool LoadSync()
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

           /* if (EditorMaster.Instance.abPackType == EditorMaster.ABPackType.Simulate && !string.Equals(mAssetName, "assetbundlemanifest"))
            {
                var ResSourceSearchKey =ESResMaster.Instance.GetInPool_ResSourceSearchKey(AssetBundleName, null, typeof(AssetBundle));

                var abR = ESResMaster.Instance.GetRes<AssetBundleResSource>(ResSourceSearchKey);
                ResSourceSearchKey.TryAutoPushToPool();

                var assetPaths = AssetBundlePathHelper.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetName, mAssetName);
                if (assetPaths.Length == 0)
                {
                    Debug.LogError("Failed Load Asset:" + mAssetName);
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
                var ResSourceSearchKey = ESResMaster.Instance.GetInPool_ResSourceSearchKey(AssetBundleName, null, typeof(AssetBundle));
                var abR = ESResMaster.Instance.GetRes<AssetBundleResSource>(ResSourceSearchKey);
                ResSourceSearchKey.TryAutoPushToPool();


                if (abR == null || !abR.AssetBundle)
                {
                    Debug.LogError("Failed to Load Asset, Not Find AssetBundleImage:" + AssetBundleName);
                    return false;
                }

                HoldDependRes();

                State = ResSourceState.Loading;

                if (AssetType != null)
                {
                    obj = abR.AssetBundle.LoadAsset(mAssetName, AssetType);
                }
                else
                {
                    obj = abR.AssetBundle.LoadAsset(mAssetName);
                }
            }*/

            UnHoldDependRes();

            if (obj == null)
            {
                Debug.LogError("Failed Load Asset:" + mAssetName + ":" + AssetType + ":" + AssetBundleName);
                OnResLoadFaild();
                return false;
            }

            mAsset = obj;

            State = ResSourceState.Ready;
            return true;
        }

        public override void LoadAsync()
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

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
           /* if (RefCountNow <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }
*/

            //Object obj = null;
            var ResSourceSearchKey = ESResMaster.Instance.GetInPool_ResSourceSearchKey(AssetBundleName, null, typeof(AssetBundle));
            var abR = ESResMaster.Instance.GetRes<AssetBundleResSource>(ResSourceSearchKey);
            ResSourceSearchKey.TryAutoPushToPool();

            /*   if (AssetBundlePathHelper.SimulationMode && !string.Equals(mAssetName, "assetbundlemanifest"))
               {
                   var assetPaths = AssetBundlePathHelper.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetName, mAssetName);
                   if (assetPaths.Length == 0)
                   {
                       Debug.LogError("Failed Load Asset:" + mAssetName);
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

                   if (abR == null || abR.AssetBundle == null)
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
                       abQ = abR.AssetBundle.LoadAssetAsync(mAssetName, AssetType);
                       mAssetBundleRequest = abQ;

                       yield return abQ;
                   }
                   else
                   {
                       abQ = abR.AssetBundle.LoadAssetAsync(mAssetName);
                       mAssetBundleRequest = abQ;

                       yield return abQ;
                   }

                   mAssetBundleRequest = null;

                   UnHoldDependRes();

                   if (!abQ.isDone)
                   {
                       Debug.LogError("Failed Load Asset:" + mAssetName);
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

        public override string[] GetDependResSource()
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

            var ResSourceSearchKey = ESResMaster.Instance.GetInPool_ResSourceSearchKey(AssetBundleName, null, typeof(AssetBundle));

            var config = AssetBundleSettings.AssetBundleConfigFile.GetAssetData(ResSourceSearchKey);

            ResSourceSearchKey.TryAutoPushToPool();

            if (config == null)
            {
                Debug.LogError("Not Find AssetData For Asset:" + mAssetName);
                return;
            }

            var assetBundleName = config.OwnerBundleName;

            if (string.IsNullOrEmpty(assetBundleName))
            {
                Debug.LogError("Not Find AssetBundle In Config:" + config.AssetBundleIndex + mOwnerBundleName);
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
    public class AssetBundleResSource : ESResSource
    {
        private bool mUnloadFlag = true;
        private string[] mDependResList;
        private AsyncOperation mAssetBundleCreateRequest;
        public string AESKey = string.Empty;
        private string mHash;


       
        public void InitAssetBundleName()
        {
            /*mDependResList = AssetBundleSettings.AssetBundleConfigFile.GetAllDependenciesByUrl(AssetName);
            mHash = AssetBundleSettings.AssetBundleConfigFile.GetABHash(AssetName);*/
        }

        public AssetBundle AssetBundle
        {
            get { return (AssetBundle)mAsset; }
            private set { mAsset = value; }
        }

        public override bool LoadSync()
        {
            if (!CheckIsWaitingToLoad())
            {
                return false;
            }

            State = ResSourceState.Loading;


            if (EditorMaster.Instance.abPackType == EditorMaster.ABPackType.Simulate)
            {

            }
            else
            {
                /* var url = AssetBundleSettings.AssetBundleName2Url(mHash != null
                     ? mAssetName + "_" + mHash
                     : mAssetName);
                 AssetBundle bundle;
                 // var zipFileHelper = ResKit.Architecture.Interface.GetUtility<IZipFileHelper>();

                 // if (File.ReadAllText(url).Contains(AES.AESHead))
                 // {
                 //     if (AESKey == string.Empty)
                 //     {
                 //         AESKey = JsonUtility.FromJson<EncryptConfig>(Resources.Load<TextAsset>("EncryptConfig").text).AESKey;
                 //     }
                 //  
                 //      bundle= AssetBundle.LoadFromMemory((AES.AESFileByteDecrypt(url, AESKey)));
                 //  
                 // }
                 // else
                 // {
                 bundle = AssetBundle.LoadFromFile(url);
                 // }

                 mUnloadFlag = true;

                 if (bundle == null)
                 {
                     Debug.LogError("Failed Load AssetBundle:" + mAssetName);
                     OnResLoadFaild();
                     return false;
                 }

                 AssetBundle = bundle;
             }

             State = ResSourceState.Ready;
 */
                return true;
            }
            return true;
        }
        public override void LoadAsync()
        {
            if (!CheckIsWaitingToLoad())
            {
                return;
            }

            State = ResSourceState.Loading;

           /* ResMgr.Instance.PushIEnumeratorTask(this);*/
        }

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            //开启的时候已经结束了
          /*  if (RefCountNow <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }*/

            /*if (AssetBundlePathHelper.SimulationMode)
            {
                yield return null;
            }
            else
            {
                var url = AssetBundleSettings.AssetBundleName2Url(mHash != null
                    ? mAssetName + "_" + mHash
                    : mAssetName);

                if (PlatformCheck.IsWebGL || PlatformCheck.IsWeixinMiniGame)
                {
                    var abcR = UnityWebRequestAssetBundle.GetAssetBundle(url);
                    var request = abcR.SendWebRequest();

                    mAssetBundleCreateRequest = request;
                    yield return request;
                    mAssetBundleCreateRequest = null;

                    if (!request.isDone)
                    {
                        Debug.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetName);
                        OnResLoadFaild();
                        finishCallback();
                        yield break;
                    }

                    var ab = DownloadHandlerAssetBundle.GetContent(abcR);

                    AssetBundle = ab;

                    // 销毁
                    abcR.Dispose();
                }
                else *//*
                {
                    var abcR = AssetBundle.LoadFromFileAsync(url);

                    mAssetBundleCreateRequest = abcR;
                    yield return abcR;
                    mAssetBundleCreateRequest = null;

                    if (!abcR.isDone)
                    {
                        Debug.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetName);
                        OnResLoadFaild();
                        finishCallback();
                        yield break;
                    }

                    AssetBundle = abcR.assetBundle;
                }
            }*/

            State = ResSourceState.Ready;
            finishCallback();
            yield return null;
        }

        public override string[] GetDependResSource()
        {
            return mDependResList;
        }

        public override bool UnloadImage(bool flag)
        {
            if (AssetBundle != null)
            {
                mUnloadFlag = flag;
            }

            return true;
        }

        public override void TryAutoPushToPool()
        {
            ESResMaster.Instance.PoolForAssetBundleResSource.PushToPool(this);
        }

        public override void OnBePushedToPool()
        {
            base.OnBePushedToPool();
            mUnloadFlag = true;
            mDependResList = null;
        }

        protected override float CalculateProgress()
        {
            if (mAssetBundleCreateRequest == null)
            {
                return 0;
            }

            return mAssetBundleCreateRequest.progress;
        }

        protected override void OnReleaseRes()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(mUnloadFlag);
                AssetBundle = null;
            }
        }

        public override string ToString()
        {
            return $"Type:AssetBundle\t {base.ToString()}";
        }
    }

    #endregion
}

