using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float arrowSpeed;
    private ObjectPool<Arrow> pool;
    private Coroutine co;
    private Coroutine hitCo;
    private int pier;
    private int maxPier;

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
    IEnumerator ReleaseTime()
    {
        yield return null;
        gameObject.transform.SetParent(null);
        yield return new WaitForSecondsRealtime(5.0f);
        pier = 0;
        pool.Release(this);
        co = null;
    }
    IEnumerator DeleteTime()
    {
        yield return null;
        pier = 0;
        pool.Release(this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(pier > maxPier)
            {
                hitCo = StartCoroutine(DeleteTime());
            }
            else
            {
                pier++;
            }
        }
        if (collision.gameObject.CompareTag("Boss"))
        {
            if(pier > maxPier)
            {
                hitCo = StartCoroutine(DeleteTime());
            }
            else
            {
                pier++;
            }
        }
    }
    public void GetMaxPiercing(int max)
    {
        maxPier = max;
    }

}