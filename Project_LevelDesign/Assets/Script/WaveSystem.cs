using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSystem : MonoBehaviour
{

    [SerializeField]
    private Wave[] wave;
    private Wave curWave;

    [Header("몬스터 프리팹")]
    [SerializeField]
    public GameObject[] setEnemyPrefabs;    // 나오는 몬스터 프리팹

    [Header("Spawner")]
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private FieldSpawner fieldSpawner;
    
    [Header("게임 데이터")]
    [SerializeField]
    private GameDataSave gameDataSave;

    [Header("난이도에 따른 변화량")]
    [SerializeField]
    private DifficultyChange difficultyChange;

    private int curWaveNum; // 현재 스테이지 번호
    private bool lastStage;
    public DifficultyChange DifficultyChange => difficultyChange;
    public Wave CurWave => curWave;
    public Wave[] Waves => wave;

    void Start(){
        curWaveNum = 1;
        curWave = wave[curWaveNum];
        lastStage = false;
        StartCoroutine(startWave());
    }

    void Update(){
        //Debug.Log(gameDataSave.DataGet(GameDataSave.DataType.curWaveKillCount));
        // 현재 웨이브에 킬을 웨이브의 총 몬스터 수만큼 잡으면 다음 스테이지
        if(gameDataSave.DataGet(GameDataSave.DataType.curWaveSpawnEnemy) >= curWave.enemyAmount && !lastStage)
        {
            // 현재 웨이브 스폰 몬스터 수 0,킬 수 0으로 초기화
            gameDataSave.DataSet(GameDataSave.DataType.curWaveSpawnEnemy, 0);
            gameDataSave.DataSet(GameDataSave.DataType.curWaveKillCount, 0);

            curWaveNum++;

            // 마지막 스테이지까지 도달하면
            Debug.Log(curWaveNum + ", " + wave.Length);
            if (curWaveNum == wave.Length) {
                Debug.Log("마지막 스테이지");
                StartCoroutine(LastStage());
                lastStage = true;
                return;
            }

            gameDataSave.DataPlus(GameDataSave.DataType.waveNum);
            gameDataSave.StageClear();

            curWave = wave[curWaveNum];
            StartCoroutine(startWave());
        }
    }

    IEnumerator LastStage()
    {
        while (true)
        {
            if (gameDataSave.DataGet(GameDataSave.DataType.enemyCount) == 0)
            {
                gameDataSave.SetOutPut();
                SceneManager.LoadScene("ClearScene");
                yield return null;
            }

            yield return null;
        }

    }

    IEnumerator startWave(){
        // 난이도 증가 SetWave
        gameDataSave.SetWave();
        curWave.difficulty = gameDataSave.Difficulty;

        // 난이도에 따른 스폰률 변화
        curWave.enemyAmount = gameDataSave.Difficulty * (int)difficultyChange.difficultySpawnAmount;
        curWave.enemyPrefabs = setEnemyPrefabs;
        // 몬스터 비율, 느려지는 지형 생성 위치 세팅
        enemySpawner.SetUp(curWave, difficultyChange);
        fieldSpawner.SetUp();
        yield return null;
    }

}

[System.Serializable]
public struct Wave{
    public int difficulty;     // 난이도
    public int enemyAmount;    // 몬스터 수
    public GameObject[] enemyPrefabs;    // 나오는 몬스터 프리팹
}

[System.Serializable]
public struct DifficultyChange
{
    public float difficultyHP;
    public float difficultySpeed;
    public float difficultyDamage;
    public float difficultySpawnAmount;
}