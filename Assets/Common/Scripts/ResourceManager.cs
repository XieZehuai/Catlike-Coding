/*
 * Author: Huai
 * Create: 2021/2/9
 */

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Common
{
	/// <summary>
	/// 资源管理器
	/// </summary>
	public class ResourceManager
	{
        /// <summary>
        /// StreamingAssets文件夹在各个平台下的实际路径
        /// </summary>
        public static string StreamingAssetsPath { get; private set; }

		private static Dictionary<string, string> configMap;

        static ResourceManager()
		{
            StreamingAssetsPath = GetStreamingAssetsPath();

            string url = StreamingAssetsPath + "ResourceConfig.txt";
            string fileContent = GetFileContent(url);
            BuildConfigMap(fileContent);
		}

        /// <summary>
        /// 根据传入的资源名称，加载资源（必须是Resources路径下的资源）
        /// </summary>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <param name="prefabName">资源名称</param>
        /// <returns>加载成功返回资源文件，失败返回null</returns>
        public static T Load<T>(string prefabName) where T : UnityEngine.Object
        {
            if (!configMap.ContainsKey(prefabName))
            {
                Debug.Log("找不到资源文件对应的路径 " + prefabName);
                return null;
            }

            string path = configMap[prefabName];
            return Resources.Load<T>(path);
        }

        /// <summary>
        /// 根据传入的url路径，获取文本内容
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <returns>资源内容</returns>
        public static string GetFileContent(string url)
        {
            WWW www = new WWW(url);

            while (true)
            {
                if (www.isDone)
                {
                    return www.text;
                }
            }
        }

        /// <summary>
        /// 生成 文件名-路径 的配置表
        /// </summary>
        /// <param name="content">配置表内容</param>
        private static void BuildConfigMap(string content)
        {
            configMap = new Dictionary<string, string>();

            using (StringReader reader = new StringReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split('=');
                    configMap.Add(values[0], values[1]);
                }
            }
        }

        /// <summary>
        /// 获取StreamingAssets文件夹在各个平台下的路径
        /// </summary>
        /// <returns></returns>
        private static string GetStreamingAssetsPath()
		{
            string path;

#if UNITY_EDITOR || UNITY_STANDALONE
            path = "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
            path = "file://" + Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
            path = "jar:file://" + Application.dataPath + "/!/assets/";
#endif

            return path;
		}
	}
}
