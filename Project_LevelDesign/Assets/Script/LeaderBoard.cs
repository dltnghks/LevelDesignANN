using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{

    public GameDataSave gameDataSave;

    [Serializable]
    public struct Rank
    {
        public Text playerName;
        public Text lastStage;
        public Text score;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct packet_user_data
    {
        public string playerName;
        public string lastStage;
        public string score;
    }


    public GameObject[] rankLine;

    public Rank[] rank;

    private void Start()
    {
        //gameObject.SetActive(false);
        if (gameDataSave.isLearningData)
        {
            for (int i = 0; i < rankLine.Length; i++)
            {
                rankLine[i].transform.GetChild(0).GetComponent<Text>().text = "#" + (i + 1).ToString();
                rank[i].playerName = rankLine[i].transform.GetChild(1).GetComponent<Text>();
                rank[i].lastStage = rankLine[i].transform.GetChild(2).GetComponent<Text>();
                rank[i].score = rankLine[i].transform.GetChild(3).GetComponent<Text>();
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < rankLine.Length; i++)
        {
            Color textColor = Color.black;
            if (gameDataSave.ranking[i].playerName == gameDataSave.userName)
                textColor = Color.red;

            rank[i].playerName.color = textColor;
            rank[i].lastStage.color = textColor;
            rank[i].score.color = textColor;

            rank[i].playerName.text = gameDataSave.ranking[i].playerName;
            rank[i].lastStage.text = gameDataSave.ranking[i].lastStage;
            rank[i].score.text = gameDataSave.ranking[i].score.ToString();

        }

    }
}
