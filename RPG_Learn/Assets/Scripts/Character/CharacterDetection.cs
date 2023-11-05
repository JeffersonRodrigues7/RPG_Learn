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
        [Header("CharacterData")]
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float attackDistance = 1f;

        private SphereCollider detectionCollider;

        private CharacterMovement characterMovement;
        private CharacterAttack characterAttack;
        
        private Transform target;

        public float DetectionRadius { set { detectionRadius = value; } }
        public float AttackDistance { set { attackDistance = value; } }

        private void Start()
        {
            characterMovement = GetComponentInParent<CharacterMovement>();
            characterAttack = GetComponentInParent<CharacterAttack>();

            detectionCollider = GetComponent<SphereCollider>();

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
                    characterAttack.startAttackAnimation();
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


