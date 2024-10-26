using System.Collections;
using UnityEngine;

namespace Manager.Audio
{
	public class SoundHandler : MonoBehaviour
	{
		[SerializeField] private AudioSource _bgmAudioSource;
		[SerializeField] private AudioSource _seAudioSource;
		[SerializeField] private float _fadeDuration;
		private float _initialBgmVolume;
		private Coroutine _fadeCoroutine;
		
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
			_initialBgmVolume = _bgmAudioSource.volume;
		}


		public void PlayBGM(AudioClip clip)
		{
			if (clip == null) return;

			// フェードインアウト処理のコルーチンを開始または再開
			if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
			_fadeCoroutine = StartCoroutine(FadeOutInBGM(clip, _fadeDuration));
		}
		
		private IEnumerator FadeOutInBGM(AudioClip newClip, float fadeDuration)
		{
			// フェードアウト処理
			var startVolume = _bgmAudioSource.volume;
			for (var t = 0.0f; t < fadeDuration; t += Time.deltaTime)
			{
				_bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
				yield return null;
			}
			_bgmAudioSource.volume = 0;
			_bgmAudioSource.Stop();
			
			_bgmAudioSource.clip = newClip;
			_bgmAudioSource.Play();

			for (float t = 0; t < fadeDuration; t += Time.deltaTime)
			{
				_bgmAudioSource.volume = Mathf.Lerp(0, _initialBgmVolume, t / fadeDuration);
				yield return null;
			}
			_bgmAudioSource.volume = _initialBgmVolume;
		}

		public void PlaySe(AudioClip clip)
		{
			if(clip == null) return;
			_seAudioSource.PlayOneShot(clip);
		}
		
		// // BGM音量変更
		// public void SetNewValueBGM(float newValueBGM)
		// {
		// 	_bgmAudioSource.volume = Mathf.Clamp01(newValueBGM);
		// }
		//
		// // BGM音量変更
		// public void SetNewValueSe(float newValueSe)
		// {
		// 	_seAudioSource.volume = Mathf.Clamp01(newValueSe);
		// }
	}
}