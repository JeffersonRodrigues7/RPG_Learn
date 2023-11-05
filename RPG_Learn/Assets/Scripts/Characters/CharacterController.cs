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
        public CharactersData characterData;

        private CharacterMovement characterMovement;
        private CharacterDetection characterDetection;
        private CharacterAttack characterAttack;
        private CharacterHealth characterHealth;

        private void Awake()
        {
            characterMovement = GetComponent<CharacterMovement>();
            characterDetection = GetComponent<CharacterDetection>();
            characterAttack = GetComponent<CharacterAttack>();
            characterHealth = GetComponent<CharacterHealth>();

            characterMovement.WalkSpeed = characterData._walkSpeed;
            characterMovement.ChaseSpeed = characterData._chaseSpeed;
        }
    }

}

