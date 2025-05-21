using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [Serializable, TypeRegistryItem("Link_开始缓存对象")]
    public struct LinkForEntity_StartCache : ILink
    {
        [LabelText("缓存标志")]
        public string cacheKey;
        [LabelText("更新(true)还是添加(false)")]
        public bool UpdateOrAdd;
        [LabelText("是否允许重复")]
        public bool CanRepeat;
        [LabelText("列表")]
        public List<Entity> entities;
    }
    [Serializable, TypeRegistryItem("Link_取消缓存对象")]
    public struct LinkForEntity_StopCache : ILink
    {
        [LabelText("全部缓存清空")]
        public bool ClearAll;
        [LabelText("按列表移除(true)还是全部移除")]
        public bool ByListOrAll;
        [LabelText("列表")]
        public List<Entity> entities;
    }

    [Serializable, TypeRegistryItem("Link_攻击真的发生了(已经造成伤害)")]
    public struct Link_EntityAttackEntityTruely : ILink
    {
        public Entity attacker;
        public Entity victim;
        public Damage damage;
    }

    [Serializable, TypeRegistryItem("Link_尝试攻击")]
    public struct Link_EntityAttackEntityTryStart : ILink
    {
        public Entity attacker;
        public Entity victim;
        public Damage damage;
    }
}