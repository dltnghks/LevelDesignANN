using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinterClearText : MonoBehaviour
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
                    Uitext.text = "��Ե� ��Ƴ��� �� ����..";
                    break;
                case 2:
                    Uitext.text = "����ü �󸶳� �ð��� �帥����? ������ȭ���� 100�� �̻��� �귯���� �� �ƴұ�?";
                    break;
                case 3:
                    Uitext.text = "���Ӿ��� ��ȭ�ϴ� ������� ���� ���ƴٴϸ� �������� ��Ƴ���� ������, ��Ƽ�Ⱑ ���������.";
                    break;
                case 4:
                    Uitext.text = "��? �ϴ� ���� ���� �Ҹ��� �鸮�µ�...�˶� �Ҹ�..�ΰ�?";
                    break;
                case 5:
                    player.isClearScene = false;
                    SceneManager.LoadScene("End");
                    break;
                default:
                    break;
            }
        }
    }
}
