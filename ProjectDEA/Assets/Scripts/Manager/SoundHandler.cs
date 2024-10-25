using UnityEngine;

namespace Manager
{
	public class SoundHandler : MonoBehaviour
	{
		[SerializeField] private AudioSource _bgmAudioSource;
		[SerializeField] private AudioSource _seAudioSource;

		private void Awake()
		{
			CheckSingleton();
		}

		private void CheckSingleton()
		{
			var target = GameObject.FindGameObjectWithTag(gameObject.tag);
			var checkResult = target != null && target != gameObject;

			if (checkResult)
			{
				Destroy(gameObject);
				return;
			}
			DontDestroyOnLoad(gameObject);
		}


		public void PlayBGM(AudioClip clip)
		{
			if(clip == null) return;
			_bgmAudioSource.clip = clip;
			_bgmAudioSource.Play();
		}

		public void PlaySe(AudioClip clip)
		{
			if(clip == null) return;
			_seAudioSource.PlayOneShot(clip);
		}
		
		// BGM音量変更
		public void SetNewValueBGM(float newValueBGM)
		{
			_bgmAudioSource.volume = Mathf.Clamp01(newValueBGM);
		}

		// BGM音量変更
		public void SetNewValueSe(float newValueSe)
		{
			_seAudioSource.volume = Mathf.Clamp01(newValueSe);
		}
	}
}