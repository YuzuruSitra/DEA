using UnityEngine;

namespace Test
{
	public class DebugDrawCd
	{
		public void DebugDrawSphere(Vector3 center, float radius, Color color)
		{
			const int resolution = 30;
			const float angleStep = 360f / resolution;

			// Draw circle in X-Y plane
			for (var i = 0; i < resolution; i++)
			{
				var angleA = Mathf.Deg2Rad * angleStep * i;
				var angleB = Mathf.Deg2Rad * angleStep * (i + 1);

				var pointA = center + new Vector3(Mathf.Cos(angleA) * radius, Mathf.Sin(angleA) * radius, 0);
				var pointB = center + new Vector3(Mathf.Cos(angleB) * radius, Mathf.Sin(angleB) * radius, 0);

				Debug.DrawLine(pointA, pointB, color);
			}

			// Draw circle in X-Z plane
			for (var i = 0; i < resolution; i++)
			{
				var angleA = Mathf.Deg2Rad * angleStep * i;
				var angleB = Mathf.Deg2Rad * angleStep * (i + 1);

				var pointA = center + new Vector3(Mathf.Cos(angleA) * radius, 0, Mathf.Sin(angleA) * radius);
				var pointB = center + new Vector3(Mathf.Cos(angleB) * radius, 0, Mathf.Sin(angleB) * radius);

				Debug.DrawLine(pointA, pointB, color);
			}

			// Draw circle in Y-Z plane
			for (var i = 0; i < resolution; i++)
			{
				var angleA = Mathf.Deg2Rad * angleStep * i;
				var angleB = Mathf.Deg2Rad * angleStep * (i + 1);

				var pointA = center + new Vector3(0, Mathf.Cos(angleA) * radius, Mathf.Sin(angleA) * radius);
				var pointB = center + new Vector3(0, Mathf.Cos(angleB) * radius, Mathf.Sin(angleB) * radius);

				Debug.DrawLine(pointA, pointB, color);
			}
		}
	}
}