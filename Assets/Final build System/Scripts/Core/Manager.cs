using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public GameObject selectedCharacter;
    public static Manager _instance;
    public CharacterStorage characterStorage;
    public Gender playerGender;
    public List<GameObject>characters;
    public GameObject male ,female;
    public bool StartScreen =true;
    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }


        if (StartScreen)
        {
            characters = GameObject.FindGameObjectsWithTag("Player").ToList();

            if (characters.Count == 0)
            {
                Initialize();
            }

            foreach (var c in characters)
            {
                c.gameObject.SetActive(false);
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("Gender"))
            {
                playerGender = (Gender)PlayerPrefs.GetInt("Gender");
                if (playerGender==Gender.Male)
                {
                    selectedCharacter = Instantiate(male, Vector3.up*2, Quaternion.identity, transform);
                }
                else
                {
                    selectedCharacter = Instantiate(female, Vector3.up*2, Quaternion.identity, transform);
                }
                selectedCharacter.transform.GetChild(selectedCharacter.transform.childCount - 1).gameObject.SetActive(true);
                selectedCharacter.GetComponent<CharacterCustomization>().SetStorage(characterStorage);
            }
            else
            {
                selectedCharacter = Instantiate(male, Vector3.up * 2, Quaternion.identity, transform);
                selectedCharacter.transform.GetChild(selectedCharacter.transform.childCount - 1).gameObject.SetActive(true);
                selectedCharacter.GetComponent<CharacterCustomization>().SetStorage(characterStorage);

            }
            

        }

       
    }

    public void EnablePlayerCamera(bool state)
    {
        selectedCharacter.GetComponent<PlayerController>().EnableCamera(state);
    }

    private void Initialize()
    {
        GameObject go = Instantiate(male, Vector3.zero, Quaternion.identity, transform);
        characters.Add(go);
        go.GetComponent<CharacterCustomization>().Initialize(characterStorage);
        go = Instantiate(female, Vector3.zero, Quaternion.identity, transform);
        characters.Add(go);
        go.GetComponent<CharacterCustomization>().Initialize(characterStorage);
    }

    public void SetSelectedCharacter(int gender)
    {
        GameObject c = null;
        playerGender = gender == 0 ? Gender.Male : Gender.Female;
        print(playerGender);
        foreach(var obj in characters)
        {
            if (playerGender == obj.GetComponent<CharacterCustomization>().gender)
            {
                c = obj;
                c.gameObject.SetActive(true);
            }
            else
            {
                obj.gameObject.SetActive(false);
            }
        }

        selectedCharacter= c;
        if (c == null)
        {
             selectedCharacter = InitializeCharacter(playerGender);
             selectedCharacter.gameObject.SetActive(true);
        }

        PlayerPrefs.SetInt("Gender", (int)playerGender);
    }

    private GameObject InitializeCharacter(Gender character)
    {
        GameObject go = null;
        if (character == Gender.Male)
        {
             go= Instantiate(male,Vector3.zero,Quaternion.identity, transform);
        }
        else
        {
             go=Instantiate(female,Vector3.zero, Quaternion.identity,transform);
        }
        characters.Add(go);

        return go;
    }

    public CharacterStorage GetStorage()
    {
        return characterStorage;
    }

    public void GoToHomeScene()
    {
         //selectedCharacter.GetComponent<CharacterCustomization>().SaveStorage(characterStorage);
         SceneManager.LoadSceneAsync("Home",LoadSceneMode.Single);
    }

   
}
