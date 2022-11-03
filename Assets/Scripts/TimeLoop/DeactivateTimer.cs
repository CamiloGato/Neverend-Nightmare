using UnityEngine;

namespace TimeLoop
{
    public class DeactivateTimer : MonoBehaviour
    {
        [SerializeField] GameObject player;
   
        public Behaviour timeLoopScript;
        public static bool isGameEnd = false;
        private void OnTriggerEnter(Collider player) {
        
            Debug.Log("collision detected");
            timeLoopScript.enabled = false;
            isGameEnd = true;
        }

   
    }
}
