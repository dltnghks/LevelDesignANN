using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private GameObject explosionParticlePrefab;

    private GameDataSave gameDataSave;

    private float time;
    private bool onTriger;

    public AudioClip clip;

    public void SetUp(float range, float time, GameDataSave gameDataSave)
    {
        SoundManager.instance.SFXPlay("explosion", clip);
        this.time = time;
        transform.localScale = new Vector3(range, range, range);
        this.gameDataSave = gameDataSave;
        onTriger = false;
        StartCoroutine(Boom());
    }



    private IEnumerator Boom()
    {
        GameObject clone = Instantiate(explosionParticlePrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // ���� �浹 �� ������ �ֱ�
        if (collision.gameObject.tag == "Enemy")
        {
            if (!onTriger)
            {
                gameDataSave.DataPlus(GameDataSave.DataType.hitCount);  // hiCount����(�÷��̾ �Ѿ��� ������ ��)
                                                                        //hit�Ÿ� �����ֱ�
                float dis = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
                gameDataSave.DataPlus(GameDataSave.DataType.hitDistanceSum, dis);

                onTriger = true;
            }

            collision.gameObject.GetComponent<Enemy>().TakeDamge(damage);
        }
    }
}
