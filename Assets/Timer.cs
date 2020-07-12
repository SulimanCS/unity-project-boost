using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {

  public Text timerText;
  static float startTime;
  static float savedTime;

  // Start is called before the first frame update
  void Start() {
  }

  public static void ResetTime() {
    startTime = Time.time;
    savedTime = 0;
  }

  // Update is called once per frame
  void Update() {
    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    float t = Time.time - startTime;

    // stop timer if player reaches the last level
    if (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1) {
      if (savedTime == 0) savedTime = t;

      // stop t from increasing, i.e: reset it to where the timer stopped
      // after we reached last level
      t = savedTime;
    }

    // timer (minutes/seconds) logic
    string minutes = ((int)t / 60).ToString();
    string seconds = (t % 60).ToString("f2");
    timerText.text = minutes + ":" + seconds;
  }
}
