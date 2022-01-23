using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials
{
    public static List<Material> MaterialsList(){
        List<Material> materialsList = new List<Material>();

        Material redMaterial = new Material(Shader.Find("Specular"));
        redMaterial.color = Color.red;

        Material blueMaterial = new Material(Shader.Find("Specular"));
        blueMaterial.color = Color.blue;

        Material yellowMaterial = new Material(Shader.Find("Specular"));
        yellowMaterial.color = Color.yellow;

        Material greenMaterial = new Material(Shader.Find("Specular"));
        greenMaterial.color = Color.green;

        Material magentaMaterial = new Material(Shader.Find("Specular"));
        magentaMaterial.color = Color.magenta;

        Material cyanMaterial = new Material(Shader.Find("Specular"));
        cyanMaterial.color = Color.cyan;

        materialsList.Add(redMaterial);
        materialsList.Add(blueMaterial);
        materialsList.Add(yellowMaterial);
        materialsList.Add(greenMaterial);
        materialsList.Add(magentaMaterial);
        materialsList.Add(cyanMaterial);

        return materialsList;
    }
}
