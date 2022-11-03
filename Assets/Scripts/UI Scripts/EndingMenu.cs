using Pickable;
using TimeLoop;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI_Scripts
{
    public class EndingMenu : MonoBehaviour
    {
        public GameObject endingScreen;

        public string mainMenuScene;

        public GameObject loadingScreen, loadingIcon;
        public Text loadingText;


        void Update()
        {
            if (PickableCanvas.Instance.gameEnded){
                PickableCanvas.Instance.gameEnded = false;
                PickableCanvas.Instance.gameObject.SetActive(false);
                EndingGame();
            }
        }

        public void EndingGame(){
            endingScreen.SetActive(true);
            Time.timeScale = 0f;
        }

        public void QuitToMain(){
            //endingScreen.SetActive(false);
            SceneManager.LoadScene(mainMenuScene);
            Time.timeScale = 1.0f;
            //StartCoroutine(LoadToMain());
        }

        /*public IEnumerator LoadToMain(){
        endingScreen.SetActive(true);
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
