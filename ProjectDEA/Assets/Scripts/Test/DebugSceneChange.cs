using Manager;
using UnityEngine;

namespace Test
{
	public class DebugSceneChange : MonoBehaviour
	{
		[SerializeField] private bool _isDebug;
		private DungeonLayerHandler _dungeonLayerHandler;

		private void Start()
		{
			if (!_isDebug) return;
			_dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
		}
		
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
