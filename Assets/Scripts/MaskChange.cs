using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MaskChange : MonoBehaviour
{
    private GameObject mask;
    private Image maskImage;
    private readonly Color[] listColor = new Color[] { new(255, 0, 0), new(0, 0, 255), new(0, 255, 0) };
    private string currentColor;

    void Awake()
    {
        mask = GameObject.FindGameObjectWithTag("Mask");
        maskImage = mask.GetComponent<Image>();
    }

    void Start()
    {
        GameObject[] gameObjectsRed = GameObject.FindGameObjectsWithTag("Red");
        GameObject[] gameObjectsBlue = GameObject.FindGameObjectsWithTag("Blue");
        GameObject[] gameObjectsGreen = GameObject.FindGameObjectsWithTag("Green");
        currentColor = "Red";

        maskImage.color = listColor[0];

        foreach (GameObject gameObject in gameObjectsRed)
        {
            MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
            ms.enabled = true;
        }
        foreach (GameObject gameObject in gameObjectsBlue)
        {
            MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
            ms.enabled = false;
        }
        foreach (GameObject gameObject in gameObjectsGreen)
        {
            MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
            ms.enabled = false;
        }
    }

    public void OnMask(InputValue value)
    {
        GameObject[] gameObjectsRed = GameObject.FindGameObjectsWithTag("Red");
        GameObject[] gameObjectsBlue = GameObject.FindGameObjectsWithTag("Blue");
        GameObject[] gameObjectsGreen = GameObject.FindGameObjectsWithTag("Green");

        if (currentColor == "Red")
        {
           
            Debug.Log("Je suis au bleu");


            currentColor = "Blue";
            foreach (GameObject gameObject in gameObjectsRed)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = false;
            }
            foreach (GameObject gameObject in gameObjectsBlue)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = true; ;
            }
            maskImage.color = listColor[1];
        }
        else if (currentColor == "Blue")
        {
            currentColor = "Green";
            foreach (GameObject gameObject in gameObjectsBlue)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = false;
            }
            foreach (GameObject gameObject in gameObjectsGreen)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = true;
            }
            maskImage.color = listColor[2];
        }
        else if (currentColor == "Green")
        {
            currentColor = "Red";
            foreach (GameObject gameObject in gameObjectsGreen)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = false;
            }
            foreach (GameObject gameObject in gameObjectsRed)
            {
                MeshRenderer ms = gameObject.GetComponent<MeshRenderer>();
                ms.enabled = true;
            }
            maskImage.color = listColor[0];
        }
    }
}
