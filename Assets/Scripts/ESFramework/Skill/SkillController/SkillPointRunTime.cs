using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ES
{
    public class SkillPointRunTime : MonoBehaviour
    {
        [LabelText("绑定的技能点")]public SkillPointDataInfo bindingInfo;
        [LabelText("当前等级")]public int currentLevel = 0;
        public List<EnumCollect.SkillPointOneLevelState> levelStates_ = new List<EnumCollect.SkillPointOneLevelState>() { EnumCollect.SkillPointOneLevelState.None, EnumCollect.SkillPointOneLevelState.None, EnumCollect.SkillPointOneLevelState.None };
        private ArchitectureKeyValuePoolTypeListIOC _ioc;
        SkillControllerSourceCore _core;
        public Image showImage;
        // Start is called before the first frame update
        void Start()
        {
            showImage ??= GetComponent<Image>();
            ShowReset();
        }
        public void RefreshAfter()
        {
            if (bindingInfo != null&& levelStates_!=null&& bindingInfo.allLevel!=null)
            {
                int more = bindingInfo.allLevel.Count- levelStates_.Count;
                if (more>0)
                {
                    for(int i=0;i< more; i++)
                    {
                        levelStates_.Add(EnumCollect.SkillPointOneLevelState.None);
                    }
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
        
        }
        void ApplyArchitectureKeyValuePool(SkillControllerSourceCore core)
        {
            _core = core;
            _ioc = core.SkillControllerKeyValueArchitecture?.KeyValueParamIOC;
            if (bindingInfo == null) return;
            //应用它 每次只评估一次
            for(int i = 0; i < 10; i++)
            {
                bool hasChange = AssessOnce(_ioc);
                if (!hasChange) break;
            }
        }
        private bool AssessOnce(ArchitectureKeyValuePoolTypeListIOC ioc)
        {
            bool hasChange = false;

            if (bindingInfo.allLevel.Count > currentLevel)
            {
                var level = bindingInfo.allLevel[currentLevel];
                var state = levelStates_[currentLevel];
                ;
                switch (state)
                {
                    case EnumCollect.SkillPointOneLevelState.None: 
                        if(level.forNone2UnKnownDetail?.pointer?.Pick(ioc) ?? level.forNone2UnKnownDetail.defaultForNull)
                        {
                            SwitchNowState(bindingInfo,currentLevel,EnumCollect.SkillPointOneLevelState.None,EnumCollect.SkillPointOneLevelState.UnknownDetail);
                            hasChange = true;
                        }

                        break;

                    case EnumCollect.SkillPointOneLevelState.UnknownDetail:
                        //正方向
                        if (level.forUnKnownDetail2CantUnlock?.pointer?.Pick(ioc) ?? level.forUnKnownDetail2CantUnlock.defaultForNull)
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.UnknownDetail, EnumCollect.SkillPointOneLevelState.CantUnlock);
                        }
                        //负方向
                        else if (!(level.forNone2UnKnownDetail?.pointer?.Pick(ioc) ?? level.forNone2UnKnownDetail.defaultForNull))
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.UnknownDetail, EnumCollect.SkillPointOneLevelState.None);
                        }
                        break;

                    case EnumCollect.SkillPointOneLevelState.CantUnlock:
                        if (level.forCantUnlock2CanUnlockButNotFeet?.pointer?.Pick(ioc) ?? level.forCantUnlock2CanUnlockButNotFeet.defaultForNull)
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.CantUnlock, EnumCollect.SkillPointOneLevelState.CanUnlockButOptionNotFeet);
                        }
                        //负方向
                        else if (!(level.forUnKnownDetail2CantUnlock?.pointer?.Pick(ioc) ?? level.forUnKnownDetail2CantUnlock.defaultForNull))
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.CantUnlock, EnumCollect.SkillPointOneLevelState.UnknownDetail);
                        }
                        break;

                    case EnumCollect.SkillPointOneLevelState.CanUnlockButOptionNotFeet:
                        if (level.forCanUnlockButNotFeet2CanUnlockComplete?.pointer?.Pick(ioc) ?? level.forCanUnlockButNotFeet2CanUnlockComplete.defaultForNull)
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.CanUnlockButOptionNotFeet, EnumCollect.SkillPointOneLevelState.CanUnlockComplete);
                        } 
                        //负方向
                        else if (!(level.forCantUnlock2CanUnlockButNotFeet?.pointer?.Pick(ioc) ?? level.forCantUnlock2CanUnlockButNotFeet.defaultForNull))
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.CanUnlockButOptionNotFeet, EnumCollect.SkillPointOneLevelState.CantUnlock);
                        }
                        break;

                    case EnumCollect.SkillPointOneLevelState.CanUnlockComplete:
                        if (level.forCanUnlockComplete2Unlock?.pointer?.Pick(ioc) ?? level.forCanUnlockComplete2Unlock.defaultForNull)
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.CanUnlockComplete, EnumCollect.SkillPointOneLevelState.Unlock);
                        }
                        //负方向
                        else if (!(level.forCanUnlockButNotFeet2CanUnlockComplete?.pointer?.Pick(ioc) ?? level.forCanUnlockButNotFeet2CanUnlockComplete.defaultForNull))
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.CanUnlockComplete, EnumCollect.SkillPointOneLevelState.CanUnlockButOptionNotFeet);
                        }
                        break;

                    case EnumCollect.SkillPointOneLevelState.Unlock:
                        /*if (level.forNone2UnKnownDetail?.pointer?.Pick() ?? level.forNone2UnKnownDetail.defaultForNull)
                        {
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.Unlock, EnumCollect.SkillPointOneLevelState.UnknownDetail);
                        }*/
                            //负方向
                        /*if (!(level.forCanUnlockComplete2Unlock?.pointer?.Pick(ioc) ?? level.forCanUnlockComplete2Unlock.defaultForNull))
                        {
                            hasChange = true;
                            SwitchNowState(bindingInfo, currentLevel, EnumCollect.SkillPointOneLevelState.Unlock, EnumCollect.SkillPointOneLevelState.CanUnlockComplete);
                        }*/
                        break;
                }
            }

            return hasChange;
        }
        public void SwitchNowState(SkillPointDataInfo info,int index, EnumCollect.SkillPointOneLevelState pre, EnumCollect.SkillPointOneLevelState after)
        {
            if (pre == after|| _ioc==null) return;


            levelStates_[index] = after;
            var level = info.allLevel[index];
            if (after == EnumCollect.SkillPointOneLevelState.Unlock)
            {
                //解锁
               
                level.whenUnlock.Pick(_ioc);//执行
                if (index + 1 < info.allLevel.Count)
                {
                    //可以让index++;
                }
                SendMessageUpwards("RefreshAll");
            }
            ShowReset(level, after);
            
        }
        private void ShowReset(SkillPointDataInfo.SkillPointLevelAllTransfomor level_=null, EnumCollect.SkillPointOneLevelState state=EnumCollect.SkillPointOneLevelState.None)
        {
            
            if (showImage == null||bindingInfo==null||bindingInfo.allLevel.Count<=currentLevel) return;
            level_ ??= bindingInfo.allLevel[currentLevel];
            Debug.Log("setset");
            Sprite sprite = level_?.sprites?.keyValues[state].sprite ?? GameCenterManager.Instance.SkillPointSpritesReference.defaultSpritesForSkillPoint.keyValues[state].sprite;
            Color c = level_?.sprites?.keyValues[state].color_??Color.white;
            Color color=(c!=Color.white)?c:GameCenterManager.Instance.SkillPointSpritesReference.defaultSpritesForSkillPoint.keyValues[state].color_;
            showImage.sprite = sprite;
            showImage.color = color;
        }
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            
            Handles.color = Color.yellow;
            Handles.Label(transform.position, bindingInfo?.key.str_direc+"LV " + currentLevel+"\n 状态 : "+KeyValueMatchingUtility.Matcher.EnumToString_SkillPointState(levelStates_[currentLevel]), GameCenterManager.Instance.style);

#endif
        }
        [Button("手动解锁")]
        private void HandleUnLock()
        {
            if (_ioc == null) return;

            if(levelStates_[currentLevel]== EnumCollect.SkillPointOneLevelState.CanUnlockComplete)
            {
                SwitchNowState(bindingInfo,currentLevel, EnumCollect.SkillPointOneLevelState.CanUnlockComplete, EnumCollect.SkillPointOneLevelState.Unlock);
            }
            
        }
    }
}
