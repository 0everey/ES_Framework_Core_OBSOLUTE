using ES;
using FishNet.Transporting;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [CreateAssetMenu(fileName = "独立数据-ES阵营机制表", menuName = "ES数据/ES阵营机制表")]
    public class ESCampMaker : SerializedScriptableObject
    {
        [LabelText("友好阵营")]
        public Dictionary<ESCampFlag, ESCampFlag> Friend = new Dictionary<ESCampFlag, ESCampFlag>();
        [LabelText("敌对阵营")]
        public Dictionary<ESCampFlag, ESCampFlag> Hostile = new Dictionary<ESCampFlag, ESCampFlag>();

        //除此之外是默认关系-》看生物自己的判断
        //会写一个策略专门处理
    }
    [Serializable,TypeRegistryItem]
    public class ESCamp
    {
        [InspectorName("阵营旗帜")]public ESCampFlag channel = ESCampFlag.Player;
        [InspectorName("标志")]public int sign = 0;//sign不同,逻辑自定义,(用来搞无限阵营)
        [InspectorName("如何对付非敌对非友好")] public WhenNormalOption whenNormal = WhenNormalOption.AttackAndTargetIfBeAttack;
    }
    [Flags]
    public enum ESCampFlag
    {
        Player=1,
        Enemy=2,
        NPCNormal=4,
        NPCBad=8,
        Blue=16,
        Red=32,
        Green=64,
        Pink=128,
        Black=256,
        White=512,
    }
    public enum WhenNormalOption
    {
        [InspectorName("反击")] AttackAndTargetIfBeAttack,
        [InspectorName("可攻击但是不会作为目标")]CanAttackButNotTarget,
        [InspectorName("忽视")] Ignore,
        [InspectorName("会主动攻击")]AttackAndTarget,
        [InspectorName("害怕")] Afriad,
    }

}

