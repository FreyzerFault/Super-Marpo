using System;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class LoopMoveTween
    {
        [SerializeField] public Transform movingObj;
        [SerializeField] public Transform startPoint;
        [SerializeField] public Transform endPoint;
        
        [Header("Animation")]
        [Min(0f)] public float durantionSeconds = 2f;
        [Min(0f)] public float delaySeconds = 1f;
        [SerializeField] private bool startReversed;
        
        private Sequence _sequence;
        private TweenerCore<Vector3,Vector3,VectorOptions> _tween;
        
        public bool IsActive => _sequence.IsActive();
        public Vector3 StartPosition => startPoint.position;
        public Vector3 EndPosition => endPoint.position;
        public Vector3 MiddlePosition => StartPosition + (EndPosition - StartPosition) * 0.5f;
        
        public void Initialize()
        {
            _tween = movingObj.transform
                .DOMove(startReversed ? startPoint.position : endPoint.position, durantionSeconds)
                .ChangeStartValue(startPoint.position)
                .SetEase(Ease.InOutSine);
            
            // This sequence have a PingPong effect from Start to End
            _sequence = DOTween.Sequence();
            _sequence
                .AppendInterval(delaySeconds)
                .Append(_tween)
                .AppendInterval(delaySeconds)
                .SetLoops(-1, LoopType.Yoyo);

            if (!_sequence.IsActive()) 
                _sequence.Pause();
        }
        
        public void Play() => _sequence.Play();
        public void Pause() => _sequence.Pause();


        public void Destroy() => _sequence.Kill();
        
        
        #region DEBUG

        public void OnDrawGizmos()
        {
            Collider2D collider = movingObj.GetComponent<Collider2D>() ?? movingObj.GetComponentInChildren<Collider2D>();
            if (collider == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(StartPosition, MiddlePosition);
            DrawColliderGizmos(collider, StartPosition);
            Handles.Label(StartPosition + Vector3.down * 0.05f, "Start", new GUIStyle { normal = {textColor = Color.red} });
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(MiddlePosition, EndPosition);
            DrawColliderGizmos(collider,EndPosition);
            Handles.Label(EndPosition + Vector3.down * 0.05f, "End", new GUIStyle { normal = {textColor = Color.yellow} });
        }

        
        private void DrawColliderGizmos(Collider2D collider, Vector3 point)
        {
            switch (collider)
            {
                case BoxCollider2D boxCollider:
                    Gizmos.DrawWireCube(point, boxCollider.size);
                    break;
                case CircleCollider2D circleCollider:
                    Gizmos.DrawWireSphere(point, circleCollider.radius);
                    break;
                case EdgeCollider2D edgeCollider:
                    Gizmos.DrawLineStrip(edgeCollider.points.Select(p => (Vector3)p).ToArray(), edgeCollider);
                    break;
                case PolygonCollider2D polygonCollider:
                    Gizmos.DrawWireMesh(polygonCollider.CreateMesh(true, true), point, Quaternion.identity);
                    break;
            }
        }

        #endregion
    }
}
