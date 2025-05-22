using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{
    public interface IESResLoader
    {
        IResSource LoadResSync(ResSourceSearchKey resSearchKeys);
        UnityEngine.Object LoadAssetSync(ResSourceSearchKey resSearchKeys);

        void Add2Load(ResSourceSearchKey resSearchKeys, Action<bool, IResSource> listener = null, bool lastOrder = true);
        void LoadAll_Async(System.Action listener = null);
    }
    public class ESResLoader : IPoolablebSelfControl, IESResLoader
    {
        public bool IsRecycled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void OnBePushedToPool()
        {
            
        }

        public void TryAutoPushToPool()
        {
          /*  if (mObject2Unload != null)
            {
                foreach (var o in mObject2Unload)

                {
                    if (o)
                    {
                        ResUnloadHelper.DestroyObject(o);
                    }
                }

                mObject2Unload.Clear();
                mObject2Unload = null;
            }*/
            ESResMaster.Instance.PoolForESLoader.PushToPool(this);
        }

        public IResSource LoadResSync(ResSourceSearchKey resSearchKeys)
        {
            throw new NotImplementedException();
        }

        public UnityEngine.Object LoadAssetSync(ResSourceSearchKey resSearchKeys)
        {
            throw new NotImplementedException();
        }

        public void Add2Load(ResSourceSearchKey resSearchKeys, Action<bool, IResSource> listener = null, bool lastOrder = true)
        {
            throw new NotImplementedException();
        }

        public void LoadAll_Async(Action listener = null)
        {
            throw new NotImplementedException();
        }

        class OneResLoadCallBackWrap
        {
            private readonly Action<bool, IResSource> mListener;
            private readonly IResSource mRes;

            public OneResLoadCallBackWrap(IResSource r, Action<bool, IResSource> l)
            {
                mRes = r;
                mListener = l;
            }

            public void Release()
            {
                mRes.OnLoadOK_WithDraw(mListener);
            }

            public bool IsRes(IResSource res)
            {
                return res.AssetName == mRes.AssetName;
            }
        }
        private readonly List<IResSource> mResList = new List<IResSource>();
        private readonly LinkedList<IResSource> mWaitLoadList = new LinkedList<IResSource>();
        private System.Action mListener;

        private int mLoadingCount;

        private LinkedList<OneResLoadCallBackWrap> mCallbackRecordList;
        public float Progress
        {
            get
            {
                if (mWaitLoadList.Count == 0)
                {
                    return 1;
                }

                var unit = 1.0f / mResList.Count;
                var currentValue = unit * (mResList.Count - mLoadingCount);

                var currentNode = mWaitLoadList.First;

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