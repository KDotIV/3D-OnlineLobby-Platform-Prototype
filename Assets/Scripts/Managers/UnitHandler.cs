using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitHandler : NetworkBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private List<Character> selectedChars = new List<Character>();

    public static UnitHandler instance;

    private void Awake()
    {
        instance = this;
    }

    [ClientCallback]
    private void Update() 
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            return;
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach (Character found in selectedChars)
            {
                found.Deselect();
            }
            selectedChars.Clear();
            //TrySelection();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            TrySelection();
        }
    }

    private void TrySelection()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        if(!hit.collider.TryGetComponent<Character>(out Character selected)) { return; }

        selectedChars.Add(selected);

        foreach (Character found in selectedChars)
        {
            found.Select();
        }

        Debug.Log($"Unit: {selected} was selected");
    }
}
