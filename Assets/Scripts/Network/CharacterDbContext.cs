using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterDbContext
{
    public string CharacterID { get; set; }
    public UserDbContext User { get; set; }
    public string CharacterName { get; set; }
    public int CurrentLevel { get; set; }
    public CurrentProfession currentProfessionType { get; set; }

}

public enum CurrentProfession
{
    Journey,
    Warrior,
    Mage,
    Rogue
}
