using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    [HideInInspector]
    public string uiParentName;//此UI的父结点名称

    /// <summary>
    /// 打开UI时
    /// </summary>
    public virtual void OnView()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭UI时
    /// </summary>
    public virtual void OnDisView()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Close(bool isDestroy)
    {
        UIMgr.S.Close(GetType().Name, isDestroy);
    }

}
