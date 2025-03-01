using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientationTransform;
    private Rigidbody _playerRigidbody;

    [Header("Movement Settings")]
    [SerializeField] private KeyCode _movementKey;
    [SerializeField] private float _movementSpeed;
    private float _horizontalInput, _verticalInput;
    private Vector3 _movementDirection;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private bool _canJump;
    [SerializeField] private float _jumpCooldown;

    [Header("Slider Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    [SerializeField] private bool _isSliding;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;


   

    
    
    private void Awake()
    {
        // get rigidbody from the gameobject which is our script attached to, then freeze rotations on all axes
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
    }

    private void Update()
    {
        SetInputs();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }

    private void FixedUpdate()
    {
        SetPlayerMovement();
    }

    private void SetInputs()
    {
        // get horizontal and vertical inputs
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");


        // state of movement
        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;
        }
        else if (Input.GetKeyDown(_movementKey))
        {
            _isSliding = false;
        }
        // catch the jump key in the gameloop and apply jump action using jump function
        else if (Input.GetKey(_jumpKey) && _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJump();
            Invoke(nameof(ResetPlayerJump), _jumpCooldown);
        }
    }

    private void SetPlayerMovement()
    {
        // get looking direction from orientation object which is looking forward and create the movement direction
        _movementDirection = _orientationTransform.forward * _verticalInput +
            _orientationTransform.right * _horizontalInput;

        // apply force according to the movement direction,
        // ForceMode.Force is a continuous force type which can be used on continuous movement
        if (_isSliding)
        {
            _playerRigidbody.AddForce(_movementDirection.normalized * _movementSpeed * _slideMultiplier, ForceMode.Force);
        }
        else
        {
            _playerRigidbody.AddForce(_movementDirection.normalized * _movementSpeed, ForceMode.Force);
        }
        
    }

    private void SetPlayerDrag()
    {
        // state of rigidbody drag
        if (_isSliding)
        {
            _playerRigidbody.linearDamping = _slideDrag;
        }
        else
        {
            _playerRigidbody.linearDamping = _groundDrag;
        }
    }
    private void SetPlayerJump()
    {
        // before jumping, make zero speed on y axis to avoid corruption on jumping action
        // for example if our player is falling down and has - speed on y axis, the jumping will be effected,
        // maybe not even jump or jump a little because of the - speed of y axis
        _playerRigidbody.linearVelocity = 
            new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        // apply jump force through up on y axis
        // ForceMode.Impulse is an instant force type which can be used instant movements such as jumping, or firing a projectile
        _playerRigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);

        if(flatVelocity.magnitude > _movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigidbody.linearVelocity = 
                new Vector3(limitedVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void ResetPlayerJump()
    {
        // will be used to activate permission of jumping when cooldown is over and grounded
        _canJump = true;
    }

    private bool IsGrounded()
    {
        // return the state of player is grounded or not, via sending raycast from player position through ground
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);
    }

}
