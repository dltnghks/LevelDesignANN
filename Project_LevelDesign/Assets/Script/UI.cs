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

    private float hp;     // HP������ ����� �ؽ�Ʈ

    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private TextMeshProUGUI textQCoolTime;     // Q��ų ��Ÿ���� ����� �ؽ�Ʈ

    [SerializeField]
    private TextMeshProUGUI textScore;     // ������ ����� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textEnemyAmount; // ���� ���� ���� ����� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textItemAmount; // ������ ������� ���� ų ���� ����� �ؽ�Ʈ


    [SerializeField]
    private Slider coffeineSlider;

    [SerializeField]
    private TextMeshProUGUI textMoney;     // ���� ����� �ؽ�Ʈ
    [SerializeField]
    private GameObject Panel;       // ī������ �����Ҽ��� ��ο����� ���
    private Image PanelImage;

    [SerializeField]
    private Image qImage;

    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject Shop; //����(��ũ�ѹ�)�� ���

    [SerializeField]
    private GameObject ExitPanel; //�������� �г�

    [SerializeField]
    private GameObject DescriptionPanel; //����Ű �г�
    [SerializeField]
    private GameObject StageClearPanel; // �������� Ŭ���� ǥ�� �г�

    [Header("���׷��̵� �� ǥ��")]
    [SerializeField]
    private TextMeshProUGUI textUpgradeDamage;     // ������ �� ��� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textUpgradeSkillCoolDown;     // ��ų ��Ÿ�� ���� ��� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textUpgradeExplosionRangeUp;     // ���� ���� �� ��� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textUpgradeMaxHealth;     // �ִ�ü�� �� ��� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textUpgradeTransfusion;     // ������ ���� ��� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textDecreaseCoffeine;     // ī���� ���ҷ� ���� ��� �ؽ�Ʈ

    [Header("���ӵ�����")]
    [SerializeField]
    private GameDataSave gameDataSave;          // ���ӵ�����
    [SerializeField]
    private WaveSystem waveSystem;              // wave����
    [SerializeField]
    private GameObject gameDataUI;              // ���ӵ����� ���� UI
    private bool isGameDataUI;
    [SerializeField]
    private TextMeshProUGUI textStage;          // ���� ���������� ����ϴ� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textDifficulty;     // ���̵��� ����� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textEnemy;          // �� ������ ����� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI textItem;           // ������ ������ ���
    [SerializeField]
    private TextMeshProUGUI textPlayTime;       // �÷��̽ð� ���
    [Header("�÷��̾� �̸� �Է�")]
    [SerializeField]
    private GameObject InputPanel;   // �̸� �Է� �г�
    [SerializeField]
    private Text textUserName;       // �÷��̾� �̸�
    [SerializeField]
    private TextMeshProUGUI textInputError;     // �÷��̾� �̸� �Է� ����

    [Header("��ŷ ����")]
    [SerializeField]
    private GameObject rankingBoard;

    bool isOpen = false; // ���׷��̵� â ON/OFF


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


        // ���׷��̵� �ؽ�Ʈ ������Ʈ
        textUpgradeDamage.text = player.needMoney[(int)UpgradeName.UpgradeAttackPower].ToString();
        textUpgradeExplosionRangeUp.text = player.needMoney[(int)UpgradeName.ExplosionRangeUp].ToString();
        textUpgradeMaxHealth.text = player.needMoney[(int)UpgradeName.MaxHealthUp].ToString();
        textUpgradeSkillCoolDown.text = player.needMoney[(int)UpgradeName.UpgradeSkillCoolTimeDown].ToString();
        textUpgradeTransfusion.text = player.needMoney[(int)UpgradeName.TransfusionUp].ToString();
        textDecreaseCoffeine.text = player.needMoney[(int)UpgradeName.DecreaseCoffeine].ToString();

        // ���ӵ����� ǥ��
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

        // ���� ���� ��, ������ ų �� ǥ��
        if (gameDataSave.Difficulty == waveSystem.Waves.Length - 1)
        {
            textEnemyAmount.text = "���� ���� �� : " + gameDataSave.DataGet(GameDataSave.DataType.enemyCount);
        }
        else
        {
            textEnemyAmount.text = "���� ���� ��ȯ �� : " + (waveSystem.CurWave.enemyAmount - gameDataSave.DataGet(GameDataSave.DataType.curWaveSpawnEnemy));
        }
        //textItemAmount.text = "������ �ʿ� ų �� : " + (gameDataSave.NeedSpawnItemKillCount - (gameDataSave.DataGet(GameDataSave.DataType.killCount) % (gameDataSave.NeedSpawnItemKillCount)));
        if (gameDataSave.IsClear)
        {
            StartCoroutine(StageClear());
        }

        // ����
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
            // ���������� 1�̻��� ��쿡�� Stage clear ���
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
            textInputError.text = "3���� �̻� �Է����ּ���.";
        }
        else if(textUserName.text.Length > 10)
        {
            textInputError.text = "10���� ���Ϸ� �Է����ּ���.";
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

    // ���� ��� ����
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
