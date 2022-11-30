using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Registration : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField emailField;

    [SerializeField] private Button submitButton;

    public static event Action<string, string, string> onRegisterRequest;

    public void CallRegister()
    {
        if(VerifyInput(passwordField.text, usernameField.text))
        {
            onRegisterRequest?.Invoke(usernameField.text, passwordField.text, emailField.text);
            Debug.Log("Calling User DB......");
        }
        else
        {
            Debug.LogError("Input does not match our registry");
        }
    }

    private bool VerifyInput(string pword, string username)
    {
        var positiveIntRegex = new System.Text.RegularExpressions.Regex("^[a-zA-z0-9-!@#]*$");

        if(!positiveIntRegex.IsMatch(pword) || !positiveIntRegex.IsMatch(username))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
