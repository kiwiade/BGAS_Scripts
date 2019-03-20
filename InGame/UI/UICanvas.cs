using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    public GameObject item;
    public GameObject skill;
    public GameObject stat;
    public GameObject icon;
    public GameObject recall;
    public GameObject tooltip;
    public GameObject itemTooltip;
    public GameObject rightTopUI;

    [Space]
    [SerializeField]
    private GameObject tabUI;
    [SerializeField]
    private GameObject enemyUI;
    [SerializeField]
    private GameObject chatUI;

    private ChatFunction chatFunction;
    Vector3 rayVector;
    Ray ray;
    RaycastHit[] hits;

    private float refreshTime = -5;
    private float refreshPeriod = 0.5f;


    void Start()
    {
        chatFunction = chatUI.transform.parent.GetComponent<ChatFunction>();
    }

    void Update()
    {
        HotKeyCheck();
        EnterCheck();
        ObjectClickCheck();
        RegularlyTabUpdate();
    }

    private void HotKeyCheck()
    {
        if (!chatFunction.chatInput.IsActive())
        {
            // P 상점
            if (Input.GetKeyDown(KeyCode.P))
            {
                item.GetComponent<ItemUI>().ShopButton();
            }

            // Tab 스코어보드
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tabUI.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                tabUI.SetActive(false);
            }
        }
    }

    private void EnterCheck()
    {
        // Enter 채팅창
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 꺼져있으면 켜기
            if (!chatUI.activeSelf)
            {
                chatUI.SetActive(true);
                chatUI.GetComponent<InputField>().ActivateInputField();
                chatFunction.RevealScroll();
            }
            else
            {
                chatFunction.Send();
                chatUI.SetActive(false);
                chatFunction.HideScroll();
            }
        }
    }

    private void ObjectClickCheck()
    {
        // 적클릭. ray로 쏘기
        if (Input.GetMouseButtonDown(0))
        {
            rayVector = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(rayVector);
            hits = Physics.RaycastAll(ray);

            bool find = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.tag.Equals("Minion") || hit.collider.tag.Equals("Tower")
                    || hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Champion"))
                    || hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
                {
                    if (hit.transform.GetComponent<FogOfWarEntity>().isCanTargeting)
                    {
                        find = true;
                        enemyUI.SetActive(true);
                        enemyUI.GetComponent<UIEnemy>().ApplyObject(hit.collider.gameObject);
                    }
                }
            }

            if (!find)
            {
                enemyUI.SetActive(false);
                enemyUI.GetComponent<UIEnemy>().selectedObject = null;
            }
        }
    }

    private void RegularlyTabUpdate()
    {
        // 주기적으로 상대 정보업데이트하기
        refreshTime += Time.deltaTime;
        if (refreshTime >= refreshPeriod)
        {
            tabUI.GetComponent<TabUI>().TabRefresh();
            refreshTime -= refreshPeriod;
        }
    }
}
