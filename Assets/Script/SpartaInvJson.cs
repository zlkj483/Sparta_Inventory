using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using TMPro;

public class SpartaInvJson : MonoBehaviour
{
    public TextAsset Sparta_Json;

    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerGold;
    [SerializeField] private TextMeshProUGUI playerDescription;


    private void Start()
    {
        Show1();
    }

     public void Show1()
    {
        JToken root = JToken.Parse(Sparta_Json.text);
        JToken players = root["players"][0];
        string name = (string)players["name"];
        int level = (int)players["level"];
        int gold = (int)players["gold"];
        string description = (string)players["description"];

        if (playerName != null)
        {
            playerName.text = name;
        }
        if (playerLevel != null)
        {
            playerLevel.text = level.ToString();
        }
        if (playerGold != null)
        {
            playerGold.text = gold.ToString();
        }
        if (playerDescription != null)
        {
            playerDescription.text = description;
        }

        Debug.Log((string)root["players"][0]["name"] + "/" + (int)root["players"][0]["level"] + "/" + (string)root["players"][0]["description"]);
    }
}
