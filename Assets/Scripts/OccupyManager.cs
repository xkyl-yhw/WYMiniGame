using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OccupyManager : MonoBehaviour
{
    public List<OccupationPos> poss = new List<OccupationPos>();
    public GameObject player;
    public GameObject slider;
    public GameObject fill;
    public bool show = false;

    private void Start()
    {
        GameObject[] possObj = GameObject.FindGameObjectsWithTag("OccupyPos");
        foreach (var item in possObj)
        {
            if (item.GetComponent<OccupationPos>().occupyMode == OccupyMode.Time)
                poss.Add(item.GetComponent<OccupationPos>());
        }
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        show = false;
        foreach (var item in poss)
        {
            //Debug.Log(item.radius);
            if (item.radius < Vector3.Distance(item.transform.position, player.transform.position))
            {
                item.OccupySlider = null;
                item.SliderBack = null;
            }
            else
            {
                item.OccupySlider = slider.GetComponent<Slider>();
                item.SliderBack = fill;
                show = true;
            }
        }
        slider.SetActive(show);
    }
}
