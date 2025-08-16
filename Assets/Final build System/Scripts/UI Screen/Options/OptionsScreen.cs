using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{

    public Transform content;
    public OpitonItem itemPrefab;
    private void Awake()
    {
        CustomizationsScreen.DisplayOption += DisplayOptions;
    }

    private void DisplayOptions(Option option)
    {
        var Dictionary= Manager._instance.selectedCharacter.GetComponent<CharacterCustomization>().keyValuePairs;
        if (Dictionary.Count == 0)
        {
            Manager._instance.selectedCharacter.GetComponent<CharacterCustomization>().Initialize(Manager._instance.characterStorage);
        }
        Dictionary = Manager._instance.selectedCharacter.GetComponent<CharacterCustomization>().keyValuePairs;
        int index = -1;
        foreach(var value in Manager._instance.selectedCharacter.GetComponent<CharacterCustomization>().customizations)
        {
            index++;
            if (Dictionary[option] == value.details)
            {
                Clear();
                DisplayItems(value.details,index);
                break;
            }
        }


        //List<CustomizationObject> values = Dictionary[option];
       
    }

    public void DisplayItems(List<CustomizationObject>values,int index)
    {

       print(values.Count);
       foreach(var value in values)
       {
            OpitonItem item = Instantiate(itemPrefab, content);
            item.item_text.text = value.gameObjects[0].name;
            item.GetComponent<Button>().onClick.AddListener(() => basic(value,values,index));
       }
    }

    public void basic(CustomizationObject value ,List<CustomizationObject>values,int index)
    {
        CharacterStorage s = Manager._instance.characterStorage;
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] == value)
            {
                foreach(var item in values[i].gameObjects)
                {
                    item.gameObject.SetActive(true);
                }
                values[i].activated= true;
            }
            else
            {
                foreach (var item in values[i].gameObjects)
                {
                    item.gameObject.SetActive(false);
                }
                values[i].activated = false;
            }

            s.customizations[index].selectedOption[i] = values[i].activated;

        }
    }

    private void Clear()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}
