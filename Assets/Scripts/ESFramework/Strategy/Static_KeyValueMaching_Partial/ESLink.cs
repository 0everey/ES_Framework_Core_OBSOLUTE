using ES;
using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public static partial class KeyValueMatchingUtility
    {
        //Link事件
        public static class ESLink
        {
            public static class ForEntityLink
            {
                public static void OnEntityLink(Entity entity, ILink linkDefault, bool ApplyOrCancel = true)
                {

                }

            }
            public static class Global
            {
                public static void GlobalLink_EntityAttackEntityTryStart(Link_EntityAttackEntityTryStart link_Attack_Try)
                {
                    if (link_Attack_Try.attacker == null || link_Attack_Try.victim == null) return;
                    Debug.Log("攻击测试开始");
                    //攻击者填充增益
                    link_Attack_Try.attacker.Invoke_TryAttackEntityCalculate(link_Attack_Try.victim, link_Attack_Try.damage);
                    if (link_Attack_Try.damage.canTrigger.Value > 0)
                    {
                        Debug.Log("攻击者测试通过");
                        //被攻击者填充增益
                        link_Attack_Try.victim.Invoke_BeAttackByEntityCalculate(link_Attack_Try.attacker, link_Attack_Try.damage);
                        if (link_Attack_Try.damage.canTrigger.Value > 0)
                        {
                            Debug.Log("被攻击测试通过");
                            //攻击者追加
                            link_Attack_Try.attacker.Invoke_TrulyAttack(link_Attack_Try.victim, link_Attack_Try.damage);
                            //被攻击者追加
                            link_Attack_Try.victim.Invoke_TrulyBeAttack(link_Attack_Try.attacker, link_Attack_Try.damage);
                            GameCenterManager.Instance.GameCenterArchitecture.SendLink(
                                new Link_EntityAttackEntityTruely()
                                {
                                    attacker = link_Attack_Try.attacker,
                                    victim = link_Attack_Try.victim,
                                    damage = link_Attack_Try.damage
                                }
                              ); ;
                        }
                    }


                }
            }
        }
    }
}

