using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    //模块的组成分为 自身逻辑体(RunTimeLogic)，共享数据(SharedData)，运行状态(Status)
    public class StandardModuleCompo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
    public interface ISharedData
    {
        
    }
    public interface IStatus : IInittable//可初始化的
    {

    }
    public abstract class BaseRunTimeLogic<Hosting,Key,SharedData,Status_> :UpdatableDelegatableWithHosting<Hosting> 
        where Hosting:RightlyCompleteDelegatableAndUpdatableAndHosting
        where SharedData:ISharedData
        where Status_:IStatus
        //where Key:string/Enum/IKey
    {
        [LabelText("托管核心"),NonSerialized,ShowInInspector]
        public Hosting host;
        public override Hosting GetHost => host;
        [ShowInInspector, LabelText("共享数据"), FoldoutGroup("只读属性")] public SharedData SharedDataShow => sharedData;
        [ShowInInspector, LabelText("运行状态"), FoldoutGroup("只读属性")] public Status_ Status => status;
        [ShowInInspector,LabelText("标识键"), FoldoutGroup("只读属性")] public abstract Key ThisKey { get; }
        [LabelText("共享数据", SdfIconType.Calendar2Date), FoldoutGroup("固有")] public SharedData sharedData;
        [LabelText("运行状态", SdfIconType.Calendar2Date), FoldoutGroup("固有")] public Status_ status;
        
        public override bool OnSubmitHosting(Hosting hosting, bool asVirtual = false)
        {
            host = hosting;
            return base.OnSubmitHosting(hosting, asVirtual);
        }
    }
}
