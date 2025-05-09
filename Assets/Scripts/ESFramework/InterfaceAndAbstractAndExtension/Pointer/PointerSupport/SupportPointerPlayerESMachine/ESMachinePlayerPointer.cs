using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;


namespace ES {
    public class ESMachinePlayerPointer : PointerPlayer
    {
        [LabelText("可测试(按T)")]
        public bool TEST = false;
        private void Start()
        {
            TryBind();
        }
        [ReadOnly, LabelText("已经绑定"),ShowInInspector] public bool hasBind => binding != null;
        [HideInInspector]public BaseOriginalStateMachine binding;

        [Header("测试键")]
        [ValueDropdown("GetKeys",AppendNextDrawer =true),LabelText("触发状态")]
        public string ActiveTest;
        public string[] cache;
        private string[] GetKeys()
        {
            if (binding == null) return new string[] { "还没绑定哈" };
            if (cache == null) CacheKeys();
            return cache;           
        }
        private void CacheKeys()
        {
            if (binding != null)
            {
                cache = binding.KeysWithLayer();
            }
        }
        public override IPointer Pointer => default;
        [PropertySpace(30)]
        [Button("绑定或者刷新键表"),ButtonGroup("we")]
        public void TryBind()
        {
            var use = GetComponent<IWithESMachine>();
            if (use != null && use.TheMachine != null)
            {
                binding = use.TheMachine;
            }
            CacheKeys();
        }
        bool use = false;
        float f = 1;
        private void Update()
        {
            if (TEST)
            {
                if (Keyboard.current.tKey.wasPressedThisFrame)
                {
                    use = true;
                    f = UnityEngine.Random.value;
                }
                if (use)
                {
                    f -= Time.deltaTime;
                    if (f < 0)
                    {
                        use = false;
                        TestActive();
                    }
                }
            }
            
        }

        [Button("测试启动"), ButtonGroup("we")]
        public bool TestActive()
        {
            if (binding == null)
            {
                TryBind();
            }
            bool back = false;
            if (binding is ESNanoStateMachine_StringKey nano)
            back= nano.TryActiveStateByKeyWithLayer(ActiveTest);
            else back= binding.TryActiveStateByKey(ActiveTest);
            if(binding is ESStandardStateMachine_StringKey stringKey)
            {
                stringKey.Test_OutPutAllStateRunning();
            }
            return back;
        }
        [Button("测试关闭"), ButtonGroup("we")]
        public bool TestInActive()
        {
            if (binding == null)
            {
                TryBind();
            }
            bool back = false;
            if (binding is ESNanoStateMachine_StringKey nano) back = nano.TryInActiveStateByKeyWithLayer(ActiveTest);
            back = binding.TryInActiveStateByKey(ActiveTest);
            if (binding is ESStandardStateMachine_StringKey stringKey)
            {
                stringKey.Test_OutPutAllStateRunning();
            }
            return back;

        }
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            return TestActive();
        }
    }
    public interface IWithESMachine
    {
        BaseOriginalStateMachine TheMachine { get; }
    }
}
