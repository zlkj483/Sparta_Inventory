using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
public static PlayerManager Instance { get; private set; }
    public event Action OnPlayerStatusChanged;
    public event Action OnPlayerInvChanged;

    [Header("Default Data & Config")]
    [SerializeField]
    private TextAsset defaultPlayerDataAsset;
    private const string SAVE_FILE_NAME = "player_save.json";
    

    public PlayerSaveData CurrentSaveData { get; private set; } = new PlayerSaveData();  //PlayerSaveData 객체가 PlayerData (struct)와 inventoryItemIds (List)를 모두 포함
    public Dictionary<string, ItemData> EquippedItems { get; private set; } = new Dictionary<string, ItemData>();

    public PlayerData CurrentStatus => CurrentSaveData.status;
    public Dictionary<int, int> InventoryStacks => CurrentSaveData.inventoryStacks;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public bool IsEquipped(int itemID)
    {
        // EquippedItems의 모든 값(ItemData)을 순회하며 ID를 비교
        foreach (var item in EquippedItems.Values)
        {
            if (item.id == itemID)
            {
                return true;
            }
        }
        return false;
    }

    private void LoadData()
    {
        string savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME); //세이브파일의 경로 설정
        bool loadSuccessful = false; //로드 성공 여부

        if(File.Exists(savePath)) //세이브 파일의 존재 여부
        {
            try
            {
                string jsonSaveData = File.ReadAllText(savePath);
                JToken saveRoot = JToken.Parse(jsonSaveData);
                CurrentSaveData = saveRoot.ToObject<PlayerSaveData>();
                loadSuccessful = true;
                Debug.Log("세이브파일 로드 성공");
            }
            catch(Exception)
            {
                Debug.LogError("세이브파일 로드 오류");
            }
        }

        if (!loadSuccessful)
        {
            LoadDefaultData();
            Debug.Log("기본 json 파일로드, 혹은 뉴 게임");
        }

        if(CurrentSaveData.inventoryStacks == null) //인벤 null일 경우 초기화
        {
            CurrentSaveData.inventoryStacks = new Dictionary<int, int>();
            Debug.Log("인벤토리 초기화");
        }

        OnPlayerStatusChanged?.Invoke();
        OnPlayerInvChanged?.Invoke(); // 초기 로드 후 UI에 알려줌
    }

    private void LoadDefaultData() // 나중에 역직렬화 방법 바꿔서도 해보자..
    {
        if(defaultPlayerDataAsset == null)
        {
            Debug.LogError("기본 플레이어 에셋이 연결 안됨");
        }
        try
        {
            JToken root = JToken.Parse(defaultPlayerDataAsset.text);
            JArray playerArray = (JArray)root["players"];

            if(playerArray != null && playerArray.Count > 0)
            {
                PlayerData defaultStatus = playerArray[0].ToObject<PlayerData>();
                CurrentSaveData.status = defaultStatus;
                CurrentSaveData.inventoryStacks = new Dictionary<int, int>
                {
                    { 201, 3 },  // 201번 아이템 3개 202번 5개
                    { 202, 5 },
                    { 203, 1 }
                };
                Debug.Log("기본 아이템을 가지고 시작합니다");
            }
        }
        catch (Exception)
        {
            Debug.Log("기본 플레이어 데이터 로드 및 파싱 오류");
        }
    }

    public void SaveData() // 그냥 외우자...
    {
        if(CurrentSaveData == null) return;

        string jsonString = JsonConvert.SerializeObject(CurrentSaveData, Formatting.Indented);
        string savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        File.WriteAllText(savePath, jsonString);
        Debug.Log("데이터 저장 성공");
    }

    public void AddExp(int amount)
    {
        PlayerData tempStatus = CurrentSaveData.status; // struct에 있는건 값 복사이므로 임시 변수에 복사하여 수정 후 다시 할당
        tempStatus.currentexp += amount;

        if(tempStatus.currentexp >= tempStatus.maxexp)
        {
            LevelUp(ref  tempStatus);
            Debug.Log("레벨업");
        }
        CurrentSaveData.status = tempStatus; // 변경 된 struct를 다시 currentsavedata에 할당
        OnPlayerStatusChanged?.Invoke();
    }

    public void LevelUp(ref PlayerData status)  // LevelUp 함수를 struct를 참조(ref)로 받아 처리하도록 수정
    {
        status.level++;
        status.currentexp -= status.maxexp;
        status.maxexp += 2;
        SaveData();
    }

    public void AddItem(int itemID) // 아이템 추가
    {
        if (CurrentSaveData.inventoryStacks.ContainsKey(itemID))
        {
            CurrentSaveData.inventoryStacks[itemID]++; // 딕셔너리에 이미 있으면 수량 추가
        }
        else
        {
            CurrentSaveData.inventoryStacks.Add(itemID, 1); // 없으면 1개 새로 추가 
        }
        OnPlayerInvChanged?.Invoke(); // 인벤 바뀐거 알려주고 오토 세이브
        SaveData();
    }

    public void RemoveItem(int itemID, int amount = 1) // 아이템 사용 이하는 additem이랑 거의 같음.
    {
        if (CurrentSaveData.inventoryStacks.ContainsKey(itemID))
        {
            CurrentSaveData.inventoryStacks[itemID] -= amount;

            if (CurrentSaveData.inventoryStacks[itemID]  < 0) 
            {
                CurrentSaveData.inventoryStacks.Remove(itemID);
            }
            OnPlayerInvChanged?.Invoke();
        }
    }

    public void EquipItem(ItemData data)
    {
        string slotKey;
        if (data.type.ToLower() == "weapon")
        {
            slotKey = "weapon"; // 무기는 'weapon' 키 사용
        }
        else
        {
            // 장착 불가능한 타입은 여기서 종료
            Debug.LogWarning($"{data.name}은(는) 장착 가능한 타입이 아닙니다.");
            return;
        }
        //string type = data.type.ToLower(); // 아이템 타입을 키로 사용
        if (EquippedItems.ContainsKey(slotKey)) // 이미 장착된 아이템 있으면
        {
            ItemData oldItem = EquippedItems[slotKey]; // 기존 아이템을 되돌림
            EquippedItems.Remove(slotKey);
        }
        EquippedItems.Add(slotKey, data);
        Debug.Log($"{data.name} 장착 완료. 상태 업데이트.");
        OnPlayerInvChanged?.Invoke();
        OnPlayerStatusChanged?.Invoke();
    }

    public void UnEquipItem(ItemData data)
    {
        string slotKey;
        if (data.type.ToLower() == "weapon")
        {
            slotKey = "weapon";
        }
        else
        {
            return;
        }
        //string type = data.type.ToLower();
        if (EquippedItems.ContainsKey(slotKey) && EquippedItems[slotKey].id == data.id)
        {
            EquippedItems.Remove(slotKey);
            Debug.Log($"{data.name} 해제 완료. 상태 업데이트.");
            OnPlayerInvChanged?.Invoke();
            OnPlayerStatusChanged?.Invoke();
        }
    }
    public int TotalAttack()
    {
        int totalAttack = CurrentStatus.baseAttack;
        foreach (var item in EquippedItems.Values)
        {
            totalAttack += item.attack;
        }
        return totalAttack;
    }

    public int TotalDefence()
    {
        int totalDef = CurrentStatus.defence;
        foreach(var item in EquippedItems.Values)
        {
            totalDef += item.defence;
        }
        return totalDef;
    }
}
