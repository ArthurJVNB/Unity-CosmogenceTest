using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _SL
{
	public class Mover : MonoBehaviour
	{
		[SerializeField]
		private Transform endPoint;

        public Vector2 StartPoint => transform.position;
        public Vector2 EndPoint => endPoint.position;

        private void Awake()
        {
            if (endPoint == null)
                endPoint = transform.GetChild(0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, endPoint.position);
        }
    }
}
