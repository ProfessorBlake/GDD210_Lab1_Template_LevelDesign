using UnityEngine;

namespace Game
{
    public class Goal : MonoBehaviour
    {
        public void Trigger()
        {
            Debug.Log("GOAL:" + Time.time);
        }
    }
}