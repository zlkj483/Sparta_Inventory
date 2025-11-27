using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI equipText;

    private int currentItemID;
    private ItemData currentItemData;

    public void SetItem(ItemData data, int amount)
    {
        currentItemID = data.id;
        currentItemData = data;
        if (ItemManager.Instance != null && !string.IsNullOrEmpty(data.iconName))
        {
            //아이콘 이름을 주고 Sprite를 받아옴
            itemIcon.sprite = ItemManager.Instance.GetItemSprite(data.iconName);
        }
        bool isEquipped = PlayerManager.Instance.IsEquipped(currentItemID);
        if (equipText != null)
        {
            // 텍스트 내용 설정
            equipText.text = "E";

            //  장착 상태에 따라 텍스트를 활성화/비활성화
            equipText.gameObject.SetActive(isEquipped);
        }

        itemIcon.gameObject.SetActive(true);
        amountText.text = amount > 1 ? amount.ToString() : "";
        Debug.Log("아이템 슬롯에 세팅완료");
    }

    public void ClearSlot() // 빈 슬롯이라면 아이콘없애고 amountText 공백처리
    {
        currentItemID = 0;
        itemIcon.gameObject.SetActive(false);
        amountText.text = "";
    }

    public void OnSlotClicked()
    {
        if (currentItemID == 0)
        {
            return;
        }
        bool isEquipped = PlayerManager.Instance.IsEquipped(currentItemID);
        ItemTooltipManager.Instance.ShowTooltip(currentItemData, isEquipped);

    }
}
