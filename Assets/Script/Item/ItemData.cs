using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EffectData
{
    public string type;
    public int value;
}

[Serializable]
public struct ItemData
{
    public int id;
    public string name;
    public string type;
    public int attack;
    public int defence;
    public EffectData effect;
    public int damage;
    public string description;
    public string iconName;

}

