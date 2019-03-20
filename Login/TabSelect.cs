using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabSelect : MonoBehaviour
{

    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        TabCheck();
    }

    private void TabCheck()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable nextObject = null;
            Selectable currentObject = null;

            CurrentObjectRenew(ref currentObject);
            NextObjectSelect(currentObject, ref nextObject);

            if (nextObject != null)
                nextObject.Select();
        }
    }

    private void CurrentObjectRenew(ref Selectable currentObject)
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            if (eventSystem.currentSelectedGameObject.activeInHierarchy)
            {
                currentObject = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
            }
        }
    }

    private void NextObjectSelect(Selectable currentObject, ref Selectable nextObject)
    {
        if (currentObject != null)
        {
            // shift 누르고 있으면 (shift + tab 이면) 뒤로
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                nextObject = currentObject.FindSelectableOnUp();
                if (nextObject == null)
                    nextObject = currentObject.FindSelectableOnLeft();
            }
            else
            {
                nextObject = currentObject.FindSelectableOnDown();
                if (nextObject == null)
                    nextObject = currentObject.FindSelectableOnRight();
            }
        }
        // current가 null이면 선택가능한 전체에서 1번째를 잡음
        else
        {
            if (Selectable.allSelectables.Count > 0)
                nextObject = Selectable.allSelectables[0];
        }
    }
}
