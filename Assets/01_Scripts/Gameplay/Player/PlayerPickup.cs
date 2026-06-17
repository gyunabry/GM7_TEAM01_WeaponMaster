using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerPickup : MonoBehaviour
{
    private CircleCollider2D cc;
    [Header("Tag")]
    [SerializeField] private string expTagName;
    [SerializeField] private string itemTagName;
    private void Awake()
    {
        cc = GetComponent<CircleCollider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(expTagName))
        {

        }
        else if (collision.CompareTag(itemTagName))
        {

        }
    }
}
