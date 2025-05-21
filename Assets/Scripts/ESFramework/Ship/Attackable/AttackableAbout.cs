using ES;
using ES.EvPointer;
using HellishBattle;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{

    [Serializable, TypeRegistryItem("可攻击")]
    public class Attackable
    {

        [FoldoutGroup("基础属性值"), LabelText("基础伤害")] public float Damage = 10;
        [FoldoutGroup("基础属性值"), LabelText("攻击距离")] public float AttackRangeDis = 1;
        [FoldoutGroup("基础属性值"), LabelText("攻击速度")] public float AttackSpeed = 1;
        [FoldoutGroup("条件和类型"), LabelText("攻击类型标识")] public EnumCollect.AttackType attackType = EnumCollect.AttackType.Melle;
        [FoldoutGroup("条件和类型"), LabelText("攻击对目标的要求")] public EnumCollect.AttackConditionRequire require = EnumCollect.AttackConditionRequire.MustTargetInRange;
        [FoldoutGroup("条件和类型"), LabelText("攻击触发机制")] public EnumCollect.AttackOnType attackOn = EnumCollect.AttackOnType.Direct;
        [FoldoutGroup("播放时"), LabelText("前摇")] public float preDelay = 0.25f;
        [FoldoutGroup("播放时"), LabelText("总时间")] public float attackExit_ = 0.8f;
        [FoldoutGroup("播放时"), LabelText("播放音效")] public AudioClip playSound;
        [FoldoutGroup("预制件资源"), LabelText("显示效果预制件"), SerializeReference] public IPointerForGameObject_Only prefabForShow = new PointerForGameObject_Direct() { };
        [FoldoutGroup("预制件资源"), LabelText("作用效果预制件"), SerializeReference] public IPointerForGameObject_Only prefabForEffector = new PointerForGameObject_Direct() { };

        public virtual float CalculatePower()
        {
            return
                Damage * 1f +

                  AttackSpeed * 5f;
        }
        public void TryAttackOn(Entity Who, Entity Target = null, Item with = null)
        {
            Entity target = Target;

            if (target == null)
            {
                target = Who?.AIDomain?.Module_AB_AITarget?.Target;

            }
            if (require == EnumCollect.AttackConditionRequire.MustTargetInRange)
            {

                if (target == null || (target.transform.position - Who.transform.position).NoY().magnitude >= AttackRangeDis * 1.2f)
                {
                    return;
                }
            }
            else if (require == EnumCollect.AttackConditionRequire.MustTargetHas)
            {
                if (target == null) return;
            }
            //Require通过√
            if (attackType.HasFlag(EnumCollect.AttackType.Melle))
            {

                if (attackOn.HasFlag(EnumCollect.AttackOnType.Direct) && target != null)
                {
                    if (playSound != null) GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(playSound, 0.8f);
                    KeyValueMatchingUtility.ESLink.Global.GlobalLink_EntityAttackEntityTryStart(
                    new Link_EntityAttackEntityTryStart() { attacker = Who, victim = target, damage = new Damage() { damage = Damage } });
                }
            }
            if (attackType.HasFlag(EnumCollect.AttackType.Range))
            {
                if (attackOn.HasFlag(EnumCollect.AttackOnType.Direct) && target != null)
                {
                    if (playSound != null) GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(playSound, 0.8f);
                    GameObject gg = prefabForEffector?.Pick();
                    Who.BaseDomain.Module_Fire?.Fire(gg);
                }
            }
        }
    }
    [Serializable, TypeRegistryItem("简易怪物攻击")]
    public class Attackable_SimpleEnemy : Attackable
    {
        [FoldoutGroup("基础属性值"), LabelText("攻击角度范围")] public float AttackRangeAngle = 60;

        public override float CalculatePower()
        {
            return AttackRangeAngle * 0.8f +
                   AttackRangeDis * 3f + base.CalculatePower();
        }
    }


    [Serializable]
    public class Damage
    {
        public float damage = 10;
        public ESReferCheckout_Float canTrigger = new ESReferCheckout_Float() { Value = 1 };
    }

}
