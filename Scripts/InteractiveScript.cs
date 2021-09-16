using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class InteractiveScript : MonoBehaviour
{

    [Serializable] struct Item
    {
        public string itemName;
        public string itemDes;
    }

    [SerializeField] Item[] allItems;

    // Start is called before the first frame update
    void Start()
    {
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;
        for (int i = 0; i < 5; i++)
        {
            g = Instantiate(buttonTemplate, transform);
            g.transform.GetChild(0).GetComponent<TMP_Text>().text = allItems[i].itemName;
            g.transform.GetChild(1).GetComponent<TMP_Text>().text = allItems[i].itemDes;

            g.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                ItemClicked(i);
            });
        }

        Destroy(buttonTemplate);
    }

    void ItemClicked(int itemIndex)
    {
        //Code that reacts to user pressing the btn
    }
}
