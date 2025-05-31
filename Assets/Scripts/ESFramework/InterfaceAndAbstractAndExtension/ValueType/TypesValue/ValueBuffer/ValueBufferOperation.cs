using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class ValueBufferOperation<ValueType>
    {

    }
    public class ValueBufferOperationFloat: ValueBufferOperation<float>
    {
        [NonSerialized]
        public BufferDataFloat bufferData=null;
        [NonSerialized]
        public float timeHasGo = 0;
    }
}

