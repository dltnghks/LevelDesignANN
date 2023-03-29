using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;
using System.Threading;
using GoogleSheetsToUnity;
using GoogleSheetsToUnity.ThirdPary;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;


/*
 * 
 * // Input Data
    // 플레이어 체력, 남은 몬스터 수, 가장 가까운 벽과의 평균 거리, 가장 가까운 몬스터와 평균 거리, 명중률, 평균 히트 거리, 1킬당 평균 발사 수, 움직이면서 발사한 비율, 체력 아이템으로 회복한 체력

    // Output Data
    // 난이도(적 능력치 및 스폰율 변화에 기여), 회복 아이템 스폰율
 * 
 */


public class GameDataSave : MonoBehaviour
{
    public enum DataType{

        // 스테이지 당 초기화 할 변수들
        /* shotCount
         * hitCount
         * CurWaveKillCount
         * moveShotCount
         * hitDistanceSum
         * 
         */
            

        shotCount,          // 발사 수
        hitCount,           // 히트 수
        killCount,          // 킬 수
        curWaveKillCount,   // 현재 웨이브 킬 수
        moveShotCount,      // 움직이면서 발사한 수
        shotCountPerKill,   // 킬당 발사 수
        shotCountPerKillSum,// 킬당 발사 수 합
        playerHP,           // 플레이어 HP
        weaponType,         // 무기 타입
        enemyCount,         // 남은 몬스터 수
        hitDistanceSum,     // hit한 경우 몬스터와 거리 합
        waveNum,            // 웨이브 수
        enemyDistance,      // 가까운 몬스터와의 거리
        obstacleDistance,   // 장애물과 거리

        // 몬스터 스폰율
        curWaveSpawnEnemy,
        commonEnemyRate,     
        speedEnemyRate,
        //tankerEnemyRate,
        
        // 느려지는 지형 생성 위치
        slowFieldCenter,
        slowFieldSide
    }

    public enum resultType
    {
        maxEnemySpawn,
        minEnemySpawn,
        maxFieldSpawn,
        minFieldSpawn,
        difficulty
    }

    [Header("플레이어")]
    [SerializeField]
    private float hitRate;          // 평균 명중률 V
    [SerializeField]
    private float hitDistanceRate;         // 평균 사격거리 V
    [SerializeField]
    private float oneKillPerShot;   // 적을 하나 죽일 때까지 사격한 평균 횟수 V
    [SerializeField]
    private float moveShotRate;     // 움직이면서 사격한 비율 V
    [SerializeField]
    private float weaponType;       // 무기 타입

    [Header("적")]
    [SerializeField]
    private int currentStage;       // 현재 스테이지 V
    [SerializeField]
    private float enemyCount;         // 현재 남아있는 적 수 V

    [Header("환경")]
    [SerializeField]
    private float closestWallDistanceAvg;       // 가장 가까운 벽과의 평균 거리 V
    [SerializeField]
    private float closestEnemyDistanceAvg;      // 가장 가까운 몬스터와의 평균 거리 V
    [SerializeField]
    private float playerHP;                     // 플레이어 HP V
    [SerializeField]
    private float slowFieldAmount;              // 느려지는 필드 개수
    private float slowFieldType;                // 느려지는 지형 생성 위치

    // OutPut 변수들
    [Header("몬스터 생성 비율")]
    [SerializeField]
    private float enemyCommonRate;              // 일반 몬스터 생성 비율
    [SerializeField]
    private float enemySpeedRate;               // 스피드 몬스터 생성 비율
    //[SerializeField]
    //private float enemyTankerRate;              // 탱커 몬스터 생성 비율
    
    private int difficulty;                     // 난이도

    [Header("아이템 생성 킬 수")]
    [SerializeField]
    private float needSpawnItemKillCount;       // 아이템 생성 필요 킬 수
    
    private bool isClear;                        // 현재 라운드 클리어 여부

