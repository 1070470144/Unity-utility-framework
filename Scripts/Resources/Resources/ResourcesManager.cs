
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MFramework
{
    ///<summary>
    ///资源加载管理类
    ///<summary>
    public class ResourcesManager : SingletonSuperMono<ResourcesManager>
    {
        //负责储存资源的名字和路径映射
        private static Dictionary<string, string> configMap;
        //缓存已经加载的资源
        private static Dictionary<string, object> cacheDic;

        private void Awake()
        {
            //加载文件
            string fileContent = ConfigReader.GetConfigFile("ConfigMap.txt");

            configMap = new Dictionary<string, string>();

            cacheDic = new Dictionary<string, object>();

            //解析文件（string ----> prefabConfigMap)
            ConfigReader.Reader(fileContent, BuildMap);
        }

        /// <summary>
        /// 负责处理解析每行字符串的功能
        /// </summary>
        /// <param name="line">每行字符串</param>
        private void BuildMap(string line)
        {
            string[] keyValue = line.Split('=');
            configMap.Add(keyValue[0], keyValue[1]);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">加载资源类型</typeparam>
        /// <param name="resourceName">资源名称</param>
        /// <returns></returns>
        public T Load<T>(string resourceName) where T : Object
        {
            //从字典中获取路径加载预制件
            if (configMap.ContainsKey(resourceName))
            {
                string resourceKey = $"{resourceName}_{typeof(T)}";
                if (!cacheDic.ContainsKey(resourceKey))
                {
                    T res = Resources.Load<T>(configMap[resourceName]);
                    cacheDic.Add(resourceKey, res);
                }
                return cacheDic[resourceKey] as T;

            }
            else return default(T);
        }

        /// <summary>
        /// 加载并初始化预制体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public T LoadAndIns<T>(string resourceName) where T : Object
        {
            T res =  Load<T>(resourceName);

            return Instantiate(res);
        }


        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceName"></param>
        /// <param name="action"></param>
        public void LoadAsync<T>(string resourceName, UnityAction<T> action = null) where T : Object
        {
            StartCoroutine(LoadAsyncCore<T>(resourceName, action));
        }
        private IEnumerator LoadAsyncCore<T>(string resourceName, UnityAction<T> action) where T : Object
        {
            if (configMap.ContainsKey(resourceName))
            {
                string resourceKey = $"{resourceName}_{typeof(T)}";
                if (!cacheDic.ContainsKey(resourceKey))
                {
                    ResourceRequest request = Resources.LoadAsync<T>(configMap[resourceName]);
                    yield return request;
                    //由于采用异步协程，需要二重判断
                    if (!cacheDic.ContainsKey(resourceKey))
                        cacheDic.Add(resourceKey, request.asset as T);
                }
                action?.Invoke(cacheDic[resourceKey] as T);

            }
            else action?.Invoke(default(T));
        }
    } 
}

