using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerPickup : MonoBehaviour
{
    
    [Header("Layer")]
    [SerializeField] private LayerMask expLayerName;
    [SerializeField] private LayerMask itemLayerName;
    [SerializeField] float pullSpeed;
   
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((expLayerName.value & (1 << collision.gameObject.layer)) !=0)
        {
            ExpGem expValue = collision.GetComponent<ExpGem>();
            if (expValue != null) 
            {
                expValue.Pull(transform, pullSpeed);
            }

        }
        else if ((itemLayerName.value & (1 << collision.gameObject.layer)) != 0)
        {
            Meal mealValue = collision.GetComponent<Meal>();
            if(mealValue != null)
            {
                mealValue.Pull(transform, pullSpeed);
            }
        }
    }
}
