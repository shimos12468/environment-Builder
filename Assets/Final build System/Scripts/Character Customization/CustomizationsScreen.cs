using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationsScreen : MonoBehaviour
{
    public Categories categories;
    public Transform content;
    public Item itemPrefab;
    public static event Action<Option> DisplayOption;
    List<Categories.Item> items = new List<Categories.Item>();
    private void Start()
    {
        Clear();
        Initialize();
    }

    private void Initialize()
    {
        items = categories.GetItems();

        foreach (var item in items)
        {
            Item g = Instantiate(itemPrefab, content);
            g.SetItem(item.categoryName.ToString());
            g.gameObject.GetComponent<Button>().onClick.AddListener(() => DisplayOption.Invoke(item.categoryName));
        }
    }

   

    private void Clear()
    {
       for(int i = 0; i < content.childCount; i++)
       {
            Destroy(content.GetChild(i).gameObject);
       }
    }

  
}
