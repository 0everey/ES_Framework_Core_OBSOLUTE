using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ES
{
    [TypeRegistryItem("Link接受器","Link")]
    public class PointerPlayerXXXReceiveLink<Link> : PointerPlayerSystemObjectCaster,IReceiveLink<Link> where Link:ILink
    {
        public override IPointer Pointer => whenRecieveLink;
        [LabelText("当成功接受接受Link时触发")]
        public IPointer whenRecieveLink = new PointerPickerEveryThing();
        private void OnEnable()
        {
            GameCenterManager.Instance.GameCenterArchitecture.AddRecieveLink(this);
        }
        private void OnDisable()
        {
            GameCenterManager.Instance.GameCenterArchitecture.RemoveRecieveLink(this);
        }
        public void OnLink(Link link)
        {
            
            if (OnLinkOptionsMatch(link))
            {
                ApplyThisLink(link);
                whenRecieveLink?.Pick();
            }
        }
        public virtual bool OnLinkOptionsMatch(Link link)
        {
            return true;
        }
        public virtual void ApplyThisLink(Link link)
        {

        }
    }
}