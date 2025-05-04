using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEditorInternal.VersionControl.ListControl;

public class HostAndMonoAndListTest : ESHostingMono_BaseESModule
{
    public float f = 2;
    ESHostingAndModule_WithDelegate 释放技能;
    public float startSkillTime = 0;
    private void Start()
    {
       
       /* ESModule_Move eSModule_ = new ESModule_Move() { g=gameObject };
        eSModule_.TrySubmitHosting(this,true);

       
        ESModule_WithDelegate dele = new ESModule_WithDelegate().
            WithEnable((self) => { f = 2; }).
            WithUpdate((self) => { f -= Time.deltaTime;if (f < 0) self.TryWithDrawHostingVirtual(); }).
            WithDisable((self) => { f = 0; })
            ; 
        dele.TrySubmitHosting(this, true);*/

        释放技能 = new ESHostingAndModule_WithDelegate().
      WithEnable((self) => { Debug.Log("开始技能"); startSkillTime = 0; }).
      WithUpdate((self) => { Debug.Log("进行技能"); startSkillTime += Time.deltaTime; }).
      WithDisable((self) => { Debug.Log("退出技能"); });

        ESHostingAndModule_WithDelegate 技能组成1_移动 = new ESHostingAndModule_WithDelegate().
    WithEnable((self) => { transform.position = default; }).
    WithUpdate((self) => { if (startSkillTime > 0.8f&&startSkillTime<1.2f) transform.position += Time.deltaTime * Vector3.up;  }).
    WithDisable((self) => {   });

        bool hasHurt = false;
        ESHostingAndModule_WithDelegate 技能组成1_伤害 = new ESHostingAndModule_WithDelegate().
    WithEnable((self) => { hasHurt = false; }).
    WithUpdate((self) => { if (startSkillTime > 1.5f && !hasHurt) { Debug.Log("测试攻击");hasHurt = true; } }).
    WithDisable((self) => {  });

        释放技能.TrySubmitHosting(this,true);
        技能组成1_移动.TrySubmitHosting(释放技能,true);
        技能组成1_伤害.TrySubmitHosting(释放技能,true);
    }




  
    protected override void Update()
    {

        base.Update();
        if(Input.GetKeyDown(KeyCode.P))
        {
           
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
           
        }
        /* timeDis -= Time.deltaTime;
         if (timeDis > 0) return;

         timeDis = 0.2f;
         last++;*/

    }


    [Serializable,TypeRegistryItem("移动的模块")]
    public class ESModule_Move : BaseESModule
    {
        public GameObject g;
        protected override void OnEnable()
        {
            if (g == null) TryWithDrawHostingVirtual();
            g.transform.position = default;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            if (g == null) TryWithDrawHostingVirtual();
            g.transform.position = Vector3.zero;
            base.OnDisable();
        }
        protected override void Update()
        {
            if (g == null) TryWithDrawHostingVirtual();
            g.transform.position += Vector3.up * Time.deltaTime;
            base.Update();
        }
    }

































        /* public int testNum = 4000;
         public ESModule_WithDelegate aa;
         public ESModule_WithDelegate bb;
         public override IEnumerable<BaseESModule> USE_Modules => null;
         private void Start()
         {
             for (int i = 0; i < testNum; i++)
             {

                 GameObject g = new GameObject();
                 g.SetActive(true);
                 var dele = new ESModule_WithDelegate()
                     .WithEnable((e) => { g.transform.position = Vector3.zero; })
                      .WithDisable((e) => { g.transform.position = Vector3.one; })
                         .WithUpdate((e) => { g.transform.position += Vector3.left; });
                 if (i == 0) aa = dele;
                 if (i == 2) bb = dele;
                 dele.TrySubmitHosting(this, true);

             }
         }
         int last = 0;
         float timeDis = 0.2f;
         protected override void Update()
         {
             base.Update();
             timeDis -= Time.deltaTime;
             if (timeDis > 0) return;

             timeDis = 0.2f;
             last++;
             last %= testNum;
             if (last % 2 == 0)
             {
                 aa.TryEnableSelf();
                 bb.TryDisableSelf();
             }
             else
             {
                 bb.TryEnableSelf();
                 aa.TryDisableSelf();
             }

         }*/
        /*
        public List<GameObject> gs = new List<GameObject>();
        public List<bool> LastStates = new List<bool>();
        public List<bool> NewStates = new List<bool>();
        public int testNum = 4000;
        Action<GameObject> move = (who)=>{ who.transform.position += Vector3.left; };
        Action<GameObject> enable= (who) => { who.transform.position = Vector3.zero; };
        Action<GameObject> disable= (who) => { who.transform.position = Vector3.one; };
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < testNum; i++)
            {
                GameObject g = new GameObject();
                gs.Add(g);
                LastStates.Add(true);
                NewStates.Add(true);
                g.SetActive(true);
                //g.AddComponent<Run>();
            }
        }
        int last = 0;
        float timeDis = 0.2f;

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < gs.Count; i++)
            {
                if (LastStates[i])
                {
                    move?.Invoke(gs[i]);
                }
            }


            timeDis -= Time.deltaTime;
            if (timeDis > 0) return;

            timeDis = 0.2f;
            last++;
            last %= testNum;
            NewStates[last] = true;
            NewStates[(last + 1) % testNum] = false;
            if (last % 2 == 0)
            {
                NewStates[5] = true;
                NewStates[4] = false;
                enable?.Invoke(gs[5]);
                disable?.Invoke(gs[4]);
            }
            else
            {
                NewStates[4] = true;
                NewStates[5] = false;
                enable?.Invoke(gs[4]);
                disable?.Invoke(gs[5]);
            }
        }*/
    
}
/*  public List<GameObject> gs = new List<GameObject>();
   public List<bool> LastStates = new List<bool>();
   public List<bool> NewStates = new List<bool>();
   public int testNum = 1000;
   // Start is called before the first frame update
   void Start()
   {
       for(int i = 0; i < testNum; i++)
       {
           GameObject g = new GameObject();
           gs.Add(g);
           LastStates.Add(true);
           NewStates.Add(true);
           g.SetActive(true);
          //g.AddComponent<Run>();
       }
   }
   int last = 0;
   float timeDis = 0.2f;

   // Update is called once per frame
   void Update()
   {
       for(int i= 0;i<gs.Count;i++)
       {
           if (LastStates[i])
           {
               gs[i].transform.position += Vector3.left;
           }
       }

       for(int i = 0; i < NewStates.Count; i++)
       {
           if (NewStates[i] != LastStates[i])
           {
               LastStates[i] = NewStates[i];
               if (LastStates[i])
               {
                   gs[i].transform.position = default;
               }
               else
               {
                   gs[i].transform.position = Vector3.zero;
               }
           }
       }
       timeDis -= Time.deltaTime;
       if (timeDis > 0) return;

       timeDis = 0.2f;
       last++;
       last %= testNum;
       NewStates[last]=true;
       NewStates[(last+1) % testNum]=false;

   }*/