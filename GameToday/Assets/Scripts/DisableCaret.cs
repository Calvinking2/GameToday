using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisableCaret : MonoBehaviour
{

    private TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.caretWidth = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
