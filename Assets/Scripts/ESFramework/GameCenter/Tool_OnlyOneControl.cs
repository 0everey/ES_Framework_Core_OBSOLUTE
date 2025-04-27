using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    
    [DefaultExecutionOrder(-3)]
    public class Tool_OnlyOneControl : MonoBehaviour
    {
        public static Tool_OnlyOneControl onlyOne;
        private void Awake()
        {
            if (onlyOne == null)
            {
                onlyOne = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);

            }
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
