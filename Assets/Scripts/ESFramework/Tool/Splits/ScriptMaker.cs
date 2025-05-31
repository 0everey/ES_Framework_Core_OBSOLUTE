using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ES
{

    public class ESTool_ScriptMaker : SingletonAsNormalClass<ESTool_ScriptMaker>
    {
        private string className = "NewBehaviour";
        private string savePath = "Assets/Scripts/";


        public void CreateScript(string Folderpath, string className, string parent = ":MonoBehaviour", string Attribute = "", string nameSpace = "ES")
        {
            if (string.IsNullOrEmpty(className))
            {
                Debug.LogError("类名不可为空!");
                return;
            }

            string fullPath = Path.Combine(Folderpath, className + ".cs");

            // 基础脚本模板
            string scriptContent =
    $@"using UnityEngine;
namespace {nameSpace}{{   
    {Attribute}
    public class {className} {parent}
    {{
    
    }}
}}
";

            // 创建目录（如果不存在）
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // 写入文件
            File.WriteAllText(fullPath, scriptContent);

            Debug.Log($"创建脚本: {fullPath}");
        }

    }
}

