using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonIcon : MonoBehaviour
{
    public Sprite defaultGraphic, graphicToChangeTo;
    private bool isImageChanged = false;

    // Start is called before the first frame update
    void Start()
    {
        Image imageComponent = GetComponent<Image>();
        imageComponent.sprite = defaultGraphic;
    }

    public void setIcon()
    {
        Image imageComponent = GetComponent<Image>();

        switch (isImageChanged)
        {
            case true:
                imageComponent.sprite = defaultGraphic;
                isImageChanged = !isImageChanged;
                break;
            case false:
                imageComponent.sprite = graphicToChangeTo;
                isImageChanged = !isImageChanged;
                break;
        }
    }
}
