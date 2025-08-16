using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text itemName;



    public void SetItem(string name)
    {
        itemName.text = name;
    }

}
