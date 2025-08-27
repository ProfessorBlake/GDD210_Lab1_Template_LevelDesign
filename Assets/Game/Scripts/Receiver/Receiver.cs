using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    /// <summary>
    /// Reacts to a laser from an Emitter.
    /// </summary>
    public class Receiver : MonoBehaviour
    {
        [Tooltip("Call event when signal received changes.")]
        [SerializeField] private UnityEvent<bool> OnSignalChange;
		[SerializeField] private MeshRenderer targetMeshRend;

		private bool recevingSignal;
        private int lastSignalTime;

		private void Start()
		{
			for (int i = 0; i < targetMeshRend.sharedMaterials.Length; i++)
			{
				targetMeshRend.sharedMaterials[i].SetColor("_EmissionColor", Color.white);
			}
		}

		private void LateUpdate()
		{
            // No more signal
			if(recevingSignal && lastSignalTime < Time.frameCount)
            {
				recevingSignal = false;
				lastSignalTime = Time.frameCount;
				OnSignalChange?.Invoke(recevingSignal);
				for (int i = 0; i < targetMeshRend.sharedMaterials.Length; i++)
				{
					targetMeshRend.sharedMaterials[i].SetColor("_EmissionColor", Color.white);
				}
			}
		}

		public void SignalReceived(Emitter emitter)
        {
            if (!recevingSignal)
            {
				recevingSignal = true;
				lastSignalTime = Time.frameCount;
				OnSignalChange?.Invoke(recevingSignal);
				for (int i = 0; i < targetMeshRend.sharedMaterials.Length; i++)
				{
					targetMeshRend.sharedMaterials[i].SetColor("_EmissionColor", emitter.Col * 5);
				}
			}
            
        }
    }
}