using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGPCommons;
using MGPCommons.Class;

public class ObjectPoolManager : SingletonSuperMono<ObjectPoolManager>, IManager
{
    private GameObject m_ObjectPool;

    private Dictionary<string, GameObjectPool> m_AllObjectPoll;

    //初始化池
    //

    private void Awake()
    {
        OnInit();
    }

    public void OnInit()
    {
        m_ObjectPool = new GameObject("ObjectPool");
        DontDestroyOnLoad(m_ObjectPool);

        m_AllObjectPoll = new Dictionary<string, GameObjectPool>();
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="poolName">对象池名字</param>
    /// <param name="resName">资源名</param>
    /// <param name="poolSize">对象池大小</param>
    public void CreateGameObjectPool(string poolName, string resName,int poolSize = 5)
    {
        if (!m_AllObjectPoll.ContainsKey(poolName.Trim()))
        {
            GameObjectPool pool = new GameObjectPool(poolName, resName, m_ObjectPool, poolSize);
            if (pool.SuccessFlag) m_AllObjectPoll.Add(poolName, pool);
            else pool = null;
        }
        else
        {
            Debug.LogError($"对象池{poolName}Pool已经存在");
        }
    }

    /// <summary>
    /// 删除对象池
    /// </summary>
    /// <param name="name"></param>
    public void DestroyGameObjectPool(string poolName)
    {
        if (m_AllObjectPoll.ContainsKey(poolName.Trim()))
        {
            m_AllObjectPoll[poolName].ClearAll();
            m_AllObjectPoll.Remove(poolName);
        }
        else
        {
            Debug.LogError($"对象池{poolName}Pool不存在");
        }
    }

    /// <summary>
    /// 获取对象
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public GameObject GetGameObject(string poolName)
    {
        if (m_AllObjectPoll.ContainsKey(poolName.Trim()))
        {
            return m_AllObjectPoll[poolName].GetGameObject();
        }
        else
        {
            Debug.LogError($"对象池{poolName}Pool不存在");
            return null;
        }
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="obj"></param>
    public void RecycleGameObject(string poolName,GameObject obj)
    {
        if (m_AllObjectPoll.ContainsKey(poolName.Trim()))
        {
             m_AllObjectPoll[poolName].RecycleGameObject(obj);
        }
        else
        {
            Debug.LogError($"对象池{poolName}Pool不存在");
        }
    }
}
