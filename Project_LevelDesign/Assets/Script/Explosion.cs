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
        // 적과 충돌 시 데미지 주기
        if (collision.gameObject.tag == "Enemy")
        {
            if (!onTriger)
            {
                gameDataSave.DataPlus(GameDataSave.DataType.hitCount);  // hiCount증가(플레이어가 총알을 맞췄을 때)
                                                                        //hit거리 더해주기
                float dis = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
                gameDataSave.DataPlus(GameDataSave.DataType.hitDistanceSum, dis);

                onTriger = true;
            }

            collision.gameObject.GetComponent<Enemy>().TakeDamge(damage);
        }
    }
}
