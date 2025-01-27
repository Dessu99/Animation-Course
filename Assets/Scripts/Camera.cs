using UnityEngine;

public class Camera : MonoBehaviour
{

    public Transform target;
    void Start()
    {
        
    }

    void Update()
    {
        target.position = transform.position;
    }
}
