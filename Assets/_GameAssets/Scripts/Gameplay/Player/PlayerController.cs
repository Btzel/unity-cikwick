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
    [SerializeField] private float _airMultiplier;
    [SerializeField] private float _airDrag;

    [Header("Slider Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    [SerializeField] private bool _isSliding;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    [Header("Player States")]
    private StateController _stateController;

    private void Awake()
    {
        // get rigidbody from the gameobject which is our script attached to, then freeze rotations on all axes
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;

        // get state controller script
        _stateController = GetComponent<StateController>();
    }

    private void Update()
    {
        SetInputs();
        SetStates();
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

    // this state machine is not working as actual, normally state machines work such as 
    // the movements or animations happen when state is something
    // for example if state is idle, player will have idle features, if run it will run
    // this state machine will work quite the opposite, when we run, the state will be run
    // this approach is to apply state machine for educational purposes, it will be for only 
    // to experience state based coding.
    // normally, we create the state machine for our states, then code our actions in it and make it work before coding actions.
    private void SetStates()
    {
        var movementDirection = GetMovementDirection();
        var isGrounded = IsGrounded();
        var isSliding = IsSliding();
        var currentState = _stateController.GetCurrentState();

        // create new state using new type of switch case structure
        var newState = currentState switch
        {
            // for in if format, same purpose as -> if (movementDirection == Vector3.zero && isGrounded && !_isSliding)
            // {newState = PlayerState.Idle} and elif and elif and so on
            // else {newState = currentState}
            _ when movementDirection  == Vector3.zero && isGrounded && !isSliding => PlayerState.Idle,
            _ when movementDirection != Vector3.zero && isGrounded && !isSliding => PlayerState.Move,
            _ when movementDirection == Vector3.zero && isGrounded && isSliding => PlayerState.SlideIdle,
            _ when movementDirection != Vector3.zero && isGrounded && isSliding => PlayerState.Slide,
            _ when !_canJump && !isGrounded => PlayerState.Jump,
            _ => currentState

        };

        // lastly change state with new state if state is changed
        if (newState != currentState)
        {
            _stateController.ChangeState(newState);
        }

        Debug.Log(newState);
    }
    private void SetPlayerMovement()
    {
        // get looking direction from orientation object which is looking forward and create the movement direction
        _movementDirection = _orientationTransform.forward * _verticalInput +
            _orientationTransform.right * _horizontalInput;

        // will be used to define multipliers according to current state
        float forceMultiplier = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => 1f,
            PlayerState.Slide => _slideMultiplier,
            PlayerState.Jump => _airMultiplier,
            _ => 1f
        };

        // apply force according to the movement direction and force multiplier,
        // ForceMode.Force is a continuous force type which can be used on continuous movement
        _playerRigidbody.AddForce(GetMovementDirection() * _movementSpeed * forceMultiplier, ForceMode.Force);


    }

    private void SetPlayerDrag()
    {

        // state of rigidbody drag
        _playerRigidbody.linearDamping = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => _groundDrag,
            PlayerState.Slide => _slideDrag,
            PlayerState.Jump => _airDrag,
            _ => _playerRigidbody.linearDamping
        };

        
        
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
        // get flat velocity by getting rigibody movement velocity on x and z axis
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);

        // if flat velocity magnitude is bigger than movement speed chosen, it will generate a limited velocity 
        // by multiplying the movement speed by flat velocity normalized and change the value of current rigidbody velocity
        // with limited velocity x and z axis values
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

    private Vector3 GetMovementDirection()
    {
        return _movementDirection.normalized;
    }

    private bool IsSliding()
    {
        return _isSliding;
    }

}
