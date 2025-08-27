using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    [RequireComponent(typeof(Collider))]
    public class Button : MonoBehaviour
    {
		[SerializeField] private UnityEvent OnButtonPressed;
		[SerializeField] private bool allowMultipleUses;

		private bool buttonPressed = false;

		public void Press()
		{
			Debug.Log("HIT");
			if (!buttonPressed) 
			{
				OnButtonPressed?.Invoke();
				if(!allowMultipleUses) buttonPressed = true;
			}
		}
	}
}