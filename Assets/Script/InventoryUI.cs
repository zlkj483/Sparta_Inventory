using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemSlotPrefabs; //ui 에서 복제 될 아이템 프리팹
    [SerializeField] private Transform slotPanel; // 슬롯이 배치될 부모개체

    private List<GameObject> activeSlot = new List<GameObject>(); // 메모리에 생성된 슬롯 오브젝트들을 관리할 리스트

    private void Start()
    {
        if (PlayerManager.Instance != null) //PlayerManager의 이벤트에 UI 갱신 함수를 연결
        {
            PlayerManager.Instance.OnPlayerInvChanged += UpdateInventoryUI;
            UpdateInventoryUI();
        }
    }

    private void OnDestroy() // 스크립트 파괴될 때 없애주기 위한 매서드
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerInvChanged -= UpdateInventoryUI;
        }
    }

    public void UpdateInventoryUI()
    {
        foreach (var slot in activeSlot) // 기존 슬롯 제거(초기화)
        {
            Destroy(slot);
        }
        activeSlot.Clear();
        if (PlayerManager.Instance == null) return;

        Dictionary<int, int> inventory = PlayerManager.Instance.InventoryStacks; // PlayerManager에서 최신 인벤토리 데이터를 가져옴

        foreach (var itemStack in inventory) // 새로운 슬롯 생성
        {
            int itemID = itemStack.Key;
            int amount = itemStack.Value;

            if(ItemManager.Instance == null)
            {
                Debug.LogError("아이템 매니저가 초기화되지 않음");
                return;
            }


            ItemData itemData = ItemManager.Instance.GetItemData(itemID);

            GameObject newSlot = Instantiate(itemSlotPrefabs, slotPanel);
            activeSlot.Add(newSlot);

            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>(); // invslot스크립트 찾아서 전달
            if (slotScript != null)
            {
                slotScript.SetItem(itemData, amount); // ItemData와 amount를 invslot에 전달
            }
            Debug.Log("인벤토리 갱신 완료");
        }
    }
}
