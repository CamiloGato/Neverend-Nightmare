using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI_Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject optionScreen, pauseScreen;

        public string mainMenuScene;
        private bool isPaused;

        public GameObject loadingScreen, loadingIcon;
        public Text loadingText;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)){
                PauseUnpause();
            }
        }

        public void PauseUnpause(){
            if (!isPaused){
                pauseScreen.SetActive(true);
                isPaused = true;
                Time.timeScale = 0f;
            } else{
                pauseScreen.SetActive(false);
                isPaused = false;
                Time.timeScale = 1f;
            }
        }

        public void OpenOptions(){
            optionScreen.SetActive(true);
        }

        public void CloseOptions(){
            optionScreen.SetActive(false);
        }

        public void QuitToMain(){
            SceneManager.LoadScene(mainMenuScene);

            Time.timeScale = 1.0f;

            //StartCoroutine(LoadMain());
        }

        /*public IEnumerator LoadMain(){
        loadingScreen.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuScene);
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone){
            if (asyncLoad.progress >= .9f){
                loadingText.text = "Press any key to continue";
                loadingIcon.SetActive(false);

                if(Input.anyKeyDown){
                    asyncLoad.allowSceneActivation = true;
                    Time.timeScale = 1;
                }
            }
            yield return null;
        }
    }*/
    }
}
