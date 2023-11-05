using RPG.Character.Attack;
using RPG.Character.Detection;
using RPG.Character.Health;
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
        private CharacterHealth characterHealth;

        private Animator animator;

        private void Awake()
        {
            characterMovement = GetComponent<CharacterMovement>();
            characterDetection = GetComponent<CharacterDetection>();
            characterAttack = GetComponent<CharacterAttack>();
            characterHealth = GetComponent<CharacterHealth>();
            animator = GetComponent<Animator>();

            characterDetection.DetectionRadius = characterData._detectionDistance;
            characterDetection.AttackDistance = characterData._attackDistance;

            characterMovement.WalkSpeed = characterData._walkSpeed;
            characterMovement.ChaseSpeed = characterData._chaseSpeed;
            characterMovement.CooldownTimeAfterChase = characterData._cooldownTimeAfterChase;
            characterMovement.ArrivalDistance = characterData._arrivalDistance;

            characterAttack.Damage = characterData._damage;

            characterHealth.MaxHealth = characterData._maxHealth;

            animator.runtimeAnimatorController = characterData._animatorOverrideController;
        }
    }

}

