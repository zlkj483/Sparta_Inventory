using UnityEngine;

[CreateAssetMenu(fileName = "UITransitionProfile", menuName = "UI/Transition Profile")]
public class UITransitionProfile : ScriptableObject
{
    [Header("Entry (Show)")]
    public UIAnimType entryAnim = UIAnimType.Slide;
    public UISlideDir entryDir = UISlideDir.Right;
    [Range(0.01f, 10f)] public float entrySpeed = 2f;

    [Header("Exit (Hide)")]
    public UIAnimType exitAnim = UIAnimType.Slide;
    public UISlideDir exitDir = UISlideDir.Left;
    [Range(0.01f, 10f)] public float exitSpeed = 2f;

    public void ResetToDefaults()
    {
        this.entryAnim = UIAnimType.Slide;
        this.entryDir = UISlideDir.Right;
        this.entrySpeed = 2f;

        this.exitAnim = UIAnimType.Slide;
        this.exitDir = UISlideDir.Left;
        this.exitSpeed = 2f;
    }

#if UNITY_EDITOR
    [Button("Reset To Defaults")]
    public void Editor_ResetToDefaults()
    {
        ResetToDefaults();
        UnityEditor.EditorUtility.SetDirty(this);
    }


#endif
}
