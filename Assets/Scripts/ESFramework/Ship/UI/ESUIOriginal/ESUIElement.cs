using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class ESUIElement : ESUIOriginal
    {
        [LabelText("注册到Panel")] public bool register = false;
        [LabelText("唯一标识"),SerializeField] private string registerKey = "注册UI";
        public string RegisterKey { get { return registerKey; } set { registerKey = value; } }
        public ESUIPanel MyPanel { get { if (dirty) GetMyParentAndRegisteThis(); return _myParentPanel; } set { _myParentPanel = value; } }
        [SerializeField,LabelText("所属面板")] protected ESUIPanel _myParentPanel;
        protected bool dirty = false;
        public virtual ESUIPanel GetMyParentAndRegisteThis()
        {
            var use = this.EX_GetCompoentInParentExcludeSelf<ESUIPanel>(includeInactive:true);
            if (use != null) { 
                _myParentPanel = use;
                if (register)
                {
                    _myParentPanel._RegisterElement(this);
                }
            }
            return _myParentPanel;
        }
        private void Awake()
        {
            GetMyParentAndRegisteThis();
            
        }
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}

