using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using Newtonsoft.Json;
using System.Linq;
using System;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("아이템 데이터 에셋")]
    [SerializeField] private TextAsset itemDataAsset; // itemdata json 연결
    public Dictionary<int, ItemData> ItemDictionary { get; private set; } = new Dictionary<int, ItemData>(); // id로 읽어옴

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadItemData();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetItemSprite(string iconName)
    {
        // Resources 폴더 안에 "Icons" 폴더가 있고 그 안에 스프라이트가 있다고 가정
        return Resources.Load<Sprite>($"Icons/{iconName}");
    }

    public void LoadItemData()
    {
        if(itemDataAsset == null)
        {
            Debug.LogError("json 연결 실패");
            return;
        }
        try
        {
            JToken root = JToken.Parse(itemDataAsset.text);
            JArray itemsArray = (JArray)root["items"]; // items로 접근하여 배열로 가져옴.

            if(itemsArray == null )
            {
                Debug.LogError("json에 배열이 없거나 잘못됨");
                return;
            }
            List<ItemData> itemList = itemsArray.ToObject<List<ItemData>>();
            if(itemList != null)
            {
                ItemDictionary = itemList.ToDictionary(item => item.id); // list를 dictionary<id,itemdata>로 변환
            }
        }
        catch(Exception)
        {
            Debug.LogError("아이템데이터 로드 오류");
        }
    }

    public ItemData GetItemData(int itemID)
    {
        if (ItemDictionary.TryGetValue(itemID, out var itemData))
        {
            return itemData;
        }
        return default;
    }

}
