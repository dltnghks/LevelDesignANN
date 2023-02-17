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
                    Uitext.text = "이제 날씨는 추워져서 겨울이 다가오고 있음이 느껴진다.";
                    break;
                case 2:
                    Uitext.text = "한순간에 바뀌는 사계절에 시간감각도 무뎌지는 기분이다.";
                    break;
                case 3:
                    Uitext.text = "하지만 나는 살아남을 것이다. 어떻게 여기까지 왔는데 포기할 순 없다.";
                    break;
                case 4:
                    Uitext.text = "저 멀리 뭔가가 몰려오고 있다.. 저건.. 눈사람?";
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
