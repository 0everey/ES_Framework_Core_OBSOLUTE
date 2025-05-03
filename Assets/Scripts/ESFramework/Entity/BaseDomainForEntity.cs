using ES;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    
    public class BaseDomainForEntity : DomainBase<Entity, BaseDomainClipForEntity, BaseDomainForEntity>
    {
       

       

        protected override void CreateLink()
        {
            base.CreateLink();
            core.BaseDomain = this;
        }
    }
    public abstract class BaseDomainClipForEntity : DomainClipForEntity
    {

    }
    public abstract class DomainClipForEntity : DomainClip<Entity, BaseDomainForEntity>
    {

    }
}
