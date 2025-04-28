using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [TypeRegistryItem("针播放器_生命周期", "针播放器")]
    public class PointerPlayerWithLifeTime : PointerPlayer,IPointerNone
    {
        public override IPointer Pointer => pointerForStart;
        
        [Space(10)]
        [LabelText("Awake时触发"),SerializeReference]public IPointerNone pointerForAwake =new PointerPickerEveryThing();
        [Space(10)]
        [LabelText("Start时触发"), SerializeReference] public IPointerNone pointerForStart = new PointerPickerEveryThing();
        [Space(10)]
        [LabelText("Update时触发"), SerializeReference] public IPointerNone pointerForUpdate = new PointerPickerEveryThing();
        [Space(10)]
        [LabelText("OnEnable时触发"), SerializeReference] public IPointerNone pointerForEnable = new PointerPickerEveryThing();
        [Space(10)]
        [LabelText("OnDisable时触发"), SerializeReference] public IPointerNone pointerForDisable= new PointerPickerEveryThing();
        [Space(10)]
        [LabelText("OnDestroy时触发"), SerializeReference] public IPointerNone pointerForDestroy = new PointerPickerEveryThing();
        // Start is called before the first frame update
        private void Awake()
        {
            pointerForAwake?.Pick();
        }
        void Start()
        {
            pointerForStart?.Pick();
            
        }
        
        // Update is called once per frame
        void Update()
        {
            pointerForUpdate?.Pick();
        }
        private void OnEnable()
        {
            pointerForEnable?.Pick();
        }
        private void OnDisable()
        {
            pointerForDisable?.Pick();
        }
        private void OnDestroy()
        {
            pointerForDestroy?.Pick();
        }
    }
}
