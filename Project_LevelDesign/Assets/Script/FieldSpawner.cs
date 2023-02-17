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

    // �ʵ尡 ������ ������ �ε��� ����
    private int startIndex;
    private int endIndex;
    [SerializeField]
    private float centerRate;
    [SerializeField]
    private float sideRate;

    int count = 0; // ���� ��
    int spawnPoint; // ���� ��ġ 0 : ����, 1 : ���̵�

    // ���� ����
    private float[,] spawnRate = { { 0.8f, 0.2f }, { 0.5f, 0.5f }, { 0.2f, 0.8f } };

    private void Awake()
    {
        gameData = GameObject.FindGameObjectWithTag("GameData");
        gameDataSave = gameData.GetComponent<GameDataSave>();

        // �ʱ�ȭ
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
        // �н� ������ ���� ���� �������� ���
        if (gameDataSave.isLearningData && gameDataSave.Difficulty == 1)
        {
            int num = Random.Range(0, 3);
            // ���� ����
            centerRate = spawnRate[num,0];
            sideRate = spawnRate[num,1];

            gameDataSave.DataSet(GameDataSave.DataType.slowFieldCenter, centerRate);
            gameDataSave.DataSet(GameDataSave.DataType.slowFieldSide, sideRate);

        }
        // �ΰ��Ű���� ��� 1���������� ����
        else if(!gameDataSave.isLearningData && gameDataSave.Difficulty == 1)
        {
            // ���� ����
            int num = Random.Range(0, 3);
            centerRate = spawnRate[num,0];
            sideRate = spawnRate[num,1];

            gameDataSave.DataSet(GameDataSave.DataType.slowFieldCenter, centerRate);
            gameDataSave.DataSet(GameDataSave.DataType.slowFieldSide, sideRate);
        }

        if (!isSpawn)
        {

            // �ʵ� ���� �ʱ�ȭ
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
            // �������� Ŭ���� �� �ʵ� ����
            GameObject[] gameObject = GameObject.FindGameObjectsWithTag("SlowField");
            for (int i = 0; i < gameObject.Length; i++)
                Destroy(gameObject[i]);
            maxSpawnCount = (int)gameDataSave.SlowFieldAmount;
            StartCoroutine(RandomRespawn_Coroutine());
            isSpawn = true;

            // �÷��̾� �ӵ� ���� ���ֱ�
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Player>().isSlow = false;
        }
        isSpawn = false;

        //Debug.Log("Start : " + centerRate);
    }

    protected override IEnumerator RandomRespawn_Coroutine()
    {
        // �ʵ� ���� �� �ʱ�ȭ
        count = 0;
        for (int i = 0; i < maxSpawnCount; i++)
        {
            // ���� ������Ʈ ����
            int num = Random.Range(0, prefabs.Length);
            // ���� ��ġ ǥ��(1��)
            Vector3 vec = Return_RandomPosition(); // ���� ��ġ
            //GameObject pointObject = Instantiate(preObject, vec, transform.rotation);
            //yield return new WaitForSeconds(1f);
            //Destroy(pointObject);

            // ������Ʈ ����
            vec.y = 0.01f;
            GameObject slowFieldObject = Instantiate(prefabs[num], vec, Quaternion.identity);
            // slowField ������ ����
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
        
        // ���� ������ŭ ����
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
        // ���̵� ������ŭ ����
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
        // �ʵ� ���� �� ++
        count++;


        // center�� ���
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
