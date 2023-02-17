using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    // Start is called before the first frame update
    private Vector3 camPos;
    // Update is called once per frame
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        camPos = player.position;
        camPos.y = 15;
        this.transform.position = camPos;
    }
    void Update()
    {
        camPos = player.position;
        camPos.y = 10;
        this.transform.position = camPos;
    }
}
