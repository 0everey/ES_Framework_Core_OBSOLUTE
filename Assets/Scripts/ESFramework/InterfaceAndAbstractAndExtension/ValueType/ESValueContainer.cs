using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace ES
{
    //这里准备了一系列高性能双层数据容器-与模块无关哈
    #region 抽象定义
    [Serializable,TypeRegistryItem("ES容器_字典包队列")]
    public class ESValueContainer_DicOverQueue<Usekey,T>
    {
        [LabelText("最多队列数量"),LabelWidth(150)]public int QueueNum = 5;
        [LabelText("最多单个队列元素数量"), LabelWidth(150)] public int ElementNum = 8;
        protected Queue<Usekey> KeepToClear = new Queue<Usekey>();
        protected Dictionary<Usekey, Queue<T>> AllQueues = new Dictionary<Usekey, Queue<T>>();
        public Queue<T> GetOrCreateQueue(Usekey key)
        {
            if (AllQueues.ContainsKey(key)) return AllQueues[key];
            Queue<T> q = null;
            KeepToClear.Enqueue(key);
            if(AllQueues.Keys.Count> QueueNum)
            {
                AllQueues.Remove(KeepToClear.Dequeue());
            }
            AllQueues.Add(key, q= new Queue<T>());
            return q;
        }
        public void AddToQueue(Usekey key,T t)
        {
            if (AllQueues.ContainsKey(key))
            {
                var qu = AllQueues[key];
                if (qu.Count > ElementNum) qu.Dequeue();
                if (!qu.Contains(t)) qu.Enqueue(t);
            }
            else
            {
                Debug.Log("到指定实体缓冲" +key+t);
                GetOrCreateQueue(key).Enqueue(t);
            }
        }
        public void AddToQueue(Usekey key,IEnumerable<T> values)
        {
            if (AllQueues.ContainsKey(key))
            {
                var qu = AllQueues[key];
                
                foreach (var i in values)
                {
                    if (i == null) continue;
                    if (qu.Count > ElementNum) qu.Dequeue();
                    if(!qu.Contains(i))qu.Enqueue(i);
                }
                
            }
            else
            {
                var qu= GetOrCreateQueue(key);
                foreach (var i in values)
                {
                    if (qu.Count > ElementNum) qu.Dequeue();
                    if (i == null) continue;
                    if (!qu.Contains(i)) qu.Enqueue(i);
                }
            }
        }
        public T Dequeue(Usekey key)
        {
            if (AllQueues.ContainsKey(key))
            {
            AllQueues[key].Dequeue();
            }
            return default(T);
        }
        public Queue<T> DequeueAll(Usekey key)
        {
            if (AllQueues.ContainsKey(key))
            {
                var back = AllQueues[key];
                AllQueues[key] = new Queue<T>();
                return back;
            }
            return new Queue<T>();
        }
        public List<T> PeekAll(Usekey key)
        {
            if (AllQueues.ContainsKey(key))
            {
                return AllQueues[key].ToList();
            }
            return new List<T>();
        }
        [Button("输出内容")]
        public void DebugIt()
        {
            Debug.Log(ToString());
        }
        public override string ToString()
        {
            string s = "这是一个双层数据容器，字典包裹队列";
            s += "键类型是" + typeof(Usekey) + "值类型" + typeof(T)+"\n";

            foreach(var i in AllQueues)
            {
                s += "队列" + i.Key + "包括::\n";
                foreach(var ii in i.Value)
                {
                    s +="<"+ ii + ">,  ";
                }
            }
            return s;

        }
    }
    [Serializable, TypeRegistryItem("ES容器_字典包队列_字符串键")]
    public class ESValueContainer_DicOverQueue_StringKey<T> : ESValueContainer_DicOverQueue<string, T>
    {

    }
    #endregion

    #region 常用类
    [Serializable, TypeRegistryItem("ES容器_字典包队列_字符串键实体队列")]
    public class ESValueContainer_DicOverQueue_StringKeyEntity : ESValueContainer_DicOverQueue_StringKey<Entity>
    {

    }
    [Serializable, TypeRegistryItem("ES容器_字典包队列_字符串键V3队列")]
    public class ESValueContainer_DicOverQueue_StringKeyVector3 : ESValueContainer_DicOverQueue_StringKey<Vector3>
    {

    }

    #endregion
}