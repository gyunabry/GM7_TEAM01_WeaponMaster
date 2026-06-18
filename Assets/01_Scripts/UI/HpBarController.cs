using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpBarController : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private PlayerController player;

    [Header("UI컴포넌트")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    void Start()
    {
        if (!hpSlider == null)
        {
            hpSlider = GetComponent<Slider>();
        }
    }

    void Update()
    {
        if (player == null || hpSlider == null) return;

        //bool isInShop = ShopManager.Instance.IsOpen; //샵 매니저가 만들어지면 여기에 함수 수정해서 입력 후 주석 풀면 상점이 켜지면 ui가 꺼집니다.
        //if(isInShop == true)
        //{
        //    gameObject.SetActive(false);
        //    return;
        //}

        // 아래는 추후에 겟메서드 입력 후 주석 풀면 작동됩니다

        //float currentHp = //플레이어 현재 체력 겟메서드 입력
        //float maxHp = //플레이어 최대체력 겟메서드 입력

        //if(maxHp>0)0
        //{
        //    hpSlider.value = currentHp / maxHp;
        //}
        //if(hpText !=null)
        //{
        //    hpText.text = $"{Mathf.RoundToInt(currentHp)}/{Mathf.RoundToInt(maxHp)}";
        //}

    }
}
