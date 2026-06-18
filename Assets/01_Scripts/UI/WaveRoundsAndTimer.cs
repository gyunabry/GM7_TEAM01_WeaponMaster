using UnityEngine;
using TMPro;

public class WaveRoundsAndTimer : MonoBehaviour
{
    [Header("웨이브 카운트 표시 및 경과시간")]
    [SerializeField] private TextMeshProUGUI waveCount;
    [SerializeField] private TextMeshProUGUI timer;
    

    void Update()
    {
        if (WaveManager.Instance == null) return;

        //bool isInShop = ShopManager.Instance.IsOpen; //샵 매니저가 만들어지면 여기에 함수 수정해서 입력 후 주석 풀면 상점이 켜지면 ui가 꺼집니다.
        //if (isInShop == true) return;


        if(waveCount !=null)
        {
            int currentWave = WaveManager.Instance.CurrentWave;
            waveCount.text = $"Wave {currentWave}";
        }
        if(timer != null)
        {
            float playTime = WaveManager.Instance.WaveTime;
            int minutes = Mathf.FloorToInt(playTime / 60f);
            int seconds = Mathf.FloorToInt(playTime % 60f);
            timer.text = $"{minutes:D2} : {seconds:D2}";
        }
    }
}
