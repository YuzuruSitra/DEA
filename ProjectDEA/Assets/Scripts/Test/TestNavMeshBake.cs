using UnityEngine;
using UnityEngine.AI;

namespace Test
{
    public class TestNavMeshBake : MonoBehaviour
    {
        private NavMeshSurface _navMeshSurface;
    
        private void Awake()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();
            BakeNavMesh();
        }

        private void BakeNavMesh()
        {
            _navMeshSurface.BuildNavMesh();
        }
    }
}
