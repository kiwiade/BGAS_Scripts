using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    public Image Bar = null;
    [SerializeField]
    private Text ProgressText;

    [Range(0, 1.0f)]
    public float value = 1;
    [HideInInspector]
    public string text = "";

	void Update () {
        if(Bar.enabled)
            Bar.fillAmount = value;

        if(ProgressText != null)
            ProgressText.text = this.text;
	}
}