    private Dictionary<DataType, float> dataDic;                                        // 게임 데이터 저장 공간
    private List<string[]> fileData = new List<string[]>();                             // 파일에 쓰일 데이터 저장 공간
    private List<string[]> TestfileData = new List<string[]>();                             // 파일에 쓰일 데이터 저장 공간
    public float time;  // 데이터 기록 시간
    public float playTime; // 플레이 시간
    public float stageTime; // 스테이지 시간

    //플레이어 체력, 남은 몬스터 수, 가장 가까운 벽과의 평균 거리, 가장 가까운 몬스터와 평균 거리,
    //명중률, 평균 히트 거리, 1킬당 평균 발사 수, 움직이면서 발사한 비율, 현재 스테이지, 무기 타입
    private string[] fileColName = {
            // input 요소
            "Player HP", "enemyCount", "closestWallDistanceAvg", "closestEnemyDistanceAvg",
            "hitRate", "hitRange", "oneKillPerShot", "moveShotRate", "currentStage", "WeaponType", "StageTime",
            // output 요소
            "CommonEnemy0.8", "CommonEnemy0.5", "CommonEnemy0.2", "Center Field0.8", "Center Field0.5", "Center Field0.2", "output(difficulty)"
        };

    // 출력 총 개수
    private int outPutAmount;

    [Header("몬스터 타입에 따른 능력치")]
    [SerializeField]
    private float defaultHP;
    [SerializeField]
    private float defaultDamage;
    [SerializeField]
    private float commonDefaultSpeed;
    [SerializeField]
    private float speedDefaultSpeed;
    //[SerializeField]
    //private float tankerDefaultSpeed;
    [SerializeField]
    private float speedHp;
    [SerializeField]
    private float speedSpeed;
    [SerializeField]
    private float speedDamage;
    //[SerializeField]
    //private float tankerHP;
    //[SerializeField]
    //private float tankerSpeed;
    //[SerializeField]
    //private float tankerDamage;

    [Header("데이터모으기O")]
    [SerializeField]
    public bool isLearningData;

    public string userName;

    // 생성 비율
    private float[,] spawnRate = { { 0.8f, 0.2f }, { 0.5f, 0.5f }, { 0.2f, 0.8f } };
    private int spawnRateNum;
    private int fieldSpawnRateNum; // 인공신경망일 때만 사용
    public float DefaultDamage => defaultDamage;
    public float DefaultHP => defaultHP;
    public float CommonDefaultSpeed => commonDefaultSpeed;
    public float SpeedDefaultSpeed => speedDefaultSpeed;
    //public float TankerDefaultSpeed => tankerDefaultSpeed;

    public float SpeedHp => speedHp;
    public float SpeedSpeed => speedSpeed;
    public float SpeedDamage => speedDamage;
    //public float TankerHp => tankerHP;
    //public float TankerSpeed => tankerSpeed;
    //public float TankerDamage => tankerDamage;

    public float EnemyCommonRate => enemyCommonRate;
    public float EnemySpeedRate => enemySpeedRate;
    //public float EnemyTankerRate => enemyTankerRate;

    [Header("구글 스프레드시트")]
    public string associatedSheet = "";
    public string associatedWorksheet = "";


    public int Difficulty => difficulty;
    public float SlowFieldAmount => slowFieldAmount;
    public float SlowFieldType => slowFieldType;
    public bool IsClear => isClear;

    public float NeedSpawnItemKillCount => needSpawnItemKillCount;
    public int FieldSpawnRateNum => fieldSpawnRateNum;

    /*
     *  TCP 관련 변수
     */
    TcpClient client;
    string serverIP = "117.16.44.113";//"127.0.0.2";
    int port = 7777;
    byte[] receivedBuffer;
    StreamReader reader;
    StreamWriter writer;
    bool socketReady = false;
    NetworkStream stream;

