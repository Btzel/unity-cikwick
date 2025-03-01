using UnityEngine;

public class StateController : MonoBehaviour
{
    // create current state
    private PlayerState _currentPlayerState;

    private void Start()
    {
        // define state to idle state at start
        ChangeState(PlayerState.Idle);
    }
    public void ChangeState(PlayerState newPlayerState)
    {
        // change current state with new state if current state is not equal with new state
        if (_currentPlayerState == newPlayerState) { return; }
        _currentPlayerState = newPlayerState;
    }

    public PlayerState GetCurrentState()
    {
        // return current state
        return _currentPlayerState;
    }
}
