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
    public EffectData effect;
    public int damage;

}

