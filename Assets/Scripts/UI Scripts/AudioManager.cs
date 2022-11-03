using UnityEngine;
using UnityEngine.Audio;

namespace UI_Scripts
{
    public class AudioManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public AudioMixer theMixer;
        void Start()
        {
        
            if (PlayerPrefs.HasKey("MasterVol")){
                theMixer.SetFloat("MasterVol", PlayerPrefs.GetFloat("MasterVol"));
            
            
            }

            if (PlayerPrefs.HasKey("MusicVol")){
                theMixer.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusicVol"));
            
            
            }

            if (PlayerPrefs.HasKey("SFXVol")){
                theMixer.SetFloat("SFXVol", PlayerPrefs.GetFloat("SFXVol"));
          
            
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
