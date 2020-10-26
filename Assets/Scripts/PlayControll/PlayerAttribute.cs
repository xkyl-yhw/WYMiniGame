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
    public string  teamTag;
    public int enduranceMax;
    public Sprite head;
    public int essencePickNum;
    public int essenceMax;
    public float essenceRate;

    

    void Start()
    {
        team = GetComponent<TeamSetup>();
        teamTag = team.teamTag.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
