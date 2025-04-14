using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class OneWayPlatformCompositor : MonoBehaviour
    {
        private GameObject _player;
        private PolygonCollider2D _collider;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _collider = GetComponent<PolygonCollider2D>();
        }

        private void Update()
        {
            for (var i = 0; i < _collider.composite.pathCount; i++)
            {
                List<Vector2> points = new();
                _collider.composite.GetPath(i, points);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            for (var i = 0; i < _collider.composite.pathCount; i++)
            {
                List<Vector2> points = new();
                _collider.composite.GetPath(i, points);
                
                Debug.Log($"Path {i} has {points.Count} points");

                foreach (Vector2 point in points) 
                    Gizmos.DrawSphere(point, 0.01f);
            }
        }
    }
}
