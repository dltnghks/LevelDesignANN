using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FallClearText : MonoBehaviour
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
                    Uitext.text = "���� ������ �߿����� �ܿ��� �ٰ����� ������ ��������.";
                    break;
                case 2:
                    Uitext.text = "�Ѽ����� �ٲ�� ������� �ð������� �������� ����̴�.";
                    break;
                case 3:
                    Uitext.text = "������ ���� ��Ƴ��� ���̴�. ��� ������� �Դµ� ������ �� ����.";
                    break;
                case 4:
                    Uitext.text = "�� �ָ� ������ �������� �ִ�.. ����.. �����?";
                    break;
                case 5:
                    player.isClearScene = false;
                    SceneManager.LoadScene("4_Winter");
                    break;
                default:
                    break;
            }

        }
    }
}
