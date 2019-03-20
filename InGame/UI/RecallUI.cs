using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RecallUI : MonoBehaviour {

    public ProgressBar recallProgressBar;
    public Text remainTime;

    public void RecallButton()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>().Recall();
    }
}
