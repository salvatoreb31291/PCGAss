using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class END : MonoBehaviour
{
    private int meshSize = 6;


    private Vector3 cubeSize = new Vector3((float)0.5, (float)0.5, (float)0.5);

    // Start is called before the first frame update
    void Start()
    {
        RenderCube();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision other)
    {
        Application.Quit();
    }

    private void RenderCube()
    {
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();

        meshFilter.mesh = CreateCube();
        meshRenderer.materials = MaterialsList().ToArray();
    }

    private Mesh CreateCube()
    {

        MeshGenerator meshGenerator = new MeshGenerator(meshSize);

        //top points
        Vector3 topPoint1 = new Vector3(cubeSize.x, cubeSize.y, -cubeSize.z);
        Vector3 topPoint2 = new Vector3(-cubeSize.x, cubeSize.y, -cubeSize.z);
        Vector3 topPoint3 = new Vector3(-cubeSize.x, cubeSize.y, cubeSize.z);
        Vector3 topPoint4 = new Vector3(cubeSize.x, cubeSize.y, cubeSize.z);

        //bottom points
        Vector3 bottomPoint1 = new Vector3(cubeSize.x, -cubeSize.y, -cubeSize.z);
        Vector3 bottomPoint2 = new Vector3(-cubeSize.x, -cubeSize.y, -cubeSize.z);
        Vector3 bottomPoint3 = new Vector3(-cubeSize.x, -cubeSize.y, cubeSize.z);
        Vector3 bottomPoint4 = new Vector3(cubeSize.x, -cubeSize.y, cubeSize.z);

        //top square
        meshGenerator.CreateTriangle(topPoint1, topPoint2, topPoint3, 0);
        meshGenerator.CreateTriangle(topPoint1, topPoint3, topPoint4, 0);

        //bottom square
        meshGenerator.CreateTriangle(bottomPoint3, bottomPoint2, bottomPoint1, 1);
        meshGenerator.CreateTriangle(bottomPoint4, bottomPoint3, bottomPoint1, 1);

        //front square
        meshGenerator.CreateTriangle(bottomPoint1, bottomPoint2, topPoint2, 2);
        meshGenerator.CreateTriangle(bottomPoint1, topPoint2, topPoint1, 2);

        //back square
        meshGenerator.CreateTriangle(bottomPoint3, topPoint4, topPoint3, 3);
        meshGenerator.CreateTriangle(bottomPoint3, bottomPoint4, topPoint4, 3);

        //left-side square
        meshGenerator.CreateTriangle(bottomPoint2, topPoint3, topPoint2, 4);
        meshGenerator.CreateTriangle(bottomPoint2, bottomPoint3, topPoint3, 4);

        //right-side square
        meshGenerator.CreateTriangle(bottomPoint4, topPoint1, topPoint4, 5);
        meshGenerator.CreateTriangle(bottomPoint4, bottomPoint1, topPoint1, 5);


        return meshGenerator.CreateMesh();
    }

    private List<Material> MaterialsList()
    {
        List<Material> materialsList = new List<Material>();
        for (int i = 0; i <= meshSize; i++)
        {

            Material redMaterial = new Material(Shader.Find("Specular"));
            redMaterial.color = Color.red;

            materialsList.Add(redMaterial);

        }
        return materialsList;
    }
}
