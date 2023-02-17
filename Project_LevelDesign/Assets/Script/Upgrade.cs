using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }

    //***********************Upgrade �Լ���*****************************/

    // ������ ����
    public void UpgradeAttackPower()
    {
        player.UpgradeAttackPower();
    }

    // ��ų ��Ÿ�� ����
    public void UpgradeSkillCoolTimeDown()
    {
        player.UpgradeSkillCoolTimeDown();
    }

    // ��ų ���� ����
    public void UpgradeExplosionRangeUp()
    {

        player.UpgradeExplosionRangeUp();
    }

    // �ִ� ü�� ����
    public void UpgradeMaxHealthUp()
    {
        player.UpgradeMaxHealthUp();
    }

    // ���� ��ų
    public void UpgradeTransfusionUp()
    {
        player.UpgradeTransfusionUp();
    }

    // ī���� ���ҷ� ����
    public void UpgradeDecreaseCoffeine()
    {
        player.UpgradeDecreaseCoffeine();
    }
}
