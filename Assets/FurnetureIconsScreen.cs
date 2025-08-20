using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnetureIconsScreen : MonoBehaviour
{
    [SerializeField] GameObject prefab ,content;
    [SerializeField] ObjectDatabaseSO database;
    // Start is called before the first frame update
    public void Init()
    {
        if (content.transform.childCount > 0) return;
        foreach(var pref in database.objectData)
        {
            var obj = Instantiate(prefab, content.transform);
            Button button = obj.GetComponent<Button>();
            TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
            text.text = pref.Name;
            button.onClick.AddListener(() => Instantiate(pref.id));
        }
           
    }

    private void Instantiate(int index)
    {
        print(index);
        BuildingSystem.instance.Initialize(index);
    }
}
