using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipManager : MonoBehaviour
{
    
public static ItemTooltipManager Instance { get; private set; }

    [SerializeField] private GameObject tooltipPanel; // InfoBG 전체 패널
    [SerializeField] private TextMeshProUGUI itemNameText; // ItemName
    [SerializeField] private TextMeshProUGUI descriptionText; // Description
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unEquipButton;

    


    private void Awake()
    {
        Instance = this;
        tooltipPanel.SetActive(false);
    }

    public void ShowTooltip(ItemData data, bool isEquipped)
    {
        itemNameText.text = data.name;
        descriptionText.text = data.description;
        //bool isEquipableType = data.type.ToLower() == "weapon";
        string type = data.type.ToLower();
        bool isEquipableType = type == "weapon" || type == "armor";
        tooltipPanel.SetActive(true);
        equipButton.onClick.RemoveAllListeners();
        unEquipButton.onClick.RemoveAllListeners();

        if (isEquipableType)
        {
            if (isEquipped)
            {
                unEquipButton.onClick.AddListener(() => OnUnequipClicked(data));
            }
            else
            {
                equipButton.onClick.AddListener(() => OnEquipClicked(data));
            }
        }
    }

    public void HideTooptip()
    {
        tooltipPanel.SetActive(false);
    }
    public void OnEquipClicked(ItemData data)
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.EquipItem(data);
            ShowTooltip(data, true);
        }
    }
    public void OnUnequipClicked(ItemData data)
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.UnEquipItem(data);
            ShowTooltip(data, false);
        }
    }
}
