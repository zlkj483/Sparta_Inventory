using UnityEngine;

/// <summary>
/// 전환 방향(슬라이드용)
/// </summary>
public enum UISlideDir
{
    Left,
    Right,
    Up,
    Down,
    None
}

/// <summary>
/// 전환 애니메이션 종류
/// </summary>
public enum UIAnimType
{
    None,
    Slide,
    Zoom,
    Fade
}

/// <summary>
/// Hide 이후 오브젝트의 라이프사이클
/// </summary>
public enum UILifecycleMode
{
    KeepActive,         // 유지(비활성/파괴하지 않음)
    DeactivateOnHide,   // SetActive(false)
    DestroyOnHide       // Destroy(gameObject)
}