    // 패킷 구조체
    public struct packetUserDataStructure
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string playerName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string strData;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string lastStage;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string score;

    }

    // 랭킹 구조체
    public struct Rank
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string playerName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string lastStage;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string score;
    }

    public packetUserDataStructure packetStructure;
    public Rank[] ranking;


    void Awake(){
        // 데이터 초기화
        InitData();
        if (isLearningData)
        {
            serverIP = "117.16.44.111";//"127.0.0.2";
            port = 7777;
        }
        else
        {
            serverIP = "127.0.0.2";
            port = 8000;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // 서버 확인
        CheckReceive();

        
        difficulty = 0;

        isClear = true;

        time = 0;
        playTime = 0;
        stageTime = 0;
        // outPutData개수
        outPutAmount = 1;

        // 파일 데이터의 세로축 이름 넣기
        if (!File.Exists(getPath()))
        {
            //StreamWriter outStream = System.IO.File.AppendText(getPath());
            //fileData.Add(fileColName);
        }

        // 랭킹 정보 가져오기
        if (isLearningData)// 학습 때 사용
        {
            if (socketReady)
            {
                if (stream.CanRead)
                {
                    // 수신할 데이터의 크기를 먼저 받음
                    byte[] sizeBytes = new byte[sizeof(int)];
                    stream.Read(sizeBytes, 0, sizeof(int));
                    int size = BitConverter.ToInt32(sizeBytes, 0);

                    // 수신할 데이터를 받아서 구조체 배열에 저장
                    ranking = new Rank[size];
                    Debug.Log(size);
                    // 구조체 배열을 받아서 저장한다.
                    for (int i = 0; i < size; i++)
                    {
                        byte[] buf = new byte[Marshal.SizeOf<Rank>()];
                        stream.Read(buf, 0, Marshal.SizeOf<Rank>());
                        ranking[i] = ByteArrayToStructure<Rank>(buf);
                        Debug.Log(ranking[i].playerName + " , " + ranking[i].lastStage + " , " + ranking[i].score);
                    }

                }
            }
        }
    }

    public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();
        return result;
    }


    void InitData(){
        dataDic = new Dictionary<DataType, float>();    // 게임 데이터 저장 공간
        dataDic.Add(DataType.shotCount, 0);
        dataDic.Add(DataType.hitCount, 0);
        dataDic.Add(DataType.killCount, 0);
        dataDic.Add(DataType.curWaveKillCount, 0);
        dataDic.Add(DataType.moveShotCount, 0);
        dataDic.Add(DataType.shotCountPerKill, 0);
        dataDic.Add(DataType.shotCountPerKillSum, 0);
        dataDic.Add(DataType.playerHP, 0);
        dataDic.Add(DataType.enemyCount, 0);
        dataDic.Add(DataType.hitDistanceSum, 0);
        dataDic.Add(DataType.waveNum, 1);
        dataDic.Add(DataType.enemyDistance, 0);
        dataDic.Add(DataType.obstacleDistance, 0);
        dataDic.Add(DataType.weaponType, 0);
        dataDic.Add(DataType.commonEnemyRate, 0);
        dataDic.Add(DataType.speedEnemyRate, 0);
        //dataDic.Add(DataType.tankerEnemyRate, 0);
        dataDic.Add(DataType.slowFieldCenter, 0);
        dataDic.Add(DataType.slowFieldSide, 0);
        dataDic.Add(DataType.curWaveSpawnEnemy, 0);

    }

    public void DataSet(DataType name, float data){
        dataDic[name] = data;
    }
    public float DataGet(DataType name){
        return dataDic[name];
    }
    public void DataPlus(DataType name, float data = 1){
        dataDic[name] += data;
    }
    public void DataMinus(DataType name, float data = 1){
        dataDic[name] -= data;
    }


    // 콜백함수로 사용
    public void UpdateWriteOne()
    {

    }

    void WriteStats(UnityAction callback, string strData)
    {
        Debug.Log("스프레드에 쓰기");
        ValueRange data = new ValueRange(strData);
        SpreadsheetManager.Append(new GSTU_Search(associatedSheet, associatedWorksheet), data, callback);
    }


    public void SetOutPut()
    {
        // 최종 스테이지 값 + 플레이 시간
        string OutPut = "," + currentStage.ToString() + ", " + playTime;

        // 파일에 들어갈 데이터 설정
        string[][] output = new string[fileData.Count][];

        Debug.Log("outputLength :" + fileData.Count);
        for (int i = 0; i < fileData.Count; i++)
        {
            output[i] = fileData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        WriteStats(UpdateWriteOne, "");
        Debug.Log("length : " + length);
        for (int index = 0; index < fileData.Count; index++)
        {
            sb.Append(string.Join(delimiter, output[index]));
            sb.AppendLine(string.Join(delimiter, OutPut));

            // 학습데이터를 모을 때
            if (isLearningData)
            {
                StringBuilder sbTmp = new StringBuilder();

                sbTmp.Append(string.Join(delimiter, output[index]));
                sbTmp.AppendLine(string.Join(delimiter, OutPut));

                string strData = sbTmp.ToString();

                WriteDataPacket(strData);
            }
        }

        string filePath = getPath();
        Debug.Log(filePath);

        StreamWriter outStream = System.IO.File.AppendText(filePath);
        //sb.Length--;

        // csv파일에 쓰기
        outStream.Write(sb);
        //스프레드 시트에 쓰기



        Debug.Log("데이터 저장");
        outStream.Close();
    }

    // Update is called once per frame
    void Update()
    {
        DataUpdate();
        time += Time.deltaTime;
        playTime += Time.deltaTime;
        stageTime += Time.deltaTime;
        if (time >= 3){
            string[] tempData = new string[fileColName.Length - outPutAmount];
            // 플레이어 체력, 남은 몬스터 수, 가장 가까운 벽과의 평균 거리, 가장 가까운 몬스터와 평균 거리, 명중률, 평균 히트 거리, 1킬당 평균 발사 수, 움직이면서 발사한 비율, 현재 스테이지, 아이템으로 회복한 체력
            tempData[0] = playerHP.ToString();
            tempData[1] = enemyCount.ToString();
            tempData[2] = closestWallDistanceAvg.ToString();
            tempData[3] = closestEnemyDistanceAvg.ToString();
            tempData[4] = hitRate.ToString();
            tempData[5] = hitDistanceRate.ToString();
            tempData[6] = oneKillPerShot.ToString();
            tempData[7] = moveShotRate.ToString();
            // 학습데이터를 모을 때는 현재 스테이지 정보에 현재 스테이지 넣기
            if (isLearningData)
                tempData[8] = currentStage.ToString();
            else
                // 난이도 넣기
                tempData[8] = difficulty.ToString();
            tempData[9] = weaponType.ToString();

            tempData[10] = " ";
            // 몬스터 데이터 업데이트 ( common enemy만 기록해두면 됨 )
            if (dataDic[DataType.commonEnemyRate] == 0.8f)
            {
                tempData[11] = 1.ToString(); // 0.8
                tempData[12] = 0.ToString(); // 0.5
                tempData[13] = 0.ToString(); // 0.2
            }
            else if (dataDic[DataType.commonEnemyRate] == 0.5f)
            {
                tempData[11] = 0.ToString(); // 0.8
                tempData[12] = 1.ToString(); // 0.5
                tempData[13] = 0.ToString(); // 0.2
            }
            else if (dataDic[DataType.commonEnemyRate] == 0.2f)
            {
                tempData[11] = 0.ToString(); // 0.8
                tempData[12] = 0.ToString(); // 0.5
                tempData[13] = 1.ToString(); // 0.2
            }
            //tempData[12] = dataDic[DataType.tankerEnemyRate].ToString();
            // 슬로우 필드 업데이트 ( Center field만 기록해두면 됨 )

            if(dataDic[DataType.slowFieldCenter] == 0.8f)
            {
                tempData[14] = 1.ToString(); // 0.8
                tempData[15] = 0.ToString(); // 0.5
                tempData[16] = 0.ToString(); // 0.2
            }
            else if (dataDic[DataType.slowFieldCenter] == 0.5f)
            {
                tempData[14] = 0.ToString(); // 0.8
                tempData[15] = 1.ToString(); // 0.5
                tempData[16] = 0.ToString(); // 0.2
            }
            else if (dataDic[DataType.slowFieldCenter] == 0.2f)
            {
                tempData[14] = 0.ToString(); // 0.8
                tempData[15] = 0.ToString(); // 0.5
                tempData[16] = 1.ToString(); // 0.2
            }


            //"Player HP", "closestWallDistanceAvg", "closestEnemyDistanceAvg", "hitRate", "hitRange", "oneKillPerShot", "moveShotRate",
            //"enemyCount", "currentStage",  "weaponType"
            fileData.Add(tempData);
            TestfileData.Add(tempData);

            /*

            string delimiter = ",";

            StringBuilder sb = new StringBuilder();

            WriteStats(UpdateWriteOne, "");
            // 학습데이터를 모을 때
            if (isLearningData)
            {
                StringBuilder sbTmp = new StringBuilder();

                sbTmp.Append(string.Join(delimiter, tempData));
                string strData = sbTmp.ToString();
                WriteDataPacket(strData);
            }
           
            */
            time = 0;
        }         
    }

    private float calRate(float data1, float data2)
    {
        float result = data1 / data2;
        if (!(result >= 0))
        {
            return 0;
        }
        else
        {
            return result;
        }
       
    }

    private void DataUpdate(){
        // Input Data
        // 플레이어 체력, 남은 몬스터 수, 가장 가까운 벽과의 평균 거리, 가장 가까운 몬스터와 평균 거리, 명중률, 평균 히트 거리, 1킬당 평균 발사 수, 움직이면서 발사한 비율, 현재 스테이지, 아이템으로 회복한 체력, 스킬 사용횟수
        // 플레이어 체력
        playerHP = dataDic[DataType.playerHP];
        // 남은 몬스터 수
        enemyCount = dataDic[DataType.enemyCount];
        // 가장 가까운 벽과의 평균 거리 ( 스테이지 당 )
        closestWallDistanceAvg = calRate(dataDic[DataType.obstacleDistance], stageTime);
        // 가장 가까운 몬스터와 평균 거리 ( 스테이지 당 )
        closestEnemyDistanceAvg = calRate(dataDic[DataType.enemyDistance], stageTime);
        // 명중률
        hitRate = calRate(dataDic[DataType.hitCount] , dataDic[DataType.shotCount]) * 100;
        // 평균 히트 거리
        hitDistanceRate = calRate(dataDic[DataType.hitDistanceSum], dataDic[DataType.hitCount]);
        // 1킬 당 평균 발사 수
        oneKillPerShot = calRate(dataDic[DataType.shotCountPerKillSum], dataDic[DataType.killCount]);
        // 움직이면서 발사한 비율
        moveShotRate = calRate(dataDic[DataType.moveShotCount] , dataDic[DataType.shotCount]) * 100;
        // 현재 스테이지
        currentStage = (int)dataDic[DataType.waveNum];
        //  무기 타입
        weaponType = dataDic[DataType.weaponType];
    }


    // 파일 경로 설정
    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "/level_design_train.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Student Data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Student Data.csv";
#else
        return Application.dataPath +"/"+"DataFile.csv";
#endif
    }

    private string getTestPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "/level_design_test.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Student Data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Student Data.csv";
