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

    //***********************Upgrade 함수들*****************************/

    // 데미지 증가
    public void UpgradeAttackPower()
    {
        player.UpgradeAttackPower();
    }

    // 스킬 쿨타임 감소
    public void UpgradeSkillCoolTimeDown()
    {
        player.UpgradeSkillCoolTimeDown();
    }

    // 스킬 범위 증가
    public void UpgradeExplosionRangeUp()
    {

        player.UpgradeExplosionRangeUp();
    }

    // 최대 체력 증가
    public void UpgradeMaxHealthUp()
    {
        player.UpgradeMaxHealthUp();
    }

    // 흡혈 스킬
    public void UpgradeTransfusionUp()
    {
        player.UpgradeTransfusionUp();
    }

    // 카페인 감소량 감소
    public void UpgradeDecreaseCoffeine()
    {
        player.UpgradeDecreaseCoffeine();
    }
}
