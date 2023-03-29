using System;
using System.Collections.Generic;
using System.Reflection;
using Models.DefaultSensor.ToGoalCollisionAvoidanceSimple;
using UnityEngine;

namespace Extensions.Unity
{
    public class MonoPrefabContainer : SingletonMono<MonoPrefabContainer>
    {
        [SerializeField]
        private ToGoalCollisionAvoidanceSimpleHelper _toGoalCollisionAvoidanceSimpleHelper;
        
        private readonly Dictionary<Type, object> _members = new();
        
        private void Awake()
        {
            AddMembers();
        }

        public T GetPrefab<T>()
        {
            if (_members.TryGetValue(typeof(T), out var member))
                return (T)member;

            throw new ArgumentException($"Invalid member type: {typeof(T)}.");
        }

        private void AddMembers()
        {
            var type = GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields) _members.Add(field.FieldType, field.GetValue(this));
        }
    }
}