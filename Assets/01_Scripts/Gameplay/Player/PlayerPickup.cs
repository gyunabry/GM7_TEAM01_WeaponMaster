using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerPickup : MonoBehaviour
{
    private CircleCollider2D cc;
    private PlayerController parentPC;
    [Header("Tag")]
    [SerializeField] private LayerMask expLayerName;
    [SerializeField] private LayerMask itemLayerName;
    private void Awake()
    {
        cc = GetComponent<CircleCollider2D>();
        
        parentPC = FindAnyObjectByType<PlayerController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((expLayerName.value & (1 << collision.gameObject.layer)) !=0)
        {
            ExpGem expValue = collision.GetComponent<ExpGem>();
            if (expValue != null) 
            {
                
            }

        }
        else if ((itemLayerName.value & (1 << collision.gameObject.layer)) != 0)
        {

        }
    }
}
