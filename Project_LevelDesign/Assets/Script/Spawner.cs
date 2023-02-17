using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    protected enum SpawnType{
        Item = 0,
        Enemy = 1,
        Field = 2
    }

    // ������ ����� Plane�� �ڽ��� RespawnRange ������Ʈ
    public GameObject[] rangeObject;
    protected BoxCollider[] rangeCollider;

    // ��ȯ�� Object
    public GameObject[] prefabs;
    [SerializeField]
    protected GameObject preObject; // ��ȯ�ϱ����� ǥ�ÿ�����Ʈ
    [SerializeField]
    protected float spawnTime;
    [SerializeField]
    protected int maxSpawnCount;
    
    [SerializeField]
    protected SpawnType spawnType;

    [SerializeField]
    protected WaveSystem waveSystem;

    [Header("게임 데이터")]
    [SerializeField]
    protected GameObject gameData;
    protected GameDataSave gameDataSave;
    [SerializeField]
    protected Player player;



    private void Awake()
    {
        gameData = GameObject.FindGameObjectWithTag("GameData");
        gameDataSave = gameData.GetComponent<GameDataSave>();
        rangeCollider = new BoxCollider[rangeObject.Length];
        for (int i = 0; i < rangeObject.Length; i++)
        {
            rangeCollider[i] = rangeObject[i].GetComponent<BoxCollider>();
        }
    }

    private void Start()
    {
        spawnTime = 30f;
    }


    protected virtual IEnumerator RandomRespawn_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);

            // 랜덤 오브젝트 선택
            int num = Random.Range(0, prefabs.Length);

            // 생성 위치 표시(1초)
            Vector3 vec = Return_RandomPosition(); // 랜덤 위치
            GameObject pointObject = Instantiate(preObject, vec, transform.rotation);
            yield return new WaitForSeconds(1f);
            Destroy(pointObject);
            player.isSlow = false;

            // 오브젝트 생성
            GameObject instantCapsul = Instantiate(prefabs[num], vec, Quaternion.identity);
            //Debug.Log("아이템 생성");
        }
    }


    // 랜덤 위치 리턴
    protected virtual Vector3 Return_RandomPosition()
    {
        int spawnPointIndex = Random.Range(0, rangeObject.Length);
        Vector3 originPosition = rangeObject[spawnPointIndex].transform.position;
        float range_X = rangeCollider[spawnPointIndex].bounds.size.x;
        float range_Z = rangeCollider[spawnPointIndex].bounds.size.z;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
        Vector3 RandomPostion = new Vector3(range_X, 0f, range_Z);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }
}
