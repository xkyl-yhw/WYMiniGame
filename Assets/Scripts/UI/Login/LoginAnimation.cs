using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginAnimation : MonoBehaviour
{
    public Text a;
    private int c = 1;

    public GameObject canvasFirst;
    public Button clickToEnter;

    // Start is called before the first frame update
    void Start()
    {
        clickToEnter.onClick.AddListener(ChangeToSelection);
    }

    // Update is called once per frame
    void Update()
    {
        if(a != null)
        {
            Vector4 b = a.color;
            b.w += Time.deltaTime * c;
            if (b.w>1.5 || b.w<-0.5)
            {
                c = -c;
            }
            a.color = b;
        }
    }

    void ChangeToSelection()
    {
        Destroy(canvasFirst);
    }
}
