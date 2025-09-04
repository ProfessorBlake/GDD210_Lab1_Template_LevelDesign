using System;
using TMPro;
using UnityEngine;

namespace Game 
{
	[RequireComponent (typeof (TMP_Text))]
	public class Timer : MonoBehaviour
	{
		private TMP_Text tmpText;
		private bool gameOver;

		private void Awake()
		{
			tmpText = GetComponent<TMP_Text>();
		}

		private void Update()
		{
			if (!gameOver)
			{
				tmpText.text = $"TIME: {TimeSpan.FromSeconds(Time.timeSinceLevelLoad).ToString(@"mm\:ss\:ff")}";
			}
		}

		public void Stop()
		{
			gameOver = true;
		}
	}
}