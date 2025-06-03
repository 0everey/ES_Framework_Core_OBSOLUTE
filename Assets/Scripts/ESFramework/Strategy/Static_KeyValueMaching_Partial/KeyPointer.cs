using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ES
{

    public static partial class KeyValueMatchingUtility
    {
        //数据针集合器
        public static class KeyPointer
        {
            //为了性能，分成5种把
            public static string[] PickBuffAllKeys(SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                return configuration?.PackForBuff?.allInfo.Keys.ToArray() ?? default;
            }
            public static string[] PickSKillAllKeys(SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                return configuration?.PackForSkill?.allInfo.Keys.ToArray() ?? default;
            }
            public static string[] PickActorAllKeys(SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                return configuration?.PackForActor?.allInfo.Keys.ToArray() ?? default;
            }
            public static string[] PickItemAllKeys(SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                return configuration?.PackForItem?.allInfo.Keys.ToArray() ?? default;
            }

            public static string[] PickPackAllKeys(ISoDataPack pack)
            {
                var skeys = pack?.allInfo_?.Keys;
                int ii = skeys?.Count ?? 0;
                string[] keys = new string[ii];
                if (ii > 0)
                {
                    for (int i = 0; i < ii; i++)
                    {
                        skeys.CopyTo(keys, 0);
                    }
                }

                return keys;

            }

        }
    }
}

