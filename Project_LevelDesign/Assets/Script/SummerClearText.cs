using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SummerClearText : MonoBehaviour
{
    public Text Uitext;
    private Player player;
    int textnumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            textnumber++;

            switch (textnumber)
            {
                case 1:
                    Uitext.text = "�ٵ� ������ �ƴ϶� ���浵 �������� �ְ�, ������ ���Ӿ��� ������� �ְ�..";
                    break;
                case 2:
                    Uitext.text = "Ŀ�Ǹ� ��� ���ž��ؼ� �ݹ��̶� ī���� �ߵ��� �ɸ� �� ���� ����̴�..";
                    break;
                case 3:
                    Uitext.text = "���� ���������� ������ ���� �ٲ��� �� ���� ������ ���� �� ����.";
                    break;
                case 4:
                    Uitext.text = "�ϴ� ��¿ �� ����, ������ ���� ���ۿ�!!";
                    break;
                case 5:
                    player.isClearScene = false;
                    SceneManager.LoadScene("3_Fall");
                    break;
                default:
                    break;
            }

        }
    }
}