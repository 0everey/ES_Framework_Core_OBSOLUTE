using ES.EvPointer;

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [ESDisplayNameKeyToType("数据单元", "房间生成数据单元")]
    public class RoomGenerateDataInfo : SoDataInfo
    {
        [Required][LabelText("预设数据")]public RoomPreset roomPreset;
        [LabelText("网格X范围"),SerializeReference] public IPointerForInt_Only GridX=new PointerForInt_Random30();
        [LabelText("网格Y范围"),SerializeReference] public IPointerForInt_Only GridY = new PointerForInt_Random30();
        [LabelText("房间高度"), SerializeReference] public int height = 2;
        [InfoBox("0是自动门，1是手动门,2是条件门")]
        [LabelText("对于门-生成门的随机Index")] public PointerForInt_BackIntIndexWithWeight getIndexOfDoor = new PointerForInt_BackIntIndexWithWeight();

    }
}
