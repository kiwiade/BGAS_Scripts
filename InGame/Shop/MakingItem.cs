using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakingItem : MonoBehaviour {

    [SerializeField]
    private GameObject makingContent;
    [SerializeField]
    private Button leftButton;
    [SerializeField]
    private Button rightButton;

    private int myItemCount = 0;
    private int pageMaxNum = 6;

    private int maxPage = 1;
    private int currentPage = 1;

    private bool leftButtonOn = false;
    private bool rightButtonOn = false;

    public void ItemCount(int count)
    {
        myItemCount = count;

        currentPage = 1;
        maxPage = (myItemCount - 1) / pageMaxNum + 1;

        makingContent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        ButtonCheck();
    }

    public void ButtonCheck()
    {
        if (maxPage == 1)
        {
            leftButtonOn = false;
            rightButtonOn = false;
        }
        else
        {
            if(currentPage == 1)
            {
                leftButtonOn = false;
                rightButtonOn = true;
            }
            else if(currentPage == maxPage)
            {
                leftButtonOn = true;
                rightButtonOn = false;
            }
            else
            {
                leftButtonOn = true;
                rightButtonOn = true;
            }
        }

        // 버튼 상태에따라 색상변경
        ColorBlock leftButtonColor = leftButton.colors;
        ColorBlock rightButtonColor = rightButton.colors;

        if (leftButtonOn == true)
        {
            leftButtonColor.normalColor = new Color(69f / 255f, 66f / 255f, 47f / 255f);
            leftButtonColor.highlightedColor = new Color(132f / 255f, 124f / 255f, 74f / 255f);
            leftButtonColor.pressedColor = new Color(69f / 255f, 66f / 255f, 47f / 255f);
        }
        else
        {
            leftButtonColor.normalColor = new Color(58f / 255f, 58f / 255f, 58f / 255f);
            leftButtonColor.highlightedColor = new Color(58f / 255f, 58f / 255f, 58f / 255f);
            leftButtonColor.pressedColor = new Color(58f / 255f, 58f / 255f, 58f / 255f);
        }
        leftButton.colors = leftButtonColor;

        if (rightButtonOn == true)
        {
            rightButtonColor.normalColor = new Color(69f / 255f, 66f / 255f, 47f / 255f);
            rightButtonColor.highlightedColor = new Color(132f / 255f, 124f / 255f, 74f / 255f);
            rightButtonColor.pressedColor = new Color(69f / 255f, 66f / 255f, 47f / 255f);
        }
        else
        {
            rightButtonColor.normalColor = new Color(58f / 255f, 58f / 255f, 58f / 255f);
            rightButtonColor.highlightedColor = new Color(58f / 255f, 58f / 255f, 58f / 255f);
            rightButtonColor.pressedColor = new Color(58f / 255f, 58f / 255f, 58f / 255f);
        }
        rightButton.colors = rightButtonColor;
    }

    public void LeftButtonClick()
    {
        if (leftButtonOn == false)
            return;

        currentPage--;
        makingContent.GetComponent<RectTransform>().localPosition += new Vector3(300, 0);

        ButtonCheck();
    }

    public void RightButtonClick()
    {
        if (rightButtonOn == false)
            return;

        currentPage++;
        makingContent.GetComponent<RectTransform>().localPosition -= new Vector3(300, 0);

        ButtonCheck();
    }
}
