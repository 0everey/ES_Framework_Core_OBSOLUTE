using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class SoDataInfoConfiguration : SerializedScriptableObject
    {
        [LabelText("Buff效果数据包")]
        public BuffDataPack PackForBuff;
        [LabelText("Skill技能数据包")]
        public SkillDataPack PackForSkill;
        [LabelText("Actor人物数据包")]
        public ActorDataPack PackForActor;
        [LabelText("Item物品数据包")]
        public ItemDataPack PackForItem;
        [LabelText("EventMessage事件数据数据包")]
        public GameEventMessageDataPack PackForEventMessage;
        [LabelText("Rune符文数据数据包")]
        public RuneDataPack PackForRune;
        [LabelText("RoomGenerate房间生成数据数据包")]
        public RoomGenerateDataPack PackForRoomGenerate;
    }
}
