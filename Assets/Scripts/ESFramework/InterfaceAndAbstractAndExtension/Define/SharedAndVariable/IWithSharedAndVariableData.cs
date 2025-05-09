using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ES
{
    public interface IWithSharedAndVariableData
    {

    }
    public interface IWithSharedAndVariableData<Shared_, Variable_> : IWithSharedAndVariableData
        where Shared_:ISharedData
        where Variable_:IVariableData
    {
        #region 两大属性
        Shared_ SharedData { get; set; }
        Variable_ VariableData { get; set; }
        #endregion
    }
    /* public static partial class KeyValueMatchingUtility
     {
          注册到KVMU-->双数据赋予  DataApply
     }*/
}
