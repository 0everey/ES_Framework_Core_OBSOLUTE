using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [Serializable,TypeRegistryItem("玩家网络信息")]
    public class ESNetPlayer 
    {
        [LabelText("玩家ID")]public string PlayerID = "";
        [LabelText("玩家名称")] public string PlayerName = "是依薇尔吗";
        [LabelText("房间号")] public int RoomNumber;
        [LabelText("房间密码")] public int RoomPassword;
    }
}

