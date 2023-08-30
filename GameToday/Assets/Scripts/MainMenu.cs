using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_InputField genderInputField;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Cursor.visible);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        if (nameInputField.text != string.Empty && genderInputField.text != string.Empty)
        {
            PlayerPrefs.SetString("Name", nameInputField.text);
            SceneManager.LoadScene(1);
        }
    }    
    
    public void StartLive()
    {
        SceneManager.LoadScene(2);
    }
}
