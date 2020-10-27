using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadChoose : MonoBehaviour
{
    public Toggle headNum;
    public Toggle tmp;
    public GameObject background;
    //public GameObject player;
    //public PlayerAttribute playerAttribute;
    public Sprite head;
    public ToggleGroup toggleGroup;
    public IEnumerable<Toggle> toggles;

    public StoragePlayerMsg storagePlayerMsg;

    private bool hasPlayAudio = false;
    public AudioClip clickClip;

    void Start()
    {
        toggles = toggleGroup.ActiveToggles();
        //playerAttribute = player.GetComponent<PlayerAttribute>();

        hasPlayAudio = false;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var toggle in toggles)
        {
            tmp = toggle;
            if  (tmp.isOn)
            {
                if(headNum != tmp && !hasPlayAudio)
                {
                    hasPlayAudio = true;
                    this.GetComponent<AudioSource>().clip = clickClip;
                    this.GetComponent<AudioSource>().Play();
                }
                headNum = tmp;
                hasPlayAudio = true;
                background = headNum.transform.Find("Background").gameObject;
                head = background.GetComponent<Image>().sprite;
                //playerAttribute.head = head;
                storagePlayerMsg.head = head;
            }
        }

    }


}
