using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Qarth;
public class UIMgr : TMonoSingleton<UIMgr>
{
    public const string UIDir = "Prefabs/UI/";//UIĿ¼

    //��ȾUI�Ļ���
    Canvas uiCanvas;
    public Canvas UICanvas { get { return uiCanvas; } }
    //��ȾUI�����
    Camera uiCamera;
    public Camera UICamera { get { return uiCamera; } }

    //�����е�UI
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
    /// ����Canvas����
    /// </summary>
    /// <param name="referResolution">�ο��ķֱ���</param>
    /// <param name="isLandscape">�Ƿ�Ϊ����</param>
    public void SetCanvasScaler(Vector2 referResolution, bool isLandscape)
    {
        CanvasScaler canvasScaler = uiCanvas.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.matchWidthOrHeight = isLandscape ? 1 : 0;
        canvasScaler.referenceResolution = referResolution;
    }

    /// <summary>
    /// ��UI
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
    /// �ر�UI
    /// </summary>
    public void Close<T>(bool isDestroy = false)
        where T : BaseUI
    {
        string uiName = typeof(T).Name;
        Close(uiName, isDestroy);
    }

    /// <summary>
    /// �ر�UI
    /// </summary>
    public void Close(string uiName, bool isDestroy = false)
    {
        if (!uiCache.ContainsKey(uiName))
        {
            Debug.LogError("������û�д�UI��" + uiName);
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
    /// ����UI
    /// </summary>
    public T Find<T>()
        where T : BaseUI
    {
        string uiName = typeof(T).Name;
        if (!uiCache.ContainsKey(uiName))
        {
            Debug.LogError("������û�д�UI��" + uiName);
            return default;
        }
        return uiCache[uiName] as T;
    }

    /// <summary>
    /// ���������UI
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
        Debug.LogError("������û��UI������ʧ��");
        return null;
    }

    #endregion

    #region Tools

    /// <summary>
    /// ����ĳһ��UI���ӽ��
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
    /// ����UI
    /// </summary>
    void Dispose(string uiName)
    {
        if (!uiCache.ContainsKey(uiName))
        {
            Debug.LogError("������û�д�UI��" + uiName);
            return;
        }
        BaseUI ui = uiCache[uiName];
        uiCache.Remove(uiName);
        DestroyImmediate(ui.gameObject);
        disposeDirty = true;
    }

    /// <summary>
    /// ����UI
    /// </summary>
    bool CreateUI(string uiName, string uiParentName)
    {
        string path = UIDir + uiName;
        GameObject uiPrefab = Resources.Load<GameObject>(path);
        if (uiPrefab == null)
        {
            Debug.LogError("UIԤ���岻���ڣ�" + path);
            return false;
        }
        if (!string.IsNullOrEmpty(uiParentName) && !uiCache.ContainsKey(uiParentName))
        {
            Debug.LogError("UI����㲻���ڣ�" + uiParentName);
            return false;
        }
        BaseUI baseUI = Instantiate(uiPrefab, string.IsNullOrEmpty(uiParentName) ? uiCanvas.transform : uiCache[uiParentName].transform).GetComponent<BaseUI>();
        baseUI.gameObject.name = uiName;
        baseUI.uiParentName = uiParentName;
        uiCache.Add(uiName, baseUI);
        return true;
    }

    /// <summary>
    /// ����UI����
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
    /// ����UI���
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
