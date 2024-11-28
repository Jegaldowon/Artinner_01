using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject[] waveControllers; // GameController1, GameController2, GameController3 ���� �迭�� ����
    private int currentWaveIndex = 0;    // ���� Ȱ��ȭ�� ���̺��� �ε���

    void Start()
    {
        // ��� ���̺긦 ��Ȱ��ȭ�� �� ù ��° ���̺길 Ȱ��ȭ
        for (int i = 0; i < waveControllers.Length; i++)
        {
            waveControllers[i].SetActive(false);
        }
        StartNextWave(); // ù ��° ���̺� ����
    }

    public void StartNextWave()
    { 
        // ���� ���̺� ��Ȱ��ȭ
        if (currentWaveIndex > 0 && currentWaveIndex <= waveControllers.Length)
        {
            waveControllers[currentWaveIndex - 1].SetActive(false);
        }

        // ���� ���̺� Ȱ��ȭ
        if (currentWaveIndex < waveControllers.Length)
        {
            waveControllers[currentWaveIndex].SetActive(true); // ���� ���̺� Ȱ��ȭ
            currentWaveIndex++;
        }
        else
        {
            Debug.Log("��� ���̺� ����!");
        }
    }

    public void OnWaveCompleted()
    {
        Debug.Log($"Wave {currentWaveIndex} �Ϸ�!");
        StartNextWave(); // ���� ���̺� ����
    }
}
