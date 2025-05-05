using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class BaseDomainForEntity : BaseDomain<Entity, BaseClipForEntity>
    {
        


        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.BaseDomain = this;
        }

       /* float timeDis = 2;
        protected override void Update()
        {
            timeDis -= Time.deltaTime;
            if (timeDis < 0)
            {
                var next = GetModule<BaseClipForEntity_改名字>();
                RemoveClip(next);
                timeDis = 2;
            }
            base.Update();
        }*/
    }
    [Serializable]
    public abstract class BaseClipForEntity : Clip<Entity, BaseDomainForEntity>
    {

    }

 /*   [Serializable,TypeRegistryItem("改名字模块")]
    public class BaseClipForEntity_改名字: BaseClipForEntity
    {
        public string newName = "依薇尔";
        private string pre;
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            Domain.Module_RenameThis = this;
        }
        protected override void OnEnable()
        {
            pre = Core.gameObject.name;
            Core.gameObject.name = newName;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            Core.gameObject.name = pre;
            base.OnDisable();
        }
        protected override void Update()
        {
            base.Update();
        }
    }*/
}
