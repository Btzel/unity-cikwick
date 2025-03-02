using UnityEngine;

public class SpatulaBooster : MonoBehaviour, IBoostable
{
    [Header("References")]
    [SerializeField] private Animator _spatulaAnimator;
    [Header("Settings")]
    [SerializeField] private float _jumpForce;

    private bool _isActivated;
    public void Boost(PlayerController playerController)
    {
        if (_isActivated) { return; }
        PlayBoostAnimation();
        Rigidbody playerRigidbody = playerController.GetPlayerRigidbody();

        // to avoid corruption on jump, we define 0 of rigidbody speed on y axis
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.y);
        playerRigidbody.AddForce(transform.forward * _jumpForce, ForceMode.Impulse);
        _isActivated = true;
        Invoke(nameof(ResetActivation), 0.2f);
    }

    private void PlayBoostAnimation()
    {
        _spatulaAnimator.SetTrigger(Consts.OtherAnimations.IS_SPATULA_JUMPING);
    }

    private void ResetActivation()
    {
        _isActivated = false;
    }
}
