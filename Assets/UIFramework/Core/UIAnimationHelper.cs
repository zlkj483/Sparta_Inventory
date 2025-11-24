using System.Collections;
using UnityEngine;
using UnityEngine.Events;



public static class UIAnimationHelper
{
    private static Vector2 GetEdgeOffset(RectTransform rect, UISlideDir dir)
    {
        RectTransform parent = rect != null ? rect.parent as RectTransform : null;
        Vector2 size = parent != null ? parent.rect.size : new Vector2(Screen.width, Screen.height);

        switch (dir)
        {
            case UISlideDir.Left:
                {
                    return new Vector2(-size.x, 0f);
                }
            case UISlideDir.Right:
                {
                    return new Vector2(size.x, 0f);
                }
            case UISlideDir.Up:
                {
                    return new Vector2(0f, size.y);
                }
            case UISlideDir.Down:
                {
                    return new Vector2(0f, -size.y);
                }
            default:
                {
                    return Vector2.zero;
                }
        }
    }

    public static IEnumerator SlideIn(RectTransform rect, UISlideDir dir, float speed, UnityAction onEnd)
    {
        if (rect == null)
        {
            onEnd?.Invoke();
            yield break;
        }

        Vector2 start = GetEdgeOffset(rect, dir);
        float t = 0f;

        while (t < 1f)
        {
            rect.anchoredPosition = Vector2.Lerp(start, Vector2.zero, t);
            t += Time.unscaledDeltaTime * speed;
            yield return null;
        }

        rect.anchoredPosition = Vector2.zero;
        onEnd?.Invoke();
    }

    public static IEnumerator SlideOut(RectTransform rect, UISlideDir dir, float speed, UnityAction onEnd)
    {
        if (rect == null)
        {
            onEnd?.Invoke();
            yield break;
        }

        Vector2 end = GetEdgeOffset(rect, dir);
        float t = 0f;

        while (t < 1f)
        {
            rect.anchoredPosition = Vector2.Lerp(Vector2.zero, end, t);
            t += Time.unscaledDeltaTime * speed;
            yield return null;
        }

        rect.anchoredPosition = end;
        onEnd?.Invoke();
    }

    public static IEnumerator FadeIn(CanvasGroup group, float speed, UnityAction onEnd)
    {
        if (group == null)
        {
            onEnd?.Invoke();
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            group.alpha = Mathf.Lerp(0f, 1f, t);
            t += Time.unscaledDeltaTime * speed;
            yield return null;
        }

        group.alpha = 1f;
        onEnd?.Invoke();
    }

    public static IEnumerator FadeOut(CanvasGroup group, float speed, UnityAction onEnd)
    {
        if (group == null)
        {
            onEnd?.Invoke();
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            group.alpha = Mathf.Lerp(1f, 0f, t);
            t += Time.unscaledDeltaTime * speed;
            yield return null;
        }

        group.alpha = 0f;
        onEnd?.Invoke();
    }

    public static IEnumerator ZoomIn(RectTransform rect, float speed, UnityAction onEnd)
    {
        if (rect == null)
        {
            onEnd?.Invoke();
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            t += Time.unscaledDeltaTime * speed;
            yield return null;
        }

        rect.localScale = Vector3.one;
        onEnd?.Invoke();
    }

    public static IEnumerator ZoomOut(RectTransform rect, float speed, UnityAction onEnd)
    {
        if (rect == null)
        {
            onEnd?.Invoke();
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            rect.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            t += Time.unscaledDeltaTime * speed;
            yield return null;
        }

        rect.localScale = Vector3.zero;
        onEnd?.Invoke();
    }
}
