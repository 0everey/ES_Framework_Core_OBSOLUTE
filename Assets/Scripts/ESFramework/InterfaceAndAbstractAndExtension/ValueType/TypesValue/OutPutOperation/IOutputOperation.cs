using ES;
using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{

    public interface IOutputOperation<On, From, With> : IPointer<bool, On, From, With>
    {
        bool TryOpeation(On on, From from, With with);
        bool TryCancel(On on, From from, With with);
        bool IPointer<bool, On, From, With>.Pick(On on, From from, With with)
        {
            return TryOpeation(on, from, with);
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick(KeyValueMatchingUtility.Matcher.SystemObjectToT<On>(a),
               KeyValueMatchingUtility.Matcher.SystemObjectToT<From>(b),
               KeyValueMatchingUtility.Matcher.SystemObjectToT<With>(c));
        }
    }
    //Buff接口
    public interface IOutputOpeationBuff : IOutputOperation<Entity, Entity, EntityState_Buff>
    {

    }


}
