using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public partial class CharacterCustomization : MonoBehaviour
{
    public List<customization> customizations;
    public Dictionary<Option, List<CustomizationObject>> keyValuePairs;
    public Gender gender;

    public void Initialize(CharacterStorage characterStorage)
    {
        
        for (int i = 0; i < customizations.Count; i++)
        {
            for (int j = 0; j < customizations[i].details.Count; j++)
            {
                foreach (var obj in customizations[i].details[j].gameObjects)
                {
                    obj.gameObject.SetActive(characterStorage.customizations[i].selectedOption[j]);
                }
            }
        }
        keyValuePairs = new Dictionary<Option, List<CustomizationObject>>();
        for (int i = 0; i < customizations.Count; i++)
        {
            keyValuePairs.Add(customizations[i].option, customizations[i].details);
        }
    }

    public void SetStorage(CharacterStorage characterStorage)
    {
        for (int i = 0; i < customizations.Count; i++)
        {
            for (int j = 0; j < customizations[i].details.Count; j++)
            {
                foreach (var obj in customizations[i].details[j].gameObjects)
                {
                    obj.gameObject.SetActive(characterStorage.customizations[i].selectedOption[j]);
                }

            }
        }
        keyValuePairs = new Dictionary<Option, List<CustomizationObject>>();
        for (int i = 0; i < customizations.Count; i++)
        {
            keyValuePairs.Add(customizations[i].option, customizations[i].details);
        }
    }

    private void Start()
    {
        keyValuePairs = new Dictionary<Option, List<CustomizationObject>>();
    }
}
