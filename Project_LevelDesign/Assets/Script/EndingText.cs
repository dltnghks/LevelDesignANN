using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingText : MonoBehaviour
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

            switch (textnumber)
            {
                case 1:
                    Uitext.text = "��..? �����.. ���� �������ݾ�.. �˶���..�� �ڵ��� �Ҹ��ΰ�.";
                    break;
                case 2:
                    Uitext.text = "���ΰǰ�? ����ü �󸶳� �����Ѱž�..��¾�� ���� ���� �ȵȴ� �;���.";
                    break;
                case 3:
                    Uitext.text = "���̶�⿣ �ʹ� �ǰ����� �������δ� �İ��ߴٴ� �����̵��. ";
                    break;
                case 4:
                    Uitext.text = "�ϴÿ� ������ ���� �� �� �� ������ ���� ������ ���߰ڴ�.";
                    break;
                case 5:
                    SceneManager.LoadScene("End"); //���� Ÿ��Ʋ
                    break;
                default:
                    break;
            }
        }
    }
}


