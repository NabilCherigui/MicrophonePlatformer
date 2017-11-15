using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicInput : MonoBehaviour
{
	//The camara. Used to get the max of platforms.
	[SerializeField] private GameObject _camera;
	[SerializeField] private GameObject _player;
	[SerializeField] private Vector3 _offset;
	//The platform used to instantiate.
	[SerializeField] private GameObject _platform;
	//X and Y positions of where the platforms need to spawn.
	[SerializeField] private List<int> _platformsX;
	[SerializeField] private List<float> _platformsY;
	[SerializeField] private List<GameObject> _platformsHolder;
	
	private int _x;
	
	private AudioSource _audio;
	
	private void Start()
	{
		_audio = gameObject.GetComponent<AudioSource>();
		_audio.clip = Microphone.Start(null,true, 1, 44100);
		_x = Mathf.RoundToInt(_camera.transform.position.x);
		_offset = _camera.transform.position - _player.transform.position;
		
		for (var i = 0; i < 128; i++)
		{
			_platformsHolder.Add(Instantiate(_platform, new Vector3(0, 0, 0), Quaternion.identity));
		}
	}

	void Update()
	{
		if (_audio.isPlaying) { } else if (_audio.clip.isReadyToPlay) { _audio.Play(); } else { _audio.clip = Microphone.Start(null, true, 1, 44100); }
		
		float[] spectrum = new float[128];

		_audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

		_camera.transform.position = _player.transform.position + _offset;
	
		if (1 < Time.time)
		{
			if (_platformsX.Count != 128 && _platformsY.Count != 128)
			{
				for (var i = 0; i < spectrum.Length; i++)
				{
					_platformsY.Add(Mathf.Abs(Mathf.Log(spectrum[i]) / 2));
					_platformsX.Add(_x);
					_x += 1;

					for (var j = 0; j < _platformsX.Count; j++)
					{
						_platformsHolder[j].transform.position = new Vector3(_platformsX[j], _platformsY[j], 0);
						_platformsHolder[j].transform.localScale = new Vector3(_platformsHolder[j].transform.localScale.x,
							Mathf.Abs(Mathf.Log(spectrum[i])), _platformsHolder[j].transform.localScale.z);
					}
				}
				/*_platformsX.Clear();
				_platformsY.Clear();
				_x = Mathf.RoundToInt(_camera.transform.position.x);*/
			}
		}
	}

	void PlaceBlocksOnPitch(float[] spectrum)
	{
		if (_platformsX.Count != 128 && _platformsY.Count != 128)
		{
			for (var i = 0; i < spectrum.Length; i++)
			{
				_platformsY.Add(Mathf.Abs(Mathf.Log(spectrum[i]) / 2));
				_platformsX.Add(_x);
				_x += 1;

				for (var j = 0; j < _platformsX.Count; j++)
				{
					_platformsHolder[j].transform.position = new Vector3(_platformsX[j], _platformsY[j], 0);
					_platformsHolder[j].transform.localScale = new Vector3(_platformsHolder[j].transform.localScale.x,
					Mathf.Abs(Mathf.Log(spectrum[i])), _platformsHolder[j].transform.localScale.z);
				}
			}
		}
		else if (_platformsX.Count != 128 && _platformsY.Count != 128)
		{
			print("BOIIIIIIIIIIIIIIIIIIII");
			_platformsX.Clear();
			_platformsY.Clear();	
		}
	}
}
