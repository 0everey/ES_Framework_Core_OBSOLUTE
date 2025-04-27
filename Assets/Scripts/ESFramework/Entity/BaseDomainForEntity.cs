using ES;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    
    public class BaseDomainForEntity : DomainBase<Entity, BaseDomainClipForEntity, BaseDomainForEntity>
    {
        public override string Description_ => "实体的基本扩展域";
        protected override void CreateLink()
        {
            base.CreateLink();
            usingCore.BaseDomain = this;
        }
    }
    public abstract class BaseDomainClipForEntity : DomainClipForEntity
    {

    }
    public abstract class DomainClipForEntity : DomainClip<Entity>
    {

    }
}
