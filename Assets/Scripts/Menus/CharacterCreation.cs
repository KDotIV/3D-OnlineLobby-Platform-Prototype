using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RPG.Managers;

public class CharacterCreation : MonoBehaviour
{
    [SerializeField] private TMP_InputField characterNameField;

    public static event Action<UserDbContext, string> onCreationRequest;

    public void CallCharacterCreation()
    {
        if(VerifyInput(characterNameField.text))
        {
            onCreationRequest?.Invoke(APIGateway.instance.GetCurrentUser(), characterNameField.text);
            Debug.Log("Calling CharacterAPI....");
        }
        else
        {
            Debug.LogError("Input does not match our registry");
        }
    }

    public void CallReturnToMenu()
    {
        SceneLoader.instance.RequestMainMenu();
    }

    private bool VerifyInput(string charactername)
    {
        var positiveIntRegex = new System.Text.RegularExpressions.Regex("^[a-zA-z0-9-]*$");

        if (!positiveIntRegex.IsMatch(charactername) && charactername.Length < 8)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
