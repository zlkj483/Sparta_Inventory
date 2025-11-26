using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


public class PlayerManager : MonoBehaviour
{
public static PlayerManager Instance { get; private set; }
    public event Action OnPlayerStatusChanged;
    public event Action OnPlayerInvChanged;

    [Header("Default Data & Config")]
    [SerializeField]
    private TextAsset defaultPlayerDataAsset;
    private const string SAVE_FILE_NAME = "player_save.json";

    public PlayerSaveData CurrentSaveData { get; private set; } //PlayerSaveData 객체가 PlayerData (struct)와 inventoryItemIds (List)를 모두 포함

    public PlayerData CurrentStatus => CurrentSaveData.status;
    public List<int> InventoryItemId => CurrentSaveData.inventoryItemIds;

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

        if(CurrentSaveData.inventoryItemIds == null) //인벤 null일 경우 초기화
        {
            CurrentSaveData.inventoryItemIds = new List<int>();
            Debug.Log("인벤토리 초기화");
        }
        OnPlayerStatusChanged?.Invoke();
        OnPlayerInvChanged?.Invoke(); // 초기 로드 후 UI에 알려줌
    }

    private void LoadDefaultData()
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
                CurrentSaveData.inventoryItemIds = new List<int> { 201, 202 };
            }
        }
        catch (Exception)
        {
            Debug.Log("기본 플레이어 데이터 로드 및 파싱 오류");
        }
    }

    public void SaveData()
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

    public void AddItem(int itemID)
    {

    }
}