#else
        return Application.dataPath +"/"+"level_design_test.csv";
#endif
    }

    // 웨이브 시작 시 output정보 세팅
    public void SetWave()
    {
        if (isLearningData && difficulty == 0) // 학습 데이터 얻을 때
        {
            // 학습데이터를 얻을 때 랜던으로 세팅되는 몬스터 비율, 느려지는 지형 생성 수
            spawnRateNum = UnityEngine.Random.Range(0, 3);
            // 몬스터 비율 설정
            float commonRate = spawnRate[spawnRateNum, 0];
            float speedRate = spawnRate[spawnRateNum, 1];
            //float tankerRate = 10 - speedRate - commonRate;
            enemyCommonRate = commonRate;
            enemySpeedRate = speedRate;
            //enemyTankerRate = tankerRate / 10;
            DataSet(DataType.commonEnemyRate, enemyCommonRate);
            DataSet(DataType.speedEnemyRate, enemySpeedRate);
            //DataSet(DataType.tankerEnemyRate, tankerRate);
            //Debug.Log("첫번째 세팅 : " + enemyCommonRate + ", " + enemySpeedRate);

        }
        else if(!isLearningData)           // 인공신경망 사용 시
        {
            currentStage = (int)dataDic[DataType.waveNum];
            if (currentStage == 1)
            {
                // 첫 스테이지는 랜덤 세팅
                int spawnRateNum = UnityEngine.Random.Range(0, 3);
                float commonRate = spawnRate[spawnRateNum, 0];
                float speedRate = spawnRate[spawnRateNum, 1];

                //float tankerRate = 10 - speedRate - commonRate;
                enemyCommonRate = commonRate;
                enemySpeedRate = speedRate;
                //enemyTankerRate = tankerRate / 10;
                DataSet(DataType.commonEnemyRate, commonRate);
                DataSet(DataType.speedEnemyRate, speedRate);
                //DataSet(DataType.tankerEnemyRate, tankerRate);
            }
            else
            {
                // 느려지는 지형 세팅

                // 몬스터 스폰율 세팅
                float commonRate = spawnRate[spawnRateNum, 0];
                float speedRate = spawnRate[spawnRateNum, 1];
                //float tankerRate = enemyTankerRate;
                enemyCommonRate = commonRate;
                enemySpeedRate = speedRate;
                //enemyTankerRate = tankerRate / 10;
                DataSet(DataType.commonEnemyRate, commonRate);
                DataSet(DataType.speedEnemyRate, speedRate);
                //DataSet(DataType.tankerEnemyRate, tankerRate);
            }
        }
        // 난이도 상승
        difficulty++;

        // 데이터 리셋
        stageTime = 0;
        DataSet(DataType.obstacleDistance, 0);
        DataSet(DataType.enemyDistance, 0);
    }
    
    public void PlayerDie()
    {
        if (!isLearningData)// 인공신경망 사용할 때 쓰는 코드
        {
            // 파이썬에 데이터 보내기
            if (socketReady)
            {
                if (stream.CanWrite)
                {
                    // 클리어 신호 보내기
                    string str = "die";
                    receivedBuffer = System.Text.Encoding.Default.GetBytes(str);
                    stream.Write(receivedBuffer, 0, receivedBuffer.Length);
                }
            }
        }
    }

    public void StageClear()
    {

        // 한 라운드 데이터 저장
        ///*
        string filePath = getTestPath();       // 경로
        StreamWriter outStream = System.IO.File.CreateText(filePath);// 쓰기 스트림
        string[][] output = new string[TestfileData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = TestfileData[i];
        }
        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
        {
            sb.AppendLine(string.Join(delimiter, output[index]));
        }

        outStream.Write(sb);
        Debug.Log(sb);
        Debug.Log("test 저장");

        TestfileData = new List<string[]>();

        outStream.Close();


        if (!isLearningData)// 인공신경망 사용할 때 쓰는 코드
        {
            // 파이썬에 데이터 보내기
            if (socketReady)
            {
                if (stream.CanWrite)
                {
                    // 클리어 신호 보내기
                    string str = "clear";
                    receivedBuffer = System.Text.Encoding.Default.GetBytes(str);
                    stream.Write(receivedBuffer, 0, receivedBuffer.Length);

                    // 인공신경망 Output가져오기
                    string msg = "msg";
                    receivedBuffer = new byte[1024];
                    stream.Read(receivedBuffer, 0, receivedBuffer.Length);
                    stream.Flush();
                    msg = System.Text.Encoding.Default.GetString(receivedBuffer);

                    Debug.Log("enemy : " + spawnRateNum + ", field : " + fieldSpawnRateNum + ", cur difficulty : " + difficulty);
                    string[] result = msg.Split(','); // 0 : 몬스터 최댓값 스폰율, 1 : 몬스터 최솟값 스폰율, 2 : 필드 최댓값 스폰율, 3: 필드 최솟값 스폰율 , 4 : 난이도
                    for (int i = 0; i < result.Length; i++)
                        Debug.Log(i + " : " + result[i]);
                    // 현재 스테이지 난이도가 낮다면
                    if (difficulty < float.Parse(result[(int)(resultType.difficulty)]))
                    {
                        Debug.Log(float.Parse(result[(int)(resultType.difficulty)]) + "현재 난이도가 낮습니다.");
                        // 몬스터 스폰율을 최솟값으로
                        spawnRateNum = Int32.Parse(result[(int)(resultType.minEnemySpawn)]);
                        enemyCommonRate = spawnRate[spawnRateNum, 0];
                        enemySpeedRate = spawnRate[spawnRateNum, 1];
                        // slow field 최솟값으로
                        fieldSpawnRateNum = Int32.Parse(result[(int)(resultType.minFieldSpawn)]);
                    }
                    // 현재 스테이지 난이도가 높다면
                    else
                    {
                        Debug.Log(float.Parse(result[(int)(resultType.difficulty)]) + "현재 난이도가 높습니다..");
                        // 몬스터 스폰율을 그대로
                        spawnRateNum = Int32.Parse(result[(int)(resultType.maxEnemySpawn)]);
                        enemyCommonRate = spawnRate[spawnRateNum, 0];
                        enemySpeedRate = spawnRate[spawnRateNum, 1];
                        // slow field 비율 그대로
                        fieldSpawnRateNum = Int32.Parse(result[(int)(resultType.maxFieldSpawn)]);

                    }
                    Debug.Log("enemy : " + spawnRateNum + ", field : " + fieldSpawnRateNum + ", result difficulty : " + (int)(resultType.difficulty));


                }
            }
        }

        isClear = true;
    }

    public void StageStart()
    {
        isClear = false;
    }

    /*
     *  TCP관련 함수
     */

    public static byte[] StructureToByte(object obj)

    {

        int datasize = Marshal.SizeOf(obj);//((PACKET_DATA)obj).TotalBytes; // 구조체에 할당된 메모리의 크기를 구한다.

        IntPtr buff = Marshal.AllocHGlobal(datasize); // 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.

        Marshal.StructureToPtr(obj, buff, false); // 할당된 구조체 객체의 주소를 구한다.

        byte[] data = new byte[datasize]; // 구조체가 복사될 배열

        Marshal.Copy(buff, data, 0, datasize); // 구조체 객체를 배열에 복사

        Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함

        return data; // 배열을 리턴

    }
    void CheckReceive()
    {
        if (socketReady) return;
        try
        {

            client = new TcpClient(serverIP, port);

            if (client.Connected)
            {
                stream = client.GetStream();

                Debug.Log("Connect Success");
                socketReady = true;
            }

        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
            Application.Quit();
        }

    }

    public void WriteDataPacket(string strData)
    {
        packetStructure.playerName =  userName;
        packetStructure.strData = strData;
        packetStructure.lastStage = currentStage.ToString();
        packetStructure.score = ((int)DataGet(DataType.killCount) * 100).ToString();
        Debug.Log("score : " + packetStructure.score);
        Debug.Log((packetStructure.playerName).ToString() + ":" + (packetStructure.strData).ToString());
        //receivedBuffer = System.Text.Encoding.Default.GetBytes(strData);
        receivedBuffer = StructureToByte(packetStructure);
        Debug.Log(Marshal.SizeOf<packetUserDataStructure>());
        stream.Write(receivedBuffer, 0, Marshal.SizeOf<packetUserDataStructure>());
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        client.Close();
        socketReady = false;
    }

}
