using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    [Header("UI Root (Canvas 아래 Panel 등)")]
    [SerializeField] private RectTransform uiRoot;   // 여기 기준으로 모든 UIView가 소환됨

    [Header("Initial View (씬 오브젝트 또는 프리팹)")]
    [SerializeField] private UIView initialView;

    [Header("Auto Show Initial On Start")]
    [SerializeField] private bool showInitialOnStart = true;

    private readonly Stack<UIView> viewStack = new();

    private void Start()
    {

        if (this.showInitialOnStart && this.initialView != null)
        {
            UIView root = this.initialView;

            // 프리팹 에셋이면 먼저 인스턴스화
            if (!root.gameObject.scene.IsValid())
            {
                root = Instantiate(this.initialView, uiRoot, false);
            }

            ResetAndPush(root);
        }
    }

    /// <summary>
    /// 현재 최상단(Top) UIView 반환(없으면 null).
    /// </summary>
    public UIView Peek()
    {
        if (this.viewStack.Count > 0)
        {
            return this.viewStack.Peek();
        }
        return null;
    }

    // ========================= (Inspector 호출 허용) =========================

    /// <summary>
    /// 일반 화면 전환: 현재 화면을 숨기고 새 UIView를 올립니다.
    /// </summary>
    public void Push(UIView view)
    {
        CorePush(view, hideCurrent: true);
    }

    /// <summary>
    /// 팝업 전환: 아래 화면을 유지한 채로 새 UIView를 위에 올립니다.
    /// </summary>
    public void ShowPopup(UIView view)
    {
        CorePush(view, hideCurrent: false);
    }

    /// <summary>
    /// 전체 초기화 후 특정 UIView 하나만 올립니다.
    /// </summary>
    public void ResetAndPush(UIView root)
    {
        if (root == null)
        {
            Debug.LogWarning("[UIManager] ResetAndPush: root is null");
            return;
        }

        PopAll();
        CorePush(root, hideCurrent: false);
    }

    /// <summary>
    /// 최상단 하나 내리기(뒤로가기).
    /// </summary>
    public void Pop()
    {
        if (this.viewStack.Count == 0)
        {
            return;
        }

        UIView top = this.viewStack.Pop();
        if (top != null)
        {
            top.Hide();
        }

        if (this.viewStack.Count > 0)
        {
            UIView below = this.viewStack.Peek();
            if (below != null)
            {
                if (!below.gameObject.activeSelf)
                {
                    below.gameObject.SetActive(true);
                }
                below.Show();
            }
        }
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public void ClosePopup()
    {

        if (this.viewStack.Count == 0)
            return;

        UIView top = this.viewStack.Pop();
        top?.Hide();

    }

    /// <summary>
    /// 모든 뷰를 내립니다(재표시 없음).
    /// </summary>
    public void PopAll()
    {
        while (this.viewStack.Count > 0)
        {
            UIView view = this.viewStack.Pop();
            if (view == null)
            {
                continue;
            }

            // 애니 없이 즉시 정리(라이프사이클 일관 처리)
            view.ForceHide();
        }
    }

    /// <summary>
    /// 루트만 남기고 모두 내립니다.
    /// </summary>
    public void PopToRoot()
    {
        if (this.viewStack.Count <= 1)
        {
            return;
        }

        while (this.viewStack.Count > 1)
        {
            UIView view = this.viewStack.Pop();
            if (view != null)
            {
                view.ForceHide();
            }
        }

        UIView root = this.viewStack.Peek();
        if (root != null)
        {
            if (!root.gameObject.activeSelf)
            {
                root.gameObject.SetActive(true);
            }
            root.Show();
        }
    }

    /// <summary>
    /// target이 최상단이 될 때까지 Pop
    /// </summary>
    public void PopTo(UIView target)
    {
        if (target == null)
        {
            return;
        }
        if (this.viewStack.Count == 0)
        {
            return;
        }
        if (!this.viewStack.Contains(target))
        {
            Debug.LogWarning("[UIManager] PopTo: target not in stack.");
            return;
        }

        while (this.viewStack.Count > 0 && this.viewStack.Peek() != target)
        {
            UIView view = this.viewStack.Pop();
            if (view != null)
            {
                view.ForceHide();
            }
        }

        if (this.viewStack.Count > 0 && this.viewStack.Peek() == target)
        {
            UIView top = this.viewStack.Peek();
            if (top != null)
            {
                if (!top.gameObject.activeSelf)
                {
                    top.gameObject.SetActive(true);
                }
                top.Show();
            }
        }
    }

    // ========================= 내부 구현 =========================

    /// <summary>
    /// 공통 Push 내부 구현
    /// </summary>
    private void CorePush(UIView uiView, bool hideCurrent)
    {
        if (uiView == null)
        {
            Debug.LogWarning("[UIManager] Push: view is null");
            return;
        }


        // 현재 상단 처리(일반 화면 전환일 때만 Hide)
        if (hideCurrent && this.viewStack.Count > 0)
        {
            UIView current = this.viewStack.Peek();
            if (current != null)
            {
                current.Hide();
            }
        }

        // 프리팹이면 생성, 씬 객체면 부모만 교체 (부모는 항상 uiRoot)
        UIView view = uiView;
        bool isPrefabAsset = !uiView.gameObject.scene.IsValid();
        if (isPrefabAsset)
        {
            view = Object.Instantiate(uiView, uiRoot, false);
        }
        else
        {
            view.transform.SetParent(uiRoot, false);
        }


        NormalizeForCanvas(view);

        this.viewStack.Push(view);

        if (!view.gameObject.activeSelf)
        {
            view.gameObject.SetActive(true);
        }
        view.Show();
    }


    private void NormalizeForCanvas(UIView uiView)
    {
        if (uiView == null)
        {
            return;
        }

        RectTransform rt = uiView.RectTransform;
        if (rt == null)
        {
            return;
        }

        //rt.anchorMin = Vector2.zero;
        //rt.anchorMax = Vector2.one;
        //rt.pivot = new Vector2(0.5f, 0.5f);
        //rt.offsetMin = Vector2.zero;  // Left, Bottom
        //rt.offsetMax = Vector2.zero;  // Right, Top

        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
    }

    // ========================= Inspector 유틸(버튼) =========================
#if UNITY_EDITOR
    [Button("Print ViewStack")]
    public void Editor_PrintViewStack()
    {
        if (this.viewStack.Count == 0)
        {
            Debug.Log("[UIManager] ViewStack is EMPTY");
            return;
        }

        var arr = this.viewStack.ToArray();   
        System.Array.Reverse(arr);           

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"[UIManager] ViewStack count = {arr.Length}");
        sb.Append("[bottom] ");

        for (int i = 0; i < arr.Length; i++)
        {
            UIView view = arr[i];
            sb.Append(view ? view.name : "null");
            if (i < arr.Length - 1)
            {
                sb.Append("  >  ");
            }
            else
            {
                sb.Append("  <-- TOP");
            }
        }

        Debug.Log(sb.ToString());
    }

    [Button("Reset To Defaults")]
    public void Editor_ResetToDefaults()
    {
        ResetToDefaults();
    }
#endif

    /// <summary>
    /// 컴포넌트 필드를 기본값으로 초기화합니다.
    /// </summary>
    private void ResetToDefaults()
    {
        this.initialView = null;
        this.showInitialOnStart = true;
        PopAll();
    }

    private void Reset()
    {

        uiRoot = GetComponent<RectTransform>();
        ResetToDefaults();
    }
}
