using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUI : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        // Faz com que a barra de vida (Canvas) esteja sempre olhando para a câmera.
        transform.LookAt(transform.position + camTransform.rotation * Vector3.forward, camTransform.rotation * Vector3.up);
    }
}
