using ES;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES {
    [Serializable]
    public struct ESTag 
    {
        [LabelText("内容"),ValueDropdown("GetTags")]public string content;
        [LabelText("获得时间")] public float getTime;
        [LabelText("程度/积累")] public int times;
        public override bool Equals(object obj)
        {
            if(obj is ESTag tag)
            {
                return content == tag.content;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return content.GetHashCode();
        }
        public List<string> GetTags()
        {
            return ESEditorRuntimePartMaster.ESTags;
        }
    }
    [Serializable]
    public class ESTagCollection
    {
        [InlineButton("Sort_", "排序整理"), ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "content"), OdinSerialize]
        [ LabelText("超级标签")] public List<ESTag> Tags = new List<ESTag>();
        private void Sort_()
        {
            Tags.Sort((left, right) => string.CompareOrdinal(left.content, right.content)); ;
        }
        public bool Contains(ESTag eSTag)
        {
            foreach(var i in Tags)
            {
                if (i.Equals(eSTag)) return true;
            }
            return false;
        }
        
    }
}
