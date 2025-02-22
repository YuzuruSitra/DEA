using Manager;
using UnityEngine;

namespace Test
{
	public class DebugSceneChange : MonoBehaviour
	{
		[SerializeField] private bool _isDebug;
		[SerializeField] private DungeonLayerHandler _dungeonLayerHandler;
		
		// Update is called once per frame
		private void Update()
		{
			if (!_isDebug) return;
			if (Input.GetKeyDown(KeyCode.G))
			{
				_dungeonLayerHandler.NextDungeonLayer();
			}
		}
	}
}
