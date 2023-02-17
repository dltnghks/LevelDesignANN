using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;      // �Ѿ� ������
    private float transfusion; // ���� ��
    private float explosionRange; // ���� ����
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private GameObject bulletEffect;
    private bool isExplosionOn; // ���� �Ѿ� ON/OFF

    private GameDataSave gameDataSave;

    private Player player;


    public AudioClip clip;

    // Start is called before the first frame update
    public void SetUp(bool isExplosionOn, float damage, float Transfusion, float explosionRange, GameObject player, GameDataSave gameDataSave)
    {
        this.player = player.GetComponent<Player>();
        this.gameDataSave = gameDataSave;
        SoundManager.instance.SFXPlay("shot", clip);
        this.explosionRange = explosionRange;
        this.transfusion = Transfusion;
        this.damage = damage;
        this.isExplosionOn = isExplosionOn;
    }
    void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    void Update()
    {
        
        //rigidbody.MovePosition(transform.position + targetPos);
    }

    // 2�� �� �ı�
    private IEnumerator DestroyBullet()
    {
        // 폭발 공격일 때
        if (isExplosionOn) {
            yield return new WaitForSeconds(0.05f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        BulletDestroy();
    }


    // �浹 ó��
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.tag);
        // ���� �浹 �� ������ �ֱ�
        if(collision.gameObject.tag == "Enemy")
        {
            if (!isExplosionOn)
            {
                // 몬스터에게 데미지
                collision.gameObject.GetComponent<Enemy>().TakeDamge(damage);

                // 게임 데이터 조절
                gameDataSave.DataPlus(GameDataSave.DataType.hitCount);  // hiCount증가(플레이어가 총알을 맞췄을 때)
                                                                        //hit거리 더해주기
                float dis = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
                gameDataSave.DataPlus(GameDataSave.DataType.hitDistanceSum, dis);

            }
            // 총알 삭제
            BulletDestroy();
        }
        // �ٴڰ� �浹 �� ����
        if (collision.gameObject.tag == "Floor")
        {
            BulletDestroy();
        }
        // ���� �浹 �� ����
        else if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Obstacle")
        {
            BulletDestroy();
        }
    }

    private void BulletDestroy()
    {
        if (isExplosionOn)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            explosion.GetComponent<Explosion>().SetUp(explosionRange, 0.5f, gameDataSave);
        }
        GameObject clone = Instantiate(bulletEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
