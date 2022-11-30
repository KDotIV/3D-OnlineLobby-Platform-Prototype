using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    //[SerializeField] private NetworkAnimator _netAnimation;

    //Player Input Values
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private bool isMovementPressed;
    private bool isJumpPressed = false;
    private bool isJumping = false;
    private bool isWallRiding = false;

    //Animations
    private int isRunningHash;
    private int isJumpingHash;
    private int isWallRideHash;
    private int verticalHash;
    private int horizontalHash;
    private bool isJumpAnimating = false;
    private bool isWallRideAnimating = false;

    [Header("Constants")]
    [SerializeField] private float _turnSpeed = 3f;
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _gravity = -9.8f;
    [SerializeField] private float _groundedGravity = -.05f;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float maxJumpHeight = 1.0f;
    [SerializeField] private float maxJumpTime = 0.75f;
    [SerializeField] private float rotationSpeed = 15.0f;
    public float jumpCooldown = 0.5f;
    private float _movementGCD = 0f;
    private float velocityZ = 0.0f;
    private float velocityX = 0.0f;
    [SerializeField] private float acceleration = 2.0f;
    private float deceleration = 2.0f;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        enabled = true;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isWallRideHash = Animator.StringToHash("isWallRiding");
        verticalHash = Animator.StringToHash("Vertical");
        horizontalHash = Animator.StringToHash("Horizontal");

        InitializeJumpVariables();
    }

    [ServerCallback]
    private void Update()
    {
        ServerHandleCooldowns();
        HandleAnimation();

        ServerHandleRotation();
        ServerUpdateMovement();
        ServerHandleGravity();
        ServerHandleJump();
        ServerHandleWallRide();
    }

    private void HandleAnimation()
    {
        bool isRunning = _animator.GetBool(isRunningHash);

        if(isMovementPressed && !isRunning)
        {
            _animator.SetBool(isRunningHash, true);
            _animator.SetFloat(verticalHash, currentMovement.z);
            _animator.SetFloat(horizontalHash, currentMovement.x);
        }
        else if(!isMovementPressed && isRunning)
        {
            _animator.SetBool(isRunningHash, false);
            _animator.SetFloat(verticalHash, currentMovement.z);
            _animator.SetFloat(horizontalHash, currentMovement.x);
        }
    }

    [Command]
    public void CmdSetMovement(Vector2 input)
    {
        currentMovement.x = input.x;
        currentMovement.z = input.y;
        isMovementPressed = input.x !=0 || input.y !=0;
    }

    [Command]
    public void CmdOnJump(bool whatJumped)
    {
        isJumpPressed = whatJumped;
    }

    #region Server Calls

    [Server]
    private void InitializeJumpVariables()
    {
        //First Jump Variable
        float timeToApex = maxJumpTime / 2;
        _gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _jumpForce = (2 * maxJumpHeight) / timeToApex;

        //Second Jump Variable
        float secondJumpGravity = (-2 * (maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
        float secondJumpVelocitiy = (2 * (maxJumpHeight + 2)) / (timeToApex * 1.25f);
    }

    [Server]
    private void ServerUpdateMovement()
    {
        _characterController.Move(currentMovement * _moveSpeed * Time.deltaTime);
    }

    [Server]
    private void ServerHandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;


        Quaternion currentRotation = transform.rotation;

        if(isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    [Server]
    private void ServerHandleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        if(_characterController.isGrounded)
        {
            if(isJumpAnimating)
            {
                _animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }
            currentMovement.y = _groundedGravity;
        } else if(isFalling) {
            float previousYvelocity = currentMovement.y;
            float newYvelocity = currentMovement.y + (_gravity * fallMultiplier * Time.deltaTime);
            float nextYvelocity = (previousYvelocity + newYvelocity) * .5f;
            currentMovement.y = nextYvelocity;
        } else {
            float previousYvelocity = currentMovement.y;
            float newYvelocity = currentMovement.y + (_gravity * Time.deltaTime);
            float nextYvelocity = (previousYvelocity + newYvelocity) * .5f;
            currentMovement.y = nextYvelocity;
        }
    }

    [Server]
    private void ServerHandleJump()
    {
        if(_movementGCD >= jumpCooldown)
        if(!isJumping && _characterController.isGrounded && !isWallRiding && isJumpPressed)
        {
            _animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y = _jumpForce * 0.7f;
            _movementGCD = 0f;
        } else if (!isJumpPressed && isJumping && _characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    [Server]
    private void ServerHandleWallRide()
    {
        if(isWallRiding && !_characterController.isGrounded)
        {
            _animator.SetBool(isWallRideHash, true);
            currentMovement.x = transform.position.x;
            if(_movementGCD >= jumpCooldown)
            if(isJumpPressed)
            {
                _animator.SetBool(isJumpingHash, true);
                isWallRideAnimating = true;
                isJumping = true;
                currentMovement.y = _jumpForce * 0.7f;
                currentMovement.x = _jumpForce * 0.5f;
                isWallRiding = false;
                _animator.SetBool(isWallRideHash, false);
                _movementGCD = 0f;
            }
        }
        if(_characterController.isGrounded)
        {
            isJumping = false;
            isWallRiding = false;
            _animator.SetBool(isWallRideHash, false);
        }
    }

    [Server]
    private void ServerHandleCooldowns()
    {
        _movementGCD += Time.deltaTime;
    }

    #endregion

    private void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if(!_characterController.isGrounded && hit.normal.y < 0.1f)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);
            isWallRiding = true;
            isJumping = false;
        }
    }
}
