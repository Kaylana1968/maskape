using UnityEngine;

public class CubeColorManager : MonoBehaviour
{

    private readonly Color32[] listColor = new Color32[]
    {
        new Color32(255, 0, 0, 255),  // le rouge monsieur
        new Color32(0, 0, 255, 255),   // le bleu messire
        new Color32(0, 255, 0, 255)    // le vert monseigneur
    };

    void Start()
    {
        // Même logique que ton fichier MaskChange
        GameObject[] gameObjectsRed = GameObject.FindGameObjectsWithTag("Red");
        GameObject[] gameObjectsBlue = GameObject.FindGameObjectsWithTag("Blue");
        GameObject[] gameObjectsGreen = GameObject.FindGameObjectsWithTag("Green");

        foreach (GameObject go in gameObjectsRed)
        {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.material.color = listColor[0];
        }

        foreach (GameObject go in gameObjectsBlue)
        {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.material.color = listColor[1];
        }

        foreach (GameObject go in gameObjectsGreen)
        {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.material.color = listColor[2];
        }
    }
}
