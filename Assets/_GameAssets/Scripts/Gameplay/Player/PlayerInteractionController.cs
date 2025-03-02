using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private PlayerController _playerController;
    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.Collect();
        }

       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IBoostable>(out var boostable))
        {
            boostable.Boost(_playerController);
        }
    }
}
