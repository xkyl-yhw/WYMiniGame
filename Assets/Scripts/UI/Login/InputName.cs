using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputName : MonoBehaviour
{
    public StoragePlayerMsg storagePlayerMsg;
    private InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField = this.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        storagePlayerMsg.playerName = inputField.text;
    }
}
