using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public static class ExtensionForCompoent 
    {
        public static T EX_GetCompoentInParentExcludeSelf<T>(this Component self,bool includeInactive=true) where T : Component
        {
            if (self == null || self.transform.parent == null) return null;
            return self.transform.parent.GetComponentInParent<T>(includeInactive);
        }
        public static List<T> EX_GetCompoentsInChildExcludeSelf<T>(this Component self, bool includeInactive = true) where T : Component
        {
            if (self == null) return new List<T>();
            List<T> result = new List<T>(self.transform.childCount);
            for(int i = 0; i < self.transform.childCount; i++)
            {
                result.AddRange(self.transform.GetChild(i).GetComponentsInChildren<T>());
            }
            return result;
        }
        // 31. 重置Transform的位置、旋转和缩放
        public static void EX_Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        // 32. 重置局部位置、旋转和缩放
        public static void EX_ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        // 33. 设置X位置
        public static void EX_SetPositionX(this Transform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        // 34. 设置Y位置
        public static void EX_SetPositionY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        // 35. 设置Z位置
        public static void EX_SetPositionZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        // 36. 设置局部X位置
        public static void EX_SetLocalPositionX(this Transform transform, float x)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        // 37. 设置局部Y位置
        public static void EX_SetLocalPositionY(this Transform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        // 38. 设置局部Z位置
        public static void EX_SetLocalPositionZ(this Transform transform, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }

        // 39. 获取所有子物体
        public static Transform[] EX_GetChildrensOneLayer(this Transform transform)
        {
            Transform[] children = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                children[i] = transform.GetChild(i);
            return children;
        }

        // 40. 销毁所有子物体
        public static void EX_DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
}

