using UnityEngine;
using UnityEngine.Pool;

public class ArrowPooling : MonoBehaviour
{
    [SerializeField] private Arrow arrowPrefab;

    [SerializeField] private int defalutCapacity = 100;
    [SerializeField] private int maxSize = 500;

    private ObjectPool<Arrow> pool;
    private void Awake()
    {
        pool = new ObjectPool<Arrow>(
            CreatePool,
            EnablePool,
            DisablePool,
            DestroyPool,
            true,
            defalutCapacity,
            maxSize
            );
    }
    private Arrow CreatePool()
    {
        Arrow arrow = Instantiate(arrowPrefab);
        return arrow;
    }
    private void EnablePool(Arrow obj)
    {
        obj.transform.position = this.transform.position;
        obj.transform.rotation = this.transform.rotation;

        obj.gameObject.SetActive(true);
    }
    private void DisablePool(Arrow obj)
    {
        obj.gameObject.SetActive(false);
    }
    private void DestroyPool(Arrow obj) 
    { 
        Destroy(obj);
    }

}
