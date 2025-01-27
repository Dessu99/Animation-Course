using UnityEngine;

public class Camera : MonoBehaviour
{

    public Transform target;
    public Vector3 Offset;
    void Start()
    {
        
    }

    void Update()
    {
        target.position = transform.position + Offset;
    }
}
