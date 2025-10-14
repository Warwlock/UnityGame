using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material worldMat;
    private Container container;

    void Start()
    {
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        container.Initialize(worldMat, Vector3.zero);

        container.GenerateMesh();
        container.UploadMesh();
    }

    void Update()
    {

    }
}
