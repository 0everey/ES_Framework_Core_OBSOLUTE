using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    //运行逻辑模块的组成分为 自身逻辑体(RunTimeLogic),共享数据(SharedData_)，变量数据(VariableData_)
    //共享数据->共享数据可以让大量数据唯一存在，尽量不产生副本,如果需要对数据加成可以写在状态里


    [Serializable,TypeRegistryItem("抽象—运行时逻辑基元")]
    public abstract class BaseRunTimeLogic<Hosting,Key,SharedData_,VariableData_> :IESOriginalModule<Hosting>,IWithSharedAndVariableData<SharedData_, VariableData_>
        where Hosting:IESOringinHosting
        where SharedData_:ISharedData
        where VariableData_:IVariableData
        //where Key_:string/Enum/IKey
    {
        #region 托管至
        [NonSerialized]
        public Hosting host;
        #endregion

        [ShowInInspector, LabelText("标识键"), FoldoutGroup("只读属性")] public abstract Key ThisKey { get; }
        [ShowInInspector, LabelText("标识键"), FoldoutGroup("只读属性"), ReadOnly] public SharedData_ SharedData { get => sharedData; set => sharedData=value; }
        [ShowInInspector, LabelText("标识键"), FoldoutGroup("只读属性"),ReadOnly] public VariableData_ VariableData { get => variableData; set => variableData=value; }


        [LabelText("共享数据", SdfIconType.Calendar2Date), FoldoutGroup("固有"),NonSerialized/*不让自动序列化*/] public SharedData_ sharedData;
        [LabelText("自变化数据", SdfIconType.Calendar3Range), FoldoutGroup("固有")] public VariableData_ variableData;
        public bool OnSubmitHosting(Hosting host)
        {
            this.host = host;
            return true;
        }
    }
}
