/*
 * Author: Huai
 * Create: 2021/2/9
 */

using System.IO;
using UnityEditor;

namespace Common
{
    /// <summary>
    /// Resources文件管理工具类，管理Resources路径下的文件
    /// </summary>
    public class ResourceTool : Editor
    {
        /// <summary>
        /// 读取Resources文件夹下的所有prefab，并生成资源名以及对应路径的配置表，保存到StreamingAssets目录下
        /// </summary>
        [MenuItem("Tool/Resource/Generate Resource Config")]
        public static void GenerateResourceConfig()
        {
            string[] resFiles = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });

            for (int i = 0; i < resFiles.Length; i++)
            {
                resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
                string fileName = Path.GetFileNameWithoutExtension(resFiles[i]);
                string filePath = resFiles[i].Replace("Assets/Resources/", string.Empty).Replace(".prefab", string.Empty);
                resFiles[i] = fileName + "=" + filePath;
            }

            File.WriteAllLines("Assets/StreamingAssets/ResourceConfig.txt", resFiles);
            AssetDatabase.Refresh();
        }
    }
}
