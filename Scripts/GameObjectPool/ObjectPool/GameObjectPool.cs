using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameObjectPool : IModel
{
    private int m_PoolSize;
    private string m_ResName;

    private GameObject m_OriginalRes;
    private GameObject m_GameObjectPoolParent;

    private Queue<GameObject> m_NotUseGameObjectPool;

    private bool m_SuccessFlag;
    public bool SuccessFlag { get { return m_SuccessFlag; } }
    public GameObjectPool(string name, string resName, GameObject objectPool, int poolSize)
    {
        m_SuccessFlag = true;
        m_NotUseGameObjectPool = new Queue<GameObject>(poolSize);

        m_PoolSize = poolSize;
        m_ResName = resName;

        string poolName = name + "Pool";
        m_GameObjectPoolParent = new GameObject(poolName);
        m_GameObjectPoolParent.transform.parent = objectPool.transform;

        OnInit();
    }

    /// <summary>
    /// ���������Ϣ
    /// </summary>
    public void ClearAll()
    {
        while (m_NotUseGameObjectPool.Count > 0) Object.Destroy(m_NotUseGameObjectPool.Dequeue());

        m_NotUseGameObjectPool = null;

        m_PoolSize = 0;
        m_ResName = string.Empty;

        m_OriginalRes = null;

        Object.Destroy(m_GameObjectPoolParent);

        ResourcesManager.Instance.UnloadUnusedAssets();
    }

    /// <summary>
    /// ��ö���
    /// </summary>
    /// <returns></returns>
    public GameObject GetGameObject()
    {
        if (m_NotUseGameObjectPool.Count > 0)
        {
            return m_NotUseGameObjectPool.Dequeue();
        }
        else
        {
            GameObject obj = CreateGameObjecet();
            if (obj != null) return obj;
            else return null;
        }
    }

    /// <summary>
    /// ���ն���
    /// </summary>
    /// <param name="obj"></param>
    public void RecycleGameObject(GameObject obj)
    {
        if (!m_NotUseGameObjectPool.Contains(obj))
        {
            obj.SetActive(false);
            obj.transform.parent = m_GameObjectPoolParent.transform;
            obj.transform.localPosition = Vector3.zero;
            m_NotUseGameObjectPool.Enqueue(obj);
        }
        else
        {
            Debug.LogError($"�ö����Ѿ��黹���ˣ�������{obj.name}");
        }
     
        ClearRedundantObjects();
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void ClearRedundantObjects()
    {
        while (m_NotUseGameObjectPool.Count > m_PoolSize) Object.Destroy(m_NotUseGameObjectPool.Dequeue());
    }

    /// <summary>
    /// �����¶���
    /// </summary>
    /// <returns></returns>
    private GameObject CreateGameObjecet()
    {
        GameObject obj = GameObject.Instantiate(m_OriginalRes, m_GameObjectPoolParent.transform);
        if (obj != null)
        {
            obj.SetActive(false);
            obj.transform.localPosition = Vector3.zero;
            return obj;
        }
        else
        {
            Debug.LogError($"ʵ��������Ϊ�գ���Դ��Ϊ{m_ResName}");
            return null;
        }
    }


    public void OnInit()
    {
        m_OriginalRes = ResourcesManager.Instance.Load<GameObject>(m_ResName);
        if (m_OriginalRes != null)
        {
            for (int i = 0; i < m_PoolSize; i++)
            {
                GameObject obj = CreateGameObjecet();
                if (obj != null) m_NotUseGameObjectPool.Enqueue(obj);
                else
                {
                    m_SuccessFlag = false;
                    break;
                }
            }
        }
        else
        {
            Debug.LogError($"��Դ{m_ResName}δ�ҵ�");
            ClearAll();
            m_SuccessFlag = false;
        }
    }
}
