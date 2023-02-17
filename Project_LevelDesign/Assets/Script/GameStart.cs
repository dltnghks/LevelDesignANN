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
                    Uitext.text = "섬..인건가? 근데 왜이리 커피냄새가 나지?";
                    break;
                case 2:
                    Uitext.text = "!! 뭐야 이거.. 왜 길가에 커피가 굴러다녀?";
                    break;
                case 3:
                    Uitext.text = "무슨 이런 섬이 다 있지.. 일단 돌아다녀볼까..";
                    break;
                case 4:
                    Uitext.text = "...!! 뭐야 저 커다란 동물들은!!";
                    break;
                case 5:
                    Uitext.text = "으아악!! 오지마!!";
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