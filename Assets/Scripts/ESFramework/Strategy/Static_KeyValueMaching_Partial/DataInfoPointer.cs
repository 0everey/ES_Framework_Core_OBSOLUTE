using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public static partial class KeyValueMatchingUtility
    {
        //取数据器
        public static class DataInfoPointer
        {
            public static BuffSoInfo PickBuffSoInfoByKey(string key, SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                if (configuration.PackForBuff.allInfo.ContainsKey(key)) return configuration.PackForBuff.allInfo[key];
                return default;
            }
            public static SkillDataInfo PickSkillSoInfoByKey(string key, SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                if (configuration.PackForSkill.allInfo.ContainsKey(key)) return configuration.PackForSkill.allInfo[key];
                return default;
            }
            public static ActorDataInfo PickActorSoInfoByKey(string key, SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                if (configuration.PackForActor.allInfo.ContainsKey(key)) return configuration.PackForActor.allInfo[key];
                return default;
            }
            public static ItemDataInfo PickItemSoInfoByKey(string key, SoDataInfoConfiguration configuration = null)
            {
                configuration ??= GameCenterManager.Instance.GameCenterArchitecture.configuration;
                if (configuration.PackForItem.allInfo.ContainsKey(key)) return configuration.PackForItem.allInfo[key];
                return default;
            }

            public static T PickASoInfoByKey<T>(string key, ISoDataPack pack = null) where T : class
            {
                var dic = pack?.allInfo_;
                if (dic != null)
                {

                    return dic[key] as T;
                }

                return default;
            }

            public static List<T> PickSoInfoListByKey<T>(string[] keys, ISoDataPack pack = null) where T : class
            {
                if (keys == null || keys.Length == 0) return new List<T>();
                var dic = pack?.allInfo_;
                if (dic != null)
                {
                    List<T> ts = new List<T>();
                    foreach (var i in keys)
                    {
                        ts.Add(dic[i] as T);
                    }
                    return ts;
                }

                return new List<T>();
            }
        }
    }
}

