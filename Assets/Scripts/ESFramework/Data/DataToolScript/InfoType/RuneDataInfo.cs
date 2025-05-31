using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [ESDisplayNameKeyToType("数据单元", "符文数据单元")]
    public class RuneDataInfo : SoDataInfo
    {
        [LabelText("符文图标"),PreviewField]public Sprite icon;
    }
}
