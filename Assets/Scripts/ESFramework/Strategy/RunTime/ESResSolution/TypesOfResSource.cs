using ES;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ES
{

    public class TypesOfResSource
    {

    }
    #region 内置资源
    public class InternalResourceRes : ESResSource
    {
        public override bool LoadSync()
        {
            if (State == ResSourceState.Ready) return true;
            mAsset = Resources.Load(AssetPath) ?? mAsset;
            if (mAsset == null)
            {
                OnResLoadFaild();
                return false;
            }
            State = ResSourceState.Ready;
            return true;
        }

        public override IEnumerator DoTaskAsync(Action finishCallback)
        {
            if (State == ResSourceState.Ready) yield break;
            var request = Resources.LoadAsync(AssetPath);
            yield return request;
            mAsset = request.asset ?? mAsset;
            if (mAsset == null)
            {
                OnResLoadFaild();
                yield break;
            }
            State = ResSourceState.Ready;
        }
    }
    #endregion

    #region AB包本体
    public class LocalAssetBundleResSource : ESResSource
    {
        private bool mUnloadFlag = true;
        private string[] needDepends;
        private AsyncOperation mAssetBundleCreateRequest;
        public string AESKey = string.Empty;
        private string mHash;



        public void InitAssetBundleName()
        {
            //依赖获取
            needDepends = ESResMaster.ABHelper.AssetBundleConfigFile.GetAllDependenciesByLocalAB(AssetPath);
            mHash = ESResMaster.ABHelper.AssetBundleConfigFile?.GetABHash(AssetPath);
        }

        public AssetBundle Asset_Bundle
        {
            get { return (AssetBundle)mAsset; }
            private set { mAsset = value; }
        }

        public bool LoadSync2()
        {
            if (!CheckIsWaitingToLoad())
            {
                return false;
            }

            State = ResSourceState.Loading;


            if (ESResMaster.Instance.abPackType == ESResMaster.ABPackType.Simulate)
            {

            }
            else
            {
                /* var url = AssetBundleSettings.AssetBundleName2Url(mHash != null
                     ? mAssetPath + "_" + mHash
                     : mAssetPath);
                 Asset_Bundle bundle;
                 // var zipFileHelper = ResKit.Architecture.Interface.GetUtility<IZipFileHelper>();

                 // if (File.ReadAllText(url).Contains(AES.AESHead))
                 // {
                 //     if (AESKey == string.Empty)
                 //     {
                 //         AESKey = JsonUtility.FromJson<EncryptConfig>(Resources.Load<TextAsset>("EncryptConfig").text).AESKey;
                 //     }
                 //  
                 //      bundle= Asset_Bundle.LoadFromMemory((AES.AESFileByteDecrypt(url, AESKey)));
                 //  
                 // }
                 // else
                 // {
                 bundle = Asset_Bundle.LoadFromFile(url);
                 // }

                 mUnloadFlag = true;

                 if (bundle == null)
                 {
                     Debug.LogError("Failed Load Asset_Bundle:" + mAssetPath);
                     OnResLoadFaild();
                     return false;
                 }

                 Asset_Bundle = bundle;
             }

             State = ResSourceState.Ready;
 */
                return true;
            }
            return true;
        }
        public void LoadAsync2()
        {
            if (!CheckIsWaitingToLoad())
            {
                return;
            }

            State = ResSourceState.Loading;

            /* ResMgr.Instance.PushIEnumeratorTask(this);*/
        }
        public override bool LoadSync()
        {
            if (State == ResSourceState.Ready) return true;
            Asset_Bundle =ESResMaster.Instance.HasLoadedAB(AssetPath);
            if (Asset_Bundle != null)
            {
                State = ResSourceState.Ready;
                return true;
            }
            var depends = GetDependResSourceAllAssetBundles();
            if (depends.Length > 0)
            {
                foreach (var i in depends)
                {
                    var key = ESResMaster.Instance.GetInPool_ResSourceSearchKey(i, i, ResSourceLoadType.AssetBundle);
                    var ii = ESResMaster.Instance.GetResSourceBySearchKeys(key, false);
                    if (ii == null) ESResMaster.Instance.MainLoader.Add2Load(key);
                }
                ESResMaster.Instance.MainLoader.LoadAll_Sync();
            }
            Asset_Bundle = AssetBundle.LoadFromFile(ESResMaster.ABHelper.StreamingAB + "/" + AssetPath) ?? Asset_Bundle;
            if (Asset_Bundle != null)
            {
                State = ResSourceState.Ready;
            }
            return true;
        }
        public override IEnumerator DoTaskAsync(System.Action finishCallback)
        {
            if (State == ResSourceState.Ready) yield break;
            Asset_Bundle = ESResMaster.Instance.HasLoadedAB(AssetPath);
            if (Asset_Bundle != null)
            {
                State = ResSourceState.Ready;
                yield break;
            }

            var depends = GetDependResSourceAllAssetBundles();
            if (depends.Length > 0)
            {
                foreach (var i in depends)
                {
                    var key = ESResMaster.Instance.GetInPool_ResSourceSearchKey(i, i, ResSourceLoadType.AssetBundle);
                    var ii = ESResMaster.Instance.GetResSourceBySearchKeys(key, false);
                    if (ii == null) ESResMaster.Instance.MainLoader.Add2Load(key, (b,res) => { Debug.Log($"AB包:{AssetPath},自动加载依赖AB"+res.AssetPath); });
                }
                ESResMaster.Instance.MainLoader.LoadAll_Async(() => {  ESResMaster.Instance.StartCoroutine(LoadSelf(finishCallback)); });
            }
            else
            {
                ESResMaster.Instance.StartCoroutine(LoadSelf(finishCallback));
            }
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
                    ? mAssetPath + "_" + mHash
                    : mAssetPath);

                if (PlatformCheck.IsWebGL || PlatformCheck.IsWeixinMiniGame)
                {
                    var abcR = UnityWebRequestAssetBundle.GetAssetBundle(url);
                    var request = abcR.SendWebRequest();

                    mAssetBundleCreateRequest = request;
                    yield return request;
                    mAssetBundleCreateRequest = null;

                    if (!request.isDone)
                    {
                        Debug.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetPath);
                        OnResLoadFaild();
                        finishCallback();
                        yield break;
                    }

                    var ab = DownloadHandlerAssetBundle.GetContent(abcR);

                    Asset_Bundle = ab;

                    // 销毁
                    abcR.Dispose();
                }
                else *//*
                {
                    var abcR = Asset_Bundle.LoadFromFileAsync(url);

                    mAssetBundleCreateRequest = abcR;
                    yield return abcR;
                    mAssetBundleCreateRequest = null;

                    if (!abcR.isDone)
                    {
                        Debug.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetPath);
                        OnResLoadFaild();
                        finishCallback();
                        yield break;
                    }

                    Asset_Bundle = abcR.assetBundle;
                }
            }*/
        }
        public IEnumerator LoadSelf(System.Action finishCallback)
        {
            if (Asset_Bundle == null)
            {
                var request = AssetBundle.LoadFromFileAsync(ESResMaster.ABHelper.StreamingAB + "/" + AssetPath);
                yield return request;
                Asset_Bundle = request.assetBundle;
                if (Asset_Bundle == null)
                {
                    OnResLoadFaild();
                    yield break;
                }
            }
            State = ResSourceState.Ready;
            finishCallback();
        }
        public override string[] GetDependResSourceAllAssetBundles()
        {
            return needDepends;
        }

        public override bool UnloadImage(bool flag)
        {
            if (Asset_Bundle != null)
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
            needDepends = null;
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
            if (Asset_Bundle != null)
            {
                Asset_Bundle.Unload(mUnloadFlag);
                Asset_Bundle = null;
            }
        }

        public override string ToString()
        {
            return $"Type:AssetBundle\t {base.ToString()}";
        }
    }


    #endregion

    #region AB资源
    public class LocalABAssetResSource : ESResSource
    {
        private bool mUnloadFlag = true;
        private string[] needDepends;
        private AsyncOperation mAssetBundleCreateRequest;
        public string AESKey = string.Empty;
        private string mHash;



        public void InitAssetBundleName()
        {
            //依赖获取
           /* needDepends = ESResMaster.ABHelper.AssetBundleConfigFile.GetAllDependenciesByLocalAB(AssetPath);
            mHash = ESResMaster.ABHelper.AssetBundleConfigFile?.GetABHash(AssetPath);*/
        }



        public bool LoadSync2()
        {
            if (!CheckIsWaitingToLoad())
            {
                return false;
            }

            State = ResSourceState.Loading;


            if (ESResMaster.Instance.abPackType == ESResMaster.ABPackType.Simulate)
            {

            }
            else
            {
                /* var url = AssetBundleSettings.AssetBundleName2Url(mHash != null
                     ? mAssetPath + "_" + mHash
                     : mAssetPath);
                 Asset_Bundle bundle;
                 // var zipFileHelper = ResKit.Architecture.Interface.GetUtility<IZipFileHelper>();

                 // if (File.ReadAllText(url).Contains(AES.AESHead))
                 // {
                 //     if (AESKey == string.Empty)
                 //     {
                 //         AESKey = JsonUtility.FromJson<EncryptConfig>(Resources.Load<TextAsset>("EncryptConfig").text).AESKey;
                 //     }
                 //  
                 //      bundle= Asset_Bundle.LoadFromMemory((AES.AESFileByteDecrypt(url, AESKey)));
                 //  
                 // }
                 // else
                 // {
                 bundle = Asset_Bundle.LoadFromFile(url);
                 // }

                 mUnloadFlag = true;

                 if (bundle == null)
                 {
                     Debug.LogError("Failed Load Asset_Bundle:" + mAssetPath);
                     OnResLoadFaild();
                     return false;
                 }

                 Asset_Bundle = bundle;
             }

             State = ResSourceState.Ready;
 */
                return true;
            }
            return true;
        }
        public void LoadAsync2()
        {
            if (!CheckIsWaitingToLoad())
            {
                return;
            }

            State = ResSourceState.Loading;

            /* ResMgr.Instance.PushIEnumeratorTask(this);*/
        }
        public override bool LoadSync()
        {
            var key = ESResMaster.Instance.GetInPool_ResSourceSearchKey(OwnerAssetBundleName, OwnerAssetBundleName, ResSourceLoadType.AssetBundle);
            var ii = ESResMaster.Instance.GetResSourceBySearchKeys(key, true);
            ii.LoadSync();
            if (ii.Asset is AssetBundle ab)
            {
                mAsset = ab.LoadAsset(AssetPath) ?? mAsset;
                if (mAsset != null)
                {
                    State = ResSourceState.Ready;
                    return true;
                }
            }
            return false;
        }
        public override IEnumerator DoTaskAsync(System.Action finishCallback)
        {
            if (State == ResSourceState.Ready) yield break;
            
            var key = ESResMaster.Instance.GetInPool_ResSourceSearchKey(OwnerAssetBundleName, OwnerAssetBundleName, ResSourceLoadType.AssetBundle);
            var ii = ESResMaster.Instance.GetResSourceBySearchKeys(key, true);
            ii.OnLoadOK_Submit((b,res)=> { if (b) { Debug.Log("资产自动加载依赖AB"+ii.AssetPath); ESResMaster.Instance.StartCoroutine(LoadSelf(res.Asset as AssetBundle, finishCallback)); } });
            ii.LoadAsync();
            yield return null;
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
                    ? mAssetPath + "_" + mHash
                    : mAssetPath);

                if (PlatformCheck.IsWebGL || PlatformCheck.IsWeixinMiniGame)
                {
                    var abcR = UnityWebRequestAssetBundle.GetAssetBundle(url);
                    var request = abcR.SendWebRequest();

                    mAssetBundleCreateRequest = request;
                    yield return request;
                    mAssetBundleCreateRequest = null;

                    if (!request.isDone)
                    {
                        Debug.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetPath);
                        OnResLoadFaild();
                        finishCallback();
                        yield break;
                    }

                    var ab = DownloadHandlerAssetBundle.GetContent(abcR);

                    Asset_Bundle = ab;

                    // 销毁
                    abcR.Dispose();
                }
                else *//*
                {
                    var abcR = Asset_Bundle.LoadFromFileAsync(url);

                    mAssetBundleCreateRequest = abcR;
                    yield return abcR;
                    mAssetBundleCreateRequest = null;

                    if (!abcR.isDone)
                    {
                        Debug.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetPath);
                        OnResLoadFaild();
                        finishCallback();
                        yield break;
                    }

                    Asset_Bundle = abcR.assetBundle;
                }
            }*/
        }
        public IEnumerator LoadSelf(AssetBundle ab, System.Action finishCallback)
        {
            if (ab != null)
            {
                var request = ab.LoadAssetAsync(AssetPath);
                yield return request;
                mAsset = request.asset ?? mAsset;

            }
            if (mAsset == null)
            {
                OnResLoadFaild();
                yield break;
            }
            State = ResSourceState.Ready;
            finishCallback();
        }
        public override string[] GetDependResSourceAllAssetBundles()
        {
            return needDepends;
        }

        public override bool UnloadImage(bool flag)
        {
            if (Asset != null)
            {
                mUnloadFlag = flag;
            }

            return true;
        }

        public override void TryAutoPushToPool()
        {
            ESResMaster.Instance.PoolForABAssetResSource.PushToPool(this);
        }

        public override void OnBePushedToPool()
        {
            base.OnBePushedToPool();
            mUnloadFlag = true;
            needDepends = null;
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
            if (Asset != null)
            {
                /*Asset.Unload(mUnloadFlag);*/
                mAsset = null;
            }
        }

        public override string ToString()
        {
            return $"Type:AssetBundle\t {base.ToString()}";
        }
    }

    #endregion
}

