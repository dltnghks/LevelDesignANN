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
                    Uitext.text = "근데 동물뿐 아니라 곤충도 몰려오고 있고, 졸음은 끊임없이 쏟아지고 있고..";
                    break;
                case 2:
                    Uitext.text = "커피를 계속 마셔야해서 금방이라도 카페인 중독에 걸릴 것 같은 기분이다..";
                    break;
                case 3:
                    Uitext.text = "슬슬 서늘해지고 나무의 색이 바뀌어가는 걸 보니 가을이 오는 것 같다.";
                    break;
                case 4:
                    Uitext.text = "일단 어쩔 수 없지, 앞으로 가는 수밖에!!";
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