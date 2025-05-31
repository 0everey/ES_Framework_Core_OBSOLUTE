using ES;
using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public class BaseESCampStrategy : BaseRunTimeStrategy<AllESCampStrategy, BaseESCampStrategy>
    {
        /*public static Dictionary<>*/
        public override Dictionary<AllESCampStrategy, BaseESCampStrategy> AllStrategy => _AllStrategies;
        public static Dictionary<AllESCampStrategy, BaseESCampStrategy> _AllStrategies = new Dictionary<AllESCampStrategy, BaseESCampStrategy>() { 
            {AllESCampStrategy.NormalPlayer_PK_Enemy,new ESCampStrategy_NormalPlayer_PK_Enemy() }

        };
        public virtual ESCampTestBack Pick(Entity attacker, Entity victim)
        {
            if (attacker == null || victim == null) return ESCampTestBack.Ignore;

            return ESCampTestBack.Ignore;
        }
    }
    public enum AllESCampStrategy
    {
        NormalPlayer_PK_Enemy,

    }
    public class ESCampStrategy_NormalPlayer_PK_Enemy : BaseESCampStrategy
    {
        
    }
    public enum ESCampTestBack
    {
        [InspectorName("忽视")] Ignore,
        [InspectorName("会主动攻击")] Attack,
        [InspectorName("害怕")] Afriad,
    }
}

