using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Heal,
        Coffee
    }

    [SerializeField]
    private float amount;

    [SerializeField]
    private ItemType itemType;

    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 50f);    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // ��
            if (itemType == ItemType.Heal)
            {
                other.gameObject.GetComponent<Player>().TakeHeal(amount);
            }
            // Ŀ��
            else if(itemType == ItemType.Coffee)
            {
                other.gameObject.GetComponent<Player>().TakeCoffee(amount);
            }
            Destroy(gameObject);
        }
    }
}
