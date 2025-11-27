using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenceText;

    private void Start()
    {
        if(PlayerManager.Instance == null)
        {
            Debug.LogError("플레이어 인스턴스 없음");
            return;
        }
        PlayerManager.Instance.OnPlayerStatusChanged += UpdateStatusUI;
        UpdateStatusUI();
    }
    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 등록 해제 (선택 사항이지만 안전함)
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerStatusChanged -= UpdateStatusUI;
        }
    }
    private void UpdateStatusUI()
    {
        int totalAtk = PlayerManager.Instance.TotalAttack();
        int totalDef = PlayerManager.Instance.TotalDefence();

        if(attackText != null)
        {
            attackText.text = $"{totalAtk}";
        }
        if(defenceText != null)
        {
            defenceText.text = $"{totalDef}";
        }
    }
}
