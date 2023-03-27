using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGPCommons;
using MGPCommons.Class;

public class ObjectPoolManager : SingletonSuperMono<ObjectPoolManager>, IManager
{
    private GameObject m_ObjectPool;

    private Dictionary<string, GameObjectPool> m_AllObjectPoll;

    //��ʼ����
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
    /// ���������
    /// </summary>
    /// <param name="poolName">���������</param>
    /// <param name="resName">��Դ��</param>
    /// <param name="poolSize">����ش�С</param>
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
            Debug.LogError($"�����{poolName}Pool�Ѿ�����");
        }
    }

    /// <summary>
    /// ɾ�������
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
            Debug.LogError($"�����{poolName}Pool������");
        }
    }

    /// <summary>
    /// ��ȡ����
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
            Debug.LogError($"�����{poolName}Pool������");
            return null;
        }
    }

    /// <summary>
    /// ���ն���
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
            Debug.LogError($"�����{poolName}Pool������");
        }
    }
}
