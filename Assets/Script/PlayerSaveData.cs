using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveData
{
    public PlayerData status; //플레이어데이터

    public Dictionary<int, int> inventoryStacks; // 인벤토리목록 리스트로 가져오면 아이템이 많아 질 경우 너무 커짐..
}
