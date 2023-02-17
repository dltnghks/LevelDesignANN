using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Common,
        Speed,
        Tanker
    }

    [SerializeField]
    private EnemyType enemyType;

    public float maxHP;     // �ִ� HP
    public float curHP;     // ���� HP
    public float speed;    // �̵� �ӵ�
    private float defaultSpeed; // 기본 이동속도
    [SerializeField]
    private float damage;
    private bool isDie;
    [SerializeField]
    private float hitCoolTime;
    [SerializeField]
    private int bonus; // ����� �� ��� ����

    [SerializeField]
    private int money; // ����� �� �ִ� ��

    [SerializeField]
    private GameObject DieEffect;

    private Transform target; //���� ���� ���(�÷��̾�)
    private NavMeshAgent navMeshAgent; //�̵���� ���� NavMeshAgent
    private bool isHitCoolTime;


    private GameDataSave gameDataSave;  // 게임 데이터

    private EnemySpawner enemySpawner;

    protected void Start()
    {
        curHP = maxHP;
        isHitCoolTime = false;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
    }

    public void SetUp(GameObject gameData, EnemySpawner enemySpawner, float difficultyHP, float difficultyDamage, float difficultySpeed)
    {
        this.gameDataSave = gameData.GetComponent<GameDataSave>();
        this.enemySpawner = enemySpawner;

        // 난이도에 따른 능력치 변화
        // 몬스터 타입에 따른 능력치 변화
        defaultSpeed = gameDataSave.CommonDefaultSpeed;
        if(enemyType == EnemyType.Speed)
        {
            defaultSpeed = gameDataSave.SpeedDefaultSpeed;
            difficultyHP *= gameDataSave.SpeedHp;
            difficultySpeed *= gameDataSave.SpeedSpeed;
            difficultyDamage *= gameDataSave.SpeedDamage;
        }
        /*
        else if(enemyType == EnemyType.Tanker)
        {
            defaultSpeed = gameDataSave.TankerDefaultSpeed;
            difficultyHP *= gameDataSave.TankerHp;
            difficultySpeed *= gameDataSave.TankerSpeed;
            difficultyDamage *= gameDataSave.TankerDamage;
        }
        */
        maxHP = gameDataSave.Difficulty * difficultyHP + gameDataSave.DefaultHP;
        speed = gameDataSave.Difficulty * difficultySpeed + defaultSpeed; //common형은 기본 이동속도 4
        damage = gameDataSave.Difficulty * difficultyDamage + gameDataSave.DefaultDamage;

        hitCoolTime = 0.5f;
        isDie = false;
    }

    protected virtual void Update()
    {
        navMeshAgent.SetDestination(target.position);
    }

    // Hit Cool Time
    private IEnumerator HitCoolTime()
    {
        isHitCoolTime = true;
        yield return new WaitForSeconds(hitCoolTime);
        isHitCoolTime = false;
    }

    // ������ �ޱ�
    public void TakeDamge(float damage)
    {
        curHP -= damage;
        
        target.gameObject.GetComponent<Player>().TransfusionAttack();
        
        if (curHP <= 0 && !isDie)
        {
            isDie = true;
            GameObject clone = Instantiate(DieEffect, transform.position, transform.rotation);
            target.gameObject.GetComponent<Player>().ScoreUP(bonus, money);
            //몬스터 수--
            gameDataSave.DataMinus(GameDataSave.DataType.enemyCount);
            //킬 당 발사 수 더하기
            gameDataSave.DataPlus(GameDataSave.DataType.shotCountPerKillSum, gameDataSave.DataGet(GameDataSave.DataType.shotCountPerKill));
            //킬 당 발사 수 초기화
            gameDataSave.DataSet(GameDataSave.DataType.shotCountPerKill, 0);
            //킬 수 ++
            gameDataSave.DataPlus(GameDataSave.DataType.killCount);
            gameDataSave.DataPlus(GameDataSave.DataType.curWaveKillCount);

            enemySpawner.DeleteEnemyList(this);
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!isHitCoolTime)
            {
                StartCoroutine(HitCoolTime());
                collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            }
        }
    }
}
