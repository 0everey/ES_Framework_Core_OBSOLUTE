using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public interface IFactory<T>
    {
        T Create();
    }
    public class ESFactory_DefaultNew<T> : IFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }
    public class ESFactory_CustomFunction<T> : IFactory<T>
    {
        protected Func<T> createFunc;
        public ESFactory_CustomFunction(Func<T> factoryMethod)
        {
            createFunc = factoryMethod;
        }
        public T Create()
        {
            return createFunc();
        }
    }
    public class Factory
    {

    }
}

