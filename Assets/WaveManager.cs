using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject[] waveControllers; // GameController1, GameController2, GameController3 등을 배열로 관리
    private int currentWaveIndex = 0;    // 현재 활성화된 웨이브의 인덱스

    void Start()
    {
        // 모든 웨이브를 비활성화한 후 첫 번째 웨이브만 활성화
        for (int i = 0; i < waveControllers.Length; i++)
        {
            waveControllers[i].SetActive(false);
        }
        StartNextWave(); // 첫 번째 웨이브 시작
    }

    public void StartNextWave()
    { 
        // 이전 웨이브 비활성화
        if (currentWaveIndex > 0 && currentWaveIndex <= waveControllers.Length)
        {
            waveControllers[currentWaveIndex - 1].SetActive(false);
        }

        // 다음 웨이브 활성화
        if (currentWaveIndex < waveControllers.Length)
        {
            waveControllers[currentWaveIndex].SetActive(true); // 현재 웨이브 활성화
            currentWaveIndex++;
        }
        else
        {
            Debug.Log("모든 웨이브 종료!");
        }
    }

    public void OnWaveCompleted()
    {
        Debug.Log($"Wave {currentWaveIndex} 완료!");
        StartNextWave(); // 다음 웨이브 시작
    }
}
