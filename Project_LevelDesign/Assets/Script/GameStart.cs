using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public Text Uitext;
    int textnumber = 0;

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            textnumber++;
            Debug.Log(textnumber);
            switch (textnumber)
            {
                case 1:
                    Uitext.text = "��..�ΰǰ�? �ٵ� ���̸� Ŀ�ǳ����� ����?";
                    break;
                case 2:
                    Uitext.text = "!! ���� �̰�.. �� �氡�� Ŀ�ǰ� �����ٳ�?";
                    break;
                case 3:
                    Uitext.text = "���� �̷� ���� �� ����.. �ϴ� ���ƴٳຼ��..";
                    break;
                case 4:
                    Uitext.text = "...!! ���� �� Ŀ�ٶ� ��������!!";
                    break;
                case 5:
                    Uitext.text = "���ƾ�!! ������!!";
                    break;
                case 6:
                    SceneManager.LoadScene("1_Spring");
                    break;
                default:
                    break;

            }

        }
    }
}