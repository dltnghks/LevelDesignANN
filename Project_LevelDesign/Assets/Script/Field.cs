using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("충돌");
            collision.gameObject.GetComponent<Player>().isSlow = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("충돌 X");
            other.gameObject.GetComponent<Player>().isSlow = false;
        }
    }
}
