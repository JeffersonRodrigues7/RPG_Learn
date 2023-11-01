using RPG.Player.Attack;
using RPG.Player.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RPG.Player.InputController
{
    public class PlayerInputController : MonoBehaviour
    {

        #region VARIABLES DECLARATION

        private PlayerInput playerInput; //Componente playerInput
        private PlayerMovement playerMovement;
        private PlayerAttack playerAttack;

        #endregion

        #region  BEGIN/END SCRIPT

        private void Awake()
        {
            // Inicializa os componentes e variáveis necessárias quando o objeto é criado
            playerInput = new PlayerInput();
            playerMovement = GetComponent<PlayerMovement>();
            playerAttack = GetComponent<PlayerAttack>();
        }

        private void OnEnable()
        {
            // Ativa o registro de inputs do jogador
            enablePlayerInputs();
        }

        private void OnDisable()
        {
            // Desativa o registro de inputs do jogador
            disablePlayerInputs();
        }

        #endregion

        #region  ENABLE/DISABLE INPUT
        // Registra callbacks para as entradas
        private void enablePlayerInputs()
        {
            playerInput.Enable();
            playerInput.CharacterControls.KeyboardWalk.performed += playerMovement.KeyboardMove; // Callback de entrada de movimento via teclado
            playerInput.CharacterControls.KeyboardWalk.canceled += playerMovement.StopKeyboardMove; // Callback para parar o movimento via teclado
            playerInput.CharacterControls.MouseWalk.performed += playerMovement.MouseMove; // Callback de entrada de movimento via Mouse
            playerInput.CharacterControls.MouseWalk.canceled += playerMovement.StopMouseMove; // Callback para parar o movimento via Mouse
            playerInput.CharacterControls.Run.started += playerMovement.Run; // Callback para iniciar a corrida
            playerInput.CharacterControls.Run.canceled += playerMovement.StopRun; // Callback para parar a corrida
            playerInput.CharacterControls.Jump.started += playerMovement.Jump; // Callback para iniciar a Pulo
            playerInput.CharacterControls.Jump.canceled += playerMovement.StopJump; // Callback para indicar que soltamos botão de pulo
            playerInput.CharacterControls.MeleeAttack.started += playerAttack.MeleeAttack; // Callback para iniciar o Melee Attack
            playerInput.CharacterControls.MeleeAttack.canceled += playerAttack.StopMeleeAttack; // Callback para indicar que soltamos botão de melee attack

        }

        // Cancela o registro de callbacks quando o script é desativado
        private void disablePlayerInputs()
        {
            playerInput.Disable();
            playerInput.CharacterControls.KeyboardWalk.performed -= playerMovement.KeyboardMove;
            playerInput.CharacterControls.KeyboardWalk.canceled -= playerMovement.StopKeyboardMove;
            playerInput.CharacterControls.MouseWalk.performed -= playerMovement.MouseMove;
            playerInput.CharacterControls.MouseWalk.canceled -= playerMovement.StopMouseMove;
            playerInput.CharacterControls.Run.started -= playerMovement.Run;
            playerInput.CharacterControls.Run.canceled -= playerMovement.StopRun;
            playerInput.CharacterControls.Jump.started -= playerMovement.Jump;
            playerInput.CharacterControls.Jump.canceled -= playerMovement.StopJump;
            playerInput.CharacterControls.MeleeAttack.started -= playerAttack.MeleeAttack;
            playerInput.CharacterControls.MeleeAttack.canceled -= playerAttack.StopMeleeAttack;
        }

        #endregion
    }

}
