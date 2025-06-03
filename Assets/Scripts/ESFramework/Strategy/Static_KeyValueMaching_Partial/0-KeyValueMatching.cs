using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


using UnityEngine.InputSystem;
using static ES.EnumCollect;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.Utilities;
using System.Runtime.CompilerServices;

namespace ES
{
    //Key_ ESValue Matching 是一系列键值对映射或者静态处理方法，用于解耦的功能合集
    public static partial class KeyValueMatchingUtility
    {
        #region  找键器
        public static T FindByIKey<T, Key>(IEnumerable<T> ts, Key key) where T : IWithKey<Key> where Key : IKey
        {
            if (ts != null)
            {
                foreach (var i in ts)
                {
                    if (i.key.Equals(key)) return i;
                }
            }
            return default(T);
        }
        /* public static CopyToWhere FindByKey<CopyToWhere, TypeSelect_>(IEnumerable<CopyToWhere> ts, TypeSelect_ key) where CopyToWhere : IWithKey<object> where TypeSelect_ : IKey
         {
             if (ts != null)
             {
                 foreach (var i in ts)
                 {
                     if (i.key.Equals(key)) return i;
                 }
             }
             return default(CopyToWhere);
         }*/
        public static T FindByAKey<T, Key>(IEnumerable<T> ts, Key key) where T : IWithKey<IKey<Key>>
        {
            if (ts != null)
            {
                foreach (var i in ts)
                {
                    // Debug.Log($"Compare{i},{i.key},{key}");
                    if (i.key.Equals(key)) return i;
                }
            }
            return default(T);
        }
        public static bool ContainsByIKey<T, Key>(IEnumerable<T> ts, Key key) where T : IWithKey<Key> where Key : IKey
        {
            if (ts != null)
            {
                foreach (var i in ts)
                {
                    if (i.key.Equals(key)) return true;
                }
            }
            return false;
        }
        public static bool ContainsByAKey<T, Key>(IEnumerable<T> ts, Key key) where T : IWithKey<IKey<Key>>
        {
            if (ts != null)
            {
                foreach (var i in ts)
                {
                    if (i.key.Equals(key)) return true;
                }
            }
            return false;
        }
        #endregion
        
        
       

       

        
    }
}
