using ES;
using ES.EvPointer;

using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.TypeSearch;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace ES
{
    public class DomainForGameCenterManager : DomainBase<GameCenterManager,DomainClipForGamecenterManager, DomainForGameCenterManager> { 
        //常用模块
       /* [NonSerialized] public DCFG_PlayerState Module_PlayerState;
        [NonSerialized] public DCFG_PlayerPick Module_PlayerPick;
        [NonSerialized] public DCFG_PlayerRuneControll Module_RuneControll;
        [NonSerialized] public DCFG_RoomGenerate Module_RoomGenerate;*/
        private void OnGUI()
        {
            return;
            /*float w  = Screen.width;
            float h = Screen.height;
            GUIStyle style = new GUIStyle("label");
            style.fontSize = 25;
            Rect rect = new Rect(0.6f*w,0.0f*h,0.4f*w,0.7f*h);
            GUILayout.BeginArea(rect);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"血量:{Module_PlayerState.m_healthPoint}最大{Module_PlayerState.m_maxHealthPoint}", style);
            GUILayout.Label($"魂:{Module_PlayerState.m_soul_}最大{Module_PlayerState.m_maxSoul}", style);
            GUILayout.Label($"伤害乘数:{Module_PlayerState.mm_AttackeDamageMutiLevel}  攻速乘数：{Module_PlayerState.mm_AttackSpeedMutiLevel}", style);
            GUILayout.Label($"暴击概率:{Module_PlayerState.mm_CriticalhitsP}  吸血加成：{Module_PlayerState.mm_VampirePercent}", style);
            GUILayout.Label($"正在格挡:{Module_PlayerState.isSpecialGeDang}  正在防御{Module_PlayerState.isSimpleDefending}", style);
            GUILayout.Label($"魔法值:{Module_PlayerState.m_magicPoint}  最大{Module_PlayerState.m_maxMagicPoint}", style);
            GUILayout.Label($"格挡值:{Module_PlayerState.m_blockMeter}  最大{Module_PlayerState.m_maxBlockMeter}", style);
            GUILayout.Label($"力量:{Module_PlayerState.m_strength}  敏捷{Module_PlayerState.m_agility} 耐力 {Module_PlayerState.m_stamina}", style);
            GUILayout.EndArea();*/
        }
        protected override void RegesterAll()
        {
            base.RegesterAll();
        }
        protected override void CreateLink()
        {
            base.CreateLink();
            core.BaseDomain = this;
        }
       /* public void Onoo(Linkint3 link)
        {

        }
        public void OnLink(Linkint3 link)
        {
            Debug.Log(link.By_);
           
        }*/
        #region 
      /*  public void QuickInvoke_AddNewRune(RuneDataInfo info)
        {
            Module_RuneControll?.TryAdd(info);
        }
        public void QuickInvoke_RemoveFirstRune()
        {
            Module_RuneControll?.TryNext();
        }*/
        #endregion

    }

    #region 基本切片模范
    [Serializable]
    public abstract class DomainClipForGamecenterManager : DomainClip<GameCenterManager,DomainForGameCenterManager>
    {
        
        public override string Description_ => "游戏管理器切片域";
        
        protected override bool OnSubmitHosting(DomainForGameCenterManager hosting)
        {
            domain = hosting as DomainForGameCenterManager;
            //
            if (domain != null) return true;
            return base.OnSubmitHosting(hosting);
        }
        protected override bool OnWithDrawHosting(DomainForGameCenterManager hosting)
        {
            throw new NotImplementedException();
        }

    }
    [Serializable,TypeRegistryItem("模块01")]
    public class module1 : DomainClipForGamecenterManager
    {
        protected override void Update()
        {
            
            base.Update();
        }
    }
    [Serializable, TypeRegistryItem("模块00")]
    public class module0 : DomainClipForGamecenterManager
    {
        protected override void Update()
        {

            base.Update();
        }
    }
    [Serializable, TypeRegistryItem("模块02")]
    public class module2 : DomainClipForGamecenterManager
    {
        protected override void Update()
        {

            base.Update();
        }
    }
    [Serializable, TypeRegistryItem("模块03")]
    public class module3 : DomainClipForGamecenterManager
    {
        protected override void Update()
        {

            base.Update();
        }
    }
    [Serializable, TypeRegistryItem("模块04")]
    public class module4 : DomainClipForGamecenterManager
    {
        protected override bool OnSubmitHosting(DomainForGameCenterManager hosting)
        {
            
            return base.OnSubmitHosting(hosting);
        }
        protected override void Update()
        {

            base.Update();
        }
    }
    /*
    [Serializable]
    public class DCFG_MODEL : DomainClipForGamecenterManager
    {
        #region 字段属性列


        #endregion
        public override string Description_ => "剪影";
        public override void OnRegester(GameCenterManager c)
        {
            base.OnRegester(c);
            //c.BaseDomain.BaseESModule
        }
    }
    */
    #endregion
    //下面的功能不能脱离游戏内容，所以全部注释了
    /*[Serializable]
    public class DCFG_PlayerPick : DomainClipForGamecenterManager,IReceiveLink<Link_AttackHappen>
        
    {
        public override string Description_ => "玩家搜集掉落物剪影";

        
        public override bool OnSubmitHostingAsNormal(DomainBase hosting, bool asVirtual = false)
        {
            if (base.OnSubmitHostingAsNormal(hosting))
            {
                (hosting as DomainForGameCenterManager).Module_PlayerPick = this;
                return true;
            }
            return base.OnSubmitHostingAsNormal(hosting);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
          //  GameCenterManager.Instance.GameCenterArchitecture.AddRecieveLink(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
          //  GameCenterManager.Instance.GameCenterArchitecture.RemoveRecieveLink(this);
        }
        public void OnLink(Link_AttackHappen link)
        {
            Debug.Log("成功接受Link" + link.On_);
            
        }

        public void ProactiveInvoke_PlayerPickBonusScript(BonusScript bonus)
        {
            //取出
            bonus?.bonus1?.Pick(by:bonus);
            if (bonus != null)
            {
                Debug.Log($"收集战利品{bonus}");
            }
        }

       
    }
    
    [Serializable]
    public class DCFG_PlayerState : DomainClipForGamecenterManager,IReceiveLink<Link_SelfDefine>,IReceiveLink<Link_BuffHandleChangeHappen>
    {
        #region 字段属性列
        [LabelText("玩家个体"),FoldoutGroup("数值内容")] public Entity PlayerSelf;
        [LabelText("游戏事件时间戳起始点"), FoldoutGroup("数值内容")] public float timeStartForEvent;
        [LabelText("当前生命值 "), FoldoutGroup("数值内容")] public float m_healthPoint = 60;
        [LabelText("最大生命值"), FoldoutGroup("数值内容")] public float m_maxHealthPoint = 100;
        [LabelText("当前魔法值"), FoldoutGroup("数值内容")] public float m_magicPoint = 100;
        [LabelText("最大魔法值"), FoldoutGroup("数值内容")] public float m_maxMagicPoint = 100;
        [LabelText("当前持有魂数量"), FoldoutGroup("数值内容")] public float m_soul_ = 0;
        [LabelText("最大持有魂数量"), FoldoutGroup("数值内容")] public float m_maxSoul = 100;
        [LabelText("当前格挡值"), FoldoutGroup("数值内容")] public float m_blockMeter = 0;
        [LabelText("最大格挡值"), FoldoutGroup("数值内容")] public float m_maxBlockMeter = 100;
        [LabelText("正在格挡中"), FoldoutGroup("数值内容")] public bool isSpecialGeDang = false;
        [LabelText("正在防御中"), FoldoutGroup("数值内容")] public bool isSimpleDefending = false;

        private float lastFrme_m_block = 0;
        private float recure_m_block = 0;
        [LabelText("完美格挡时间"), FoldoutGroup("数值内容")] public float m_parryDuration_ = 1.5f; 
        [LabelText("力量属性"), FoldoutGroup("数值内容")] public float m_strength = 100;
        [LabelText("敏捷属性"), FoldoutGroup("数值内容")] public float m_agility = 100;
        [LabelText("耐力属性"), FoldoutGroup("数值内容")] public float m_stamina = 100;
        [LabelText("伤害乘数"), FoldoutGroup("数值内容")] public float mm_AttackeDamageMutiLevel = 1;
        [LabelText("暴击效果乘数"), FoldoutGroup("数值内容")] public float mm_CriticalhitsMutiLevel = 2;
        [LabelText("暴击效果概率"), FoldoutGroup("数值内容")] public float mm_CriticalhitsP = 0.1f;
        [LabelText("攻速效果加成"), FoldoutGroup("数值内容")] public float mm_AttackSpeedMutiLevel = 1;
        [LabelText("吸血效果百分比加成"), FoldoutGroup("数值内容")] public float mm_VampirePercent = 0;
        [LabelText("初始装备"), FoldoutGroup("数值内容")] public List<Transform> startWeapons = new List<Transform>();

        #endregion

        #region 数据输出
        [LabelText("数值输出—游戏页面"),FoldoutGroup("数据输出")] public Canvas BaseGameCanves;
        [LabelText("输出血量参数"), FoldoutGroup("数据输出")] public TMP_Text TextForHelath;
        [LabelText("输出耐力参数"), FoldoutGroup("数据输出")] public TMP_Text TextForStamina;
        [LabelText("输出灵魂数量"), FoldoutGroup("数据输出")] public TMP_Text TextForSoulNum;
        [LabelText("输出玩家状态"), FoldoutGroup("数据输出")] public TMP_Text TextForStatus;
        [Space(10)]
        [LabelText("BUFF存放"), FoldoutGroup("数据输出")] public RectTransform GroupForBuff;
        [LabelText("信息存放"), FoldoutGroup("数据输出")] public RectTransform GroupForEventMessage;
        [LabelText("还没用"), FoldoutGroup("数据输出")] public RectTransform GroupForOther;

        [LabelText("BUFF预制件"), FoldoutGroup("数据输出")] public RectTransform PrefabForBuff;
        [LabelText("信息预制件"), FoldoutGroup("数据输出")] public RectTransform PrefabForEventMessage;

        #endregion

        #region 数据缓存
        public List<TimeSpanAndMessage> timeSpanAndMessages = new List<TimeSpanAndMessage>();

        #endregion
        public override string Description_ => "玩家状态剪影";
        public override bool OnSubmitHostingAsNormal(DomainBase hosting, bool asVirtual = false)
        {
            if (base.OnSubmitHostingAsNormal(hosting))
            {
                (hosting as DomainForGameCenterManager).Module_PlayerState = this;

                Ev_PoolManager.Instance.CreatePool(PrefabForBuff.gameObject,10);
                Ev_PoolManager.Instance.CreatePool(PrefabForEventMessage.gameObject, 10);


                return true;
            }
            return base.OnSubmitHostingAsNormal(hosting);
        }
       
        protected override void OnEnable()
        {
            base.OnEnable();
            
            domain.core.OnSceneLoaded.AddListener(PassiveDelegate_SceneLoad);
            domain.core.OnLevelEndSave.AddListener(PassiveDelegate_LevelEndSave);
            domain.core.OnWeaponSetup.AddListener(PassiveDelegate_OnWeaponSetup);
            GameCenterManager.Instance.GameCenterArchitecture.AddRecieveLink<Link_SelfDefine>(this);
            GameCenterManager.Instance.GameCenterArchitecture.AddRecieveLink<Link_BuffHandleChangeHappen>(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            domain.core.OnSceneLoaded.RemoveListener(PassiveDelegate_SceneLoad);
            domain.core.OnLevelEndSave.RemoveListener(PassiveDelegate_LevelEndSave);
            domain.core.OnWeaponSetup.RemoveListener(PassiveDelegate_OnWeaponSetup);
            GameCenterManager.Instance.GameCenterArchitecture.RemoveRecieveLink<Link_SelfDefine>(this);
            GameCenterManager.Instance.GameCenterArchitecture.RemoveRecieveLink<Link_BuffHandleChangeHappen>(this);
        }
        private void Method_OpenGameCenterCanves()
        {
            BaseGameCanves?.gameObject.SetActive(true);
        }
        private void Method_CloseGameCenterCanves()
        {
            BaseGameCanves?.gameObject.SetActive(false);
        }
        private void PassiveDelegate_SceneLoad(Scene scene, LoadSceneMode model)
        {
            Entity e= UnityEngine.Object.FindObjectOfType<Entity>();
            if (e != null)
            {
                PlayerSelf = e;
                Method_OpenGameCenterCanves();
            }
            else
            {
                Method_CloseGameCenterCanves();
            }
            if (e != null) 
            if (PlayerSelf != null)
            {
                Method_LoadData();
            }
        } 
        
        private void PassiveDelegate_LevelEndSave()
        {

        }
        private void PassiveDelegate_OnWeaponSetup(WeaponSwitch ws)
        {
            
            if(ws!=null&&startWeapons!=null)
            for (int i = 0; i < startWeapons.Count; i++)
            {
                if (ws.noWeaponObject.name != startWeapons[i].gameObject.name)
                {
                       
                        Transform weapon2 = MonoBehaviour.Instantiate(startWeapons[i], ws.transform);
                    ws.AllWeaponList.Add(weapon2);
                    if (weapon2.GetComponent<BaseWeaponScript>().Slot == SlotType.PrimaryWeapon) { ws.PrimarySlot.Add(weapon2); }
                    if (weapon2.GetComponent<BaseWeaponScript>().Slot == SlotType.SecoundaryWeapon) { ws.SecoundarySlot.Add(weapon2); }
                    if (weapon2.GetComponent<BaseWeaponScript>().Slot == SlotType.MelleWeapon) { ws.MelleSlot.Add(weapon2); }
                    if (weapon2.GetComponent<BaseWeaponScript>().Slot == SlotType.ThrowableWeapon) { ws.ThrowableSlot.Add(weapon2); }
                }
            }
            
        }
        
        public override void Update()
        {
            Part_PlayerStateUpdate();//玩家数据计算更新
            Part_DataOutPutToUI();
            
            base.Update();
        }
        private void Part_PlayerStateUpdate()
        {
            if (PlayerSelf != null)
            {
                PlayerSelf.GetComponent<PlayerHealth>()?.SetHelath(m_healthPoint);
                if (lastFrme_m_block != m_blockMeter)
                {
                    lastFrme_m_block = m_blockMeter;
                    recure_m_block = 0;
                }
                else
                {
                    recure_m_block += Time.deltaTime;
                    if (recure_m_block > 1)
                    {
                        m_blockMeter += Time.deltaTime * 10;
                        lastFrme_m_block = m_blockMeter;
                    }
                }
                m_blockMeter = Mathf.Clamp(m_blockMeter, 0, m_maxBlockMeter);
            }
        }
        private void Part_DataOutPutToUI()
        {
            if (TextForHelath != null)
            {
                TextForHelath.text = "Helath:  " + m_healthPoint + "/" + m_maxHealthPoint;
            }
            if (TextForStamina != null)
            {
                TextForStamina.text = "Stamina:  " + m_stamina + "/" + 100;
            }
            if (TextForSoulNum != null)
            {
                TextForSoulNum.text = "Soul:  " + m_soul_ + "/" + m_maxSoul;
            }
            if (TextForStatus != null)
            {
                TextForStatus.text = "Status:  " +"NULL";
            }
        }
        private void Method_LoadData()
        {
            Method_HandleBuffList(new Link_BuffHandleChangeHappen() {who=PlayerSelf  });
            timeStartForEvent = Time.time;
            Method_RemakeEventMessage();
        }

        public void OnLink(Link_SelfDefine link)
        {
            Debug.Log(link.name_);
           
        }

        public void OnLink(Link_BuffHandleChangeHappen link)
        {
            if (link != default)
            {
                //整理BuffUI显示
                {
                    Debug.Log("整理Buff显示");
                    if (GroupForBuff == null || PrefabForBuff == null||link.who!=PlayerSelf) return;
                    
                    domain.StartCoroutine( Method_HandleBuffList(link));
                }
            }
        }

        private IEnumerator Method_HandleBuffList(Link_BuffHandleChangeHappen link)
        {
            yield return null;
            RectTransform buffRoot = GroupForBuff;
            for(int i = 0; i < buffRoot.childCount; i++)
            {
                Ev_PoolManager.Instance.PushToPool(PrefabForBuff.gameObject, buffRoot.GetChild(i).gameObject);
            }
            Debug.Log("回收");
            var safeList = link.who.BuffDomain.buffHosting.buffRTLs;
            
            List<BuffRunTimeLogic> buffs = safeList.valuesNow_.ToList().OrderBy((n)=>n.buffSoInfo.buffGoodOrBad.GetHashCode()).ToList();
            Debug.Log("添加");
            foreach (var i in buffs)
            {
                if (i != null)
                {
                    GameObject g = Ev_PoolManager.Instance.GetInPool(PrefabForBuff.gameObject);
                    g.transform.SetParent(buffRoot);
                    g.transform.localPosition = default;
                    g.transform.localScale = Vector3.one;
                    g.GetComponent<BuffUIInfoSetter>()?.Setup(i);
                }
            }
        }
        public void Method_RemakeEventMessage()
        {
            for(int i = 0; i < GroupForEventMessage.childCount; i++)
            {
                Ev_PoolManager.Instance.PushToPool(PrefabForEventMessage.gameObject,GroupForEventMessage.GetChild(i).gameObject);
            }
            timeStartForEvent = Time.time;
            timeSpanAndMessages = new List<TimeSpanAndMessage>();
        }
        public void ProtactiveInvoke_EventMessage(string key)
        {
            if (GroupForEventMessage != null && PrefabForEventMessage != null)
            {
                var info_ = KeyValueMatchingUtility.DataInfoPointer.PickEventMessageSoInfoByKey(key);
                GameObject g = Ev_PoolManager.Instance.GetInPool(PrefabForEventMessage.gameObject);
                TimeSpan timeSpan = g.GetComponent<EventMessageUIInfoSetter>().Setup(info_);
                if (timeSpan != default)
                {
                    timeSpanAndMessages.Add(new TimeSpanAndMessage() { info = info_,timespan=timeSpan });
                    g.transform.SetParent(GroupForEventMessage);
                    g.transform.SetAsFirstSibling();
                    g.transform.localPosition = default;
                    g.transform.localScale = Vector3.one;
                }
            }

        }
        public struct TimeSpanAndMessage
        {
            public TimeSpan timespan;
            public GameEventMessageDataInfo info;
        }
    }

    [Serializable]
    public class DCFG_PlayerRuneControll : DomainClipForGamecenterManager
    {
        [LabelText("所有持有的符文")]public Queue<RuneDataInfo> allRunes = new Queue<RuneDataInfo>();

        [FoldoutGroup("数据输出"),LabelText("主贴图")] public ImageSpriteSetter MainIcon;
        [FoldoutGroup("数据输出"),LabelText("缓冲列")] public List<ImageSpriteSetter> CacheIcons=new List<ImageSpriteSetter>();
        [FoldoutGroup("数据输出"),LabelText("额外")] public TMP_Text more_Text;

        [FoldoutGroup("Feel"), LabelText("播放使用动画MMF")] public MMF_Player OnStartUse;
        [FoldoutGroup("Feel"), LabelText("播放刷新主符文")] public MMF_Player OnRefreshMain;
        [FoldoutGroup("Feel"), LabelText("播放刷新缓冲符文")] public  MMF_Player OnRefreshCacheRune;
        [FoldoutGroup("Feel"), LabelText("摇一下骰子")] public MMF_Player OnRoll;
        public override void Update()
        {
            base.Update();
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {

                OnRoll?.PlayFeedbacks();
            }
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                //按下了空格
                TryUseOnly();
                OnStartUse?.PlayFeedbacks();
                Debug.Log("按下了空格触发效果");
            }
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                //按下了R
                TryNext();
                OnStartUse?.PlayFeedbacks();
                Debug.Log("按下了R");
            }
        }
        public override string Description_ => "符文功能剪影";
        public override bool OnSubmitHostingAsNormal(DomainBase hosting, bool asVirtual = false)
        {

            if (base.OnSubmitHostingAsNormal(hosting)) {
                domain.Module_RuneControll = this;
                return true;
            }
            return false;
        }
        
        public void ProactiveInvoke_Refresh(bool add=false)
        {
           // Sprite orMain = MainIcon.ui.texture;
            MainIcon.SetImage(null);
            foreach (var i in CacheIcons)
            {
                i.SetImage(null);
            }
            more_Text.text = "";
            if (allRunes != null && allRunes.Count > 0)
            {
                
                MainIcon.SetImage(allRunes.First().icon);
               
                int index = 0;
                foreach (var i in allRunes)
                {
                    if (index == 0) {
                        
                        
                        
                    }
                    else if (index - 1 < CacheIcons.Count)
                    {
                        CacheIcons[index - 1].SetImage(i.icon);
                    }
                    else
                    {
                        break;
                    }
                    index++;
                }
                int more = allRunes.Count - index;
                if (more > 0)
                {
                    more_Text.text = $"+{more}";
                }
            }
            
        }
        [Button("加入")]
        public void TryAdd(RuneDataInfo info)
        {
            if (info != null)
            {
                allRunes.Enqueue(info);
                ProactiveInvoke_Refresh();
                OnRefreshMain?.PlayFeedbacks();
                OnRefreshCacheRune?.PlayFeedbacks();
            }
        }

        [Button("仅用")]
        public RuneDataInfo TryUseOnly()
        {
            if (allRunes != null && allRunes.Count > 0)
            {
                var i = allRunes.Peek();
                ProactiveInvoke_Refresh();
                return i;
            }
            return default;
        }

        [Button("出队")]
        public RuneDataInfo TryNext()
        {
            if (allRunes != null&&allRunes.Count>0)
            {
                 var i= allRunes.Dequeue();
                ProactiveInvoke_Refresh();
                return i;
            }
            return default;
        }
    }

    [Serializable]
    public class DCFG_RoomGenerate : DomainClipForGamecenterManager
    {
        [FoldoutGroup("全局设置"), LabelText("房间长宽整体增益")] public Vector2Int RoomSizeAdder = new Vector2Int(0, 0);
        [FoldoutGroup("全局设置"), LabelText("房间整体缩放倍率")] public float GlobalRoomSizeMutipler = 4;
        [FoldoutGroup("全局设置"), LabelText("门预制件列表")] public List<GameObject> doorPrefabs = new List<GameObject>();
        [FoldoutGroup("全局设置"), LabelText("Boss通关门预制件")] public GameObject completeBossRoom;
        [FoldoutGroup("全局设置"), LabelText("数据打包列表")] public List<RoomGenerateDataPack> packs = new List<RoomGenerateDataPack>();
        [FoldoutGroup("全局设置"), LabelText("应用的数据包索引")] public int applyingPacksIndex = 0;
        public RoomGenerateDataPack currentPack => packs[Mathf.Clamp(applyingPacksIndex,0, packs.Count)];

        [FoldoutGroup("房间设置"), LabelText("房间总长度范围"),SerializeReference] public IPointerForInt_Only TotalNumberOfRooms = new PointerForInt_Random() { int_range = new Vector2Int(30, 100) };
        [FoldoutGroup("房间设置"), LabelText("宝箱房配置")] public RoomTypePreData dataForRoomPref_Chest ;
        [FoldoutGroup("房间设置"), LabelText("精英房配置")] public RoomTypePreData dataForRoomPref_Elite ;
        [FoldoutGroup("房间设置"), LabelText("符文商人房配置")] public RoomTypePreData dataForRoomfPref_Rune_Trader ;
        [FoldoutGroup("房间设置"), LabelText("陷阱机关配置")] public RoomTypePreData dataForRoomPref_Trap;
        [FoldoutGroup("房间设置"), LabelText("空房配置")] public RoomTypePreData dataForRoomPref_Empty; 
        [FoldoutGroup("房间设置"), LabelText("关联房配置")] public RoomTypePreData dataForRoomAttacing ;
        [FoldoutGroup("房间设置"), LabelText("走廊配置")] public RoomTypePreData dataForHallWay;
        [FoldoutGroup("房间设置"), LabelText("Boss配置")] public RoomTypePreData dataForBoss;
        [FoldoutGroup("房间设置"), LabelText("精英房出现范围"),SerializeReference] public IPointerForVector2_Only rangeOfEliteRoomGeneratable = new PointerForVector2_DirectClamp01();
        [FoldoutGroup("默认设置"), LabelText("默认的房间数据")] public RoomGenerateDataInfo defaultInfo;
        [FoldoutGroup("生成引用准备")][LabelText("开始变换")]public Transform StartTrans;
        [FoldoutGroup("生成引用准备")][LabelText("当前变换")] public Transform CurrentTrans;
        
        [FoldoutGroup("生成引用准备")] public CompleteRoomData currentData;
        [FoldoutGroup("生成引用准备")] public GameObject Prefab;
        [FoldoutGroup("生成引用准备")] public GameObject shower;
        [FoldoutGroup("生成引用准备")] public Transform birthParent;
        [FoldoutGroup("生成引用准备")] public DoorPin doorPin;

        [FoldoutGroup("数据生成完成"), LabelText("生成房间数")] public int EndTotalNumberOfRooms = 50;
        [FoldoutGroup("数据生成完成"), LabelText("已经确定的数量")] public int HasDefineNum = 0;

        [FoldoutGroup("数据生成完成"),LabelText("初步生成的数据")] public RoomTypePreData[] PreGenerateRoomWithoutHallWays;
        [FoldoutGroup("数据生成完成"), LabelText("完整的数据")] public List<CompleteRoomData> CompletedGenerateRooms = new List<CompleteRoomData>();

        [FoldoutGroup("房间生成完成"), LabelText("已经实例化的可生成房间")] public List<GeneratorValueSetter> valueSetters = new List<GeneratorValueSetter>();
        [FoldoutGroup("房间生成完成"), LabelText("全部房间")] public List<Transform> allRooms = new List<Transform>();
        WaitForSeconds waitInsDistance = new WaitForSeconds(0.1f);
        WaitForSeconds waitGeneDistance = new WaitForSeconds(0.25f);
        [FoldoutGroup("房间开闭性能优化"),LabelText("房间显示锚点"),SerializeReference]
        public IPointerForVector3_Only RoomActiveAnchor =new PointerForVector3_Transform_Init();
        [FoldoutGroup("房间开闭性能优化"), LabelText("房间显示锚点—简易变换")]public Transform quickTrans;
        [FoldoutGroup("房间开闭性能优化"),LabelText("影响Index数量")]public int ActiveNumRange = 3;
        [FoldoutGroup("房间开闭性能优化"), LabelText("当前判定点")] public int currentJudgeAnchorPointIndex = 0;
        [FoldoutGroup("房间开闭性能优化"), LabelText("刷新间隔")] public float refreshTimeDis = 1;
        [FoldoutGroup("房间开闭性能优化"), LabelText("禁止显示提示方块")] public bool BanInsShowCube = true;
        [FoldoutGroup("房间开闭性能优化"), LabelText("完成生成后删除以往信息")] public bool DeleteOldMessageAfterGenerate = true;
        [FoldoutGroup("房间开闭性能优化"), LabelText("生成时就提前关闭房间")] public bool DisableWhenIns = true;
        private bool hasInit = false;
        private float refreshTimeDisHas = 0;
        
        public override bool OnSubmitHostingAsNormal(DomainBase hosting, bool asVirtual = false)
        {
          
            if (base.OnSubmitHostingAsNormal(hosting))
            {
                domain.Module_RoomGenerate = this;
                foreach (var i in doorPrefabs)
                {
                    Ev_PoolManager.Instance.CreatePool(i, 40);
                }
                return true;
            }
            return false;
        }
        public override void Update()
        {
            base.Update();
            if (Keyboard.current.numpad0Key.wasPressedThisFrame)
            {
                
                Method_CreatePreRoomDataList();//初步生成
                Method_CreateCompleteRoomDataList();//填充完整数据
                Debug.Log(domain);
                Debug.Log(domain.core);
                domain.core.StartCoroutine(StartGenerate());
            }
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                domain.core.StopCoroutine("StartGenerate");
                domain.core.StopCoroutine("InstantiateRoom");
                domain.core.StopCoroutine("ReGenerateAll");
                for (int i = 0; i < birthParent.childCount; i++)
                {
                    MonoBehaviour.Destroy(birthParent.GetChild(i).gameObject);
                }
            }
            
         
            refreshTimeDisHas += Time.deltaTime;
            if(refreshTimeDisHas> refreshTimeDis)
            {
                refreshTimeDisHas = 0;
                Method_TryRefresh();
            }
             IEnumerator StartGenerate()
            {
                yield return domain.core.StartCoroutine(InstantiateRoom());//实例化
                yield return domain.core.StartCoroutine(ReGenerateAll());//重生成
                yield return waitGeneDistance;
                
                InitActiveIndex();
                //销毁
               

            }
        }
        private void InitActiveIndex()
        {
            CompletedGenerateRooms?.Clear();
            valueSetters?.Clear();
            Vector3 vector3 = RoomActiveAnchor?.Pick() ?? default;
            if (quickTrans != null)
            {
                vector3 = quickTrans.position;
            }
            if (allRooms != null && allRooms.Count>0)
            {
                float dis = 200;
                int index = 0;
                foreach(var i in allRooms)
                {
                    float f;
                    if((f=Vector3.Distance(i.position, vector3))<dis){
                        dis = f;
                        currentJudgeAnchorPointIndex = index;
                    }
                    i.transform.localScale *= GlobalRoomSizeMutipler;
                    i.gameObject.SetActive(false);
                    index++;
                }
                
                hasInit = true;
            }
        }
        private void Method_TryRefresh() {
            //
            if (!hasInit || allRooms == null) return;
            float dis = 200;
            Vector3 vector3 = RoomActiveAnchor?.Pick() ?? default;
            if (quickTrans != null)
            {
                vector3 = quickTrans.position;
            }
            for (int i=Mathf.Max(0, currentJudgeAnchorPointIndex- ActiveNumRange);i< Mathf.Min(allRooms.Count, currentJudgeAnchorPointIndex + ActiveNumRange+1); i++)
            {
                float f;
                Transform tt = allRooms[i];
                tt.gameObject.SetActive(false);
                if ((f=Vector3.Distance(vector3, tt.position))<dis)
                {
                    dis = f;
                    currentJudgeAnchorPointIndex = i;
                }
            }
            for (int i = Mathf.Max(0, currentJudgeAnchorPointIndex - ActiveNumRange); i < Mathf.Min(allRooms.Count, currentJudgeAnchorPointIndex + ActiveNumRange+1); i++)
            {
                Transform tt = allRooms[i];
                tt.gameObject.SetActive(true);
            }
            GameCenterManager.Instance.GameCenterArchitecture.SendLink(new Link_DestrolyCollideWall());
        }
        private void Method_CreatePreRoomDataList()
        {
            EndTotalNumberOfRooms = TotalNumberOfRooms?.Pick() ?? 50;
            HasDefineNum = 0;
            PreGenerateRoomWithoutHallWays = new RoomTypePreData[EndTotalNumberOfRooms];//容量已经确定了
            PreGenerateRoomWithoutHallWays[EndTotalNumberOfRooms - 1] = dataForBoss;
            //先搞精英房
            int forElite = dataForRoomPref_Elite.MinGenerateNum;
            Vector2 range = rangeOfEliteRoomGeneratable?.Pick() ?? new Vector2(0.5f,1);
            int startElite = (int)(EndTotalNumberOfRooms * range.x);
            int endElite = (int)(EndTotalNumberOfRooms * range.x);
            bool b= InSome(startElite,endElite, forElite,forElite*3, dataForRoomPref_Elite);
            if (b) Debug.Log("精英房数据准备顺利"); else Debug.LogWarning("精英房数据准备有问题");
            //搞符文商人
            int forRune_Trader = dataForRoomfPref_Rune_Trader.MinGenerateNum;
             b = InSome(3, 100, forRune_Trader, forRune_Trader * 3, dataForRoomfPref_Rune_Trader);
            if (b) Debug.Log("符文商人数据准备顺利"); else Debug.LogWarning("符文商人数据准备有问题");
            //搞宝箱
            int forChest = dataForRoomPref_Chest.MinGenerateNum;
            b = InSome(3, 100, forChest, forChest * 3+3, dataForRoomPref_Chest);
            if (b) Debug.Log("宝箱数据准备顺利"); else Debug.LogWarning("宝箱数据准备有问题");
            //搞陷阱
            int forTrap = dataForRoomPref_Trap.MinGenerateNum;
            b = InSome(3, 100, forTrap, forTrap * 4+5, dataForRoomPref_Trap);
            if (b) Debug.Log("陷阱机关数据准备顺利"); else Debug.LogWarning("陷阱机关准备有问题");
            //搞关联
            int forAttaching = dataForRoomAttacing.MinGenerateNum;
            b = InSome(3, 100, forAttaching, forAttaching * 4 + 5, dataForRoomAttacing);
            if (b) Debug.Log("关联房数据准备顺利"); else Debug.LogWarning("关联房准备有问题");
            //填充空房子
            for(int i=0;i< EndTotalNumberOfRooms; i++)
            {
                TryIn(i,dataForRoomPref_Empty);
            }

        }

        void  Method_CreateCompleteRoomDataList()
        {
            CompletedGenerateRooms = new List<CompleteRoomData>(120);
            for(int i=0;i< EndTotalNumberOfRooms; i++)
            {
                RoomTypePreData predata = PreGenerateRoomWithoutHallWays[i];

                CompletedGenerateRooms.Add(MakeComplete(predata));
                CompletedGenerateRooms.Add(MakeComplete(dataForHallWay,true));
            }
        }
        private CompleteRoomData MakeComplete(RoomTypePreData predata,bool isHallWay=false)
        {
            RoomGenerateDataInfo info = defaultInfo;
            //重整Data
            
            
            if (predata != null && predata.keys != null && predata.keys.Length > 0)
            {
               
                var use = predata.keys[UnityEngine.Random.Range(0, predata.keys.Length)];
                
                info = KeyValueMatchingUtility.DataInfoPointer.PickASoInfoByKey<RoomGenerateDataInfo>(use?.Pick(),pack:currentPack);
               
            }
            CompleteRoomData data = new CompleteRoomData();
            data.useInfo = info;
            data.usePrefab = Prefab;//还没准备
            if (predata != null && predata.usePrefabs != null && predata.usePrefabs.Length > 0)
            {
                data.usePrefab = predata.usePrefabs[UnityEngine.Random.Range(0, predata.usePrefabs.Length)];
            }

            data.Grid = new Vector2Int() { x = info.GridX?.Pick() ?? 4, y = info.GridY?.Pick() ?? 4 };
            data.Grid += RoomSizeAdder;
            data.thenRot =UnityEngine.Random.value> 0.5f?Vector3.right : Vector3.left;

            data.isHallWay = isHallWay;

            return data;
        }
        private bool InSome(int start,int end,int minNum,int maxLoop,RoomTypePreData data)
        {
            start = Mathf.Max(1,start);
            end = Mathf.Min(EndTotalNumberOfRooms-1,end);
            int complete = 0;
            for(int i = 0; i < maxLoop; i++)
            {
                int thisRandom = UnityEngine.Random.Range(start,end);
                if (TryIn(thisRandom,data))
                {
                    complete++;
                    if (complete >= minNum) {
                        HasDefineNum++;
                        return true;
                    }
                }
            }
            return false;
        }
        private bool TryIn(int index,RoomTypePreData data)
        {
            if (data == null) return false;
            if (PreGenerateRoomWithoutHallWays[index]==null)//允入
            {
                PreGenerateRoomWithoutHallWays[index] = data;
                return true;
            }
            return false;
        }
    
        public IEnumerator InstantiateRoom()
        {

            CurrentTrans.position = StartTrans.position;
            CurrentTrans.rotation = StartTrans.rotation;
            float currentDistanceForRot = 0;
            float nextDistanceForRot = 10;
            valueSetters = new List<GeneratorValueSetter>(150);
            allRooms = new List<Transform>(200);

            GeneratorValueSetter LastGene = null;
            for (int i = 0; i < CompletedGenerateRooms.Count-1; i++)
            {
                CompleteRoomData last = CompletedGenerateRooms[i];
                CompleteRoomData timeDis = CompletedGenerateRooms[i+1];
                if(!BanInsShowCube)
                {
                    //展示框专属
                    GameObject insShow = Ev_PoolManager.Instance.GetInPool(shower, CurrentTrans.position);
                    insShow.transform.rotation = CurrentTrans.rotation;
                    insShow.transform.SetParent(birthParent);
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    Color c = UnityEngine.Random.ColorHSV();
                    c.a = 0.25f;
                    block.SetColor("_Color", c);
                    insShow.GetComponent<Renderer>().SetPropertyBlock(block);
                    insShow.transform.localScale = new Vector3(last.Grid.x, 5, last.Grid.y);
                }
                yield return waitInsDistance;
                
                    GameObject ins = Ev_PoolManager.Instance.GetInPool(last.usePrefab, CurrentTrans.position);
                    ins.transform.rotation = Quaternion.identity;
                    ins.transform.SetParent(birthParent);
                    
                    allRooms.Add(ins.transform);
                    if (DisableWhenIns)
                    {
                        if (allRooms.Count > 5)
                        {
                            allRooms[allRooms.Count - 2].gameObject.SetActive(false);
                        }
                    }
                    GeneratorValueSetter setter = ins.GetComponent<GeneratorValueSetter>();
                    if (setter == null)
                    {
                        MaterialPropertyBlock block = new MaterialPropertyBlock();
                        Color c = UnityEngine.Random.ColorHSV();
                        c.a = 0.25f;
                        block.SetColor("_Color", c);
                        ins.GetComponent<Renderer>().SetPropertyBlock(block);
                        ins.transform.localScale = new Vector3(last.Grid.x, 5, last.Grid.y);
                    }
                    else
                    {

                        if (i == 0)
                        {
                        bool onZl = UseZAsDirect(CurrentTrans.forward);
                            (Vector2Int,int, bool) v2_b = setter.Apply(last, onZl);
                            last.Grid = v2_b.Item1;
                        
                        float dis = (float)(onZl ? last.Grid.y : last.Grid.x) / 2*GlobalRoomSizeMutipler+0.25f;
                        CurrentTrans.Translate(0,0,dis);
                        ins.transform.position += CurrentTrans.forward * dis;
                        }
                        setter.Generator.gridX = last.Grid.x;
                        setter.Generator.gridZ = last.Grid.y;
                        
                        bool canGeneRate = setter.CanReGene();
                        if (canGeneRate) valueSetters.Add(setter);
                        Debug.Log(last.Grid.x);
                        
                     //  if (doorPin != null)
                       // {
                         //   DoorPin pin = MonoBehaviour.Instantiate(doorPin, CurrentTrans.position+ CurrentTrans.right*0.75f+CurrentTrans.forward*(-0.5f)* (UseZAsDirect(CurrentTrans.forward) ? last.Grid.y : last.Grid.x), Quaternion.identity, birthParent);
                          //  pin.roomGenerator = setter.Generator;
    //
                          //  if (LastGene != null)
                          //  {
                           //     DoorPin pinLast = MonoBehaviour.Instantiate(doorPin, CurrentTrans.position  + CurrentTrans.forward * (-0.5f) * (UseZAsDirect(CurrentTrans.forward) ? last.Grid.y : last.Grid.x), Quaternion.identity, birthParent);
                             //   pinLast.roomGenerator = LastGene.Generator;
                          //  }
                          //  LastGene = setter;
                        }

                    }
                
               
               

                //旋转
                Vector3 dd = Vector3.forward;

                if (currentDistanceForRot >= nextDistanceForRot&&!last.isHallWay)
                {
                    currentDistanceForRot = 0;
                    nextDistanceForRot *= UnityEngine.Random.Range(1.5f,2.2f);
                    dd = timeDis.thenRot;
                }
                else
                {
                    currentDistanceForRot+= timeDis.Grid.y;
                }
                 
                CurrentTrans.rotation*=Quaternion.LookRotation(dd);
                bool onZ = UseZAsDirect(CurrentTrans.forward);
                //下一个整改

                int nextY = timeDis.Grid.y;
                int nextX = timeDis.Grid.x;
                if (timeDis.usePrefab != null)
                {
                    GeneratorValueSetter vs = timeDis.usePrefab.GetComponent<GeneratorValueSetter>();
                    if (vs != null)
                    {
                        
                        var use= vs.Apply(timeDis, onZ);
                        nextY = onZ? use.grid.y : use.grid.x;
                        nextX= onZ ? use.grid.x : use.grid.y;
                        //Debug.Log("哈哈"+ timeDis.Grid+"哦"+ use_.grid);
                        timeDis.Grid = use.grid;
                    }
                }
                int last_ = (onZ ? last.Grid.x : last.Grid.y) % 2;
                int next_ = (nextX) % 2;
               // Debug.Log("..." +i+"---"+ last_ + next_);
                float xOffset = 0;
                if (last_ != next_) xOffset = 1f;
                //  if ((UseZAsDirect(CurrentTrans.forward) ? last.Grid.x : last.Grid.y) % 2 == 0) {  CurrentTrans.right * 0.5f; Debug.Log("偶数修正"); }

                if (doorPrefabs != null&&doorPrefabs.Count>0)
                {
                    int index = last.useInfo.getIndexOfDoor.Pick();
                    index = Mathf.Clamp(index, 0, doorPrefabs.Count - 1);
                    GameObject gg = doorPrefabs[index];
                    if(i==CompletedGenerateRooms.Count - 2)
                    {
                        gg = completeBossRoom;
                    }
                    GameObject door = Ev_PoolManager.Instance.GetInPool(gg, CurrentTrans.position +   (   CurrentTrans.right * -0.5f*((onZ? last.Grid.x: last.Grid.y)%2)     +    CurrentTrans.forward * ((0.5f) * (onZ ? last.Grid.y : last.Grid.x)+0.125f)    ));
                    door.transform.rotation = Quaternion.LookRotation(CurrentTrans.forward);
                    door.transform.localScale *= 0.25f;
                    door.transform.SetParent(ins.transform);
                }
                

                CurrentTrans.Translate(new Vector3(xOffset, 0, (onZ? last.Grid.y:last.Grid.x)+ nextY) /2* GlobalRoomSizeMutipler);
                yield return waitInsDistance;
            }
            yield return null;
        }
        public IEnumerator ReGenerateAll()
        {
            Debug.Log("重新生成");
            for(int i = 0; i < valueSetters.Count; i++)
            {
                yield return waitGeneDistance;
                valueSetters[i].Generator.id = i;
                valueSetters[i].gameObject.SetActive(true);
                valueSetters[i].Generator.GenerateRoom(i);
                
                yield return waitGeneDistance;
                if(i>8) valueSetters[i].gameObject.SetActive(false);
            }
        }
        
        public bool UseZAsDirect(Vector3 direc)
        {
            Vector3 vv = direc;
            if (Mathf.Abs( vv.z) > Mathf.Abs(vv.x)) return true;
            else return false;
        }
        [Serializable]
        public class CompleteRoomData
        {
            public Vector2Int Grid = new Vector2Int(6,4);
            public Vector3 thenRot = Vector3.right;
            [LabelText("使用的数据")] public RoomGenerateDataInfo useInfo;
            [LabelText("可用预制件")] public GameObject usePrefab;
            public bool isHallWay = false;
        }
        [Serializable]
        public class RoomTypePreData
        {
            public string name = "房间";
            [FoldoutGroup("详情配置")][LabelText("最小生成数量")] public int MinGenerateNum = 1;
            [FoldoutGroup("详情配置")]
            [LabelText("用于加载的数据键列表")]
            public PSD_RoomGene[] keys; 
            [FoldoutGroup("详情配置")][LabelText("使用的数据(目前没用了)")] public RoomGenerateDataInfo[] useInfos;
            [FoldoutGroup("详情配置")][LabelText("可用预制件")] public GameObject[] usePrefabs;
        }
        [Serializable,TypeRegistryItem("房间数据专属Key")]
        public class PSD_RoomGene : PointerStringDataKey
        {
            public override string[] Keys => KeyValueMatchingUtility.KeyPointer.PickPackAllKeys(GameCenterManager.Instance.GetComponent<DomainForGameCenterManager>().GetModule<DCFG_RoomGenerate>().currentPack);
        }
    }*/
}
