using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace MGPMFramework
{
    public class GenrateResConfig : Editor
    {
        //ӳ���ļ�����λ��
        private static string configPath = "Assets/StreamingAssets/ConfigMap.txt";
        //��Unity�༭�������� ����һ������ѡ�ָ��ѡ��·���������ͻᴥ���˺���
        [MenuItem("Tools/Resources/Generate ResConfig File")]
        public static void Generate()
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            File.Delete(configPath);
            //TODO: ��������Ҫ�������Դ���ͺ�ɸѡ�����뵽���ֵ���
            //��Դ���Ϳ��Բ���Unity API�������������׺���ܶ��ֶ�������ʵ����Դ����
            dic.Add("prefab", new List<string>() { "prefab" });
            dic.Add("audioclip", new List<string>() { "mp3", "mp4" });
            dic.Add("texture", new List<string>() { "png", "jpg", "bmp" });
            foreach (var item in dic)
            {
                string[] mapArr = getMapping(item.Key, item.Value);
                //3.д���ļ�
                if (!Directory.Exists("Assets/StreamingAssets"))
                {
                    Directory.CreateDirectory("Assets/StreamingAssets");
                }
                File.AppendAllLines(configPath, mapArr);
            }
            //ˢ��
            AssetDatabase.Refresh();

        }

        private static string[] getMapping(string type, List<string> suffixes)
        {
            //������Դ�����ļ�
            //1.����ResourcesĿ¼������Ԥ�Ƽ�������·��
            //����ֵΪGUID ��Դ��� �� ����1 ָ�����ͣ�����2 ����Щ·���²���
            string[] resFiles = AssetDatabase.FindAssets($"t:{type}", new string[] { "Assets/Resources" });
            for (int i = 0; i < resFiles.Length; i++)
            {
                resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
                //2.���ɶ�Ӧ��ϵ  ����=·��
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
