using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct SetCharacterMessage : NetworkMessage
{
    public string characterName;
    public int currentLevel;
    public CurrentProfession currentProfession;
}

public enum Race
{
    Human
}