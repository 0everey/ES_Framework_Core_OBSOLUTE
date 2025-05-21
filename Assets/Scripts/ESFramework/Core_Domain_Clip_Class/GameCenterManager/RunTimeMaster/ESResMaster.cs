using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


namespace ES
{
    public class ESResMaster : SingletonAsMono<ESResMaster>
    {

        
        public ESSimpleObjectPool<ResSourceSearchKey> PoolForResSourceSearchKey = new ESSimpleObjectPool<ResSourceSearchKey>(()=>new ResSourceSearchKey(), 
            (f) => { f.AssetName = null;
                f.AssetType = null;
                f.OwnerAssetBundle = null;
            }
            ,30);
        public ESSimpleObjectPool<AssetResSource> PoolForAssetResSource = new ESSimpleObjectPool<AssetResSource>(() => new AssetResSource(),
           (f) => {
               
           }
           , 30);
        public ESSimpleObjectPool<AssetBundleResSource> PoolForAssetBundleResSource = new ESSimpleObjectPool<AssetBundleResSource>(() => new AssetBundleResSource(),
         (f) => {

         }
         , 30);
        public ESSimpleObjectPool<ESResLoader> PoolForESLoader = new ESSimpleObjectPool<ESResLoader>(() => new ESResLoader(),
       (f) => {

       }
       , 30);
        #region 池操作在这里
        public ResSourceSearchKey GetInPool_ResSourceSearchKey(string assetName, string ownerBundleName = null, Type assetType = null)
        {
            var resSearchRule = ESResMaster.Instance.PoolForResSourceSearchKey.GetInPool();
            resSearchRule.AssetName = assetName.ToLower();
            resSearchRule.OwnerAssetBundle = ownerBundleName == null ? null : ownerBundleName.ToLower();
            resSearchRule.AssetType = assetType;
            resSearchRule.OriginalAssetName = assetName;
            return resSearchRule;
        }
        public AssetResSource GetOneInPool_AssetResSource(string name, string onwerBundleName, Type assetTypde)
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
        public AssetBundleResSource GetInPool_AssetBundleResSource(string name)
        {
            var res = ESResMaster.Instance.PoolForAssetBundleResSource.GetInPool();

            res.AssetName = name;
            res.AssetType = typeof(AssetBundle);
            res.InitAssetBundleName();

            return res;
        }
        public ESResLoader GetInPool_ESLoader()
        {
            return PoolForESLoader.GetInPool();
        }
        #endregion
        public IResSource GetRes(ResSourceSearchKey ResSourceSearchKey, bool createNew = false)
        {
             /*var res = mTable.GetResBySearchKeys(ResSourceSearchKey);

             if (res != null)
             {
                 return res;
             }

             if (!createNew)
             {
                 Debug.LogFormat("createNew:{0}", createNew);
                 return null;
             }

             res = ResFactory.Create(ResSourceSearchKey);

             if (res != null)
             {
                 mTable.Add(res);
             }*/

            return null;
        }

        public T GetRes<T>(ResSourceSearchKey ResSourceSearchKey) where T : class, IResSource
        {
            return GetRes(ResSourceSearchKey) as T;
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


    }
}