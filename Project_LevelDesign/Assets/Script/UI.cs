using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.IO;

public class UI : MonoBehaviour
{

    public enum UpgradeName
    {
        UpgradeAttackPower = 0,
        UpgradeSkillCoolTimeDown = 1,
        ExplosionRangeUp = 2,
        MaxHealthUp = 3,
        TransfusionUp = 4,
        DecreaseCoffeine = 5
    }

    private float hp;     // HP정보를 출력할 텍스트

    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private TextMeshProUGUI textQCoolTime;     // Q스킬 쿨타임을 출력할 텍스트

    [SerializeField]
    private TextMeshProUGUI textScore;     // 점수를 출력할 텍스트
    [SerializeField]
    private TextMeshProUGUI textEnemyAmount; // 남은 몬스터 수를 출력할 텍스트
    [SerializeField]
    private TextMeshProUGUI textItemAmount; // 아이템 등장까지 남은 킬 수를 출력할 텍스트


    [SerializeField]
    private Slider coffeineSlider;

    [SerializeField]
    private TextMeshProUGUI textMoney;     // 돈을 출력할 텍스트
    [SerializeField]
    private GameObject Panel;       // 카페인이 감소할수록 어두워지는 배경
    private Image PanelImage;

    [SerializeField]
    private Image qImage;

    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject Shop; //상점(스크롤바)을 출력

    [SerializeField]
    private GameObject ExitPanel; //게임종료 패널

    [SerializeField]
    private GameObject DescriptionPanel; //조작키 패널
    [SerializeField]
    private GameObject StageClearPanel; // 스테이지 클리어 표시 패널

    [Header("업그레이드 돈 표시")]
    [SerializeField]
    private TextMeshProUGUI textUpgradeDamage;     // 데미지 업 비용 텍스트
    [SerializeField]
    private TextMeshProUGUI textUpgradeSkillCoolDown;     // 스킬 쿨타임 감소 비용 텍스트
    [SerializeField]
    private TextMeshProUGUI textUpgradeExplosionRangeUp;     // 폭발 범위 업 비용 텍스트
    [SerializeField]
    private TextMeshProUGUI textUpgradeMaxHealth;     // 최대체력 업 비용 텍스트
    [SerializeField]
    private TextMeshProUGUI textUpgradeTransfusion;     // 흡혈량 증가 비용 텍스트
    [SerializeField]
    private TextMeshProUGUI textDecreaseCoffeine;     // 카페인 감소량 감소 비용 텍스트

    [Header("게임데이터")]
    [SerializeField]
    private GameDataSave gameDataSave;          // 게임데이터
    [SerializeField]
    private WaveSystem waveSystem;              // wave정보
    [SerializeField]
    private GameObject gameDataUI;              // 게임데이터 정보 UI
    private bool isGameDataUI;
    [SerializeField]
    private TextMeshProUGUI textStage;          // 현재 스테이지를 출력하는 텍스트
    [SerializeField]
    private TextMeshProUGUI textDifficulty;     // 난이도를 출력할 텍스트
    [SerializeField]
    private TextMeshProUGUI textEnemy;          // 적 정보를 출력할 텍스트
    [SerializeField]
    private TextMeshProUGUI textItem;           // 아이템 정보를 출력
    [SerializeField]
    private TextMeshProUGUI textPlayTime;       // 플레이시간 출력
    [Header("플레이어 이름 입력")]
    [SerializeField]
    private GameObject InputPanel;   // 이름 입력 패널
    [SerializeField]
    private Text textUserName;       // 플레이어 이름
    [SerializeField]
    private TextMeshProUGUI textInputError;     // 플레이어 이름 입력 에러

    [Header("랭킹 보드")]
    [SerializeField]
    private GameObject rankingBoard;

    bool isOpen = false; // 업그레이드 창 ON/OFF


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        PanelImage = Panel.GetComponent<Image>();
        Shop.SetActive(false);
        isGameDataUI = false;
        Time.timeScale = 0;
        
