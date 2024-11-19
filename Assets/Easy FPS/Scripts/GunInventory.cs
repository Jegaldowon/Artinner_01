using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MenuStyle
{
    horizontal, vertical
}

public class GunInventory : MonoBehaviour
{
    [Tooltip("현재 무기 게임 오브젝트.")]
    public GameObject currentGun;
    private Animator currentHAndsAnimator;
    private int currentGunCounter = 0;

    [Tooltip("Resources 폴더에서 무기 객체의 문자열을 입력하세요.")]
    public List<string> gunsIHave = new List<string>();
    [Tooltip("무기의 아이콘입니다. (게임 실행 시 가져옵니다) *Resources 폴더에 해당 이름의 아이콘이 있어야 합니다*")]
    public Texture[] icons;

    [HideInInspector]
    public float switchWeaponCooldown;

    /*
	 * 시작할 때 소지하고 있는 무기의 아이콘을 업데이트하는 메서드를 호출합니다.
	 * 또한 시작할 때 무기를 생성합니다.
	 */
    void Awake()
    {
        StartCoroutine("UpdateIconsFromResources");

        StartCoroutine("SpawnWeaponUponStart"); // 무기를 가지고 시작하기 위함

        if (gunsIHave.Count == 0)
            print("인벤토리에 총이 없습니다.");
    }

