using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ES
{
    public class SkillRuneControllerApplier : MonoBehaviour
    {
        [ReadOnly,LabelText("已经应用的")]
        public SkillPointRunTime lastBinding;//之前绑定的(已经绑定的)
        [LabelText("想要应用的")]
        public SkillPointRunTime nextBinding;//现在绑定的
        // Start is called before the first frame update
        private ArchitectureKeyValuePoolTypeListIOC _ioc;
        SkillControllerSourceCore _core;

        public Image showImage;
        void Start()
        {
            showImage = GetComponent<Image>();
        }
        void ApplyArchitectureKeyValuePool(SkillControllerSourceCore core)
        {
            _core = core;
            _ioc = core.SkillControllerKeyValueArchitecture?.KeyValueParamIOC;
            if (lastBinding == null) return;
            
        }
        [Button("应用")]
        void ApplyNew()
        {
            var last= lastBinding?.bindingInfo?.allLevel[lastBinding.currentLevel];
            var next = nextBinding?.bindingInfo?.allLevel[nextBinding.currentLevel];
            last?.whenDeApply_?.Pick(_ioc);
            next?.whenApply_?.Pick(_ioc);
            lastBinding = nextBinding;
            ShowReset(last, lastBinding.levelStates_[lastBinding.currentLevel]);
            nextBinding = null;
        }
        private void ShowReset(SkillPointDataInfo.SkillPointLevelAllTransfomor level_ = null, EnumCollect.SkillPointOneLevelState state = EnumCollect.SkillPointOneLevelState.None)
        {

            if (showImage == null || lastBinding?.bindingInfo == null || lastBinding?.bindingInfo.allLevel.Count <= lastBinding.currentLevel) return;
            level_ ??= lastBinding?.bindingInfo.allLevel[lastBinding.currentLevel];
            Debug.Log("setset");
            Sprite sprite = level_?.sprites?.keyValues[state].sprite ?? GameCenterManager.Instance.SkillPointSpritesReference.defaultSpritesForSkillPoint.keyValues[state].sprite;
            Color c = level_?.sprites?.keyValues[state].color_ ?? Color.white;
            Color color = (c != Color.white) ? c : GameCenterManager.Instance.SkillPointSpritesReference.defaultSpritesForSkillPoint.keyValues[state].color_;
            showImage.sprite = sprite;
            showImage.color = color;
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
