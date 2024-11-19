using System.Collections;
using UnityEngine;

public class DestroyAfterTimeParticle : MonoBehaviour {
	[Tooltip("Time to destroy")]
	public float timeToDestroy = 0.8f;
    /*
   * 씬에 생성된 후 게임 오브젝트를 파괴합니다.
   * 이 기능은 파티클과 플래시에 사용됩니다.
   */
    void Start () {
		Destroy (gameObject, timeToDestroy);
	}

}
