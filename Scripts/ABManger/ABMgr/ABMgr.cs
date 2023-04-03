using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ABMgr : TMonoSingleton<ABMgr>
{
    private string m_ResPath = Application.streamingAssetsPath + "/ABRes/";

    private Dictionary<string, AssetBundle> m_AllABRes = new Dictionary<string, AssetBundle>();

    private AssetBundle m_MainAB;
    private AssetBundleManifest m_MainABManifest;

    private string m_MainABName = "ABRes";


    /// <summary>
    /// 加载AB包
    /// </summary>
    /// <param name="abName"></param>
    public void LoadAB(string abName)
    {
        //加载主包和相关信息
        if (!m_AllABRes.ContainsKey(m_MainABName))
        {
            m_MainAB = AssetBundle.LoadFromFile(m_ResPath + m_MainABName);
            m_MainABManifest = m_MainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        AssetBundle bundle = null;
        string[] allDepend = m_MainABManifest.GetAllDependencies(abName);

        //加载abName的依赖包
        for (int i = 0; i < allDepend.Length; i++)
        {
            if (!m_AllABRes.ContainsKey(allDepend[i]))
            {
                bundle = AssetBundle.LoadFromFile(m_ResPath + allDepend[i]);
                m_AllABRes.Add(allDepend[i], bundle);
            }
        }

        //加载abName
        if (!m_AllABRes.ContainsKey(abName))
        {
            bundle = AssetBundle.LoadFromFile(m_ResPath + abName);
            m_AllABRes.Add(abName, bundle);
        }
    }

    public Object[] LoadAllRes(string abName, string resName)
    {
        if (!m_AllABRes.ContainsKey(abName)) LoadAB(abName);

        return m_AllABRes[abName].LoadAllAssets();
    }

    public T[] LoadAllRes<T>(string abName, string resName) where T : Object
    {
        if (!m_AllABRes.ContainsKey(abName)) LoadAB(abName);

        return m_AllABRes[abName].LoadAllAssets<T>();
    }


    public Object LoadRes(string abName, string resName)
    {
        if (!m_AllABRes.ContainsKey(abName)) LoadAB(abName);

        return m_AllABRes[abName].LoadAsset(resName);
    }


    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        if (!m_AllABRes.ContainsKey(abName)) LoadAB(abName);

        return m_AllABRes[abName].LoadAsset<T>(resName);
    }

    public void LoadAllResAsync(string abName, string resName, UnityAction<Object[]> callback)
    {
        IEnumeratorLoadAllRes(abName, callback);
    }

    public void LoadAllResAsync<T>(string abName, string resName, UnityAction<T[]> callback) where T : Object
    {
        IEnumeratorLoadAllRes<T>(abName, callback);
    }

    public void LoadResAsync(string abName, string resName, UnityAction<Object> callback)
    {
        IEnumeratorLoadRes(abName, resName, callback);
    }

    public void LoadResAsync<T>(string abName ,string resName, UnityAction<T> callback) where T : Object
    {
        IEnumeratorLoadRes<T>(abName, resName, callback);
    }

    private IEnumerator IEnumeratorLoadAllRes<T>(string abName, UnityAction<T[]> callback) where T : Object
    {
        if (!m_AllABRes.ContainsKey(abName)) LoadAB(abName);

        AssetBundleRequest asset = m_AllABRes[abName].LoadAllAssetsAsync();

        yield return asset;

        callback((T[])asset.allAssets);
    }

    private IEnumerator IEnumeratorLoadAllRes(string abName, UnityAction<Object[]> callback)
    {
        if (!m_AllABRes.ContainsKey(abName)) LoadAB(abName);

        AssetBundleRequest asset = m_AllABRes[abName].LoadAllAssetsAsync();

        yield return asset;

        callback(asset.allAssets);
    }

    private IEnumerator IEnumeratorLoadRes(string abName, string resName, UnityAction<Object> callback)
    {
        if (!m_AllABRes.ContainsKey(abName)) LoadAB(abName);

        AssetBundleRequest asset = m_AllABRes[abName].LoadAssetAsync(resName);

        yield return asset;

        callback(asset.asset);
    }

    private IEnumerator IEnumeratorLoadRes<T>(string abName, string resName, UnityAction<T> callback) where T : Object
    {
        if (!m_AllABRes.ContainsKey(abName)) LoadAB(abName);

        AssetBundleRequest asset = m_AllABRes[abName].LoadAssetAsync(resName);

        yield return asset;

        callback((T)asset.asset);
    }

    public void UnloadAB(string abName,bool destroyAllPre = false)
    {
        if (m_AllABRes.ContainsKey(abName))
        {
            m_AllABRes[abName].Unload(destroyAllPre);
            m_AllABRes.Remove(abName);
        }
    }

    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        m_AllABRes.Clear();
        m_MainAB = null;
        m_MainABManifest = null;
    }

}
