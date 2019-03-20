using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseAtHome : MonoBehaviour {

    private float recoveryTime = 0;

    private void OnTriggerStay(Collider other)
    {
        // 구매가능
        if (other.tag.Equals("Player"))
        {
            PlayerData.Instance.purchaseState = true;
            ChampionData inColliderChampionData = other.GetComponent<ChampionData>();
            if (inColliderChampionData != null)
            {
                recoveryTime += Time.deltaTime;
                if (recoveryTime >= 0.25f)
                {
                    // 체력회복
                    if (inColliderChampionData.totalStat.Hp < inColliderChampionData.totalStat.MaxHp)
                    {
                        // 0.25초당 2.5%회복
                        inColliderChampionData.totalStat.Hp += inColliderChampionData.totalStat.MaxHp * 0.025f;
                        if (inColliderChampionData.totalStat.Hp > inColliderChampionData.totalStat.MaxHp)
                            inColliderChampionData.totalStat.Hp = inColliderChampionData.totalStat.MaxHp;
                    }
                    // 마나회복
                    if (inColliderChampionData.totalStat.Mp < inColliderChampionData.totalStat.MaxMp)
                    {
                        // 0.25초당 2.5%회복
                        inColliderChampionData.totalStat.Mp += inColliderChampionData.totalStat.MaxMp * 0.025f;
                        if (inColliderChampionData.totalStat.Mp > inColliderChampionData.totalStat.MaxMp)
                            inColliderChampionData.totalStat.Mp = inColliderChampionData.totalStat.MaxMp;
                    }
                    recoveryTime -= 0.25f;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            // 구매불가
            PlayerData.Instance.purchaseState = false;
            // 되돌리기 불가
            PlayerData.Instance.ItemUndoListReset();
        }
    }
}
