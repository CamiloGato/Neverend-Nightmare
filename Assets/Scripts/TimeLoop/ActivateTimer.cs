using UnityEngine;

namespace TimeLoop
{
    public class ActivateTimer : MonoBehaviour
    {
        [SerializeField] GameObject player;
   
        public Behaviour timeLoopScript;

        void Start(){
            timeLoopScript.enabled = false;
        }

        private void OnTriggerExit(Collider player)
        {
            timeLoopScript.enabled = true;
        }

   
    }
}
