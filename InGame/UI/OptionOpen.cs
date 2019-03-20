using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionOpen : MonoBehaviour
{
    private bool isOptionCanvsActive = false;
    [SerializeField]
    private GameObject optionCanvas = null;
    [SerializeField]
    private GameObject tabUI = null;
    [SerializeField]
    private GameObject shopCanvas;
    [SerializeField]
    private GameObject bigPing;
    [SerializeField]
    private GameObject smallPing;

    Vector3 initialCamPos;
    PingSign pingSign;
    [SerializeField]
    private float pingResetTime = 7f;
    [SerializeField]
    private bool isPointerOverGameObject = false;
    [SerializeField]
    private bool isInitialCamPos = false;
    [SerializeField]
    private bool keyInput = false;

    void Start()
    {
        optionCanvas.SetActive(false);
        tabUI.SetActive(false);
        pingSign = bigPing.GetComponent<PingSign>();
        initialCamPos = Camera.main.transform.position;       
    }

    void Update()
    {
        OptionOpenCheck();
        PingInputCheck();
    }

    private void OptionOpenCheck()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!shopCanvas.gameObject.GetActive() && !optionCanvas.gameObject.GetActive())
            {
                isOptionCanvsActive = true;
                optionCanvas.SetActive(true);
                SoundManager.Instance.PlaySound(SoundManager.Instance.UI_Open);
            }
            else if (optionCanvas.gameObject.GetActive())
            {
                isOptionCanvsActive = false;
                optionCanvas.GetComponent<KTYOPTION>().CloseOptionWindow();
                SoundManager.Instance.PlaySound(SoundManager.Instance.UI_Close);
            }
        }        
    }

    private void PingInputCheck()
    {
        //Big 핑UI
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false
            && !smallPing.GetActive())
        {
            bigPing.SetActive(true);
        }
    }

    IEnumerator PingSignOff()
    {
        yield return new WaitForSeconds(5f);
        bigPing.SetActive(false);
    }
}   
