using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Save
{
    public string _locationId;
    public CharacterDbContext _characterData;
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
