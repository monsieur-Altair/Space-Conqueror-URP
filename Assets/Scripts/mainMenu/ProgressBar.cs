//using Microsoft.Unity.VisualStudio.Editor;//it throws errors when i try to build project

using System;
using System.Collections.Generic;
using Managers;
using Planets;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Scientific = Resources.Scientific;

public class ProgressBar : MonoBehaviour
{
    private float currentScience;
    public static int maximumScience = 100;
    public Image progressBerScience;
    
    public Image progressBerPlanetsBlue;
    public Image progressBerPlanetsRed;
    private int blue;
    private int red;
    private int maximum;

    //public Image mask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFillPlanets();
        GetCurrentFillScience();
    }

    void GetCurrentFillPlanets()
    {
        List<int> planets = Managers.Main._objectsCount;
        blue = planets[0];
        red = planets[1];
        maximum = planets[0] + planets[1] + planets[2];
        progressBerPlanetsBlue.fillAmount = (float) blue / (float) maximum;
        progressBerPlanetsRed.fillAmount = (float) red / (float) maximum;
    }

    void GetCurrentFillScience()
    {
        currentScience = Planets.Scientific.ScientificCount;
        float fill = (float) currentScience / (float) maximumScience;
        progressBerScience.fillAmount = fill;
    }
}
