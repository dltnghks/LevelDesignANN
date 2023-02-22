using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Spawner
{
    [SerializeField]
    private ItemSpawner itemSpawner;

    private List<Enemy> enemyList;
    private float enemyCount;

    private DifficultyChange difficultyChange;

    private int commonCount;
    private int speedCount;
    int num1;
    int num2;

    void Update(){
        float minDis = 10000;
        if(enemyCount <= 0){
            minDis = 0;
        }else{
            minDis = float.MaxValue;
            for(int i = 0; i < enemyCount; i++){
                float distance = Vector3.Distance(player.transform.position, enemyList[i].transform.position);
                if(distance < minDis){
                    minDis = distance;
                }
            }
        }
        // 데이터를 모으는 경우
        if (gameDataSave.isLearningData)
        {
            gameDataSave.DataPlus(GameDataSave.DataType.enemyDistance, minDis * Time.deltaTime);
        }
        else{
            gameDataSave.DataPlus(GameDataSave.DataType.enemyDistance, minDis * Time.deltaTime);
        }
    }

    public void SetUp(Wave wave, DifficultyChange difficultyChange){
        commonCount = 0;
        speedCount = 0;
        enemyCount = 0;

        enemyList = new List<Enemy>();
        prefabs = wave.enemyPrefabs;

        this.difficultyChange = difficultyChange;

        spawnTime = 0.25f + 0.5f/(gameDataSave.Difficulty);
        //Debug.Log("SpawnTime : " + spawnTime);
        maxSpawnCount = wave.enemyAmount;

        StartCoroutine(RandomRespawn_Coroutine());
    }
    protected override IEnumerator RandomRespawn_Coroutine()
    {
        num1 = (int)(gameDataSave.EnemyCommonRate * maxSpawnCount);
        num2 = maxSpawnCount - num1;
        StartCoroutine(SpawnEnemyCoroutine());
        yield return null;
    }

    private IEnumerator SpawnPointCoroutine(Vector3 vec, int num)
    {
        GameObject pointObject = Instantiate(preObject, vec, transform.rotation);
        yield return new WaitForSeconds(1f);
        Destroy(pointObject);

        // 몬스터 생성
        GameObject instantEnemy = Instantiate(prefabs[num], vec, Quaternion.identity);
        enemyCount++;
        enemyList.Add(instantEnemy.GetComponent<Enemy>());

        instantEnemy.GetComponent<Enemy>().SetUp(gameData, this, difficultyChange.difficultyHP, difficultyChange.difficultyDamage, difficultyChange.difficultySpeed);    //게임데이터 정보를 넘겨주고
        gameDataSave.DataPlus(GameDataSave.DataType.enemyCount);  //enemyCount를 ++해준다.
        gameDataSave.DataPlus(GameDataSave.DataType.curWaveSpawnEnemy);

    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        for(int i = 0; i < maxSpawnCount; i++)
        {
            int num = Random.Range(0, 2);
            //Debug.Log(commonCount + ", " + num1);
            if(num == 0 && commonCount == num1)
            {
                num = 1;
            }
            if(num == 1 && speedCount == num2)
            {
                num = 0;
            }

            if (num == 0) commonCount++;
            if (num == 1) speedCount++;

            Vector3 vec = Return_RandomPosition();

            // 몬스터 생성 위치 표시(1초 후 몬스터 생성)
            StartCoroutine(SpawnPointCoroutine(vec, num));

            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DeleteEnemyList(Enemy enemy){  
        enemyCount--;
        enemyList.Remove(enemy);
        if ((gameDataSave.NeedSpawnItemKillCount - (gameDataSave.DataGet(GameDataSave.DataType.killCount) % (gameDataSave.NeedSpawnItemKillCount))) == gameDataSave.NeedSpawnItemKillCount)
        {
            //Debug.Log("아이템 스폰");
            //itemSpawner.SpawnItem();
        }
    }
    
}
