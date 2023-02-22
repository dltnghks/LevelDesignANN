using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class Player : MonoBehaviour
{
    private enum WeaponType
    {
        Gun,
        Boom
    }

    // Upgrade Name
    public enum UpgradeName
    {
        UpgradeAttackPower = 0,
        UpgradeSkillCoolTimeDown = 1,
        ExplosionRangeUp = 2,
        MaxHealthUp = 3,
        TransfusionUp = 4,
        DecreaseCoffeine = 5
    }

    public float currentSpeed;
    Rigidbody rigidbody;       
    Vector3 movement;
    Vector3 nextVec;

    // Player
    [Header("Player")]
    [SerializeField]
    private float damage;               //Damage
    public float walkSpeed;             //Walk Speed
    public float runSpeed;              //Run Speed
    [SerializeField]
    private float explosionRange = 3;   //Skill Range
    private float Transfusion = 0;      //Transfusion
    [SerializeField]
    private float ExplosionCoolTime;    //Skill CoolTime
    private float curQCoolTime;         //Current Skill CoolTime
    [SerializeField]
    private float maxHP;                //Max HP
    private float curHP;                //Cuurent HP
    [SerializeField]
    private float decreaseCoffeine;     //Decrease Coffeine;
    [SerializeField]
    private float shotTime;
    private bool isShotCoolTime;
    private bool isDie;
    public bool isSlow;
    [SerializeField]
    private float slowSpeed;        // 느려지는 속도
    [SerializeField]
    private WeaponType weaponType;



    [Header("Object")]
    public Camera followCamera;         //Camera
    // �Ѿ�
    [SerializeField]
    public GameObject bullet;           //Bullet
    public Transform bulletPos;

    [SerializeField]
    private GameObject HitPanel;        //If Hit, Red Panel On

    private PlayerAnimationController playerAnimationController;

    private bool isExplosionOn = false; //Skill ON/OFF
    private bool isExplosionCoolTime = false;



    [Header("Game Info")]
    public int score = 0;               //Score
    [SerializeField]
    private int currentStage;           //Current Stage

    private int money = 0;              //Money
    public int[] needMoney;             //Upgrade Money

    public float coffeine = 100f;       //Coffein Amount

    public bool isShoping;              //Shop ON/OFF

    public bool isClearScene;



    [Header("Game Data")]
    [SerializeField]
    private GameObject gameData;        //GameData Object
    private GameDataSave gameDataSave;  //GameDataSave Component
    private GameObject[] obstacleList;
    private float minDis;



    public float DecreaseCoffeine => decreaseCoffeine;
    public float CurHP => curHP;
    public float MaxHP => maxHP;
    public int Money => money;
    public float CurQCoolTime => curQCoolTime;
    public float explosionCoolTime => ExplosionCoolTime;

    private void Awake()
    {
        gameDataSave = gameData.GetComponent<GameDataSave>();
        rigidbody = GetComponent<Rigidbody>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        obstacleList = GameObject.FindGameObjectsWithTag("Wall");

        isClearScene = false;
        needMoney = new int[6];
        for (int i = 0; i < needMoney.Length; i++)
        {
            needMoney[i] = 1000;
        }

        curHP = maxHP;
        currentSpeed = walkSpeed;
        weaponType = WeaponType.Gun;

        isShoping = false;
        isDie = false;
        isSlow = false;
    }

    float writeTime = 2f;
    float playTime = 0;
    private void Update()
    {
        /*
        playTime += Time.deltaTime;
        if (playTime >= writeTime)
        {
            Debug.Log("전송");
            gameDataSave.packet.x = (int)(gameObject.transform.position.x + 60f);
            gameDataSave.packet.y = (int)(gameObject.transform.position.z + 60f);
            gameDataSave.WritePacket();
            playTime = 0f;
        }
        */
        
        minDis = float.MaxValue;
        for(int i = 0; i < obstacleList.Length; i++){
            float dis = Vector3.Distance(transform.position, obstacleList[i].transform.position);
            if(minDis > dis){
                minDis = dis;
            }
        }
        // �Ѿ� �߻�
        //if (Input.GetMouseButtonDown(0))
        if (Input.GetKeyDown(KeyCode.Q) && !isExplosionCoolTime)
        {

            // Q를 눌렀을 때 무기 변경
            if(weaponType == WeaponType.Gun)
            {
                weaponType = WeaponType.Boom;
                shotTime = 1.2f;
            }
            else
            {
                weaponType = WeaponType.Gun;
                shotTime = 0.07f;
            }
            //isExplosionOn = true;
            isExplosionCoolTime = true;
            gameDataSave.DataSet(GameDataSave.DataType.weaponType, (float)weaponType);
            StartCoroutine(ExplosionCoolTimeOn());
        }
        if (Input.GetMouseButton(0) && !isShoping)
        {
            if (!isShotCoolTime)
            {
                isShotCoolTime = true;
                gameDataSave.DataPlus(GameDataSave.DataType.shotCount);
                gameDataSave.DataPlus(GameDataSave.DataType.shotCountPerKill);
                if (movement != Vector3.zero)
                {
                    gameDataSave.DataPlus(GameDataSave.DataType.moveShotCount);
                }
                // 무기가 Boom이면 폭발 무기로 변경
                if(weaponType == WeaponType.Boom)
                {
                    isExplosionOn = true;
                }
                StartCoroutine(Shot());
                playerAnimationController.OnShoot();
            }
        }
        /* 달리기
        if (movement != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("��");
            currentSpeed = runSpeed;
            if (isSlow) currentSpeed -= slowSpeed;
            playerAnimationController.OnRun();

        }
        */
        // 움직일 때
        if (movement != Vector3.zero)
        {
            playerAnimationController.OnWalk();
            currentSpeed = walkSpeed;
            // 슬로우 필드 위이면
            if (isSlow)
            {
                currentSpeed = walkSpeed - slowSpeed;
            }
        }
        else
        {
            //Debug.Log("����");
            playerAnimationController.OnIdle();
        }
        if(!isClearScene)
            coffeine -= Time.deltaTime * decreaseCoffeine;


        gameDataSave.DataSet(GameDataSave.DataType.playerHP, curHP);

        // 학습하는 경우
        if (gameDataSave.isLearningData)
        {
            gameDataSave.DataPlus(GameDataSave.DataType.obstacleDistance, minDis * Time.deltaTime);
        }
        else
        {
            gameDataSave.DataPlus(GameDataSave.DataType.obstacleDistance, minDis * Time.deltaTime);
        }
        
    }
    
    void FixedUpdate()
    {
        // Ű �Է�
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Debug.Log(h + ", " + v);    // w d :  + , ws : v / ad : h
        
        /*
        if (v == 1)            // w
        {
            if (h == 0)
            {
                if (nextVec.z - transform.position.z >= 0)
                    currentSpeed = runSpeed;
                else
                    currentSpeed = walkSpeed;
            } else if (h == -1)     // a
            {
                if (nextVec.z - transform.position.z >= 0 && nextVec.x - transform.position.x <= 0)
                    currentSpeed = runSpeed;
                else
                    currentSpeed = walkSpeed;
            } else                  // d
            {
                if (nextVec.z - transform.position.z >= 0 && nextVec.x - transform.position.x >= 0)
                    currentSpeed = runSpeed;
                else
                    currentSpeed = walkSpeed;
            }

        }
        else if (v == -1)    // s
        {
            if (h == 0)
            {
                if (nextVec.z - transform.position.z <= 0)
                    currentSpeed = runSpeed;
                else
                    currentSpeed = walkSpeed;
            }
            else if (h == -1)   // a
            {
                if (nextVec.z - transform.position.z <= 0 && nextVec.x - transform.position.x <= 0)
                    currentSpeed = runSpeed;
                else
                    currentSpeed = walkSpeed;
            }
            else                // d
            {
                if (nextVec.z - transform.position.z <= 0 && nextVec.x - transform.position.x >= 0)
                    currentSpeed = runSpeed;
                else
                    currentSpeed = walkSpeed;
            }
        }
        else if (v == 0 && h == 1)     // d
        {
            if (nextVec.x - transform.position.x >= 0)
                currentSpeed = runSpeed;
            else
                currentSpeed = walkSpeed;
        }
        else if (v == 0 && h == -1)    // a
        {
            if (nextVec.x - transform.position.x <= 0)
                currentSpeed = runSpeed;
            else
                currentSpeed = walkSpeed;
        }
        */
        Run(h, v);
        Turn();
        // ȸ��

    }

    // �÷��̾� �̵�
    void Run(float h, float v)
    {
        movement.Set(h, 0, v);
        movement = movement.normalized * currentSpeed * Time.deltaTime;
        rigidbody.MovePosition(transform.position + movement);
    }

    // �÷��̾� ���콺 ��ġ �ٶ󺸱�
    void Turn()
    {
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("Player"));
        Ray ray;
        // ���콺 ��ġ ���� �ٶ󺸱�
        try
        {
            ray = followCamera.ScreenPointToRay(Input.mousePosition);
        }
        catch
        {
            followCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            ray = followCamera.ScreenPointToRay(Input.mousePosition);
        }
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100, layerMask))
        {
            nextVec = rayHit.point;
            nextVec.y = transform.position.y;
            // �ٶ󺸱�
            transform.LookAt(nextVec);
        }
    }

    IEnumerator ShotCoolTime()
    {
        yield return new WaitForSeconds(shotTime);
        isShotCoolTime = false;
    }
    // �Ѿ� �߻�
    IEnumerator Shot()
    {
        StartCoroutine(ShotCoolTime());
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);  // �Ѿ� ����
        intantBullet.GetComponent<Bullet>().SetUp(isExplosionOn, damage, Transfusion, explosionRange, gameObject, gameDataSave);
        isExplosionOn = false;
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();                         // �Ѿ� Rigidbody ��������
        bulletRigid.velocity = bulletPos.forward * 50;                                          // ������ �̵�

        yield return null;
    }

    // ���� ��ų ��Ÿ��
    private IEnumerator ExplosionCoolTimeOn()
    {
        curQCoolTime = ExplosionCoolTime;
        while (curQCoolTime >= 0)
        {
            //Debug.Log("coolTime : " + curQCoolTime);
            float tmpTime = Time.deltaTime;
            curQCoolTime -= tmpTime;
            yield return new WaitForSeconds(tmpTime);
        }
        isExplosionCoolTime = false;
        yield return null;
    }


    private IEnumerator Hit()
    {
        try
        {
            HitPanel.GetComponent<Image>().color = new Color(1, 0, 0, 0.2f);
        }
        catch
        {
            HitPanel = GameObject.FindGameObjectWithTag("HitPanel");
            HitPanel.GetComponent<Image>().color = new Color(1, 0, 0, 0.2f);
        }
            yield return new WaitForSeconds(0.5f);
        try
        {
            HitPanel.GetComponent<Image>().color = new Color(1, 0, 0, 0);
        }
        catch
        {
            HitPanel = GameObject.FindGameObjectWithTag("HitPanel");
            HitPanel.GetComponent<Image>().color = new Color(1, 0, 0, 0);
        }
    }

    // �������� ����
    public void TakeDamage(float damage)
    {
        if (!isDie)
        {
            curHP -= damage;
            StartCoroutine("Hit");
            if (curHP <= 0)
            {
                curHP = 0;
                isDie = true;
                StartCoroutine(GameOver());
            }
        }
        //Debug.Log(curHP);
    }

    // ȸ��
    public void TakeHeal(float heal)
    {
        curHP += heal;
        if (curHP >= maxHP)
        {
            curHP = maxHP;
        }
    }

    // ī���� ����
    public void TakeCoffee(float coffeine)
    {
        this.coffeine += coffeine;
        if (this.coffeine >= 100)
        {
            this.coffeine = 100;
        }
    }

    public void TransfusionAttack()
    {
        TakeHeal(damage * (Transfusion / 100));
    }

    // ���� ȹ��
    public void ScoreUP(int bonus, int money)
    {
        score += bonus;
        this.money += money;
    }

    IEnumerator GameOver()
    {
        gameDataSave.SetOutPut();
        gameDataSave.PlayerDie();
        playerAnimationController.OnDie();
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene("GameOver");
        //SceneManager.LoadScene("Ranking");
    }


    //***********************Upgrade �Լ���*****************************/

    // ������ ����
    public void UpgradeAttackPower()
    {
        if(money >= needMoney[(int)UpgradeName.UpgradeAttackPower])
        {
            money -= needMoney[(int)UpgradeName.UpgradeAttackPower];
            damage += 5;
            needMoney[(int)UpgradeName.UpgradeAttackPower] += 500;
        }
    }

    // ��ų ��Ÿ�� ����
    public void UpgradeSkillCoolTimeDown()
    {
        if (money >= needMoney[(int)UpgradeName.UpgradeSkillCoolTimeDown])
        {
            money -= needMoney[(int)UpgradeName.UpgradeSkillCoolTimeDown];
            ExplosionCoolTime -= 1;
            needMoney[(int)UpgradeName.UpgradeSkillCoolTimeDown] += 500;
        }
    }

    // ��ų ���� ����
    public void UpgradeExplosionRangeUp()
    {
        if (money >= needMoney[(int)UpgradeName.ExplosionRangeUp])
        {
            money -= needMoney[(int)UpgradeName.ExplosionRangeUp];
            explosionRange += 1;
            needMoney[(int)UpgradeName.ExplosionRangeUp] += 500;
        }
    }

    // �ִ� ü�� ����
    public void UpgradeMaxHealthUp()
    {
        if (money >= needMoney[(int)UpgradeName.MaxHealthUp])
        {
            money -= needMoney[(int)UpgradeName.MaxHealthUp];
            maxHP += 20;
            needMoney[(int)UpgradeName.MaxHealthUp] += 500;
        }
    }

    // ���� ��ų
    public void UpgradeTransfusionUp()
    {
        if (money >= needMoney[(int)UpgradeName.TransfusionUp])
        {
            money -= needMoney[(int)UpgradeName.TransfusionUp];
            Transfusion += 2;
            needMoney[(int)UpgradeName.TransfusionUp] += 500;
        }
    }

    // ī���� ������ ����
    public void UpgradeDecreaseCoffeine()
    {
        if (money >= needMoney[(int)UpgradeName.DecreaseCoffeine])
        {
            money -= needMoney[(int)UpgradeName.DecreaseCoffeine];
            decreaseCoffeine -= 0.1f;
            needMoney[(int)UpgradeName.DecreaseCoffeine] += 500;
        }
    }

}
