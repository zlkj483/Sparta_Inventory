using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class UIView : MonoBehaviour
{
    [Header("Transition Profile (옵션)")]
    [Tooltip("체크 해제 시 프로필 값을 사용합니다. 체크 시 아래 Override 설정을 사용합니다.")]
    public bool overrideTransition = false;
    public UITransitionProfile profile;

    [Header("Override (overrideTransition=true 일 때만 유효)")]
    public UIAnimType entryAnim = UIAnimType.Slide;
    public UISlideDir entryDir = UISlideDir.Right;
    [Range(0.01f, 10f)] public float entrySpeed = 2f;

    public UIAnimType exitAnim = UIAnimType.Slide;
    public UISlideDir exitDir = UISlideDir.Left;
    [Range(0.01f, 10f)] public float exitSpeed = 2f;

    [Header("Lifecycle")]
    public UILifecycleMode lifecycle = UILifecycleMode.DeactivateOnHide;

    [Header("Events")]
    public UnityEvent onPreShow;
    public UnityEvent onPostShow;
    public UnityEvent onPreHide;
    public UnityEvent onPostHide;


    public RectTransform RectTransform
    {
        get
        {
            return rectTransform;
        }
    }
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private Coroutine animationCoroutine;
    public bool IsAnimating { get; private set; }

    // ----------------- 내부 유틸 -----------------

    private void EnsureCached()
    {
        if (this.rectTransform == null)
        {
            this.rectTransform = GetComponent<RectTransform>();
        }
        if (this.canvasGroup == null)
        {
            this.canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnDisable()
    {
        this.IsAnimating = false;
        this.animationCoroutine = null;
    }

    private void ResolveTransition(out UIAnimType eAnim, out UISlideDir eDir, out float eSpeed,
                                   out UIAnimType xAnim, out UISlideDir xDir, out float xSpeed)
    {
        if (!this.overrideTransition && this.profile != null)
        {
            eAnim = this.profile.entryAnim;
            eDir = this.profile.entryDir;
            eSpeed = this.profile.entrySpeed;

            xAnim = this.profile.exitAnim;
            xDir = this.profile.exitDir;
            xSpeed = this.profile.exitSpeed;
        }
        else
        {
            eAnim = this.entryAnim;
            eDir = this.entryDir;
            eSpeed = this.entrySpeed;

            xAnim = this.exitAnim;
            xDir = this.exitDir;
            xSpeed = this.exitSpeed;
        }
    }

    private void PlayAnimation(IEnumerator routine)
    {
        if (routine == null)
        {
            return;
        }

        if (this.animationCoroutine != null)
        {
            StopCoroutine(this.animationCoroutine);
        }

        this.IsAnimating = true;
        this.animationCoroutine = StartCoroutine(Wrap(routine));
    }

    private IEnumerator Wrap(IEnumerator inner)
    {
        yield return StartCoroutine(inner);
        this.IsAnimating = false;
        this.animationCoroutine = null;
    }

    // ----------------- Public API -----------------

    public void Show()
    {
     

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        transform.SetAsLastSibling();

        ResolveTransition(out var eAnim, out var eDir, out var eSpeed,
                          out _, out _, out _);

        if (this.canvasGroup != null)
        {
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.alpha = (eAnim == UIAnimType.Fade) ? 0f : 1f;
        }

        this.onPreShow?.Invoke();

        UnityAction post = () =>
        {
            if (this.canvasGroup != null)
            {
                this.canvasGroup.interactable = true;
                this.canvasGroup.blocksRaycasts = true;
            }
            this.onPostShow?.Invoke();
        };

        switch (eAnim)
        {
            case UIAnimType.Slide:
                {
                    PlayAnimation(UIAnimationHelper.SlideIn(this.rectTransform, eDir, eSpeed, post));
                    break;
                }
            case UIAnimType.Zoom:
                {
                    PlayAnimation(UIAnimationHelper.ZoomIn(this.rectTransform, eSpeed, post));
                    break;
                }
            case UIAnimType.Fade:
                {
                    PlayAnimation(UIAnimationHelper.FadeIn(this.canvasGroup, eSpeed, post));
                    break;
                }
            default:
                {
                    post?.Invoke();
                    break;
                }
        }
    }

    public void Hide()
    {

        ResolveTransition(out _, out _, out _,
                          out var xAnim, out var xDir, out var xSpeed);

        if (this.canvasGroup != null)
        {
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
            if (xAnim == UIAnimType.Fade)
            {
                this.canvasGroup.alpha = 1f;
            }
        }

        this.onPreHide?.Invoke();

        UnityAction post = () =>
        {
            ApplyLifecycle();
            this.onPostHide?.Invoke();
        };

        switch (xAnim)
        {
            case UIAnimType.Slide:
                {
                    PlayAnimation(UIAnimationHelper.SlideOut(this.rectTransform, xDir, xSpeed, post));
                    break;
                }
            case UIAnimType.Zoom:
                {
                    PlayAnimation(UIAnimationHelper.ZoomOut(this.rectTransform, xSpeed, post));
                    break;
                }
            case UIAnimType.Fade:
                {
                    PlayAnimation(UIAnimationHelper.FadeOut(this.canvasGroup, xSpeed, post));
                    break;
                }
            default:
                {
                    post?.Invoke();
                    break;
                }
        }
    }

    public void ForceHide()
    {


        if (this.animationCoroutine != null)
        {
            StopCoroutine(this.animationCoroutine);
            this.animationCoroutine = null;
        }
        this.IsAnimating = false;

        if (this.canvasGroup != null)
        {
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.alpha = 0f;
        }

        ApplyLifecycle();
    }

    private void ApplyLifecycle()
    {
        switch (this.lifecycle)
        {
            case UILifecycleMode.KeepActive:
                {
                    break;
                }
            case UILifecycleMode.DeactivateOnHide:
                {
                    gameObject.SetActive(false);
                    break;
                }
            case UILifecycleMode.DestroyOnHide:
                {
                    Destroy(gameObject);
                    break;
                }
        }
    }

    // ----------------- Reset(버튼/메뉴) -----------------

    public void ResetToDefaults()
    {
        // 1) 기본 속성 값
        this.overrideTransition = false;

        this.entryAnim = UIAnimType.Slide;
        this.entryDir = UISlideDir.Right;
        this.entrySpeed = 2f;

        this.exitAnim = UIAnimType.Slide;
        this.exitDir = UISlideDir.Left;
        this.exitSpeed = 2f;

        this.lifecycle = UILifecycleMode.DeactivateOnHide;

        EnsureCached();


#if UNITY_EDITOR
        if (this.profile == null)
        {
            this.profile = FindOrCreateDefaultProfile();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    private void Reset()
    {
        ResetToDefaults();
    }
#if UNITY_EDITOR
    [Button("Reset To Defaults")]
    public void Editor_ResetToDefaults()
    {
        ResetToDefaults();
    }

    private UITransitionProfile FindOrCreateDefaultProfile()
    {
        // 1) 프로젝트 내에서 "Default_UITransitionProfile" 탐색
        string[] guids = UnityEditor.AssetDatabase.FindAssets("Default_UITransitionProfile t:UITransitionProfile");
        if (guids != null && guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            var def = UnityEditor.AssetDatabase.LoadAssetAtPath<UITransitionProfile>(path);
            if (def != null)
            {
                return def;
            }
        }

        // 2) 없으면 새로 생성하여 저장
        const string folderA = "Assets/UIFramework";
        const string folderB = "Assets/UIFramework/Profiles";

        if (!UnityEditor.AssetDatabase.IsValidFolder(folderA))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "UIFramework");
        }
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderB))
        {
            UnityEditor.AssetDatabase.CreateFolder(folderA, "Profiles");
        }


        var asset = ScriptableObject.CreateInstance<UITransitionProfile>();
        asset.ResetToDefaults();

        string uniquePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"{folderB}/Default_UITransitionProfile.asset");
        UnityEditor.AssetDatabase.CreateAsset(asset, uniquePath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        return asset;
    }
#endif
}
