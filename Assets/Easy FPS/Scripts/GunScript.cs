using System.Collections;
using UnityEngine;
//using UnityStandardAssets.ImageEffects;

public enum GunStyles
{
    nonautomatic, automatic
}
public class GunScript : MonoBehaviour
{
    [Tooltip("빠르게 발사할 무기 유형 또는 클릭당 한 발을 선택합니다.")]
    public GunStyles currentStyle;
    [HideInInspector]
    public MouseLookScript mls;

    [Header("플레이어 이동 속성")]
    [Tooltip("속도는 무기에 따라 결정됩니다. 모든 무기가 동일한 특성이나 무게를 가지지 않으므로 여기에 속도를 설정해야 합니다.")]
    public int walkingSpeed = 3;
    [Tooltip("속도는 무기에 따라 결정됩니다. 모든 무기가 동일한 특성이나 무게를 가지지 않으므로 여기에 속도를 설정해야 합니다.")]
    public int runningSpeed = 5;

    [Header("총알 속성")]
    [Tooltip("우리의 무기가 옆으로 생성할 총알의 수를 설정하는 기본값입니다.")]
    public float bulletsIHave = 20;
    [Tooltip("우리의 무기가 소총 내부에 생성할 총알의 수를 설정하는 기본값입니다.")]
    public float bulletsInTheGun = 5;
    [Tooltip("하나의 탄창이 수용할 수 있는 총알의 수를 설정하는 기본값입니다.")]
    public float amountOfBulletsPerLoad = 5;

    private Transform player;
    private Camera cameraComponent;
    private Transform gunPlaceHolder;

    private PlayerMovementScript pmS;

    /*
	 * Collection the variables upon awake that we need.
	 */
    void Awake()
    {


        mls = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLookScript>();
        player = mls.transform;
        mainCamera = mls.myCamera;
        secondCamera = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();
        cameraComponent = mainCamera.GetComponent<Camera>();
        pmS = player.GetComponent<PlayerMovementScript>();

        bulletSpawnPlace = GameObject.FindGameObjectWithTag("BulletSpawn");
        hitMarker = transform.Find("hitMarkerSound").GetComponent<AudioSource>();

        startLook = mouseSensitvity_notAiming;
        startAim = mouseSensitvity_aiming;
        startRun = mouseSensitvity_running;

        rotationLastY = mls.currentYRotation;
        rotationLastX = mls.currentCameraXRotation;

    }


    [HideInInspector]
    public Vector3 currentGunPosition;
    [Header("총 위치 조정")]
    [Tooltip("비조준 값에 대한 플레이어의 Vector 3 위치 설정입니다.")]
    public Vector3 restPlacePosition;
    [Tooltip("조준 값에 대한 플레이어의 Vector 3 위치 설정입니다.")]
    public Vector3 aimPlacePosition;
    [Tooltip("총이 조준 자세로 들어가는 데 걸리는 시간입니다.")]
    public float gunAimTime = 0.1f;


    [HideInInspector]
    public bool reloading;

    private Vector3 gunPosVelocity;
    private float cameraZoomVelocity;
    private float secondCameraZoomVelocity;

    private Vector2 gunFollowTimeVelocity;

    /*
시작되는 위치 아래에 설명된 메서드를 호출하는 업데이트 루프입니다.
*/
    void Update()
    {

        Animations();

        GiveCameraScriptMySensitvity();

        PositionGun();

        Shooting();
        MeeleAttack();
        LockCameraWhileMelee();

        Sprint(); // 여기서 질주하는 총이 있다면, 총이 없다면 이동 스크립트에서 호출됩니다.


        CrossHairExpansionWhenWalking();


    }

