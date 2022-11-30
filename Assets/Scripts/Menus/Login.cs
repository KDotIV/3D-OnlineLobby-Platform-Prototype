using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    private bool _checkLogin;
    [SerializeField] private GameObject _loginOverlay;
    [SerializeField] private GameObject _loginForm;
    [SerializeField] private GameObject _loginError;

    public static event Action<string, string> onLoginRequest;


    private void OnEnable()
    {
        APIGateway.onLoginSuccess += LoginSucces;
        APIGateway.onLoginFail += LoginFail;
    }

    private void OnDisable()
    {
        APIGateway.onLoginSuccess -= LoginSucces;
        APIGateway.onLoginFail -= LoginFail;
    }

    public void CallLogin()
    {
        if(VerifyInput(passwordField.text, usernameField.text))
        {
            onLoginRequest?.Invoke(usernameField.text, passwordField.text);
            Debug.Log("Logging into Server....");
        }
        else
        {
            Debug.LogError("User does not match our registry");
        }
    }

    private bool VerifyInput(string pword, string username)
    {
        var positiveIntRegex = new System.Text.RegularExpressions.Regex("^[a-zA-z0-9-!@#]*$");

        if (!positiveIntRegex.IsMatch(pword) || !positiveIntRegex.IsMatch(username))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void LoginFail()
    {
        _loginError.SetActive(true);
    }

    private void LoginSucces()
    {
        _loginForm.SetActive(false);
        _loginOverlay.SetActive(false);
    }
}
