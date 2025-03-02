using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Consts.WheatTypes.GOLD_WHEAT))
        {
            other.gameObject?.GetComponent<GoldWheatCollectible>().Collect();
        }
        else if (other.CompareTag(Consts.WheatTypes.HOLY_WHEAT))
        {
            other.gameObject?.GetComponent<HolyWheatCollectible>().Collect();
        }
        else if (other.CompareTag(Consts.WheatTypes.ROTTEN_WHEAT))
        {
            other.gameObject?.GetComponent<RottenWheatCollectible>().Collect();
        }

    }
}
