using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpringClearText : MonoBehaviour
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
                    Uitext.text = "Ŀ�ǰ� �������� ū�ϳ� ���߾�..";
                    break;
                case 2:
                    Uitext.text = "�� ��ģ ���� �ѽö� ���� Ż���ؾ���..�ٵ� ���ڱ� ���̸� ����?";
                    break;
                case 3:
                    Uitext.text = "���� �Ź� �Ҹ��� �鸮�� �� ����..���� ������ ���� �ٲ�ǰ�?";
                    break;
                case 4:
                    Uitext.text = "����ü �� ���� ��ü�� ���淡 �� ����ΰž�!!";
                    break;
                case 5:
                    player.isClearScene = false;
                    SceneManager.LoadScene("2_Summer");
                    break;
                default:
                    break;

            }

        }
    }
}
