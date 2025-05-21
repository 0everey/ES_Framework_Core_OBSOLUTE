/*using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;

namespace ES
{
    #region 引用计数器
    public interface IRefCounter
    {
        int RefCountNow { get; }

        void Retain(object refOwner = null);
        void Release(object refOwner = null);
    }
    public class ESSimpleReferCounter : IRefCounter
    {
        public ESSimpleReferCounter()
        {
            RefCountNow = 0;
        }

        public int RefCountNow { get; private set; }

        public void Retain(object refOwner = null)
        {
            RefCountNow++;
        }

        public void Release(object refOwner = null)
        {
            RefCountNow--;
            if (RefCountNow == 0)
            {
                OnZeroRef();
            }
        }

        protected virtual void OnZeroRef()
        {
            //0引用处理事件
        }
    }
    #endregion


    #region 资源源声明
    public enum ResSourceState
    {
        [InspectorName("等待中(未使用)")]Waiting = 0,
        [InspectorName("加载中")] Loading = 1,
        [InspectorName("完毕")] Ready = 2,
    }
    public enum ResSourceLoadType
    {
        [InspectorName("AB包")] AssetBundle = 0,
        [InspectorName("AB资源")] ABAsset = 1,
        [InspectorName("AB场景")] ABScene = 2,
        [InspectorName("内置的")] Internal = 3,
        [InspectorName("网络图片")] NetImageRes = 4,
        [InspectorName("本地图片")] LocalImageRes = 5,
    }
    public interface IResSource : IRefCounter
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
    public class ESResSource : ESSimpleReferCounter, IResSource, IPoolable, IPoolablebSelfControl
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
            protected set { mAssetName = value; }
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
                var resSearchRule = ResSourceSearchKey.GetOne(depends[i], null, typeof(AssetBundle));
                var res = ESResMaster.Instance.GetRes(resSearchRule, false);
                resSearchRule.TryAutoPushToPool();

                if (res != null)
                {
                    res.Retain();
                }
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
                var resSearchRule = ResSourceSearchKey.GetOne(depends[i]);
                var res = ESResMaster.Instance.GetRes(resSearchRule, false);
                resSearchRule.TryAutoPushToPool();

                if (res != null)
                {
                    res.Release();
                }
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
                var resSearchRule = ResSourceSearchKey.GetOne(depends[i]);
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

        protected override void OnZeroRef()
        {
            if (mResSourceState == ResSourceState.Loading)
            {
                return;
            }

            ReleaseTheResSource();
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
            return string.Format("This is ResSourceForAsset ,Name:{0}\t State:{1}\t RefCount:{2}", AssetName, State, RefCountNow);
        }

        #endregion
    }
    public class ResSourceSearchKey : IPoolable, IPoolablebSelfControl
    {
        public string AssetName { get; set; }

        public string OwnerAssetBundle { get; set; }

        public Type AssetType { get; set; }

        public string OriginalAssetName { get; set; }


        public static ResSourceSearchKey GetOne(string assetName, string ownerBundleName = null, Type assetType = null)
        {
            var resSearchRule = ESResMaster.Instance.PoolForResSourceSearchKey.GetInPool();
            resSearchRule.AssetName = assetName.ToLower();
            resSearchRule.OwnerAssetBundle = ownerBundleName == null ? null : ownerBundleName.ToLower();
            resSearchRule.AssetType = assetType;
            resSearchRule.OriginalAssetName = assetName;
            return resSearchRule;
        }

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

        void IPoolable.OnBePushedToPool()
        {
            AssetName = null;

            OwnerAssetBundle = null;

            AssetType = null;
        }

        bool IPoolable.IsRecycled { get; set; }
    }
    #endregion

    #region 资源类型:常规资源
    public class AssetResSource : ESResSource
    {
        protected string[] mAssetBundleArray;
        protected AssetBundleRequest mAssetBundleRequest;
        protected string mOwnerBundleName;

        public override string OwnerAssetBundleName 
        {
            get { return mOwnerBundleName; }
            set { mOwnerBundleName = value; }
        }

        public static AssetResSource GetOne(string name, string onwerBundleName, Type assetTypde)
        {
            var res = ESResMaster.Instance.PoolForAssetResSource.GetInPool();
            if (res != null)
            {
                res.AssetName = name;
                res.mOwnerBundleName = onwerBundleName;
                res.AssetType = assetTypde;
                res.InitAssetBundleName();
            }

            return res;
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

            if (EditorMaster.Instance.abPackType== EditorMaster.ABPackType.Simulate&& !string.Equals(mAssetName, "assetbundlemanifest"))
            {
                var ResSourceSearchKey = ResSourceSearchKey.GetOne(AssetBundleName, null, typeof(AssetBundle));

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
                var ResSourceSearchKey = ResSourceSearchKey.GetOne(AssetBundleName, null, typeof(AssetBundle));
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
            }

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

            ResMgr.Instance.PushIEnumeratorTask(this);
        }

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            if (RefCountNow <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }


            //Object obj = null;
            var ResSourceSearchKey = ResSourceSearchKey.GetOne(AssetBundleName, null, typeof(AssetBundle));
            var abR = ESResMaster.Instance.GetRes<AssetBundleResSource>(ResSourceSearchKey);
            ResSourceSearchKey.TryAutoPushToPool();

            if (AssetBundlePathHelper.SimulationMode && !string.Equals(mAssetName, "assetbundlemanifest"))
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

            finishCallback();
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

        protected void InitAssetBundleName()
        {
            mAssetBundleArray = null;

            var ResSourceSearchKey = ResSourceSearchKey.GetOne(mAssetName, mOwnerBundleName, AssetType);

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
            mAssetBundleArray[0] = assetBundleName;
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


        public static AssetBundleResSource GetInPool_ESLoader(string name)
        {
            var res = ESResMaster.Instance.PoolForAssetBundleResSource.GetInPool();

            res.AssetName = name;
            res.AssetType = typeof(AssetBundle);
            res.InitAssetBundleName();

            return res;
        }

        private void InitAssetBundleName()
        {
            mDependResList = AssetBundleSettings.AssetBundleConfigFile.GetAllDependenciesByUrl(AssetName);
            mHash = AssetBundleSettings.AssetBundleConfigFile.GetABHash(AssetName);
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


            if (EditorMaster.Instance.abPackType== EditorMaster.ABPackType.Simulate)
            {

            }
            else
            {
                var url = AssetBundleSettings.AssetBundleName2Url(mHash != null
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

            return true;
        }

        public override void LoadAsync()
        {
            if (!CheckIsWaitingToLoad())
            {
                return;
            }

            State = ResSourceState.Loading;

            ResMgr.Instance.PushIEnumeratorTask(this);
        }

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            //开启的时候已经结束了
            if (RefCountNow <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            if (AssetBundlePathHelper.SimulationMode)
            {
                yield return null;
            }
            else
            {
                var url = AssetBundleSettings.AssetBundleName2Url(mHash != null
                    ? mAssetName + "_" + mHash
                    : mAssetName);

               *//* if (PlatformCheck.IsWebGL || PlatformCheck.IsWeixinMiniGame)
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
                else*//*
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
            }

            State = ResSourceState.Ready;
            finishCallback();
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
    #region 资源类型:网络图片资源


    #endregion
    #region 资源类型:常规资源


    #endregion
    #region 资源类型:常规资源


    #endregion

    #region 扩展辅助功能
    public interface IResDatas
    {
        string[] GetAllDependenciesByUrl(string url);

        void LoadFromFile(string outRes);
        void Reset();
        IEnumerator LoadFromFileAsync(string outRes);
        AssetData GetAssetData(ResSourceSearchKey ResSourceSearchKey);
        int AddAssetBundleName(string abName, string[] depends, out AssetDataGroup @group);
        string GetABHash(string assetName);
    }
    public class ResDatas : IResDatas
    {
        public string AESKey = string.Empty;


        [Serializable]
        public class SerializeData
        {
            private AssetDataGroup.SerializeData[] mAssetDataGroup;

            public AssetDataGroup.SerializeData[] AssetDataGroup
            {
                get { return mAssetDataGroup; }
                set { mAssetDataGroup = value; }
            }
        }

        /// <summary>
        /// 如果是以前的命名错误版本，大家可以通过设置 ResDatas.FileName = "asset_bindle_config.bin" 来兼容以前的代码; 
        /// </summary>
        public static string FileName = "asset_bundle_config.bin";

        public IList<AssetDataGroup> AllAssetDataGroups => mAllAssetDataGroup;

        private readonly List<AssetDataGroup> mAllAssetDataGroup = new List<AssetDataGroup>();

        private AssetDataTable mAssetDataTable = null;

        public void Reset()
        {
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                mAllAssetDataGroup[i].Reset();
            }

            mAllAssetDataGroup.Clear();

            if (mAssetDataTable != null)
            {
                mAssetDataTable.Dispose();
            }

            mAssetDataTable = null;
        }

        public int AddAssetBundleName(string name, string[] depends, out AssetDataGroup group)
        {
            group = null;

            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            var key = GetKeyFromABName(name);

            if (key == null)
            {
                return -1;
            }

            group = GetAssetDataGroup(key);

            if (group == null)
            {
                group = new AssetDataGroup(key);
                Debug.Log("#Create Config Group:" + key);
                mAllAssetDataGroup.Add(group);
            }

            return group.AddAssetBundleName(name, depends);
        }

        public string GetABHash(string assetName)
        {
            foreach (var assetDataGroup in mAllAssetDataGroup)
            {
                var abUnit = assetDataGroup.GetABUnit(assetName);

                if (abUnit != null)
                {
                    return abUnit.Hash;
                }
            }

            return null;
        }

        public string[] GetAllDependenciesByUrl(string url)
        {
            var abName = AssetBundleSettings.AssetBundleUrl2Name(url);

            for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string[] depends;
                if (!mAllAssetDataGroup[i].GetAssetBundleDepends(abName, out depends))
                {
                    continue;
                }

                return depends;
            }

            return null;
        }


        public AssetData GetAssetData(ResSourceSearchKey ResSourceSearchKey)
        {
            if (mAssetDataTable == null)
            {
                mAssetDataTable = new AssetDataTable();

                for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
                {
                    foreach (var assetData in mAllAssetDataGroup[i].AssetDatas)
                    {
                        mAssetDataTable.Add(assetData);
                    }
                }
            }

            return mAssetDataTable.GetAssetDataByResSourceSearchKey(ResSourceSearchKey);
        }

        public void LoadFromFile(string path)
        {
            var binarySerializer = ResKit.Get.Container.Get<IBinarySerializer>();
            var zipFileHelper = ResKit.Get.Container.Get<IZipFileHelper>();



            object data;

            //  if (File.ReadAllText(path).Contains(AES.AESHead))
            //  {
            //      if (AESKey == string.Empty)
            //      {
            //         AESKey=JsonUtility.FromJson<EncryptConfig>( Resources.Load<TextAsset>("EncryptConfig").text).AESKey;
            //      }
            //      data = binarySerializer
            // .DeserializeBinary((AES.AESFileByteDecrypt(path, AESKey)));
            //      //try
            //      //{
            //
            //      //}
            //      //catch (Exception e)
            //      //{
            //      //    Log.E("解密AB包失败,请检查秘钥!!当前使用的秘钥:" + AESKey);
            //      //}
            //
            //  }
            //  else
            //  {
            data = binarySerializer
       .DeserializeBinary(zipFileHelper.OpenReadStream(path));
            // }


            Debug.Log(path);

            if (data == null)
            {
                Debug.LogError("Failed Deserialize AssetDataTable:" + path);
                return;
            }

            var sd = data as SerializeData;

            if (sd == null)
            {
                Debug.LogError("Failed Load AssetDataTable:" + path);
                return;
            }


            SetSerializeData(sd);
        }


        public IEnumerator LoadFromFileAsync(string path)
        {
#pragma warning disable CS0618
            using (var www = new WWW(path))
#pragma warning restore CS0618
            {
                yield return www;

                if (www.error != null)
                {
                    Debug.LogError("Failed Deserialize AssetDataTable:" + path + " Error:" + www.error);
                    yield break;
                }

                var stream = new MemoryStream(www.bytes);

                var data = ResKit.Get.Container.Get<IBinarySerializer>()
                    .DeserializeBinary(stream);

                if (data == null)
                {
                    Debug.LogError("Failed Deserialize AssetDataTable:" + path);
                    yield break;
                }

                var sd = data as SerializeData;

                if (sd == null)
                {
                    Debug.LogError("Failed Load AssetDataTable:" + path);
                    yield break;
                }

                Debug.Log("Load AssetConfig From File:" + path);
                SetSerializeData(sd);
            }
        }

        public virtual void Save(string outPath)
        {
            var sd = new SerializeData
            {
                AssetDataGroup = new AssetDataGroup.SerializeData[mAllAssetDataGroup.Count]
            };

            for (var i = 0; i < mAllAssetDataGroup.Count; ++i)
            {
                sd.AssetDataGroup[i] = mAllAssetDataGroup[i].GetSerializeData();
            }

            if (ResKit.Get.Container.Get<IBinarySerializer>()
                .SerializeBinary(outPath, sd))
            {
                Debug.Log("Success Save AssetDataTable:" + outPath);
            }
            else
            {
                Debug.LogError("Failed Save AssetDataTable:" + outPath);
            }
        }

        private void SetSerializeData(SerializeData data)
        {

            if (data == null || data.AssetDataGroup == null)
            {
                return;
            }

            for (int i = data.AssetDataGroup.Length - 1; i >= 0; --i)
            {
                mAllAssetDataGroup.Add(BuildAssetDataGroup(data.AssetDataGroup[i]));
            }

            if (mAssetDataTable == null)
            {
                mAssetDataTable = new AssetDataTable();

                foreach (var serializeData in data.AssetDataGroup)
                {
                    foreach (var assetData in serializeData.assetDataArray)
                    {
                        mAssetDataTable.Add(assetData);
                    }
                }
            }
        }

        private AssetDataGroup BuildAssetDataGroup(AssetDataGroup.SerializeData data)
        {
            return new AssetDataGroup(data);
        }

        private AssetDataGroup GetAssetDataGroup(string key)
        {
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                if (mAllAssetDataGroup[i].key.Equals(key))
                {
                    return mAllAssetDataGroup[i];
                }
            }

            return null;
        }

        private static string GetKeyFromABName(string name)
        {
            var pIndex = name.IndexOf('/');

            if (pIndex < 0)
            {
                return name;
            }

            var key = name.Substring(0, pIndex);

            return key;
        }
    }
    [Serializable]
    public class AssetData
    {
        private string mAssetName;
        private string mOwnerBundleName;
        private int mAbIndex;
        private ResSourceLoadType mAssetType;
        private short mAssetObjectTypeCode = 0;

        public string AssetName
        {
            get => mAssetName;
            set => mAssetName = value;
        }

        public int AssetBundleIndex
        {
            get => mAbIndex;
            set => mAbIndex = value;
        }

        public string OwnerBundleName
        {
            get => mOwnerBundleName;
            set => mOwnerBundleName = value;
        }

        public short AssetObjectTypeCode
        {
            get => mAssetObjectTypeCode;
            set => mAssetObjectTypeCode = value;
        }

        public string UUID =>
            string.IsNullOrEmpty(mOwnerBundleName)
                ? AssetName
                : OwnerBundleName + AssetName;

        public ResSourceLoadType AssetType
        {
            get => mAssetType;
            set => mAssetType = value;
        }

        public AssetData(string assetName, ResSourceLoadType assetType, int abIndex, string ownerBundleName,
            short assetObjectTypeCode = 0)
        {
            mAssetName = assetName;
            mAssetType = assetType;
            mAbIndex = abIndex;
            mOwnerBundleName = ownerBundleName;
            mAssetObjectTypeCode = assetObjectTypeCode;
        }

        public AssetData()
        {
        }
    }
    public class AssetDataGroup
    {
        public IEnumerable<AssetData> AssetDatas => mAssetDataMap.Values;
        public IEnumerable<ABUnit> AssetBundleDatas => mABUnitArray;

        /// <summary>
        /// 代表依赖关系的类
        /// </summary>
        [Serializable]
        public class ABUnit
        {
            public string abName;
            public string[] abDepends;
            public string Hash;

            public ABUnit(string name, string[] depends)
            {
                abName = name;
                if (depends == null || depends.Length == 0)
                {

                }
                else
                {
                    abDepends = depends;
                }
            }

            public override string ToString()
            {
                var result = string.Format("ABName:" + abName);
                if (abDepends == null)
                {
                    return result;
                }

                foreach (var abDepend in abDepends)
                {
                    result += string.Format(" #:{0}", abDepend);
                }

                return result;
            }
        }

        [Serializable]
        public class SerializeData
        {
            private string mKey;
            private ABUnit[] mAbUnitArray;
            private AssetData[] mAssetDataArray;

            public string key
            {
                get { return mKey; }
                set { mKey = value; }
            }

            public ABUnit[] abUnitArray
            {
                get { return mAbUnitArray; }
                set { mAbUnitArray = value; }
            }

            public AssetData[] assetDataArray
            {
                get { return mAssetDataArray; }
                set { mAssetDataArray = value; }
            }
        }

        private string m_Key;

        private List<ABUnit> mABUnitArray;
        private Dictionary<string, AssetData> mAssetDataMap;
        private Dictionary<string, AssetData> mUUID4AssetData;
        public string key => m_Key;

        public AssetDataGroup(string key)
        {
            m_Key = key;
        }

        public AssetDataGroup(SerializeData data)
        {
            m_Key = data.key;
            SetSerializeData(data);
        }

        public void Reset()
        {
            mABUnitArray?.Clear();

            mAssetDataMap?.Clear();
        }

        public int AddAssetBundleName(string name, string[] depends)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            if (mABUnitArray == null)
            {
                mABUnitArray = new List<ABUnit>();
            }

            var resSearchRule = ResSourceSearchKey.GetOne(name);
            var config = GetAssetData(resSearchRule);
            resSearchRule.TryAutoPushToPool();

            if (config != null)
            {
                return config.AssetBundleIndex;
            }

            mABUnitArray.Add(new ABUnit(name, depends));

            int index = mABUnitArray.Count - 1;

            AddAssetData(new AssetData(name, ResSourceLoadType.AssetBundle, index, null));

            return index;
        }

        public bool GetAssetBundleName(string assetName, int index, out string result)
        {
            result = null;

            if (mABUnitArray == null)
            {
                return false;
            }

            if (index >= mABUnitArray.Count)
            {
                return false;
            }

            if (mAssetDataMap.ContainsKey(assetName))
            {
                result = mABUnitArray[index].abName;
                return true;
            }

            return false;
        }

        public ABUnit GetABUnit(string assetName)
        {
            var resSearchRule = ResSourceSearchKey.GetOne(assetName);

            AssetData data = GetAssetData(resSearchRule);

            resSearchRule.TryAutoPushToPool();

            if (data == null)
            {
                return null;
            }

            if (mABUnitArray == null)
            {
                return null;
            }

            return mABUnitArray[data.AssetBundleIndex];
        }

        public bool GetAssetBundleDepends(string abName, out string[] result)
        {
            result = null;

            ABUnit unit = GetABUnit(abName);

            if (unit == null)
            {
                return false;
            }

            result = unit.abDepends;

            return true;
        }

        public AssetData GetAssetData(ResSourceSearchKey resSearchRule)
        {
            AssetData result = null;

            if (resSearchRule.OwnerAssetBundle != null && mUUID4AssetData != null)
            {
                return mUUID4AssetData.TryGetValue(resSearchRule.OwnerAssetBundle + resSearchRule.AssetName, out result) ? result : null;
            }

            if (resSearchRule.OwnerAssetBundle == null && mAssetDataMap != null)
            {
                return mAssetDataMap.TryGetValue(resSearchRule.AssetName, out result) ? result : null;
            }

            return result;
        }

        public bool AddAssetData(AssetData data)
        {
            if (mAssetDataMap == null)
            {
                mAssetDataMap = new Dictionary<string, AssetData>();
            }

            if (mUUID4AssetData == null)
            {
                mUUID4AssetData = new Dictionary<string, AssetData>();
            }

            string key = data.AssetName.ToLower();

            if (mAssetDataMap.ContainsKey(key))
            {
                var resSearchRule = ResSourceSearchKey.GetOne(data.AssetName);
                var old = GetAssetData(resSearchRule);
                resSearchRule.TryAutoPushToPool();

                try
                {
                    Debug.LogFormat("Already Add AssetData :{0} \n OldAB:{1}      NewAB:{2}", data.AssetName,
                        mABUnitArray[old.AssetBundleIndex].abName, mABUnitArray[data.AssetBundleIndex].abName);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            else
            {
                mAssetDataMap.Add(key, data);
            }

            if (mUUID4AssetData.ContainsKey(data.UUID))
            {
                var resSearchRule = ResSourceSearchKey.GetOne(data.AssetName, data.OwnerBundleName);

                var old = GetAssetData(resSearchRule);
                resSearchRule.TryAutoPushToPool();

                Debug.LogWarningFormat("Already Add AssetData :{0} \n OldAB:{1}      NewAB:{2}", data.UUID,
                    mABUnitArray[old.AssetBundleIndex].abName, mABUnitArray[data.AssetBundleIndex].abName);
            }
            else
            {
                mUUID4AssetData.Add(data.UUID, data);
            }
            return true;
        }

        public SerializeData GetSerializeData()
        {
            var sd = new SerializeData();
            sd.key = m_Key;
            sd.abUnitArray = mABUnitArray.ToArray();
            if (mAssetDataMap != null)
            {
                var acArray = new AssetData[mAssetDataMap.Count];

                int index = 0;
                foreach (var item in mAssetDataMap)
                {
                    acArray[index++] = item.Value;
                }

                sd.assetDataArray = acArray;
            }

            return sd;
        }


        private void SetSerializeData(SerializeData data)
        {
            if (data == null)
            {
                return;
            }

            mABUnitArray = new List<ABUnit>(data.abUnitArray);

            if (data.assetDataArray != null)
            {
                mAssetDataMap = new Dictionary<string, AssetData>();

                foreach (var config in data.assetDataArray)
                {
                    AddAssetData(config);
                }
            }
        }
    }
    public class AssetBundleSettings
    {
        private static IResDatas mAssetBundleConfigFile = null;

        /// <summary>
        /// 默认
        /// </summary>
        private static Func<IResDatas> mAssetBundleConfigFileFactory = () => new ResDatas();

        public static Func<IResDatas> AssetBundleConfigFileFactory
        {
            set { mAssetBundleConfigFileFactory = value; }
        }

        /// <summary>
        /// 获取自定义的 资源信息
        /// </summary>
        /// <returns></returns>
        public static IResDatas AssetBundleConfigFile
        {
            get
            {
                if (mAssetBundleConfigFile == null)
                {
                    mAssetBundleConfigFile = mAssetBundleConfigFileFactory.Invoke();
                }

                return mAssetBundleConfigFile;
            }
            set { mAssetBundleConfigFile = value; }
        }

        public static List<IResDatas> SubProjectAssetBundleConfigFiles = new List<IResDatas>();

        public static bool LoadAssetResFromStreamingAssetsPath
        {
            get { return PlayerPrefs.GetInt("LoadResFromStreamingAssetsPath", 1) == 1; }
            set { PlayerPrefs.SetInt("LoadResFromStreamingAssetsPath", value ? 1 : 0); }
        }


        #region AssetBundle 相关

        public static string AssetBundleUrl2Name(string url)
        {
            string retName = null;
            string parren = AssetBundlePathHelper.StreamingAssetsPath + "AssetBundles/" +
                            AssetBundlePathHelper.GetPlatformName() + "/";
            retName = url.Replace(parren, "");

            parren = AssetBundlePathHelper.PersistentDataPath + "AssetBundles/" +
                     AssetBundlePathHelper.GetPlatformName() + "/";
            retName = retName.Replace(parren, "");
            return retName;
        }

        public static string AssetBundleName2Url(string name)
        {
            string retUrl = AssetBundlePathHelper.PersistentDataPath + "AssetBundles/" +
                            AssetBundlePathHelper.GetPlatformName() + "/" + name;

            if (File.Exists(retUrl))
            {
                return retUrl;
            }

            return AssetBundlePathHelper.StreamingAssetsPath + "AssetBundles/" +
                   AssetBundlePathHelper.GetPlatformName() + "/" + name;
        }

        //导出目录

        /// <summary>
        /// AssetBundle存放路径
        /// </summary>
        public static string RELATIVE_AB_ROOT_FOLDER
        {
            get { return "/AssetBundles/" + AssetBundlePathHelper.GetPlatformName() + "/"; }
        }
        public class AssetBundlePathHelper
        {
#if UNITY_EDITOR
            public static string GetPlatformForAssetBundles(BuildTarget target)
            {
                switch (target)
                {
                    case BuildTarget.Android:
                        return "Android";
                    case BuildTarget.WSAPlayer:
                        return "WSAPlayer";
                    case BuildTarget.iOS:
                        return "iOS";
                    case BuildTarget.WebGL:
                        return "WebGL";
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        return "Windows";
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinux:
#endif
                    case BuildTarget.StandaloneLinux64:
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinuxUniversal:
#endif
                        return "Linux";
#if !UNITY_2017_3_OR_NEWER
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
#elif UNITY_5
			case BuildTarget.StandaloneOSXUniversal:
#else
                    case BuildTarget.StandaloneOSX:
#endif
                        return "OSX";
                    // Add more build targets for your own.
                    // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                    default:
                        return target.ToString();
                }
            }
#endif

            // 资源路径，优先返回外存资源路径
            public static string GetResPathInPersistentOrStream(string relativePath)
            {
                string resPersistentPath = string.Format("{0}{1}", PersistentDataPath4Res, relativePath);
                if (File.Exists(resPersistentPath))
                {
                    return resPersistentPath;
                }
                else
                {
                    return StreamingAssetsPath + relativePath;
                }
            }


            private static string mPersistentDataPath;
            private static string mStreamingAssetsPath;
            private static string mPersistentDataPath4Res;
            private static string mPersistentDataPath4Photo;

            // 外部目录  
            public static string PersistentDataPath
            {
                get
                {
                    if (null == mPersistentDataPath)
                    {
                        mPersistentDataPath = Application.persistentDataPath + "/";
                    }

                    return mPersistentDataPath;
                }
            }

            // 内部目录
            public static string StreamingAssetsPath
            {
                get
                {
                    if (null == mStreamingAssetsPath)
                    {
#if UNITY_IPHONE && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
					mStreamingAssetsPath =
 Application.streamingAssetsPath + "/";//GetParentDir(Application.dataPath, 2) + "/BuildRes/";
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#else
                        mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#endif
                    }

                    return mStreamingAssetsPath;
                }
            }


            // 外部头像缓存目录
            public static string PersistentDataPath4Photo
            {
                get
                {
                    if (null == mPersistentDataPath4Photo)
                    {
                        mPersistentDataPath4Photo = PersistentDataPath + "Photos\\";

                        if (!Directory.Exists(mPersistentDataPath4Photo))
                        {
                            Directory.CreateDirectory(mPersistentDataPath4Photo);
                        }
                    }

                    return mPersistentDataPath4Photo;
                }
            }

            // 外部资源目录
            public static string PersistentDataPath4Res
            {
                get
                {
                    if (null == mPersistentDataPath4Res)
                    {
                        mPersistentDataPath4Res = PersistentDataPath + "Res/";

                        if (!Directory.Exists(mPersistentDataPath4Res))
                        {
                            Directory.CreateDirectory(mPersistentDataPath4Res);
#if UNITY_IPHONE && !UNITY_EDITOR
						UnityEngine.iOS.Device.SetNoBackupFlag(mPersistentDataPath4Res);
#endif
                        }
                    }

                    return mPersistentDataPath4Res;
                }
            }
            public static string GetPlatformName()
            {
#if UNITY_EDITOR
                return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(UnityEngine.Application.platform);
#endif
            }

            public static string GetPlatformForAssetBundles(RuntimePlatform platform)
            {
                switch (platform)
                {
                    case RuntimePlatform.Android:
                        return "Android";
                    case RuntimePlatform.WSAPlayerARM:
                    case RuntimePlatform.WSAPlayerX64:
                    case RuntimePlatform.WSAPlayerX86:
                        return "WSAPlayer";
                    case RuntimePlatform.IPhonePlayer:
                        return "iOS";
                    case RuntimePlatform.WebGLPlayer:
                        return "WebGL";
                    case RuntimePlatform.WindowsPlayer:
                        return "Windows";
                    case RuntimePlatform.OSXPlayer:
                        return "OSX";
                    case RuntimePlatform.LinuxPlayer:
                        return "Linux";
                    // Add more build targets for your own.
                    // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                    default:
                        return platform.ToString().RemoveString("Player");
                }
            }

            public static string[] GetAssetPathsFromAssetBundleAndAssetName(string abRAssetName, string assetName)
            {
#if UNITY_EDITOR
                return AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abRAssetName, assetName);
#else
            return null;
#endif
            }

            public static UnityEngine. Object LoadAssetAtPath(string assetPath, Type assetType)
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath(assetPath, assetType);
#else
            return null;
#endif
            }

            public static T LoadAssetAtPath<T>(string assetPath) where T : UnityEngine.Object
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            return null;
#endif
            }



            // 上一级目录
            public static string GetParentDir(string dir, int floor = 1)
            {
                string subDir = dir;

                for (int i = 0; i < floor; ++i)
                {
                    int last = subDir.LastIndexOf('/');
                    subDir = subDir.Substring(0, last);
                }

                return subDir;
            }

            public static void GetFileInFolder(string dirName, string fileName, List<string> outResult)
            {
                if (outResult == null)
                {
                    return;
                }

                var dir = new DirectoryInfo(dirName);

                if (null != dir.Parent && dir.Attributes.ToString().IndexOf("System", StringComparison.Ordinal) > -1)
                {
                    return;
                }

                var fileInfos = dir.GetFiles(fileName);
                outResult.AddRange(fileInfos.Select(fileInfo => fileInfo.FullName));

                var dirInfos = dir.GetDirectories();
                foreach (var dinfo in dirInfos)
                {
                    GetFileInFolder(dinfo.FullName, fileName, outResult);
                }
            }

            public static string PathPrefix
            {
                get
                {
#if UNITY_EDITOR || UNITY_IOS
                    return "file://";
#else
                return string.Empty;
#endif
                }
            }
#if UNITY_EDITOR
            const string kSimulateAssetBundles = "SimulateAssetBundles"; //此处跟editor中保持统一，不能随意更改

            public static bool SimulationMode
            {
                get { return UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true); }
                set { UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value); }
            }
#else
         public static bool SimulationMode
         {
             get { return false; }
             set {  }
         }
#endif
        }
        #endregion
    }
    public class AssetBundlePathHelper
    {
#if UNITY_EDITOR
        public static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.WSAPlayer:
                    return "WSAPlayer";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinux:
#endif
                case BuildTarget.StandaloneLinux64:
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinuxUniversal:
#endif
                    return "Linux";
#if !UNITY_2017_3_OR_NEWER
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
#elif UNITY_5
			case BuildTarget.StandaloneOSXUniversal:
#else
                case BuildTarget.StandaloneOSX:
#endif
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return target.ToString();
            }
        }
#endif

        // 资源路径，优先返回外存资源路径
        public static string GetResPathInPersistentOrStream(string relativePath)
        {
            string resPersistentPath = string.Format("{0}{1}", PersistentDataPath4Res, relativePath);
            if (File.Exists(resPersistentPath))
            {
                return resPersistentPath;
            }
            else
            {
                return StreamingAssetsPath + relativePath;
            }
        }


        private static string mPersistentDataPath;
        private static string mStreamingAssetsPath;
        private static string mPersistentDataPath4Res;
        private static string mPersistentDataPath4Photo;

        // 外部目录  
        public static string PersistentDataPath
        {
            get
            {
                if (null == mPersistentDataPath)
                {
                    mPersistentDataPath = Application.persistentDataPath + "/";
                }

                return mPersistentDataPath;
            }
        }

        // 内部目录
        public static string StreamingAssetsPath
        {
            get
            {
                if (null == mStreamingAssetsPath)
                {
#if UNITY_IPHONE && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
					mStreamingAssetsPath =
 Application.streamingAssetsPath + "/";//GetParentDir(Application.dataPath, 2) + "/BuildRes/";
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#else
                    mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#endif
                }

                return mStreamingAssetsPath;
            }
        }


        // 外部头像缓存目录
        public static string PersistentDataPath4Photo
        {
            get
            {
                if (null == mPersistentDataPath4Photo)
                {
                    mPersistentDataPath4Photo = PersistentDataPath + "Photos\\";

                    if (!Directory.Exists(mPersistentDataPath4Photo))
                    {
                        Directory.CreateDirectory(mPersistentDataPath4Photo);
                    }
                }

                return mPersistentDataPath4Photo;
            }
        }

        // 外部资源目录
        public static string PersistentDataPath4Res
        {
            get
            {
                if (null == mPersistentDataPath4Res)
                {
                    mPersistentDataPath4Res = PersistentDataPath + "Res/";

                    if (!Directory.Exists(mPersistentDataPath4Res))
                    {
                        Directory.CreateDirectory(mPersistentDataPath4Res);
#if UNITY_IPHONE && !UNITY_EDITOR
						UnityEngine.iOS.Device.SetNoBackupFlag(mPersistentDataPath4Res);
#endif
                    }
                }

                return mPersistentDataPath4Res;
            }
        }
        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(UnityEngine.Application.platform);
#endif
        }

        public static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                    return "WSAPlayer";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.LinuxPlayer:
                    return "Linux";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return platform.ToString().RemoveString("Player");
            }
        }

        public static string[] GetAssetPathsFromAssetBundleAndAssetName(string abRAssetName, string assetName)
        {
#if UNITY_EDITOR
            return AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abRAssetName, assetName);
#else
            return null;
#endif
        }

        public static UnityEngine.Object LoadAssetAtPath(string assetPath, Type assetType)
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath(assetPath, assetType);
#else
            return null;
#endif
        }

        public static T LoadAssetAtPath<T>(string assetPath) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            return null;
#endif
        }



        // 上一级目录
        public static string GetParentDir(string dir, int floor = 1)
        {
            string subDir = dir;

            for (int i = 0; i < floor; ++i)
            {
                int last = subDir.LastIndexOf('/');
                subDir = subDir.Substring(0, last);
            }

            return subDir;
        }

        public static void GetFileInFolder(string dirName, string fileName, List<string> outResult)
        {
            if (outResult == null)
            {
                return;
            }

            var dir = new DirectoryInfo(dirName);

            if (null != dir.Parent && dir.Attributes.ToString().IndexOf("System", StringComparison.Ordinal) > -1)
            {
                return;
            }

            var fileInfos = dir.GetFiles(fileName);
            outResult.AddRange(fileInfos.Select(fileInfo => fileInfo.FullName));

            var dirInfos = dir.GetDirectories();
            foreach (var dinfo in dirInfos)
            {
                GetFileInFolder(dinfo.FullName, fileName, outResult);
            }
        }

        public static string PathPrefix
        {
            get
            {
#if UNITY_EDITOR || UNITY_IOS
                return "file://";
#else
                return string.Empty;
#endif
            }
        }
#if UNITY_EDITOR
        const string kSimulateAssetBundles = "SimulateAssetBundles"; //此处跟editor中保持统一，不能随意更改

        public static bool SimulationMode
        {
            get { return UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true); }
            set { UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value); }
        }
#else
         public static bool SimulationMode
         {
             get { return false; }
             set {  }
         }
#endif
    }
    #endregion
}*/