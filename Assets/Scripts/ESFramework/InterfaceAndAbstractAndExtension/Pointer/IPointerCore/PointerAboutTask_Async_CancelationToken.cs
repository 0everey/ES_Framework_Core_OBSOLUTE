using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于 异步操作 取消源 部分
    #region 支持协程补间的取消源CancellationTokenSource
    public interface IPointerForCancellationTokenSource<On, From, With> : IPointer<CancellationTokenSource, On, From, With>
    {

    }
    public interface IPointerForCancellationTokenSource_Only : IPointerForCancellationTokenSource<object, object, object>, IPointerOnlyBack<CancellationTokenSource>
    {

    }
    [Serializable, TypeRegistryItem("应用取消源-取消")]
    public class PointerCancelTokenSourceApply : IPointerNone
    {
        [SerializeReference, LabelText("取消源")] public IPointerForCancellationTokenSource_Only cancelSource;
        public object Pick(object on= null, object from = null, object with = null)
        {

            cancelSource?.Pick()?.Cancel();
            return -1;
        }
    }
    #endregion


}
