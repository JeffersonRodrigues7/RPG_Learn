using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        private Transform player;
        private NavMeshAgent navMeshAgent;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform; //Capturando objeto
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (player != null)
            {
                navMeshAgent.SetDestination(player.position);
            }
        }
    }

}
