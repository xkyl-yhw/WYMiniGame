using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerAttribute : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    public float healthMax;
    public float endurance;
    public TeamSetup team;
    public string teamTag;
    public int enduranceMax;
    public Sprite head;
    public int essencePickNum;
    public int essenceMax;
    public float essenceRate;
    public string playerName;
    public Text textName;
    public StoragePlayerMsg storagePlayerMsg;



    void Start()
    {
        team = GetComponent<TeamSetup>();
        teamTag = team.teamTag.ToString();
        storagePlayerMsg = GameObject.Find("StoragePlayerMsg").GetComponent<StoragePlayerMsg>();
        head = storagePlayerMsg.head;
        playerName = storagePlayerMsg.playerName;
        if (transform.Find("Name") != null)
            textName = transform.Find("Name").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.Find("Name") != null && textName == null)
        {
            textName = transform.Find("Name").GetComponent<Text>();
            textName.text = playerName;
        }
    }
}
