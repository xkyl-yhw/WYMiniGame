using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour,IPointerDownHandler
{
    public bool isOpen;//是否点开大地图
    public PolygonCollider2D polygonCollider2D;
    public Transform OrPoint;
    private Vector3 mapSize;
    private Vector2 miniMapSize;
    public Canvas bigMap;
    void Start()
    {
    }
    void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        OrPoint = transform.Find("OrPoint");
        mapSize = new Vector3(200f, 0.01f, 200f);//地图实体大小
        miniMapSize = new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);//小地图大小
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (polygonCollider2D.OverlapPoint(eventData.position)|| (Input.GetKeyDown(KeyCode.Tab)))
        {
            OrPoint.position = eventData.position;
            Debug.Log("点击了地图,点击的点为" + eventData.position.ToString());
            Debug.Log("OrPoint的本地坐标为" + OrPoint.localPosition.ToString());
            isOpen = !isOpen;
            bigMap.gameObject.SetActive(isOpen);

        }
    }

}
