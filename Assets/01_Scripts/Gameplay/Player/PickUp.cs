using UnityEngine;

public class PickUp : MonoBehaviour
{
    private PlayerController parentPC;
    [Header("Layer")]
    [SerializeField] private LayerMask expLayerName;
    [SerializeField] private LayerMask itemLayerName;
    private void Awake()
    {
        parentPC = FindAnyObjectByType<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((expLayerName.value & (1 << collision.gameObject.layer)) != 0)
        {
            ExpGem expValue = collision.GetComponent<ExpGem>();
            if (expValue != null)
            {
                expValue.Collect(parentPC);
            }

        }
        else if ((itemLayerName.value & (1 << collision.gameObject.layer)) != 0)
        {
            Meal mealValue = collision.GetComponent<Meal>();
            if(mealValue != null)
            {
                mealValue.Collect(parentPC);
            }
        }
    }
}
