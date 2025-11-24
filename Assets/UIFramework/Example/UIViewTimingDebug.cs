using UnityEngine;

public class UIViewTimingDebug : MonoBehaviour
{
    private string ViewName => gameObject.name;

    public void DebugTime1()
    {
        Debug.Log($"{ViewName} ▶ 1 : OnPreShow");
    }

    public void DebugTime2()
    {
        Debug.Log($"{ViewName} ▶ 2 : OnPostShow");
    }

    public void DebugTime3()
    {
        Debug.Log($"{ViewName} ▶ 3 : OnPreHide");
    }

    public void DebugTime4()
    {
        Debug.Log($"{ViewName} ▶ 4 : OnPostHide");
    }
}