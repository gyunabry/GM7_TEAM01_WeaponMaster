using UnityEngine;
using TMPro;


public class GoldText : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private PlayerController player;

    [Header("보유 골드량")]
    [SerializeField] private TextMeshProUGUI goldAmount;
    
  

    void Update()
    {
        if (player == null) return;

        //아래 겟메서드 완료시 작성 후 주석제거

        //int MoneyPocket = //골드 겟메서드 입력

        //if(goldAmount != null)
        //{
        //    goldAmount.text = MoneyPocket.ToString("N0");
        //}
    }
}
