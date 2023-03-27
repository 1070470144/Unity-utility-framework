using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace MGPMFramework
{
    public class GenrateResConfig : Editor
    {
        //映射文件保存位置
        private static string configPath = "Assets/StreamingAssets/ConfigMap.txt";
        //在Unity编辑器界面中 设置一组下拉选项，指定选项路径，点击后就会触发此函数
        [MenuItem("Tools/Resources/Generate ResConfig File")]
        public static void Generate()
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            File.Delete(configPath);
            //TODO: 将更多需要保存的资源类型和筛选器加入到此字典中
            //资源类型可以查阅Unity API，根据类型其后缀可能多种多样根据实际资源决定
            dic.Add("prefab", new List<string>() { "prefab" });
            dic.Add("audioclip", new List<string>() { "mp3", "mp4" });
            dic.Add("texture", new List<string>() { "png", "jpg", "bmp" });
            foreach (var item in dic)
            {
                string[] mapArr = getMapping(item.Key, item.Value);
                //3.写入文件
                if (!Directory.Exists("Assets/StreamingAssets"))
                {
                    Directory.CreateDirectory("Assets/StreamingAssets");
                }
                File.AppendAllLines(configPath, mapArr);
            }
            //刷新
            AssetDatabase.Refresh();

        }

        private static string[] getMapping(string type, List<string> suffixes)
        {
            //生成资源配置文件
            //1.查找Resources目录下所有预制件的完整路径
            //返回值为GUID 资源编号 ， 参数1 指明类型，参数2 在哪些路径下查找
            string[] resFiles = AssetDatabase.FindAssets($"t:{type}", new string[] { "Assets/Resources" });
            for (int i = 0; i < resFiles.Length; i++)
            {
                resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
                //2.生成对应关系  名称=路径
                string fileName = Path.GetFileNameWithoutExtension(resFiles[i]);
                string filePath = resFiles[i].Replace("Assets/Resources/", string.Empty);
                foreach (string filter in suffixes)
                {
                    filePath = filePath.Replace("." + filter, string.Empty);
                }
                resFiles[i] = fileName + "=" + filePath;
            }
            return resFiles;
        }
    }


}
