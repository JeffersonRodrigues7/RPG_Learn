using RPG.Health;
using RPG.Character.Attack;
using RPG.Character.Detection;
using RPG.Character.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Character.Controll
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private CharactersData characterData;

        private CharacterMovement characterMovement;
        private CharacterDetection characterDetection;
        private CharacterAttack characterAttack;
        private HealthController healthController;

        private Animator animator;

        private void Awake()
        {
            characterDetection = GetComponentInChildren<CharacterDetection>();
            characterMovement = GetComponent<CharacterMovement>();
            characterAttack = GetComponent<CharacterAttack>();
            healthController = GetComponent<HealthController>();
            animator = GetComponent<Animator>();

            characterDetection.DetectionRadius = characterData._detectionDistance;
            characterDetection.AttackDistance = characterData._attackDistance;

            characterMovement.WalkSpeed = characterData._walkSpeed;
            characterMovement.ChaseSpeed = characterData._chaseSpeed;
            characterMovement.CooldownTimeAfterChase = characterData._cooldownTimeAfterChase;
            characterMovement.ArrivalDistance = characterData._arrivalDistance;
            characterMovement.PatrolPoints = characterData._patrolPoints;

            characterAttack.Damage = characterData._damage;

            healthController.MaxHealth = characterData._maxHealth;

            animator.runtimeAnimatorController = characterData._animatorOverrideController;
        }
    }

}

