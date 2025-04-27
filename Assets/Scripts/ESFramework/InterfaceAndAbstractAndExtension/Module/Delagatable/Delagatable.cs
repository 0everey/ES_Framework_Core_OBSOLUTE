using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    public interface IModule
    {

    }
    public interface IDelegatable : IModule
    {
        bool HasEnable { get; set; }
        void TryEnable();
        void TryDisable();
        void OnEnable();
        void OnDisable();
    }
    public interface IDelegatableWithHosting<Host> : IDelegatable,IWithHosting<Host> where Host:IHosting
    {
      
    }
    public abstract class Delegatable : IDelegatable
    {
        public bool HasEnable { get; set; }
        public virtual void OnEnable()
        {

        }
        public virtual void OnDisable()
        {

        }

        public void TryEnable()
        {
            if (HasEnable) return;
            HasEnable = true;
            OnEnable();
        }

        public void TryDisable()
        {
            if (HasEnable)
            {
                OnDisable();
                HasEnable = false;
            }
            return;
        }
    }
    public abstract class DelegatableWithHosting<Host> :Delegatable, IDelegatableWithHosting<Host> where Host: IHosting
    {
        public abstract Host GetHost { get; }
        public bool HasSubmit { get; set; }

        public abstract bool OnSubmitHosting(Host hosting, bool asVirtual = false);

        public abstract bool OnWithDrawHosting(Host hosting, bool asVirtual = false);

        public bool TrySubmitHosting(Host hosting, bool asVirtual = false)
        {
            if (HasSubmit) return true;
            if (asVirtual)
            {
                if (hosting.virtualBeHosted != null)
                {
                    if(!hosting.virtualBeHosted.Contains(this)) 
                        hosting.virtualBeHosted.Add(this);
                    return true;
                }
            }
            return HasSubmit = OnSubmitHosting(hosting, asVirtual);
        }

        public bool TryWithDrawHosting(Host hosting, bool asVirtual = false)
        {
            if (!HasSubmit) return false;
            if (asVirtual)
            {
                if (hosting.virtualBeHosted != null)
                {
                    if (hosting.virtualBeHosted.Contains(this))
                        hosting.virtualBeHosted.Remove(this);
                    HasSubmit = false;
                    return true;
                }
            }
            return HasSubmit = !OnWithDrawHosting(hosting, asVirtual);
        }
    }
    public abstract class UpdatableDelegatableWithHosting<Host> : DelegatableWithHosting<Host>, IDelegatedUpdatable where Host:IHosting
    {
        public override Host GetHost { get; }
                
                
                
           public override void OnEnable() { }
        public override bool OnSubmitHosting(Host hosting, bool asVirtual = false) { if (HasSubmit) return true; HasSubmit = true; return false; }
        public override bool OnWithDrawHosting(Host hosting, bool asVirtual = false)
        {
            if (!HasSubmit) return false;
            return false;
        }

        public override void OnDisable()
        {
        }

        public virtual void Update()
        {  
        }
       
    }
}
