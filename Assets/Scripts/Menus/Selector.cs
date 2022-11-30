using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Button selectButton;

    public static event Action<string> onCharacterSelect;

    private string GetCharacterName()
    {
        return characterName.text;
    }

    public void SetCharacter(string text)
    {
        characterName.text = text;
    }

    public void SetActiveUI(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public void SelectCharacter()
    {
        onCharacterSelect?.Invoke(characterName.text);
    }
}
