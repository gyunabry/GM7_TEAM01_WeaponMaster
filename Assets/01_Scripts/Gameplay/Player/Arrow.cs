using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float arrowSpeed;
    private ObjectPool<Arrow> pool;
    private Coroutine co;
    private Coroutine hitCo;
    private int Pier = 0;
    private int MaxPier = 3;

    
    public void SetPool(ObjectPool<Arrow> pool)
    {
        this.pool = pool;
    }
    

    void Update()
    {
        Vector2 dir = new Vector2(1f, 1f);
        transform.Translate(dir * arrowSpeed * Time.deltaTime);
        co = StartCoroutine(ReleaseTime());
    }
    public void GetMaxPiercing(int value)
    {
        MaxPier = value;
    }
    IEnumerator ReleaseTime()
    {
        yield return null;
        gameObject.transform.SetParent(null);
        yield return new WaitForSecondsRealtime(5.0f);
        pool.Release(this);
        co = null;
    }
    IEnumerator DeleteTime()
    {
        yield return null;
        pool.Release(this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (Pier > MaxPier)
            { 
                hitCo = StartCoroutine(DeleteTime());
                Pier = 0;
            }
            else
            {
                Pier++;
            }
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            if (Pier > MaxPier)
            {
                hitCo = StartCoroutine(DeleteTime());
                Pier = 0;
            }
            else
            {
                Pier++;
            }
        }
    }
}
