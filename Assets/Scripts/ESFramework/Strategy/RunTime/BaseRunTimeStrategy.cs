using ES;
using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ES {


    public abstract class BaseRunTimeStrategy<Enum_,Self> : SingletonAsNormalClass<Self>
        where Self:BaseRunTimeStrategy<Enum_, Self>, new()
    {
        public abstract Dictionary<Enum_, Self> AllStrategy { get; }
        public Self UseStrategy(Enum_ enum_)
        {
            if (AllStrategy == null) return null;
            if (AllStrategy.ContainsKey(enum_)) return AllStrategy[enum_];
            return AllStrategy.Values.FirstOrDefault();
        }
    }
    
    
}
