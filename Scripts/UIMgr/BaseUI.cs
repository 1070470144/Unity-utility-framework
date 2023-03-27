using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    [HideInInspector]
    public string uiParentName;//��UI�ĸ��������

    /// <summary>
    /// ��UIʱ
    /// </summary>
    public virtual void OnView()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// �ر�UIʱ
    /// </summary>
    public virtual void OnDisView()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �رս���
    /// </summary>
    public void Close(bool isDestroy)
    {
        UIMgr.S.Close(GetType().Name, isDestroy);
    }

}
