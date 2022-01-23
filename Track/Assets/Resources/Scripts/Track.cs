using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Track : MonoBehaviour
{
   
    private int subMeshCount = 3;

    private float radius = 25f;

    private int quadCount = 300;

    private float roadMarkerWidth = 0.1f;

    private float roadWidth = 3.0f;

    private float barrierWidth = 0.4f;

    private float variance = 25f;

    private float varianceScale = 2f;

    private Vector2 varianceOffset = new Vector2(5.6f, 4.5f);

    private Vector2 varianceStep = new Vector2(0.01f, 0.01f);

    private MeshGenerator meshGenerator;

    private const int whiteMaterialIndex = 0;

    private const int blackMaterialIndex = 1;

    private const int redMaterialIndex = 2;

    private bool isWhiteBarrier = false;

    private static GameObject car;

    private int carSpawn;

    private static Vector3 CarSpawnPoint;

    private static Quaternion CarSpawnRotation;

    private GameObject arrive;

    private static GameObject carGameObject;

    private GameObject plane;

    // Start is called before the first frame update
    void Start()
    {
        varianceScale = Random.Range(0f, 3f);
        carSpawn = Random.Range(1,299);
        RenderTrack();
    }


    public static void RelocateCar()
    {
        Destroy(carGameObject);
        carGameObject = Instantiate(car);
        carGameObject.name = "Car";
        carGameObject.transform.position = CarSpawnPoint;
        carGameObject.transform.rotation = CarSpawnRotation;
    }

    private void RenderTrack()
    {
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = this.GetComponent<MeshCollider>();

        meshFilter.mesh = CreateTrack();
        meshCollider.sharedMesh = meshFilter.mesh;
        meshRenderer.materials = MaterialList().ToArray();
    }

    private Mesh CreateTrack()
    {
        meshGenerator = new MeshGenerator(subMeshCount);

        //code to generate the track
        float quadDistance = 360f / quadCount;

        List<Vector3> pointRefList = new List<Vector3>();

        for (float degrees = 0; degrees < 360f; degrees += quadDistance)
        {

            Vector3 point = Quaternion.AngleAxis(degrees, Vector3.up) * Vector3.forward * radius;
            pointRefList.Add(point);
        }


        Vector2 wave = varianceOffset;
        for(int i = 0; i < pointRefList.Count; i++){
            wave += varianceStep;

            Vector3 pointRef = pointRefList[i].normalized;

            float noise = Mathf.PerlinNoise(wave.x * varianceScale, wave.y * varianceScale);
            noise *= variance;

            float fixConnectionPoint = Mathf.PingPong(i, pointRefList.Count/2f) / (pointRefList.Count / 2f);

            pointRefList[i] += pointRef * noise * fixConnectionPoint;

        }

        for(int i = 1; i <= pointRefList.Count; i++){
            Vector3 prevQuadRef = pointRefList[i - 1];
            Vector3 curQuadRef = pointRefList[i % pointRefList.Count];
            Vector3 nextQuadRef = pointRefList[(i + 1) % pointRefList.Count];
            if (carSpawn == i)
            {
                car = Resources.Load("Prefabs/Car") as GameObject;
                carGameObject = Instantiate(car, this.transform.position, this.transform.rotation);
                CarSpawnPoint = curQuadRef;
                carGameObject.transform.position = curQuadRef;
                carGameObject.transform.LookAt(nextQuadRef);
                Quaternion rota = carGameObject.transform.rotation;
                rota.x = 0;
                rota.y = 0;
                carGameObject.transform.rotation = rota;
                CarSpawnRotation = rota;
                carGameObject.name = "Car";
                arrive = Resources.Load("Prefabs/Objective") as GameObject;
                GameObject arriveGameObject = Instantiate(arrive, this.transform.position, this.transform.rotation);
                arriveGameObject.transform.position = curQuadRef;
                plane = Resources.Load("Prefabs/Plane") as GameObject;
                GameObject planeGameObject = Instantiate(plane);
            }
            CreateTrackSegment(prevQuadRef, curQuadRef,nextQuadRef);
        }


        return meshGenerator.CreateMesh();
    }

    private void CreateTrackSegment(Vector3 prevQuadRef, Vector3 curQuadRef, Vector3 nextQuadRef){

        //track line marker
        Vector3 offset = Vector3.zero;
        Vector3 targetOffset = Vector3.forward * roadMarkerWidth;
        CreateRightQuad(prevQuadRef,curQuadRef,nextQuadRef, whiteMaterialIndex, offset, targetOffset);
        CreateLeftQuad(prevQuadRef,curQuadRef,nextQuadRef, whiteMaterialIndex, offset, targetOffset);

        //road
        offset += targetOffset;
        targetOffset = Vector3.forward * roadWidth;
        CreateRightQuad(prevQuadRef,curQuadRef,nextQuadRef, blackMaterialIndex, offset, targetOffset);
        CreateLeftQuad(prevQuadRef,curQuadRef,nextQuadRef, blackMaterialIndex, offset, targetOffset);

        //barrier
        offset += targetOffset;
        targetOffset = Vector3.forward * barrierWidth;

        isWhiteBarrier = !isWhiteBarrier;

        int barrierSubMeshIndex = redMaterialIndex;
        if(isWhiteBarrier){
            barrierSubMeshIndex = whiteMaterialIndex;
        }

        CreateRightQuad(prevQuadRef,curQuadRef,nextQuadRef, barrierSubMeshIndex, offset, targetOffset);
        CreateLeftQuad(prevQuadRef,curQuadRef,nextQuadRef, barrierSubMeshIndex, offset, targetOffset);

    }

    private void CreateRightQuad(Vector3 prevQuadRef, Vector3 curQuadRef, Vector3 nextQuadRef, int subMeshIndex,
                            Vector3 offset, Vector3 targetOffset)
    {

        Vector3 nextDistance = (nextQuadRef - curQuadRef).normalized;

        Vector3 prevDistance = (curQuadRef - prevQuadRef).normalized;

        Quaternion nextQuaternion = Quaternion.LookRotation(Vector3.Cross(nextDistance, Vector3.up));

        Quaternion prevQuaternion = Quaternion.LookRotation(Vector3.Cross(prevDistance,Vector3.up));

        Vector3 topLeft = curQuadRef + (prevQuaternion * offset);
        Vector3 topRight = curQuadRef + (prevQuaternion * (offset + targetOffset));

        Vector3 bottomLeft = nextQuadRef + (nextQuaternion * offset);
        Vector3 bottomRight = nextQuadRef + (nextQuaternion * (offset + targetOffset));

        meshGenerator.CreateTriangle(topLeft, topRight, bottomLeft, subMeshIndex);
        meshGenerator.CreateTriangle(topRight, bottomRight, bottomLeft, subMeshIndex);
    }

    
    private void CreateLeftQuad(Vector3 prevQuadRef, Vector3 curQuadRef, Vector3 nextQuadRef, int subMeshIndex,
                            Vector3 offset, Vector3 targetOffset)
    {

        Vector3 nextDistance = (nextQuadRef - curQuadRef).normalized;

        Vector3 prevDistance = (curQuadRef - prevQuadRef).normalized;

        Quaternion nextQuaternion = Quaternion.LookRotation(Vector3.Cross(-nextDistance, Vector3.up));

        Quaternion prevQuaternion = Quaternion.LookRotation(Vector3.Cross(-prevDistance,Vector3.up));

        Vector3 topLeft = curQuadRef + (prevQuaternion * offset);
        Vector3 topRight = curQuadRef + (prevQuaternion * (offset + targetOffset));

        Vector3 bottomLeft = nextQuadRef + (nextQuaternion * offset);
        Vector3 bottomRight = nextQuadRef + (nextQuaternion * (offset + targetOffset));

        meshGenerator.CreateTriangle(bottomLeft, bottomRight, topLeft, subMeshIndex);
        meshGenerator.CreateTriangle(bottomRight, topRight, topLeft, subMeshIndex);
    }

    private List<Material> MaterialList(){
        List<Material> materialList = new List<Material>();

        Material whiteMaterial = new Material(Resources.Load("Materials/WhiteLine") as Material);

        Material blackMaterial = new Material(Resources.Load("Materials/Asphalt") as Material);

        Material redMaterial = new Material(Shader.Find("Specular"));
        redMaterial.color = Color.red;

        //to add a texture from an external resource to a material
        // Material assetSoreMaterial = new Material(Shader.Find("Specular"));
        //NOTE: This assumes you have a folder called Textures in your Asset Folder
        // var texture = Resources.Load<Texture2D>("Textures/texture01");
        // assetSoreMaterial.SetTexture("_MainTex", texture);

        materialList.Add(whiteMaterial);
        materialList.Add(blackMaterial);
        materialList.Add(redMaterial);
        
        //materialList.Add(assetSoreMaterial);

        return materialList;

    }
}
