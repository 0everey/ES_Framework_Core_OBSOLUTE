using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeRegistryItem("Link接受器_字符串名称事件","针播放器/Link")]
public class PointerPlayerReceiveLink_StringNameEvent : PointerPlayerXXXReceiveLink<Link_StringNameEvent> 
{
    [LabelText("匹配的事件名称")]
    public string matchName = "事件名称";
    public override void ApplyThisLink(Link_StringNameEvent link)
    {
        //会把Link的参数存下来
        Recieve(link.param);
    }
    public override bool OnLinkOptionsMatch(Link_StringNameEvent link)
    {
       
        return matchName.Equals(link.eventName);
    }
}
