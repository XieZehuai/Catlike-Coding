using System;
using System.IO;
using System.Linq;
using UnityEngine;

/*
 * 在新建脚本时，修改脚本文件模板头部的注释内容
 */
namespace Common
{
    public class AddFileHeadComment : UnityEditor.AssetModificationProcessor
    {
        // 在创建新资源时调用
        public static void OnWillCreateAsset(string newFileMeta)
        {
            string newFilePath = newFileMeta.Replace(".meta", "");
            string fileExt = Path.GetExtension(newFilePath);
            if (fileExt != ".cs")
            {
                return;
            }

            //注意，Application.dataPath会根据使用平台不同而不同
            string realPath = Application.dataPath.Replace("Assets", "") + newFilePath;
            string scriptContent = File.ReadAllText(realPath);

            string[] names = newFilePath.Split('/');

            for (int i = 0; i < names.Length; i++)
            {
                names[i] = names[i].Replace(" ", "");
            }

            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Equals("Scripts"))
                {
                    var temp = names.Select((str, index) => new { str, index }).Where(a => a.index >= i
                        && a.index < names.Length - 1).OrderBy(b => b.index);
                    string name = string.Join(".", temp.Select(s => s.str));
                    scriptContent = scriptContent.Replace("#NAMESPACE#", name);
                    break;
                }
            }

            scriptContent = scriptContent.Replace("#CREATEDATE#", DateTime.Now.ToString());

            File.WriteAllText(realPath, scriptContent);
        }
    }
}