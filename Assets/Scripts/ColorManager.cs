using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //XRSettings.eyeTextureResolutionScale = 1.3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<Color> colors = new List<Color>
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        Color.black,
        Color.white,
        Color.gray,
        Color.grey,
        Color.clear,
        new Color(255, 0, 0, 255),
        new Color(0, 255, 0, 255),
        new Color(0, 0, 255, 255),
        new Color(255, 255, 0, 255),
        new Color(0, 255, 255, 255),
        new Color(255, 0, 255, 255),
        new Color(255, 0, 0, 255),
        new Color(0, 255, 0, 255),
        new Color(0, 0, 255, 255),
        new Color(255, 255, 0, 255),
        new Color(0, 255, 255, 255),
        new Color(255, 0, 255, 255),
        new Color(255, 0, 0, 255),
        new Color(0, 255, 0, 255),
        new Color(0, 0, 255, 255),
        new Color(255, 255, 0, 255),
        new Color(0, 255, 255, 255),
        new Color(255, 0, 255, 255),
    };

}
