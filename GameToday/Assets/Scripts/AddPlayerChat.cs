using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddPlayerChat : MonoBehaviour
{
    [SerializeField] private GameObject chatGameObject;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text playerChat;
    [SerializeField] private GameObject panel;
    [SerializeField] private float newChatYPos;
    [SerializeField] private List<GameObject> chatList = new();
    [SerializeField] private float yIncrease;
    [SerializeField] private Movement parser;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FinishEditing()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            if (chatList.Count > 0)
            {
                foreach (GameObject text in chatList)
                {
                    text.transform.localPosition = new Vector3(text.transform.localPosition.x, text.transform.localPosition.y + yIncrease, text.transform.localPosition.z);
                }
            }
            GameObject instatiateChat = Instantiate(chatGameObject, panel.transform);
            instatiateChat.transform.localPosition = new Vector3(instatiateChat.transform.localPosition.x, newChatYPos, instatiateChat.transform.localPosition.z);
            chatList.Add(instatiateChat);
            playerChat = instatiateChat.GetComponentInChildren<TMP_Text>();
            playerChat.text = PlayerPrefs.GetString("Name") +": "+ inputField.text;
            parser.StringParser(inputField.text);
            inputField.text = "";
        }
    }

    public void AddChat(string chatMessage)
    {
        if (chatList.Count > 0)
        {
            foreach (GameObject text in chatList)
            {
                text.transform.localPosition = new Vector3(text.transform.localPosition.x, text.transform.localPosition.y + yIncrease, text.transform.localPosition.z);
            }
        }
        GameObject instatiateChat = Instantiate(chatGameObject, panel.transform);
        
        instatiateChat.GetComponentInChildren<TMP_Text>().text = chatMessage;
        //if (instatiateChat != null)
        //    instatiateChat.GetComponent<RectTransform>().sizeDelta = new Vector2(instatiateChat.GetComponent<RectTransform>().rect.height, instatiateChat.GetComponentInChildren<RectTransform>().rect.y);
        instatiateChat.transform.localPosition = new Vector3(instatiateChat.transform.localPosition.x, newChatYPos, instatiateChat.transform.localPosition.z);
        chatList.Add(instatiateChat);
        
    }
}
