using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSearch : MonoBehaviour {
    [SerializeField]
    private GameObject searchView;

    public void Search(string input)
    {
        if (input.Length == 0)
        {
            searchView.gameObject.SetActive(false);
        }
        else
        {
            searchView.gameObject.SetActive(true);

            GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().ItemSearch(input);
        }
    }

    public void SearchClose()
    {
        GetComponent<InputField>().text = "";
    }
}
