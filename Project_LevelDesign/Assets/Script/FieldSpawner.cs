using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSpawner : Spawner
{
    private List<Enemy> FieldList;

    private bool isSpawn;

    [SerializeField]
    private GameObject[] centerRange;
    private BoxCollider[] centerCollider;
    private bool[] isCenterSlowField;
    [SerializeField]
    private GameObject[] sideRange;
    protected BoxCollider[] sideCollider;
    private bool[] isSideSlowField;

    int index;

    // 필드가 스폰될 프리팹 인덱스 지정
    private int startIndex;
    private int endIndex;
    [SerializeField]
    private float centerRate;
    [SerializeField]
    private float sideRate;

    int count = 0; // 생성 수
    int spawnPoint; // 생성 위치 0 : 센터, 1 : 사이드

    // 생성 비율
    private float[,] spawnRate = { { 0.8f, 0.2f }, { 0.5f, 0.5f }, { 0.2f, 0.8f } };

    private void Awake()
    {
        gameData = GameObject.FindGameObjectWithTag("GameData");
        gameDataSave = gameData.GetComponent<GameDataSave>();

        // 초기화
        centerCollider = new BoxCollider[centerRange.Length];
        isCenterSlowField = new bool[centerRange.Length];
        for (int i = 0; i < centerRange.Length; i++)
        {
            isCenterSlowField[i] = false;
            centerCollider[i] = centerRange[i].GetComponent<BoxCollider>();
        }

        sideCollider = new BoxCollider[sideRange.Length];
        isSideSlowField = new bool[sideRange.Length];
        for (int i = 0; i < sideRange.Length; i++)
        {
            isSideSlowField[i] = false;
            sideCollider[i] = sideRange[i].GetComponent<BoxCollider>();
        }

    }
    private void Start()
    {
        maxSpawnCount = (int)gameDataSave.SlowFieldAmount;
        isSpawn = false;

    }

    private void Update()
    {
    }

    public void SetUp()
    {
        // 학습 데이터 쌓을 때는 랜덤으로 계속
        if (gameDataSave.isLearningData && gameDataSave.Difficulty == 1)
        {
            int num = Random.Range(0, 3);
            // 비율 설정
            centerRate = spawnRate[num,0];
            sideRate = spawnRate[num,1];

            gameDataSave.DataSet(GameDataSave.DataType.slowFieldCenter, centerRate);
            gameDataSave.DataSet(GameDataSave.DataType.slowFieldSide, sideRate);

        }
        // 인공신경망일 경우 1스테이지만 랜덤
        else if(!gameDataSave.isLearningData && gameDataSave.Difficulty == 1)
        {
            // 비율 설정
            int num = Random.Range(0, 3);
            centerRate = spawnRate[num,0];
            sideRate = spawnRate[num,1];

            gameDataSave.DataSet(GameDataSave.DataType.slowFieldCenter, centerRate);
            gameDataSave.DataSet(GameDataSave.DataType.slowFieldSide, sideRate);
        }

        if (!isSpawn)
        {

            // 필드 스폰 초기화
            //Debug.Log(isCenterSlowField[0]);
            for(int i = 0; i < isCenterSlowField.Length; i++)
            {
                isCenterSlowField[i] = false;
            }
            for(int i = 0; i < isSideSlowField.Length; i++)
            {
                isSideSlowField[i] = false;
            }

            Debug.Log("Slow Field Spawn");
            // 스테이지 클리어 시 필드 삭제
            GameObject[] gameObject = GameObject.FindGameObjectsWithTag("SlowField");
            for (int i = 0; i < gameObject.Length; i++)
                Destroy(gameObject[i]);
            maxSpawnCount = (int)gameDataSave.SlowFieldAmount;
            StartCoroutine(RandomRespawn_Coroutine());
            isSpawn = true;

            // 플레이어 속도 감소 없애기
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Player>().isSlow = false;
        }
        isSpawn = false;

        //Debug.Log("Start : " + centerRate);
    }

    protected override IEnumerator RandomRespawn_Coroutine()
    {
        // 필드 생성 수 초기화
        count = 0;
        for (int i = 0; i < maxSpawnCount; i++)
        {
            // 랜덤 오브젝트 선택
            int num = Random.Range(0, prefabs.Length);
            // 생성 위치 표시(1초)
            Vector3 vec = Return_RandomPosition(); // 랜덤 위치
            //GameObject pointObject = Instantiate(preObject, vec, transform.rotation);
            //yield return new WaitForSeconds(1f);
            //Destroy(pointObject);

            // 오브젝트 생성
            vec.y = 0.01f;
            GameObject slowFieldObject = Instantiate(prefabs[num], vec, Quaternion.identity);
            // slowField 사이즈 랜덤
            float slowFieldScaleX = 0;
            float slowFieldScaleZ = 0;
            if (spawnPoint == 1)
            {
                slowFieldScaleX = Random.Range(0.7f, 0.9f);
                slowFieldScaleZ = Random.Range(0.7f, 0.9f);
            }
            else
            {
                slowFieldScaleX = Random.Range(0.7f, 0.9f);
                slowFieldScaleZ = Random.Range(0.7f, 0.9f);
            }
            slowFieldObject.transform.localScale = new Vector3(slowFieldScaleX, 1, slowFieldScaleZ);
        }
        yield return null;
    }

    protected override Vector3 Return_RandomPosition()
    {
        
        // 센터 비율만큼 생성
        if (count < centerRate * maxSpawnCount)
        {
            while (true) {
                index = Random.Range(0, centerCollider.Length);
                if(isCenterSlowField[index] == false)
                {
                    isCenterSlowField[index] = true;
                    break;
                }
            }
            spawnPoint = 0;
        }
        // 사이드 비율만큼 생성
        else
        {
            while (true)
            {
                index = Random.Range(0, sideCollider.Length);
                if (isSideSlowField[index] == false)
                {
                    isSideSlowField[index] = true;
                    break;
                }
            }
            spawnPoint = 1;
        }
        // 필드 생성 수 ++
        count++;


        // center인 경우
        if (spawnPoint == 0)
        {
            Vector3 originPosition = centerCollider[index].transform.position;
            float range_X = centerCollider[index].bounds.size.x;
            float range_Z = centerCollider[index].bounds.size.z;

            range_X = Random.Range((range_X / 2) * -1, range_X / 2);
            range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
            Vector3 RandomPostion = new Vector3(range_X, 0f, range_Z);

            Vector3 respawnPosition = originPosition + RandomPostion;
            return respawnPosition;
        }
        else
        {
            Vector3 originPosition = sideCollider[index].transform.position;
            float range_X = sideCollider[index].bounds.size.x;
            float range_Z = sideCollider[index].bounds.size.z;

            range_X = Random.Range((range_X / 2) * -1, range_X / 2);
            range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
            Vector3 RandomPostion = new Vector3(range_X, 0f, range_Z);

            Vector3 respawnPosition = originPosition + RandomPostion;
            return respawnPosition;
        }
        
        
    }

}
