using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        if (_camera)
            transform.LookAt(transform.position + _camera.transform.forward);
    }
}