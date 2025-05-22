using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{
    public interface IESResLoader
    {
        
    }
    public class ESResLoader : IPoolablebSelfControl, IESResLoader
    {
        public bool IsRecycled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void OnBePushedToPool()
        {
            throw new System.NotImplementedException();
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
    }
}