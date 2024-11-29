using UnityEngine;

namespace Manager.Map
{
    public class NavMeshHandler : MonoBehaviour
    {
        private NavMeshBaker[] _navMeshBaker;
        
        public void SetNavMeshParents(GameObject[] parents)
        {
            _navMeshBaker = new NavMeshBaker[parents.Length];
            for (var i = 0; i < parents.Length; i++)
            {
                var target = parents[i];
                _navMeshBaker[i] = target.GetComponent<NavMeshBaker>();
                _navMeshBaker[i].BakeNavMesh();
            }
        }

        public void BakeTargetNavMesh(int roomNum)
        {
            if (roomNum < 0 || roomNum >= _navMeshBaker.Length) return;
            _navMeshBaker[roomNum].BakeNavMesh();
        }
    }
}