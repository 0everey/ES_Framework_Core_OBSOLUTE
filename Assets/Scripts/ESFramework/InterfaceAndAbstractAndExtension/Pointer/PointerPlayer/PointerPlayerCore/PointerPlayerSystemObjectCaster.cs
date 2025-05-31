using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [TypeRegistryItem("针播放器_系统物体投射", "针播放器")]
    public class PointerPlayerSystemObjectCaster : PointerPlayer, IPointer, IPointerForSystemObjectCaster
    {
        public override IPointer Pointer => pointerFoSystemObject;
        public PointerPlayerSystemObjectCaster playerCaster => this;
        [LabelText("投射的物体", SdfIconType.Link45deg), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")]
        public object aObject;
        [LabelText("Pick一个物体"), SerializeReference, GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public IPointer pointerFoSystemObject;
        public object Cast()
        {
            return aObject;
        }
        public override object Pick(object on= null, object from = null, object with = null)
        {
            if (pointerFoSystemObject == (IPointer)this)
            {
                Debug.LogError("系统物体投射针不要把自己当做来源！！！");
                return null;
            }
            object oo = pointerFoSystemObject?.Pick();
            if (oo != null)
            {
                aObject = oo;
            }
            return aObject;
           
        }
        public object Recieve(object oo)
        {
            return aObject = oo;
        }
    }
}
