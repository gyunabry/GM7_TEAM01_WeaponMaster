using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EXPBarController : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private PlayerController player;

    [Header("EXP UI")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI expText;
    void Start()
    {
        if(!expSlider==null)
        {
            expSlider = GetComponent<Slider>();
        }
    }

    void Update()
    {
        
            if (player == null || expSlider == null) return;

        //bool isInShop = ShopManager.Instance.IsOpen; //샵 매니저가 만들어지면 여기에 함수 수정해서 입력 후 주석 풀면 상점이 켜지면 ui가 꺼집니다.
        //if(isInShop == true)
        //{
        //    gameObject.SetActive(false);
        //    return;
        //}

        //float currentExp = //플레이어 현재 보유 경험치 겟메서드
        //float maxExp = //레벨업에 필요한 총 경험치 겟메서드

        //if(maxExp>0)
        //{
        //    expSlider.value = currentExp / maxHp;
        //}
        //if(expText !=null)
        //{
        //    expText.text = $"{Mathf.RoundToInt(currentExp)}/{Mathf.RoundToInt(maxExp)}";
        //}



    }
}
