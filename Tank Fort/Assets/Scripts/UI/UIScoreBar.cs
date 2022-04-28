using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreBar : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] RectTransform rectTransform;
    public bool growing;
    [SerializeField] float speed = 100f;
    float targetLength = 100;
    private void Update()
    {
        if (growing)
        {
            float length = rectTransform.sizeDelta.x;
            if(length + speed * Time.deltaTime > targetLength)
            {
                rectTransform.sizeDelta = new Vector2(targetLength, rectTransform.sizeDelta.y);
                growing = false;
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(length + speed * Time.deltaTime, rectTransform.sizeDelta.y);
            }
        }
    }

    public void setup(float length)
    {
        Vector2 size = new Vector2(length, rectTransform.sizeDelta.y);
        setup(size);
    }
    public void setup(float length, Vector2 pos)
    {
        Vector2 size = new Vector2(length, rectTransform.sizeDelta.y);
        setup(size, pos);
    }
    public void setup(Vector2 size)
    {
        targetLength = size.x;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size.y);
        growing = true;
    }
    public void setup(Vector2 size, Vector2 pos)
    {
        rectTransform.localPosition = pos;
        setup(size);
    }
    public void setup(Vector2 size, Vector2 pos, Color color)
    {
        image.color = color;
        setup(size, pos);
    }
}