    /*
	*시작되는 위치 아래에 설명된 메서드를 호출하는 업데이트 루프입니다.
	*+
	*조준 여부에 따른 무기 위치 계산.
	*/
    void FixedUpdate()
    {
        RotationGun();

        MeeleAnimationsStates();

        /*
        * 감도, 줌 비율, 웨폰 위치 등 조준 시 일부 값을 변경합니다.
        */
        //조준하는 경우
        if (Input.GetAxis("Fire2") != 0 && !reloading && !meeleAttack)
        {
            gunPrecision = gunPrecision_aiming;
            recoilAmount_x = recoilAmount_x_;
            recoilAmount_y = recoilAmount_y_;
            recoilAmount_z = recoilAmount_z_;
            currentGunPosition = Vector3.SmoothDamp(currentGunPosition, aimPlacePosition, ref gunPosVelocity, gunAimTime);
            cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
            secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
        }
        //if not aiming
        else
        {
            gunPrecision = gunPrecision_notAiming;
            recoilAmount_x = recoilAmount_x_non;
            recoilAmount_y = recoilAmount_y_non;
            recoilAmount_z = recoilAmount_z_non;
            currentGunPosition = Vector3.SmoothDamp(currentGunPosition, restPlacePosition, ref gunPosVelocity, gunAimTime);
            cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_notAiming, ref cameraZoomVelocity, gunAimTime);
            secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_notAiming, ref secondCameraZoomVelocity, gunAimTime);
        }

    }
    [Header("총의 감도")]
    [Tooltip("조준하지 않을 때 이 총의 감도입니다.")]
    public float mouseSensitvity_notAiming = 10;
    //[HideInInspector]
    [Tooltip("조준할 때 이 총의 감도입니다.")]
    public float mouseSensitvity_aiming = 5;
    //[HideInInspector]
    [Tooltip("달리는 동안 이 총의 감도입니다.")]
    public float mouseSensitvity_running = 4;
    /*
     * 각 총에 대해 메인 카메라에 다양한 감도 옵션을 제공하는 데 사용됩니다.
     */

    void GiveCameraScriptMySensitvity()
    {
        mls.mouseSensitvity_notAiming = mouseSensitvity_notAiming;
        mls.mouseSensitvity_aiming = mouseSensitvity_aiming;
    }

    /*
	 * Used to expand position of the crosshair or make it dissapear when running
	 */
    void CrossHairExpansionWhenWalking()
    {

        if (player.GetComponent<Rigidbody>().velocity.magnitude > 1 && Input.GetAxis("Fire1") == 0)
        {//ifnot shooting

            expandValues_crosshair += new Vector2(20, 40) * Time.deltaTime;
            if (player.GetComponent<PlayerMovementScript>().maxSpeed < runningSpeed)
            { //not running
                expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
                fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);
            }
            else
            {//running
                fadeout_value = Mathf.Lerp(fadeout_value, 0, Time.deltaTime * 10);
                expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 20), Mathf.Clamp(expandValues_crosshair.y, 0, 40));
            }
        }
        else
        {//if shooting
            expandValues_crosshair = Vector2.Lerp(expandValues_crosshair, Vector2.zero, Time.deltaTime * 5);
            expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
            fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);

        }

    }

    /* 
  * 플레이어가 이동할 수 있는 최대 속도를 변경합니다.
  * 또한 최대 속도는 애니메이터와 연결되어 있어 달리기 애니메이션을 트리거합니다.
  */

    void Sprint()
    {// Running();  so i can find it with CTRL + F
        if (Input.GetAxis("Vertical") > 0 && Input.GetAxisRaw("Fire2") == 0 && meeleAttack == false && Input.GetAxisRaw("Fire1") == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (pmS.maxSpeed == walkingSpeed)
                {
                    pmS.maxSpeed = runningSpeed;//sets player movement peed to max

                }
                else
                {
                    pmS.maxSpeed = walkingSpeed;
                }
            }
        }
        else
        {
            pmS.maxSpeed = walkingSpeed;
        }

    }

    [HideInInspector]
    public bool meeleAttack;
    [HideInInspector]
    public bool aiming;
    /*
     * 이미 근접 공격이 실행 중인지 확인합니다.
     * 재장전 중이 아니라면 IENumerator에서 근접 공격 애니메이션을 트리거할 수 있습니다.
     */
    void MeeleAnimationsStates()
    {
        if (handsAnimator)
        {
            meeleAttack = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(meeleAnimationName);
            aiming = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(aimingAnimationName);
        }
    }
    /*
     * 사용자가 키보드의 Q 키를 눌러 근접 공격을 입력하면 애니메이션 및 피해 공격을 위한 코루틴을 시작합니다.
     */

    void MeeleAttack()
    {

        if (Input.GetKeyDown(KeyCode.Q) && !meeleAttack)
        {
            StartCoroutine("AnimationMeeleAttack");
        }
    }
    /*
	* Sets meele animation to play.
	*/
    IEnumerator AnimationMeeleAttack()
    {
        handsAnimator.SetBool("meeleAttack", true);
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        handsAnimator.SetBool("meeleAttack", false);
    }

    private float startLook, startAim, startRun;
    /*
	* Setting the mouse sensitvity lower when meele attack and waits till it ends.
	*/
    void LockCameraWhileMelee()
    {
        if (meeleAttack)
        {
            mouseSensitvity_notAiming = 2;
            mouseSensitvity_aiming = 1.6f;
            mouseSensitvity_running = 1;
        }
        else
        {
            mouseSensitvity_notAiming = startLook;
            mouseSensitvity_aiming = startAim;
            mouseSensitvity_running = startRun;
        }
    }


    private Vector3 velV;
    [HideInInspector]
    public Transform mainCamera;
    private Camera secondCamera;
    /*
	 * Calculatin the weapon position accordingly to the player position and rotation.
	 * After calculation the recoil amount are decreased to 0.
	 */
    void PositionGun()
    {
        transform.position = Vector3.SmoothDamp(transform.position,
            mainCamera.transform.position -
            (mainCamera.transform.right * (currentGunPosition.x + currentRecoilXPos)) +
            (mainCamera.transform.up * (currentGunPosition.y + currentRecoilYPos)) +
            (mainCamera.transform.forward * (currentGunPosition.z + currentRecoilZPos)), ref velV, 0);



        pmS.cameraPosition = new Vector3(currentRecoilXPos, currentRecoilYPos, 0);

        currentRecoilZPos = Mathf.SmoothDamp(currentRecoilZPos, 0, ref velocity_z_recoil, recoilOverTime_z);
        currentRecoilXPos = Mathf.SmoothDamp(currentRecoilXPos, 0, ref velocity_x_recoil, recoilOverTime_x);
        currentRecoilYPos = Mathf.SmoothDamp(currentRecoilYPos, 0, ref velocity_y_recoil, recoilOverTime_y);

    }


    [Header("회전")]
    private Vector2 velocityGunRotate;
    private float gunWeightX, gunWeightY;
    [Tooltip("무기가 카메라 뷰에 따라 지연되는 시간. '0'으로 설정하는 것이 가장 좋습니다.")]
    public float rotationLagTime = 0f;
    private float rotationLastY;
    private float rotationDeltaY;
    private float angularVelocityY;
    private float rotationLastX;
    private float rotationDeltaX;
    private float angularVelocityX;
    [Tooltip("앞으로 회전 배수 값.")]
    public Vector2 forwardRotationAmount = Vector2.one;
    /*
     * 마우스 시점 회전에 따라 무기를 회전시킵니다.
     * Call Of Duty의 무기 무게처럼 앞으로 회전을 계산합니다.
     */

    void RotationGun()
    {

        rotationDeltaY = mls.currentYRotation - rotationLastY;
        rotationDeltaX = mls.currentCameraXRotation - rotationLastX;

        rotationLastY = mls.currentYRotation;
        rotationLastX = mls.currentCameraXRotation;

        angularVelocityY = Mathf.Lerp(angularVelocityY, rotationDeltaY, Time.deltaTime * 5);
        angularVelocityX = Mathf.Lerp(angularVelocityX, rotationDeltaX, Time.deltaTime * 5);

        gunWeightX = Mathf.SmoothDamp(gunWeightX, mls.currentCameraXRotation, ref velocityGunRotate.x, rotationLagTime);
        gunWeightY = Mathf.SmoothDamp(gunWeightY, mls.currentYRotation, ref velocityGunRotate.y, rotationLagTime);

        transform.rotation = Quaternion.Euler(gunWeightX + (angularVelocityX * forwardRotationAmount.x), gunWeightY + (angularVelocityY * forwardRotationAmount.y), 0);
    }

    private float currentRecoilZPos;
    private float currentRecoilXPos;
    private float currentRecoilYPos;
    /*
	 * Called from ShootMethod();, upon shooting the recoil amount will increase.
	 */
    public void RecoilMath()
    {
        currentRecoilZPos -= recoilAmount_z;
        currentRecoilXPos -= (Random.value - 0.5f) * recoilAmount_x;
        currentRecoilYPos -= (Random.value - 0.5f) * recoilAmount_y;
        mls.wantedCameraXRotation -= Mathf.Abs(currentRecoilYPos * gunPrecision);
        mls.wantedYRotation -= (currentRecoilXPos * gunPrecision);

        expandValues_crosshair += new Vector2(6, 12);

    }

    [Header("사격 설정 - 필수")]
    [HideInInspector] public GameObject bulletSpawnPlace;
    [Tooltip("이 무기가 발사할 총알 프리팹입니다.")]
    public GameObject bullet;
    [Tooltip("무기가 자동 발사로 설정된 경우 초당 발사 수입니다.")]
    public float roundsPerSecond;
    private float waitTillNextFire;

    /*
	 * Checking if the gun is automatic or nonautomatic and accordingly runs the ShootMethod();.
	 */
    // 이 부분 따로 수정함
    void Shooting()
    {
        if (meeleAttack) return; // Exit early if melee attack is active.

        if (currentStyle == GunStyles.nonautomatic)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                ShootMethod();
            }
        }
        else if (currentStyle == GunStyles.automatic)
        {
            if (Input.GetButton("Fire1"))
            {
                ShootMethod();
            }
        }
        waitTillNextFire -= roundsPerSecond * Time.deltaTime;
    }


    [HideInInspector] public float recoilAmount_z = 0.5f;
    [HideInInspector] public float recoilAmount_x = 0.5f;
    [HideInInspector] public float recoilAmount_y = 0.5f;
    [Header("Recoil Not Aiming")]
    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    public float recoilAmount_z_non = 0.5f;
    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    public float recoilAmount_x_non = 0.5f;
    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    public float recoilAmount_y_non = 0.5f;
    [Header("Recoil Aiming")]
    [Tooltip("Recoil amount on that AXIS while aiming")]
    public float recoilAmount_z_ = 0.5f;
    [Tooltip("Recoil amount on that AXIS while aiming")]
    public float recoilAmount_x_ = 0.5f;
    [Tooltip("Recoil amount on that AXIS while aiming")]
    public float recoilAmount_y_ = 0.5f;
    [HideInInspector] public float velocity_z_recoil, velocity_x_recoil, velocity_y_recoil;
    [Header("")]
    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    public float recoilOverTime_z = 0.5f;
    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    public float recoilOverTime_x = 0.5f;
    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    public float recoilOverTime_y = 0.5f;

    [Header("Gun Precision")]
    [Tooltip("Gun rate precision when player is not aiming. THis is calculated with recoil.")]
    public float gunPrecision_notAiming = 200.0f;
    [Tooltip("Gun rate precision when player is aiming. THis is calculated with recoil.")]
    public float gunPrecision_aiming = 100.0f;
    [Tooltip("FOV of first camera when NOT aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    public float cameraZoomRatio_notAiming = 60;
    [Tooltip("FOV of first camera when aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    public float cameraZoomRatio_aiming = 40;
    [Tooltip("FOV of second camera when NOT aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    public float secondCameraZoomRatio_notAiming = 60;
    [Tooltip("FOV of second camera when aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    public float secondCameraZoomRatio_aiming = 40;
    [HideInInspector]
    public float gunPrecision;

    [Tooltip("Audios for shootingSound, and reloading.")]
    public AudioSource shoot_sound_source, reloadSound_source;
    [Tooltip("Sound that plays after successful attack bullet hit.")]
    public static AudioSource hitMarker;

    /*
     * 타겟에 명중했을 때 호출되는 소리.
     */
    public static void HitMarkerSound()
    {
        hitMarker.Play();
    }

    [Tooltip("총알이 발사될 때 무작위로 하나가 나타나는 총구 플래시 배열입니다.")]
    public GameObject[] muzzelFlash;
    [Tooltip("총구 플래시가 나타날 총의 위치.")]
    public GameObject muzzelSpawn;
    private GameObject holdFlash;
    private GameObject holdSmoke;
    /*
     * Shooting()에서 호출됩니다.
     * 총알과 총구 플래시를 생성하고 반동을 호출합니다.
     */

    private void ShootMethod()
    {
        if (waitTillNextFire <= 0 && !reloading && pmS.maxSpeed < 5)
        {

            if (bulletsInTheGun > 0)
            {

                int randomNumberForMuzzelFlash = Random.Range(0, 5);
                if (bullet)
                    Instantiate(bullet, bulletSpawnPlace.transform.position, bulletSpawnPlace.transform.rotation);
                else
                    print("Missing the bullet prefab");
                holdFlash = Instantiate(muzzelFlash[randomNumberForMuzzelFlash], muzzelSpawn.transform.position /*- muzzelPosition*/, muzzelSpawn.transform.rotation * Quaternion.Euler(0, 0, 90)) as GameObject;
                holdFlash.transform.parent = muzzelSpawn.transform;
                if (shoot_sound_source)
                    shoot_sound_source.Play();
                else
                    print("Missing 'Shoot Sound Source'.");

                RecoilMath();

                waitTillNextFire = 1;
                bulletsInTheGun -= 1;
            }

            else
            {
                //if(!aiming)
                StartCoroutine("Reload_Animation");
                //if(emptyClip_sound_source)
                //	emptyClip_sound_source.Play();
            }

        }

    }


    /*
     * 재장전, 애니메이터에 재장전 설정,
     * 2초를 기다린 후 재장전된 클립 설정.
     */
    [Header("애니메이션 후 재장전 시간")]
    [Tooltip("재장전 후 경과하는 시간. 재장전 애니메이션 길이에 따라 다릅니다. 재장전은 근접 공격이나 달리기로 중단될 수 있으므로 이 작업이 완료되기 전에 수행하는 모든 동작은 재장전을 중단합니다.")]
    public float reloadChangeBulletsTime;

    IEnumerator Reload_Animation()
    {
        if (bulletsIHave > 0 && bulletsInTheGun < amountOfBulletsPerLoad && !reloading/* && !aiming*/)
        {

            if (reloadSound_source.isPlaying == false && reloadSound_source != null)
            {
                if (reloadSound_source)
                    reloadSound_source.Play();
                else
                    print("'재장전 사운드 소스'가 누락되었습니다.");
            }

            handsAnimator.SetBool("reloading", true);
            yield return new WaitForSeconds(0.5f);
            handsAnimator.SetBool("reloading", false);

            yield return new WaitForSeconds(reloadChangeBulletsTime - 0.5f); // 기다리는 시간에서 해당 시간 차감
            if (meeleAttack == false && pmS.maxSpeed != runningSpeed)
            {
                //print ("여기 있습니다.");
                if (player.GetComponent<PlayerMovementScript>()._freakingZombiesSound)
                    player.GetComponent<PlayerMovementScript>()._freakingZombiesSound.Play();
                else
                    print("사운드가 누락되었습니다: Freaking Zombies Sound");

                if (bulletsIHave - amountOfBulletsPerLoad >= 0)
                {
                    bulletsIHave -= amountOfBulletsPerLoad - bulletsInTheGun;
                    bulletsInTheGun = amountOfBulletsPerLoad;
                }
                else if (bulletsIHave - amountOfBulletsPerLoad < 0)
                {
                    float valueForBoth = amountOfBulletsPerLoad - bulletsInTheGun;
                    if (bulletsIHave - valueForBoth < 0)
                    {
                        bulletsInTheGun += bulletsIHave;
                        bulletsIHave = 0;
                    }
                    else
                    {
                        bulletsIHave -= valueForBoth;
                        bulletsInTheGun += valueForBoth;
                    }
                }
            }
            else
            {
                reloadSound_source.Stop();
                print("근접 공격으로 재장전이 중단되었습니다.");
            }
        }
    }

    /*
     * HUD UI 게임 오브젝트에 총알 수를 설정합니다. 
     * 여기서 조준선(CrossHair)을 그립니다.
     */
    [Tooltip("화면에 총알 수를 표시하는 HUD 총알. 씬에서 'HUD_bullets'라는 이름 아래에 찾을 수 있습니다.")]
    public TextMesh HUD_bullets;

    void OnGUI()
    {
        if (!HUD_bullets)
        {
            try
            {
                HUD_bullets = GameObject.Find("HUD_bullets").GetComponent<TextMesh>();
            }
            catch (System.Exception ex)
            {
                print("HUD_Bullets를 찾을 수 없습니다. ->" + ex.StackTrace.ToString());
            }
        }
        if (mls && HUD_bullets)
            HUD_bullets.text = bulletsIHave.ToString() + " - " + bulletsInTheGun.ToString();

        DrawCrosshair();
    }

    [Header("조준선 속성")]
    public Texture horizontal_crosshair, vertical_crosshair;
    public Vector2 top_pos_crosshair, bottom_pos_crosshair, left_pos_crosshair, right_pos_crosshair;
    public Vector2 size_crosshair_vertical = new Vector2(1, 1), size_crosshair_horizontal = new Vector2(1, 1);
    [HideInInspector]
    public Vector2 expandValues_crosshair;
    private float fadeout_value = 1;

    /*
     * 조준선을 그립니다.
     */
    void DrawCrosshair()
    {
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, fadeout_value);
        if (Input.GetAxis("Fire2") == 0)
        {// 조준하지 않을 경우 그리기
            GUI.DrawTexture(new Rect(vec2(left_pos_crosshair).x + position_x(-expandValues_crosshair.x) + Screen.width / 2, Screen.height / 2 + vec2(left_pos_crosshair).y, vec2(size_crosshair_horizontal).x, vec2(size_crosshair_horizontal).y), vertical_crosshair);// 왼쪽
            GUI.DrawTexture(new Rect(vec2(right_pos_crosshair).x + position_x(expandValues_crosshair.x) + Screen.width / 2, Screen.height / 2 + vec2(right_pos_crosshair).y, vec2(size_crosshair_horizontal).x, vec2(size_crosshair_horizontal).y), vertical_crosshair);// 오른쪽

            GUI.DrawTexture(new Rect(vec2(top_pos_crosshair).x + Screen.width / 2, Screen.height / 2 + vec2(top_pos_crosshair).y + position_y(-expandValues_crosshair.y), vec2(size_crosshair_vertical).x, vec2(size_crosshair_vertical).y), horizontal_crosshair);// 위
            GUI.DrawTexture(new Rect(vec2(bottom_pos_crosshair).x + Screen.width / 2, Screen.height / 2 + vec2(bottom_pos_crosshair).y + position_y(expandValues_crosshair.y), vec2(size_crosshair_vertical).x, vec2(size_crosshair_vertical).y), horizontal_crosshair);// 아래
        }
    }

    //#####		GUI 이미지의 크기 및 위치 반환 ##################
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
    //#


    public Animator handsAnimator;
    /*
     * 현재 실행 중인 애니메이션이 있는지 확인합니다.
     * R 키를 눌러 재장전 애니메이션 설정.
     */
    void Animations()
    {

        if (handsAnimator)
        {
            reloading = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(reloadAnimationName);
            handsAnimator.SetFloat("walkSpeed", pmS.currentSpeed);
            handsAnimator.SetBool("aiming", Input.GetButton("Fire2"));
            handsAnimator.SetInteger("maxSpeed", pmS.maxSpeed);
            if (Input.GetKeyDown(KeyCode.R) && pmS.maxSpeed < 5 && !reloading && !meeleAttack/* && !aiming*/)
            {
                StartCoroutine("Reload_Animation");
            }
        }
    }

    [Header("애니메이션 이름")]
    public string reloadAnimationName = "Player_Reload";
    public string aimingAnimationName = "Player_AImpose";
    public string meeleAnimationName = "Character_Malee";
}