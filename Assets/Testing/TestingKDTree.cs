using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Testing
{
    public class TestingKDTree : MonoBehaviour
    {
        [SerializeField]
        private Transform prefab;

        [SerializeField]
        private List<Transform> _objects;
        
        private int _count = 100;
        private Vector2 _minMax = new (-20, 20);
        
        private int[,] _map;
        
        public Transform crr;
        
        private Transform _oldCrr;
        private Transform[] _crrNeighbours;
        
        private Color _crrColor = Color.blue;
        private Color _crrNeighboursColor = Color.red;
        
        private int posCanSee = 5;

        private void Start()
        {
            for (var i = 0; i != _count; i++)
            {
                var pos = new Vector3(
                    Random.Range(_minMax.x, _minMax.y),
                    Random.Range(_minMax.x, _minMax.y),
                    0
                    );

                _objects.Add(Instantiate(prefab, pos, Quaternion.identity));
            }
            
            var points = new Vector2[_count];
            for (var i = 0; i != _count; i++)
            {
                points[i] = _objects[i].position;
            }
            
            _map = Utilities.MyGetClosePositionIndices(points, posCanSee);
            _crrNeighbours = new Transform[posCanSee];
        }

        private void Update()
        {
            if(crr == null)
                return;

            if (crr != _oldCrr)
            {
                if (_oldCrr != null)
                {
                    _oldCrr.GetComponent<Renderer>().material.color = Color.white;
                    foreach (var neighbour in _crrNeighbours)
                    {
                        neighbour.GetComponent<Renderer>().material.color = Color.white;
                    }
                }
                crr.GetComponent<Renderer>().material.color = _crrColor;
                for(var i = 0; i != posCanSee; i++)
                {
                    _crrNeighbours[i] = _objects[_map[Array.IndexOf(_objects.ToArray(), crr), i ]];
                }
                
                foreach (var neighbour in _crrNeighbours)
                {
                    neighbour.GetComponent<Renderer>().material.color = _crrNeighboursColor;
                }
            }
            _oldCrr = crr;
        }
    }
}
