using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class ESValueMaster : SingletonAsMono<ESValueMaster>
    {
        #region 池化-结算与缓冲等
        public ESSimpleObjectPool<SettleOperationFloat> floatSettleOpsPool = new ESSimpleObjectPool<SettleOperationFloat>(
            () => new SettleOperationFloat(),
            (it) => { it.Source = null;it.Value = 0; }
            );

        public ESSimpleObjectPool<ValueBufferOperationFloat> floatBufferOpsPool = new ESSimpleObjectPool<ValueBufferOperationFloat>(
            () => new ValueBufferOperationFloat(),
            (it) => { it.timeHasGo = 0; }
            );
        #endregion

    }
}

