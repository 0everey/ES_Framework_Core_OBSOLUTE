using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ESDebugMaster : MonoBehaviour
{
    public TMP_Text TMP_Text;
   

    
    void Update()
    {
        if (TMP_Text != null)
        {
            TMP_Text.text = "frame" + (int)(1 / Time.deltaTime);
        }
    }
}
