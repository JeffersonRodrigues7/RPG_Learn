using RPG.Character.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterDetection : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 10f;

    private CharacterMovement characterMovement;
    private SphereCollider detectionCollider;

    Transform target;

    void Start()
    {
        characterMovement = GetComponentInParent<CharacterMovement>();
        detectionCollider = GetComponent<SphereCollider>(); // Obtenha o Collider do campo de detecção.

        if (detectionCollider != null)
        {
            detectionCollider.radius = detectionRadius;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifique se o objeto que entrou no campo de detecção é o jogador.
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            characterMovement.startMoving(target);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Verifique se o objeto que saiu do campo de detecção é o jogador.
        if (other.CompareTag("Player"))
        {
            characterMovement.stopMoving();
        }
    }

    // Função para desenhar o raio de detecção no Editor.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
