using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Extensions.Ml_Agents
{
    public interface IObservation<out O>
    where O : struct, IObservation<O>
    {
        public void AddObservationToVectorSensor(global::Unity.MLAgents.Sensors.VectorSensor sensor)
        {
            var value = GetValue();
            var childType = typeof(O);
            AddToObservationRecursive(childType, value, sensor);
        }
        
        public int GetObservationSize()
        {
            var value = GetValue();
            var childType = typeof(O);
            return GetObservationSizeRecursively(childType, value);
        }
        
        private O GetValue()
        {
            return (O)this;
        }
        
        private static void AddToObservationRecursive(Type type, object value, global::Unity.MLAgents.Sensors.VectorSensor sensor)
        {
            if (AddToObservationIfTypeMatch(type, value, sensor)) return;
            
            if(typeof(IEnumerable).IsAssignableFrom(type))
            {
                var collection = (IEnumerable)value;
                var elementType = type.GetElementType();
                foreach (var element in collection)
                {
                    AddToObservationRecursive(elementType, element, sensor);
                }
                return;
            }
            
            if (type.IsValueType)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    var fieldValue = field.GetValue(value);
                    AddToObservationRecursive(fieldType, fieldValue, sensor);
                }
                return;
            }
            Debug.LogWarning($"Type: {type} is not a valid type in an observation");
        }

        private static bool AddToObservationIfTypeMatch(Type type, object value, global::Unity.MLAgents.Sensors.VectorSensor sensor)
        {
            if (type == typeof(float))
            {
                sensor.AddObservation((float)value);
                return true;
            }
            if (type == typeof(int))
            {
                sensor.AddObservation((int)value);
                return true;
            }
            if (type == typeof(bool))
            {
                sensor.AddObservation((bool)value);
                return true;
            }
            if (type == typeof(Vector3))
            {
                sensor.AddObservation((Vector3)value);
                return true;
            }
            if (type == typeof(Vector2))
            {
                sensor.AddObservation((Vector2)value);
                return true;
            }
            if (type == typeof(Quaternion))
            {
                sensor.AddObservation((Quaternion)value);
                return true;
            }
            return false;
        }
        
        private static int GetObservationSizeRecursively(Type type, object value)
        {
            var count = 0;
            if(AddToCountIfTypeMatch(type, ref count)) return count;
            
            if(typeof(IEnumerable).IsAssignableFrom(type))
            {
                var collection = (IEnumerable)value;
                var elementType = type.GetElementType();
                foreach (var element in collection)
                {
                    count += GetObservationSizeRecursively(elementType, element);
                }
                return count;
            }
            
            if (type.IsValueType)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    var fieldValue = field.GetValue(value);
                    count += GetObservationSizeRecursively(fieldType, fieldValue);
                }
                return count;
            }
            
            Debug.LogWarning($"Type: {type} is not a valid type in an observation");
            return 0;
        }
        
        private static bool AddToCountIfTypeMatch(Type type, ref int count)
        {
            if (type == typeof(float))
            {
                count++;
                return true;
            }
            if (type == typeof(int))
            {
                count++;
                return true;
            }
            if (type == typeof(bool))
            {
                count++;
                return true;
            }
            if (type == typeof(Vector3))
            {
                count += 3;
                return true;
            }
            if (type == typeof(Vector2))
            {
                count += 2;
                return true;
            }
            if (type == typeof(Quaternion))
            {
                count += 4;
                return true;
            }
            return false;
        }
    }
}