using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class GameEventMessageDataInfo : SoDataInfo
    {
    
        [LabelText("信息类型")]public EnumCollect.GameEventMessageType messageType;
        [LabelText("本地化信息内容")]public Dictionary<EnumCollect.Localization, string> LocalizedStringMessage = new Dictionary<EnumCollect.Localization, string>();

    }
}
