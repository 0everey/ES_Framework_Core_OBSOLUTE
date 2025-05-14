using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ES.SkillPointDataInfo.SkillPointLevelAllTransfomor;

namespace ES
{
    [CreateAssetMenu( fileName ="创建技能点图标",menuName ="EvData/SkillPointSprites")]
    public class SkillPointSpritesReference : SerializedScriptableObject
    {
        [LabelText("技能点默认图配置"),OdinSerialize,NonSerialized] public SkillPointSprites defaultSpritesForSkillPoint = new SkillPointSprites();
        
    }
}
