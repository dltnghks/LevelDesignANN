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
                    Uitext.text = "어떻게든 살아남은 것 같다..";
                    break;
                case 2:
                    Uitext.text = "도대체 얼마나 시간이 흐른거지? 전래동화마냥 100년 이상이 흘러버린 건 아닐까?";
                    break;
                case 3:
                    Uitext.text = "끊임없이 변화하는 사계절의 섬을 돌아다니며 악착같이 살아남고는 있지만, 버티기가 힘들어진다.";
                    break;
                case 4:
                    Uitext.text = "어? 하늘 위에 무슨 소리가 들리는데...알람 소리..인가?";
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
