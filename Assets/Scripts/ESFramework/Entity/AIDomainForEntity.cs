using ES;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class AIDomainForEntity : DomainBase<Entity, AIDomainClipForDomainForEntity, AIDomainForEntity>
    {
        
        protected override void CreateLink()
        {
            base.CreateLink();
            core.AIDomain = this;
        }
    }
    public abstract class AIDomainClipForDomainForEntity : DomainClipForEntity
    {

    }
 
}
