using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _orientationTransform;
    [SerializeField] private Transform _playerVisualTransform;
    [Header("Settings")]
    [SerializeField] private float _rotationSpeed;
    private float _horizontalInput, _verticalInput;

    private void Update()
    {
        // get the vector that points from the camera toward the player transform
        Vector3 viewDirection = _playerTransform.position 
            - new Vector3(transform.position.x, _playerTransform.position.y, transform.position.z);

        // point through player transform from the perspective of the camera in orientation transform
        _orientationTransform.forward = viewDirection.normalized;

        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        // get looking direction from orientation object which is looking forward and create the movement direction
        Vector3 inputDirection 
            = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;

        // Vector3.Slerp and Vector3.Lerp is used for smooth panning from one position or rotation to another
        // Vector3.Slerp is used for rotation interpolation
        // which we are going to use in this case to make our player to look towards our movement direction
        // Vector3.Lerp is used for position interpolations
        if(inputDirection != Vector3.zero)
        {
            _playerVisualTransform.forward
                = Vector3.Slerp(_playerVisualTransform.forward, inputDirection.normalized,Time.deltaTime * _rotationSpeed);
        }

       
    }
}
