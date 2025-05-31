using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{
    
    public class  ESNetLocalDomain : BaseDomain<ESNetManager, LocalClipForESNetManager>
    {
        [NonSerialized] public ClipLocal_LocalFunctionBlock Modole_BlockFunc;
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.LocalDomain = this;
        }
    }
    [Serializable]
    public abstract class LocalClipForESNetManager : Clip<ESNetManager, ESNetLocalDomain>
    {

    }
    [Serializable,TypeRegistryItem("本地功能拦截")]
    public class ClipLocal_LocalFunctionBlock  : LocalClipForESNetManager
    {
        [LabelText("别人的拦截模块")]public ESTypeSelecter ForModuleOtherBan;
        [LabelText("自己的拦截模块")] public ESTypeSelecter ForModuleSelfBan;
        [LabelText("别人的拦截脚本")] public ESTypeSelecter ForESmonoOther;
        protected override string Description_ => "这个功能用于拦截一些非自己掌控的脚本和模块的运转，防止运行问题";
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Modole_BlockFunc = this;
        }
    }
    

    /*
    
     [Serializable, TypeRegistryItem("Clip名字")]
    public class TheClip : BaseClipForXXX
    {
        
        protected override void Update()
        {
            
        }
         protected override void CreateRelationship()
        {
            base.CreateRelationship();
            //Domain.Module_XXX = this;  显性引用
        }
    }
    */
}
