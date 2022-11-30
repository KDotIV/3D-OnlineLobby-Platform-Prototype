using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class BaseMovement : NetworkBehaviour
{
    [SerializeField] private float _turnSpeed = 3f;
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _jumpForce = 0.5f;
    [SerializeField] private float _gravity = 2f;

    [SerializeField] private CharacterController _characterController;
    private Vector3 _moveDirection;
    public bool isGrounded;
    [SerializeField] private Camera _mainCamera;

    #region ServerCalls

    [Command]
    private void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
    }

    [Command]
    private void InteractWithMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);
        Vector3 transformDirection = transform.TransformDirection(inputDirection);

        Vector3 flatMovement = _moveSpeed * Time.deltaTime * transformDirection;

        _moveDirection = new Vector3(flatMovement.x, _moveDirection.y, flatMovement.z);

        if(PLayerJumped)
            _moveDirection.y = _jumpForce;
        else if (_characterController.isGrounded)
            _moveDirection.y = 0f;
        else
            _moveDirection.y -= _gravity * Time.deltaTime;
        
        _characterController.Move(_moveDirection);
    }
    #endregion

    #region ClientCalls

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    [ClientCallback]
    private void Update()
    {
        if(!hasAuthority) {return;}
        if(_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            return;
        }
        InteractWithMovement();
        GetMouseRay();
        if(!Mouse.current.rightButton.wasPressedThisFrame) { return; }
        //if(!Input.GetMouseButtonDown(1)) { return; }

        //Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

        CmdMove(hit.point);
    }

    private void GetMouseRay()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if(groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 pointToLook = ray.GetPoint(rayLength);
            Debug.DrawLine(ray.origin, pointToLook, Color.blue);

            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }
    private bool PLayerJumped => _characterController.isGrounded && Input.GetKey(KeyCode.Space);
    
    #endregion
}
