using UnityEngine;
using UnityEngine.AI;

namespace Manager.Map
{
    public class NavMeshBaker : MonoBehaviour
    {
        private NavMeshSurface _navMeshSurface;

        private void Awake()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();
        }

        public void BakeNavMesh()
        {
            _navMeshSurface.BuildNavMesh();
        }
    }
}
