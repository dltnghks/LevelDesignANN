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
                    Uitext.text = "음..? 여기는.. 동네 공원이잖아.. 알람은..내 핸드폰 소리인가.";
                    break;
                case 2:
                    Uitext.text = "꿈인건가? 도대체 얼마나 몰입한거야..어쩐지 모든게 말이 안된다 싶었지.";
                    break;
                case 3:
                    Uitext.text = "꿈이라기엔 너무 실감나서 한편으로는 식겁했다는 생각이든다. ";
                    break;
                case 4:
                    Uitext.text = "하늘에 구름이 낀게 비가 올 것 같으니 빨리 집으로 가야겠다.";
                    break;
                case 5:
                    SceneManager.LoadScene("End"); //엔딩 타이틀
                    break;
                default:
                    break;
            }
        }
    }
}


