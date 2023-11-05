using RPG.Character.Attack;
using RPG.Character.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Detection
{
    public class CharacterDetection : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float attackDistance = 1f;

        private CharacterMovement characterMovement;
        private CharacterAttack characterAttack;
        private SphereCollider detectionCollider;

        private Transform target;

        private void Start()
        {
            characterMovement = GetComponentInParent<CharacterMovement>();
            characterAttack = GetComponentInParent<CharacterAttack>();
            detectionCollider = GetComponent<SphereCollider>(); // Obtenha o Collider do campo de detecção.

            if (detectionCollider != null)
            {
                detectionCollider.radius = detectionRadius;
            }
        }

        private void Update()
        {
            if(target != null)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    Debug.Log("Chamando Ataqie");
                    characterAttack.startAttack();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Verifique se o objeto que entrou no campo de detecção é o jogador.
            if (other.CompareTag("Player"))
            {
                target = other.transform;
                characterMovement.startChase(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
            // Verifique se o objeto que saiu do campo de detecção é o jogador.
            if (other.CompareTag("Player"))
            {
                target = null;
                characterMovement.stopChase();
            }
        }

        // Função para desenhar o raio de detecção no Editor.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}