        if (File.Exists(getPath()))
        {
            FileInfo fileInfo = new FileInfo(getPath());
            StreamReader reader = new StreamReader(getPath());
            string value = reader.ReadToEnd();
            gameDataSave.userName = value;
            reader.Close();

            InputPanel.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        SetHPValue();
        SetCoffeineValue();
        SetQCoolValue();
        textScore.text = "Score : " + player.score.ToString();
        textMoney.text = "Money : " + player.Money.ToString();
        PanelImage.color = new Color(0, 0, 0, (100 - player.coffeine) / 100);


        // 업그레이드 텍스트 업데이트
        textUpgradeDamage.text = player.needMoney[(int)UpgradeName.UpgradeAttackPower].ToString();
        textUpgradeExplosionRangeUp.text = player.needMoney[(int)UpgradeName.ExplosionRangeUp].ToString();
        textUpgradeMaxHealth.text = player.needMoney[(int)UpgradeName.MaxHealthUp].ToString();
        textUpgradeSkillCoolDown.text = player.needMoney[(int)UpgradeName.UpgradeSkillCoolTimeDown].ToString();
        textUpgradeTransfusion.text = player.needMoney[(int)UpgradeName.TransfusionUp].ToString();
        textDecreaseCoffeine.text = player.needMoney[(int)UpgradeName.DecreaseCoffeine].ToString();

        // 게임데이터 표시
        textDifficulty.text = "Difficulty : " + gameDataSave.Difficulty.ToString();
        textEnemy.text = "Enemy\n" +
                        "Hp : " + (waveSystem.DifficultyChange.difficultyHP * gameDataSave.Difficulty + gameDataSave.DefaultHP) + ", "
                        + (waveSystem.DifficultyChange.difficultyHP * gameDataSave.SpeedHp * gameDataSave.Difficulty + gameDataSave.DefaultHP) + ", " + "\n" +
                        //+ (waveSystem.DifficultyChange.difficultyHP * gameDataSave.TankerHp * gameDataSave.Difficulty + gameDataSave.DefaultHP) +
                        "Speed : " + (waveSystem.DifficultyChange.difficultySpeed * gameDataSave.Difficulty + gameDataSave.CommonDefaultSpeed) + ", "
                        + (waveSystem.DifficultyChange.difficultySpeed * gameDataSave.SpeedSpeed * gameDataSave.Difficulty + gameDataSave.SpeedDefaultSpeed) + ", " + "\n" +
                        //+ (waveSystem.DifficultyChange.difficultySpeed * gameDataSave.TankerSpeed * gameDataSave.Difficulty + gameDataSave.TankerDefaultSpeed) + "\n" +
                        "Damage : " + (waveSystem.DifficultyChange.difficultyDamage * gameDataSave.Difficulty + gameDataSave.DefaultDamage) + ", "
                        + (waveSystem.DifficultyChange.difficultyDamage * gameDataSave.SpeedDamage * gameDataSave.Difficulty + gameDataSave.DefaultDamage) + ", " + "\n" +
                        "CommonRate : " + (gameDataSave.EnemyCommonRate * waveSystem.CurWave.enemyAmount).ToString() + "\n" +
                        "SpeedRate : " + ((waveSystem.CurWave.enemyAmount - (gameDataSave.EnemyCommonRate * waveSystem.CurWave.enemyAmount)).ToString()) + "\n";
                        //+ (waveSystem.DifficultyChange.difficultyDamage * gameDataSave.TankerDamage * gameDataSave.Difficulty + gameDataSave.DefaultDamage) + "\n" +
                        //"Amount : " + (waveSystem.DifficultyChange.difficultySpawnAmount * gameDataSave.Difficulty) + "\n" +
                        //"curEnemy : " + gameDataSave.DataGet(GameDataSave.DataType.enemyCount);

        textItem.text = "SlowFieldCenter : " + gameDataSave.DataGet(GameDataSave.DataType.slowFieldCenter) + "\n" +
                        "SlowFieldSide : " + gameDataSave.DataGet(GameDataSave.DataType.slowFieldSide) + "\n";
        textPlayTime.text = "Time : " + gameDataSave.playTime.ToString("F1");

        // 남은 몬스터 수, 아이템 킬 수 표시
        if (gameDataSave.Difficulty == waveSystem.Waves.Length - 1)
        {
            textEnemyAmount.text = "남은 몬스터 수 : " + gameDataSave.DataGet(GameDataSave.DataType.enemyCount);
        }
        else
        {
            textEnemyAmount.text = "남은 몬스터 소환 수 : " + (waveSystem.CurWave.enemyAmount - gameDataSave.DataGet(GameDataSave.DataType.curWaveSpawnEnemy));
        }
        //textItemAmount.text = "아이템 필요 킬 수 : " + (gameDataSave.NeedSpawnItemKillCount - (gameDataSave.DataGet(GameDataSave.DataType.killCount) % (gameDataSave.NeedSpawnItemKillCount)));
        if (gameDataSave.IsClear)
        {
            StartCoroutine(StageClear());
        }

        // 상점
        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isOpen)
            {
                Shop.SetActive(true);
                player.isShoping = true;
                isOpen = true;
                Time.timeScale = 0;
            }
            else
            {
                Shop.SetActive(false);
                player.isShoping = false;
                isOpen = false;
                Time.timeScale = 1;
            }
        }
        */

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isOpen)
            {
                ExitPanel.SetActive(true);
                isOpen = true;
                Time.timeScale = 0;
            }
            else
            {
                ExitPanel.SetActive(false);
                isOpen = false;
                DescriptionPanel.SetActive(false);
                Time.timeScale = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!isGameDataUI)
                gameDataUI.SetActive(true);
            else
                gameDataUI.SetActive(false);
            isGameDataUI = !isGameDataUI;
        }
    }

    private IEnumerator StageClear()
    {
        if (gameDataSave.Difficulty != 1)
        {
            // 스테이지가 1이상인 경우에만 Stage clear 출력
            StageClearPanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            StageClearPanel.SetActive(false);
            if (gameDataSave.Difficulty == waveSystem.Waves.Length - 1)
            {
                textStage.text = "Last Stage";
            }
            else
            {
                textStage.text = "Stage " + (gameDataSave.DataGet(GameDataSave.DataType.waveNum)).ToString();
            }
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        gameDataSave.StageStart();
    }

    private void SetQCoolValue()
    {
        qImage.fillAmount = (player.CurQCoolTime / player.explosionCoolTime);
    }

    public void SetHPValue()
    {
        hpSlider.value = player.CurHP/player.MaxHP;
    }  

    public void SetCoffeineValue()
    {
        coffeineSlider.value = player.coffeine / 100;
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickDescriptionButton()
    {
        DescriptionPanel.SetActive(true);
    }

    public void OnClickDescriptionExitButton()
    {
        ExitPanel.SetActive(false);
        isOpen = false;
        DescriptionPanel.SetActive(false);
        Time.timeScale = 1;
        DescriptionPanel.SetActive(false);
    }


    public void OnClickInputUserName()
    {
        if (textUserName.text.Length < 3)
        {
            textInputError.text = "3글자 이상 입력해주세요.";
        }
        else if(textUserName.text.Length > 10)
        {
            textInputError.text = "10글자 이하로 입력해주세요.";
        }
        else
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(getPath()));
            FileStream fileStream= new FileStream(getPath(), FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.Unicode);
            gameDataSave.userName = textUserName.text.ToString();
            //gameDataSave.userName = gameDataSave.userName.Remove(gameDataSave.userName.Length);

            writer.Write(gameDataSave.userName);
            writer.Close();

            InputPanel.SetActive(false);
        }
    }

    // 파일 경로 설정
    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/name/" + "/playerName.txt";
#elif UNITY_ANDROID
        return Application.persistentDataPath+ "/name/" + "/playerName.txt";
#elif UNITY_IPHONE
        return Application.persistentDataPath++ "/name/" + "/playerName.txt";
#else
        return Application.dataPath + "/name/" + "/playerName.txt"
#endif
    }
}
