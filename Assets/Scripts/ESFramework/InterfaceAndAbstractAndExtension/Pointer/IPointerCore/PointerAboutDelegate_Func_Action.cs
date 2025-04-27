using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 扩展时 建议创建新的脚本 
 修改文件时 使用 #region + 自己的名字
 格式尽量统一 
 多交流 --- Everey
 */
namespace ES.EvPointer
{
    //核心 Ev针支持 关于委托，Func,Action之类的部分
    #region 委托部分_无内容
    public interface IPointerForDelegate : IPointer<Delegate, object, object, object>
    {

    }
    public interface IPointerForDelegate_Only : IPointerForDelegate, IPointerOnlyBack<Delegate>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForAction : IPointer<Action, object, object, object>
    {

    }
    public interface IPointerForAction_Only : IPointerForAction, IPointerOnlyBack<Action>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #endregion
}
