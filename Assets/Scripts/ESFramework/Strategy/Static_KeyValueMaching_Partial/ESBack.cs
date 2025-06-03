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
        public static class ESBack
        {
            public static class ForEntityBack
            {
                public static List<Entity> GetEntityAroundFriend(Entity entity, float r, Vector3? center = null)
                {

                    var use = Physics.OverlapSphere(center ?? entity.transform.position, r);
                    List<Entity> entities = new List<Entity>();
                    foreach (var i in use)
                    {
                        Entity ee = i.GetComponent<Entity>();
                        if (ee != null && !entities.Contains(ee)) entities.Add(ee);
                    }
                    //查找把，找啊找r
                    return entities;
                }
                public static List<Entity> GetEntityAround(Entity entity, float r, Vector3? center = null)
                {

                    var use = Physics.OverlapSphere(center ?? entity.transform.position, r);
                    List<Entity> entities = new List<Entity>();
                    foreach (var i in use)
                    {
                        Entity ee = i.GetComponent<Entity>();
                        if (ee != null && !entities.Contains(ee)) entities.Add(ee);
                    }
                    //查找把，找啊找r
                    return entities;
                }
                public static List<Entity> GetEntityTargetEntityCache(Entity entity, string Key = "Main", bool useAndClear = true)
                {

                    //查找把，找啊找r
                    if (entity?.BaseDomain.Module_Cache != null)
                    {
                        if (useAndClear)
                        {
                            return entity.BaseDomain.Module_Cache.CacheEntity.DequeueAll(Key).ToList();
                        }
                        else
                        {
                            return entity.BaseDomain.Module_Cache.CacheEntity.PeekAll(Key);
                        }

                    }
                    return null;//返回缓冲池
                }
                public static List<Vector3> GetEntityTargetVector3Cache(Entity entity, string Key = "Main", bool useAndClear = true)
                {

                    //查找把，找啊找r
                    if (entity?.BaseDomain.Module_Cache != null)
                    {
                        if (useAndClear)
                        {
                            return entity.BaseDomain.Module_Cache.CacheVector3.DequeueAll(Key).ToList();
                        }
                        else
                        {
                            return entity.BaseDomain.Module_Cache.CacheVector3.PeekAll(Key);
                        }

                    }
                    return null;//返回缓冲池
                }
                public static List<Entity> GetEntityVision(Entity entity, int maxCount = 5, bool reTry = false)
                {

                    //查找把，找啊找r
                    return null;
                }
            }
        }
    }
}

