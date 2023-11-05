using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Character
{


    [CreateAssetMenu(fileName = "CharactersData", menuName = "Characters Data")]
    public class CharactersData : ScriptableObject
    {
        public string _id;
        public string _name;
        public string _description;
        public string _type;
        public BehaviorType _behavior;
        public float _detectionDistance;
        public float _walkSpeed;
        public float _chaseSpeed;
        public float _attackDistance;
        public float _damage;
        public float _maxHealth;
        public Transform[] _patrolPoints;
        public AnimatorOverrideController _animatorOverrideController;
        public GameObject _prefab;
    }

    public enum BehaviorType
    {
        Dialogue, 
        Trader,
        MeleeAttack,
        RangedAttack,
    }

}

