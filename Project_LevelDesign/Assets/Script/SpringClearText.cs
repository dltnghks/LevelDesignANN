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
                    Uitext.text = "커피가 없었으면 큰일날 뻔했어..";
                    break;
                case 2:
                    Uitext.text = "이 미친 섬을 한시라도 빨리 탈출해야해..근데 갑자기 왜이리 덥지?";
                    break;
                case 3:
                    Uitext.text = "무슨 매미 소리도 들리는 것 같고..설마 계절이 벌써 바뀐건가?";
                    break;
                case 4:
                    Uitext.text = "도대체 이 섬은 정체가 뭐길래 이 모양인거야!!";
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
