using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public interface IWithEnumeratorTask 
    {
        IEnumerator DoTaskAsync(System.Action finishCallback);
    }
}

