using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

  [SerializeField] float rcsThrust = 100f;
  [SerializeField] float mainThrust = 100f;
  [SerializeField] float levelLoadDelay = 2f;

  [SerializeField] AudioClip mainEngine;
  [SerializeField] AudioClip success;
  [SerializeField] AudioClip death;

  [SerializeField] ParticleSystem mainEngineParticles;
  [SerializeField] ParticleSystem successParticles;
  [SerializeField] ParticleSystem deathParticles;

  Rigidbody rigidBody;
  AudioSource audioSource;

  enum State { Alive, Dying, Transcending }
  State state = State.Alive;

  bool collisionsDisabled = false;

  // Start is called before the first frame update
  void Start() {
    rigidBody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update() {
    if (state == State.Alive) {
      RespondToThrustInput();
      RespondToRotateInput();
    }
    if (Debug.isDebugBuild) RespondToDebugKeys();
  }

  private void RespondToDebugKeys() {
    if (Input.GetKeyDown(KeyCode.L)) LoadNextLevel();
    if (Input.GetKeyDown(KeyCode.C)) collisionsDisabled = !collisionsDisabled;
  }

  void OnCollisionEnter(Collision collision) {
    if (state != State.Alive || collisionsDisabled) return;

    switch (collision.gameObject.tag) {
      case "Friendly":
        print("WOAH FRIENDLY BOI");
        break;
      case "Finish":
        print("NEXT LEVEL");
        StartSuccessSequence();
        break;
      default:
        print("YOU MESSED UP");
        StartDeathSequence();
        // kill player
        break;
    }
  }

  private void StartSuccessSequence() {
    state = State.Transcending;
    audioSource.Stop();
    audioSource.PlayOneShot(success);
    successParticles.Play();
    Invoke("LoadNextLevel", levelLoadDelay);
  }

  private void StartDeathSequence() {
    state = State.Dying;
    audioSource.Stop();
    audioSource.PlayOneShot(death);
    deathParticles.Play();
    Invoke("LoadFirstLevel", levelLoadDelay);
  }

  private void LoadFirstLevel() {
    SceneManager.LoadScene(0);
  }

  private void LoadNextLevel() {
    SceneManager.LoadScene(1);
  }

  private void RespondToRotateInput() {
    rigidBody.freezeRotation = true; // take manual control of rotation

    float rotationThisFrame = rcsThrust * Time.deltaTime;

    if (Input.GetKey(KeyCode.A)) {
      transform.Rotate(Vector3.forward * rotationThisFrame);
    }
    else if (Input.GetKey(KeyCode.D)) {
      transform.Rotate(-Vector3.forward * rotationThisFrame);
    }

    rigidBody.freezeRotation = false; // resume physics control of rotation
  }

  private void RespondToThrustInput() {
    if (Input.GetKey(KeyCode.Space)) {
      ApplyThrust();
    }
    else {
      audioSource.Stop();
      mainEngineParticles.Stop();
    }
  }

  private void ApplyThrust() {
    rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
    if (!audioSource.isPlaying) audioSource.PlayOneShot(mainEngine);
    if (!mainEngineParticles.isPlaying) mainEngineParticles.Play();
  }
}
