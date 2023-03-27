using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Qarth;
public class UIMgr : TMonoSingleton<UIMgr>
{
    public const string UIDir = "Prefabs/UI/";//UI目录

    //渲染UI的画布
    Canvas uiCanvas;
    public Canvas UICanvas { get { return uiCanvas; } }
    //渲染UI的相机
    Camera uiCamera;
    public Camera UICamera { get { return uiCamera; } }

    //场景中的UI
    Dictionary<string, BaseUI> uiCache = new Dictionary<string, BaseUI>();
    public Dictionary<string, BaseUI> UICache { get { return uiCache; } }

    public  void Awake()
    {
        if (uiCanvas == null)
        {
            uiCanvas = CreateUICanvas(CreateUICamera());
        }
    }

    #region Interface

    /// <summary>
    /// 设置Canvas缩放
    /// </summary>
    /// <param name="referResolution">参考的分辨率</param>
    /// <param name="isLandscape">是否为横屏</param>
    public void SetCanvasScaler(Vector2 referResolution, bool isLandscape)
    {
        CanvasScaler canvasScaler = uiCanvas.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.matchWidthOrHeight = isLandscape ? 1 : 0;
        canvasScaler.referenceResolution = referResolution;
    }

    /// <summary>
    /// 打开UI
    /// </summary>
    public T Open<T>(string uiParentName = "")
        where T : BaseUI
    {
        string uiName = typeof(T).Name;
        if (!uiCache.ContainsKey(uiName))
        {
            if (!CreateUI(uiName, uiParentName))
            {
                return default;
            }
        }

        BaseUI ui = uiCache[uiName];
        ui.OnView();
        ui.transform.SetAsLastSibling();

        return ui as T;
    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    public void Close<T>(bool isDestroy = false)
        where T : BaseUI
    {
        string uiName = typeof(T).Name;
        Close(uiName, isDestroy);
    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    public void Close(string uiName, bool isDestroy = false)
    {
        if (!uiCache.ContainsKey(uiName))
        {
            Debug.LogError("场景中没有此UI：" + uiName);
            return;
        }
        List<string> uiChildList = FindUIChild(uiName);
        foreach (var child in uiChildList)
        {
            uiCache[child].OnDisView();
        }
        uiCache[uiName].OnDisView();
        if (isDestroy)
        {
            foreach (var child in uiChildList)
            {
                Dispose(child);
            }
            Dispose(uiName);
        }
    }

    /// <summary>
    /// 查找UI
    /// </summary>
    public T Find<T>()
        where T : BaseUI
    {
        string uiName = typeof(T).Name;
        if (!uiCache.ContainsKey(uiName))
        {
            Debug.LogError("场景中没有此UI：" + uiName);
            return default;
        }
        return uiCache[uiName] as T;
    }

    /// <summary>
    /// 查找最顶部的UI
    /// </summary>
    public BaseUI FindTop()
    {
        for (int i = uiCanvas.transform.childCount - 1; i >= 0; i--)
        {
            if (uiCanvas.transform.GetChild(i).GetComponent<BaseUI>() != null
                && uiCanvas.transform.GetChild(i).gameObject.activeSelf)
            {
                return uiCanvas.transform.GetChild(i).GetComponent<BaseUI>();
            }
        }
        Debug.LogError("场景中没有UI，查找失败");
        return null;
    }

    #endregion

    #region Tools

    /// <summary>
    /// 查找某一个UI的子结点
    /// </summary>
    public List<string> FindUIChild(string uiName)
    {
        List<string> uiChildList = new List<string>();
        foreach (var ui in uiCache)
        {
            if (ui.Value.uiParentName == uiName)
            {
                foreach (var child in FindUIChild(ui.Key))
                {
                    uiChildList.Add(child);
                }
                uiChildList.Add(ui.Key);
            }
        }
        return uiChildList;
    }

    void LateUpdate()
    {
        if (disposeDirty)
        {
            Resources.UnloadUnusedAssets();
            disposeDirty = false;
        }
    }

    bool disposeDirty = false;
    /// <summary>
    /// 销毁UI
    /// </summary>
    void Dispose(string uiName)
    {
        if (!uiCache.ContainsKey(uiName))
        {
            Debug.LogError("场景中没有此UI：" + uiName);
            return;
        }
        BaseUI ui = uiCache[uiName];
        uiCache.Remove(uiName);
        DestroyImmediate(ui.gameObject);
        disposeDirty = true;
    }

    /// <summary>
    /// 创建UI
    /// </summary>
    bool CreateUI(string uiName, string uiParentName)
    {
        string path = UIDir + uiName;
        GameObject uiPrefab = Resources.Load<GameObject>(path);
        if (uiPrefab == null)
        {
            Debug.LogError("UI预制体不存在：" + path);
            return false;
        }
        if (!string.IsNullOrEmpty(uiParentName) && !uiCache.ContainsKey(uiParentName))
        {
            Debug.LogError("UI父结点不存在：" + uiParentName);
            return false;
        }
        BaseUI baseUI = Instantiate(uiPrefab, string.IsNullOrEmpty(uiParentName) ? uiCanvas.transform : uiCache[uiParentName].transform).GetComponent<BaseUI>();
        baseUI.gameObject.name = uiName;
        baseUI.uiParentName = uiParentName;
        uiCache.Add(uiName, baseUI);
        return true;
    }

    /// <summary>
    /// 创建UI画布
    /// </summary>
    Canvas CreateUICanvas(Camera uiCamera)
    {
        GameObject canvas = new GameObject("UICanvas");
        canvas.layer = 5;
        Canvas uiCanvas = canvas.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.worldCamera = uiCamera;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
        eventSystem.transform.SetParent(canvas.transform);

        DontDestroyOnLoad(uiCanvas);
        return uiCanvas;
    }

    /// <summary>
    /// 创建UI相机
    /// </summary>
    Camera CreateUICamera()
    {
        GameObject camera = new GameObject("UICamera");
        uiCamera = camera.AddComponent<Camera>();
        uiCamera.clearFlags = CameraClearFlags.Depth;
        uiCamera.cullingMask = 1 << 5;
        uiCamera.orthographic = true;
        uiCamera.depth = 0;

        DontDestroyOnLoad(uiCamera);
        return uiCamera;
    }
 
    #endregion
}
