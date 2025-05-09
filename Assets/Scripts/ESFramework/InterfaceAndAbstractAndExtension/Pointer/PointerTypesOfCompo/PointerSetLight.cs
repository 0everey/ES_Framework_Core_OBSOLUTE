using ES.EvPointer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ES
{
    public class PinSetLight : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        { 
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
    [Serializable,TypeRegistryItem("3D灯光直接设置","渲染")]
    public class PointerSetLight : IPointerNone
    {
        [LabelText("使用的灯光")]
        public Light light;
        [LabelText("发光强度"),SerializeReference]public IPointerForFloat_Only pointerForFloat_ = new PointerForFloat_Direct() { float_=2 };
        [LabelText("光色"), SerializeReference] public IPointerForColor_Only forColor_Only = new PointerForColor_Direc() { color=Color.white };
        [LabelText("范围"), SerializeReference] public IPointerForFloat_Only pointerForFloat2_ = new PointerForFloat_Direct() { float_=20 };
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (light != null)
            {
                light.intensity = pointerForFloat_?.Pick()?? light.intensity;
                light.color = forColor_Only?.Pick() ?? light.color;
                light.range = pointerForFloat2_?.Pick() ?? light.range;
                
            }
            return null;
        }
    }
    [Serializable, TypeRegistryItem("2D灯光直接设置", "渲染")]
    public class PointerSetLight2D : IPointerNone
    {
        [LabelText("使用的灯光")]
        public Light2D light;
        [LabelText("发光强度"), SerializeReference] public IPointerForFloat_Only pointerForFloat_ = new PointerForFloat_Direct() { float_ = 2 };
        [LabelText("光色"), SerializeReference] public IPointerForColor_Only forColor_Only = new PointerForColor_Direc() { color = Color.white };
        [LabelText("范围"), SerializeReference] public IPointerForFloat_Only pointerForFloat2_ = new PointerForFloat_Direct() { float_ = 20 };
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (light != null)
            {
                light.intensity = pointerForFloat_?.Pick() ?? light.intensity;
                light.color = forColor_Only?.Pick() ?? light.color;
                light.shapeLightFalloffSize = pointerForFloat2_?.Pick() ?? light.shapeLightFalloffSize;
            }
            return null;
        }
    }

   
}
