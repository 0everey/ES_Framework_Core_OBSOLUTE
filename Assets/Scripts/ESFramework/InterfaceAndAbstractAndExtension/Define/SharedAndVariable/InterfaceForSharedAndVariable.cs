using ES;
using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    //共享数据->共享数据可以让大量数据唯一存在，尽量不产生副本,如果需要对数据加成可以写在状态里
    public interface ISharedData
    {

    }

    //变量数据->变量数据可以创建副本，并且每个对象持有独立的
    public interface IVariableData : IInittable//可初始化的 运行的情形，也可以认为是不共享的，容易变化的数据
    {

    }

    
    
    public interface ICopyToClass<in CopyToWhere> where CopyToWhere:class
    {
        public void CopyTo(CopyToWhere other);
    }
}
