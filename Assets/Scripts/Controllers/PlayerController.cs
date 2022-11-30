using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    //Reference Variables
    private PlayerInput _playerInput;
    [SerializeField] private PlayerMovement _playerMovement = null;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private PlayerInput PlayerInput
    {
        get
        {
            if(_playerInput != null) { return _playerInput; }
            return _playerInput = new PlayerInput();
        }
    }
    
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        enabled = true;

        _playerInput.PlayerControls.Move.started += ctx => _playerMovement.CmdSetMovement(ctx.ReadValue<Vector2>());
        _playerInput.PlayerControls.Move.performed += ctx => _playerMovement.CmdSetMovement(ctx.ReadValue<Vector2>());
        _playerInput.PlayerControls.Move.canceled += ctx => _playerMovement.CmdSetMovement(ctx.ReadValue<Vector2>());
        _playerInput.PlayerControls.Jump.started += ctx => _playerMovement.CmdOnJump(ctx.ReadValueAsButton());
        _playerInput.PlayerControls.Jump.canceled += ctx => _playerMovement.CmdOnJump(ctx.ReadValueAsButton());
    }

    #region Client Calls

    [ClientCallback]
    private void OnEnable() 
    {
        PlayerInput.Enable();
    }

    [ClientCallback]
    private void OnDisable()
    {
        PlayerInput.Disable();
    }

    [ClientCallback]
    private void Update()
    {
        if(!hasAuthority) return;
    }
    #endregion

    public PlayerMovement GetPlayerMovement()
    {
        return _playerMovement;
    }
}
