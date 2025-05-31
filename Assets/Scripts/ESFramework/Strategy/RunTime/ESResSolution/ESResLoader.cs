using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{
    public interface IESResLoader
    {
        IResSource _LoadResSync(ResSourceSearchKey resSearchKeys);
        UnityEngine.Object LoadAssetSync(ResSourceSearchKey resSearchKeys);

        void Add2Load(ResSourceSearchKey resSearchKeys, Action<bool, IResSource> listener = null, bool lastOrder = true);
        void LoadAll_Async(System.Action listener = null);
        void LoadAll_Sync();
    }
    public class ESResLoader : IPoolablebSelfControl, IESResLoader
    {
        #region 池化
        public bool IsRecycled { get ; set; }

        public void OnBePushedToPool()
        {
            
        }

        public void TryAutoPushToPool()
        {
            if (AllResSources != null)
            {
               /* foreach (var o in mObject2Unload)

                {
                    if (o)
                    {
                        ResUnloadHelper.DestroyObject(o);
                    }
                }*/
                AllResSources.Clear();
                AllResSourcesWaitToLoad.Clear();
              /*  mCallbackRecordList.Clear();
                mCallbackRecordList = null;*/
            }
            ESResMaster.Instance.PoolForESLoader.PushToPool(this);
        }
        #endregion

        #region ResSource相关
        public IResSource _LoadResSync(ResSourceSearchKey resSearchKeys)
        {
            return null;
        }
        #endregion

        #region 同步加载
        public UnityEngine.Object LoadAssetSync(ResSourceSearchKey resSearchKeys)
        {
            var res = ESResMaster.Instance.GetResSourceBySearchKeys(resSearchKeys);
            if (res != null)
            {
                if(res.State== ResSourceState.Ready)
                {
                    return res.Asset;
                }else
                {
                    res.LoadSync();
                    return res.Asset;
                }
            }
            return null;
        }
        #endregion


        #region 异步队列实现
        public void Add2Load(ResSourceSearchKey resSearchKeys, Action<bool, IResSource> listener = null, bool AtLastOrFirst = true)
        {
            
            var res = FindResInNowList(resSearchKeys);
            if (res != null)
            {
                /*AddResListenerRecord(res,listener);*/
                if (listener != null) res.OnLoadOK_Submit(listener);
                return;
            }
            res = ESResMaster.Instance.GetResSourceBySearchKeys(resSearchKeys);
          
            if (res != null)
            {
                if (listener != null) res.OnLoadOK_Submit(listener);
                //添加依赖支持
                {
                    var dependsAssetBundles = res.GetDependResSourceAllAssetBundles();

                    if (dependsAssetBundles != null)
                    {
                        foreach (var depend in dependsAssetBundles)
                        {
                            var searchRule = ESResMaster.Instance.GetInPool_ResSourceSearchKey(depend, null,loadType: ResSourceLoadType.AssetBundle, typeof(AssetBundle));
                            Add2Load(searchRule);
                            searchRule.TryAutoPushToPool();
                        }
                    }
                }
                AddRes2AllResource(res,AtLastOrFirst);
            }
        }
        public IResSource FindResInNowList(ResSourceSearchKey resSearchKeys)
        {
            foreach(var i in AllResSources)
            {
                if (i.AssetPath == resSearchKeys.AssetPath)
                {
                    return i;
                }
            }
            return null;
        }
        public IResSource FindResInNowList(IResSource theRes)
        {
            foreach (var i in AllResSources)
            {
                if (i.AssetPath == theRes.AssetPath)
                {
                    return i;
                }
            }
            return null;
        }
        private void AddRes2AllResource(IResSource res, bool lastOrder)
        {
            IResSource resSource= FindResInNowList(res);

            if (resSource != null)//只要新的
            {
                return;
            }

           /* res.();*/
            AllResSources.Add(res);

            if (res.State !=  ResSourceState.Ready)
            {
                ++mLoadingCount;
                if (lastOrder)
                {
                    AllResSourcesWaitToLoad.AddLast(res);
                }
                else
                {
                    AllResSourcesWaitToLoad.AddFirst(res);
                }
            }
        }

        private readonly List<IResSource> AllResSources = new List<IResSource>();
        private readonly LinkedList<IResSource> AllResSourcesWaitToLoad = new LinkedList<IResSource>();
        #endregion

        public void LoadAll_Async(Action listener = null)
        {
            mListener_ForLoadAllOK = listener;
            DoLoadAsync();
        }
        private void OnOneResLoadFinish(bool result, IResSource res)
        {
            --mLoadingCount;
            DoLoadAsync();
            if (mLoadingCount == 0)
            {
                if (mListener_ForLoadAllOK != null)
                {
                    mListener_ForLoadAllOK();
                }
            }
        }
        private void DoLoadAsync()
        {
            if (mLoadingCount == 0)
            {
                if (mListener_ForLoadAllOK != null)
                {
                    mListener_ForLoadAllOK();
                    mListener_ForLoadAllOK = null;
                }
                return;
            }

            var nextNode = AllResSourcesWaitToLoad.First;
            LinkedListNode<IResSource> currentNode = null;
            while (nextNode != null)
            {
                currentNode = nextNode;
                var res = currentNode.Value;
                nextNode = currentNode.Next;//循环判定
                if (res.IsDependResLoadFinish())
                {
                    AllResSourcesWaitToLoad.Remove(currentNode);

                    if (res.State !=  ResSourceState.Ready)
                    {
                        res.OnLoadOK_Submit(OnOneResLoadFinish);
                        res.LoadAsync();
                    }
                    else
                    {
                        --mLoadingCount;
                    }
                }
            }
        }

        public void LoadAll_Sync()
        {
            while (AllResSourcesWaitToLoad.Count > 0)
            {
                var first = AllResSourcesWaitToLoad.First.Value;
                --mLoadingCount;
                AllResSourcesWaitToLoad.RemoveFirst();

                if (first == null)
                {
                    return;
                }

                if (first.LoadSync())
                {

                }
            }
        }

        private System.Action mListener_ForLoadAllOK;

        private int mLoadingCount;

        /*包裹机制
         class OneResLoadCallBackWrap
        {
            private readonly Action<bool, IResSource> mListener_ForLoadAllOK;
            private readonly IResSource mRes;

            public OneResLoadCallBackWrap(IResSource r, Action<bool, IResSource> l)
            {
                mRes = r;
                mListener_ForLoadAllOK = l;
            }

            public void Release()
            {
                mRes.OnLoadOK_WithDraw(mListener_ForLoadAllOK);
            }

            public bool IsRes(IResSource res)
            {
                return res.AssetPath == mRes.AssetPath;
            }
        }
        private LinkedList<OneResLoadCallBackWrap> mCallbackRecordList = new LinkedList<OneResLoadCallBackWrap>();


        private void AddResListenerRecord(IResSource res, Action<bool, IResSource> listener)
        {
            if (mCallbackRecordList == null)
            {
                mCallbackRecordList = new LinkedList<OneResLoadCallBackWrap>();
            }
            mCallbackRecordList.AddLast(new OneResLoadCallBackWrap(res, listener));
        }
    */
        public float Progress
        {
            get
            {
                if (AllResSourcesWaitToLoad.Count == 0)
                {
                    return 1;
                }

                var unit = 1.0f / AllResSources.Count;//所有资源
                var currentValue = unit * (AllResSources.Count - mLoadingCount);//已经加载的

                var currentNode = AllResSourcesWaitToLoad.First;

                while (currentNode != null)
                {
                    currentValue += unit * currentNode.Value.Progress;
                    currentNode = currentNode.Next;
                }

                return currentValue;
            }
        }
    }
}