    /*
	 * 잠시 대기한 후 무기 생성을 호출합니다.
	 */
    IEnumerator SpawnWeaponUponStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("Spawn", 0);
    }

    /* 
	 * switchWeaponCooldown을 계산하여 무기를 초당 수백 번 변경할 수 없도록 합니다.
	 * 언젠가 switchWeaponCooldown이 음수 값으로 변경되어 0.0f를 초과할 때까지 기다려야 합니다.
	 */
    void Update()
    {
        switchWeaponCooldown += 1 * Time.deltaTime;
        if (switchWeaponCooldown > 1.2f && Input.GetKey(KeyCode.LeftShift) == false)
        {
            Create_Weapon();
        }
    }

    /*
	 * Resources/Weapo_Icons/에서 아이콘을 가져옵니다. -> 이미지의 총 이름.
	 * (!!!!!!!1!중요 읽기)
	 * 무기 이미지는 무기와 같은 이름을 가져야 하며 확장자는 _img로 설정되어야 합니다.
	 * 예를 들어 총 프리팹이 "Sniper_Piper"라고 한다면, 해당 이미지는 이전 위치에 "Sniper_Piper_img"라는 이름으로 있어야 합니다.
	 */
    IEnumerator UpdateIconsFromResources()
    {
        yield return new WaitForEndOfFrame();

        icons = new Texture[gunsIHave.Count];
        for (int i = 0; i < gunsIHave.Count; i++)
        {
            icons[i] = (Texture)Resources.Load("Weap_Icons/" + gunsIHave[i].ToString() + "_img");
        }
    }

    /*
	 * 마우스 휠이나 화살표를 위아래로 스크롤하면 플레이어가 무기를 변경합니다.
	 * GunPlaceSpawner는 Player 게임 오브젝트의 자식이며, 무기가 소환될 위치입니다.
	 */
    void Create_Weapon()
    {
        /*
		 * 휠 스크롤 무기 변경
		 */
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            switchWeaponCooldown = 0;

            currentGunCounter++;
            if (currentGunCounter > gunsIHave.Count - 1)
            {
                currentGunCounter = 0;
            }
            StartCoroutine("Spawn", currentGunCounter);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            switchWeaponCooldown = 0;

            currentGunCounter--;
            if (currentGunCounter < 0)
            {
                currentGunCounter = gunsIHave.Count - 1;
            }
            StartCoroutine("Spawn", currentGunCounter);
        }

        /*
		 * 숫자 패드
		 */
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentGunCounter != 0)
        {
            switchWeaponCooldown = 0;
            currentGunCounter = 0;
            StartCoroutine("Spawn", currentGunCounter);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && currentGunCounter != 1)
        {
            switchWeaponCooldown = 0;
            currentGunCounter = 1;
            StartCoroutine("Spawn", currentGunCounter);
        }
    }

    /*
	 * 이 메서드는 Create_Weapon()에서 호출되며 화살표를 위아래로 누르거나 마우스 휠을 스크롤할 때 호출됩니다.
	 * 무기를 가지고 있는지 확인하고 현재 무기를 파괴한 후 Resources 폴더에서 무기 프리팹을 로드합니다.
	 */
    IEnumerator Spawn(int _redniBroj)
    {
        if (weaponChanging)
            weaponChanging.Play();
        else
            print("무기 변경 음악 클립이 없습니다.");
        if (currentGun)
        {
            if (currentGun.name.Contains("Gun"))
            {
                currentHAndsAnimator.SetBool("changingWeapon", true);
                yield return new WaitForSeconds(0.8f); // 무기를 변경하는 시간, 하지만 무기 교체 애니메이션이 없으므로 대기할 필요는 없습니다.
                Destroy(currentGun);

                GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
                currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
                AssignHandsAnimator(currentGun);
            }
            else if (currentGun.name.Contains("Sword"))
            {
                currentHAndsAnimator.SetBool("changingWeapon", true);
                yield return new WaitForSeconds(0.25f); // 0.5f

                currentHAndsAnimator.SetBool("changingWeapon", false);
                yield return new WaitForSeconds(0.6f); // 1
                Destroy(currentGun);

                GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
                currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
                AssignHandsAnimator(currentGun);
            }
        }
        else
        {
            GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
            currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
            AssignHandsAnimator(currentGun);
        }
    }

    /*
	 * 현재 무기를 다른 스크립트에서 사용할 수 있도록 Animator를 할당합니다.
	 */
    void AssignHandsAnimator(GameObject _currentGun)
    {
        if (_currentGun.name.Contains("Gun"))
        {
            currentHAndsAnimator = currentGun.GetComponent<GunScript>().handsAnimator;
        }
    }

    /*
	 * Unity 내장 메서드로 GUI를 그립니다.
	 * 여기서 소지하고 있는 총을 나열하고 해당 이미지를 화면에 그립니다.
	 */
    void OnGUI()
    {
        if (currentGun)
        {
            for (int i = 0; i < gunsIHave.Count; i++)
            {
                DrawCorrespondingImage(i);
            }
        }
    }

    [Header("GUI 총 미리보기 변수")]
    [Tooltip("무기 아이콘 스타일 선택.")]
    public MenuStyle menuStyle = MenuStyle.horizontal;
    [Tooltip("아이콘 간 간격.")]
    public int spacing = 10;
    [Tooltip("화면의 백분율로 시작 위치.")]
    public Vector2 beginPosition;
    [Tooltip("아이콘의 크기(화면의 백분율로).")]
    public Vector2 size;
    /*
	 * 이미지 번호를 전달하면 총 목록과 같은 정렬을 가지고,
	 * 현재 총 또는 소지하고 있는 총의 이미지가 일치하게 됩니다.
	 * 현재 선택된 총의 이미지는 약간 확대됩니다.
	 */
    void DrawCorrespondingImage(int _number)
    {
        string deleteCloneFromName = currentGun.name.Substring(0, currentGun.name.Length - 7);

        if (menuStyle == MenuStyle.horizontal)
        {
            if (deleteCloneFromName == gunsIHave[_number])
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing)), vec2(beginPosition).y, // 위치 변수
                    vec2(size).x, vec2(size).y), // 크기
                    icons[_number]);
            }
            else
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing) + 10), vec2(beginPosition).y + 10, // 위치 변수
                    vec2(size).x - 20, vec2(size).y - 20), // 크기
                    icons[_number]);
            }
        }
        else if (menuStyle == MenuStyle.vertical)
        {
            if (deleteCloneFromName == gunsIHave[_number])
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + (_number * position_y(spacing)), // 위치 변수
                    vec2(size).x, vec2(size).y), // 크기
                    icons[_number]);
            }
            else
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + 10 + (_number * position_y(spacing)), // 위치 변수
                    vec2(size).x - 20, vec2(size).y - 20), // 크기
                    icons[_number]);
            }
        }
    }

    /*
	 * 플레이어가 죽을 때 이 메서드를 호출합니다.
	 */
    public void DeadMethod()
    {
        Destroy(currentGun);
        Destroy(this);
    }

    //#####		GUI 이미지의 크기와 위치 반환
    //(백분율을 전달하면 화면에서 해당 비율로 나타나는 숫자를 반환합니다) ##################
    private float position_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float position_y(float var)
    {
        return Screen.height * var / 100;
    }
    private float size_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float size_y(float var)
    {
        return Screen.height * var / 100;
    }
    private Vector2 vec2(Vector2 _vec2)
    {
        return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
    }
    //######################################################

    /*
	 * 소리
	 */
    [Header("소리")]
    [Tooltip("무기 변경 소리.")]
    public AudioSource weaponChanging;
}
