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
  GameObject rocket;

  enum State { Alive, Dying, Transcending }
  State state = State.Alive;

  bool collisionsDisabled = false;
  bool thrustDisabled = false;

  // Start is called before the first frame update
  void Start() {
    rocket = GameObject.Find("Rocket Ship");
    rigidBody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update() {
    if (state == State.Alive) {
      RespondToThrustInput();
      RespondToRotateInput();
      RespondToRestartInput();
    }
    if (Debug.isDebugBuild) RespondToDebugKeys();
  }

  private void RespondToThrustInput() {
    if (Input.GetKey(KeyCode.Space) && !thrustDisabled) {
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

  private void RespondToRotateInput() {
    float rotationThisFrame = rcsThrust * Time.deltaTime;

    if (Input.GetKey(KeyCode.A)) {
      rigidBody.angularVelocity = Vector3.zero; // remove rotation due to physics
      transform.Rotate(Vector3.forward * rotationThisFrame);
    }
    else if (Input.GetKey(KeyCode.D)) {
      rigidBody.angularVelocity = Vector3.zero; // remove rotation due to physics
      transform.Rotate(-Vector3.forward * rotationThisFrame);
    }
  }

  private void RespondToRestartInput() {
    if (Input.GetKey(KeyCode.R)) LoadFirstLevel();
  }

  private void RespondToDebugKeys() {
    if (Input.GetKeyDown(KeyCode.L)) LoadNextLevel();
    if (Input.GetKeyDown(KeyCode.C)) collisionsDisabled = !collisionsDisabled;
  }

  void OnCollisionEnter(Collision collision) {
    if (state != State.Alive || collisionsDisabled) return;

    switch (collision.gameObject.tag) {
      case "Friendly":
        break;
      case "Finish":
        StartSuccessSequence();
        break;
      default:
        StartDeathSequence();
        break;
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (state != State.Alive || collisionsDisabled) return;

    switch (collider.gameObject.tag) {
      case "NoThrustZone":
        thrustDisabled = true;
        break;
      default:
        // print(collider.gameObject.tag);
        break;
    }
  }

  void OnTriggerStay(Collider collider) {
    if (state != State.Alive) return;

    switch (collider.gameObject.tag) {
      case "ScaleZone":
        IncreaseScale();
        break;
      default:
        break;
    }
  }

  private void IncreaseScale() {
    float increaseFactor = 0.1f * Time.deltaTime;
    Vector3 newVector = new Vector3(increaseFactor, increaseFactor, increaseFactor);
    rocket.transform.localScale += newVector;
  }

  void OnTriggerExit(Collider collider) {
    if (state != State.Alive || collisionsDisabled) return;

    switch (collider.gameObject.tag) {
      case "NoThrustZone":
        thrustDisabled = false;
        break;
      default:
        break;
    }
  }

  private void StartSuccessSequence() {
    state = State.Transcending;
    audioSource.Stop();
    mainEngineParticles.Stop();
    audioSource.PlayOneShot(success);
    successParticles.Play();
    Invoke("LoadNextLevel", levelLoadDelay);
  }

  private void StartDeathSequence() {
    state = State.Dying;
    audioSource.Stop();
    mainEngineParticles.Stop();
    audioSource.PlayOneShot(death);
    deathParticles.Play();
    Invoke("LoadFirstLevel", levelLoadDelay);
  }

  private void LoadNextLevel() {
    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    int nextSceneIndex = currentSceneIndex + 1;
    nextSceneIndex = nextSceneIndex % SceneManager.sceneCountInBuildSettings;
    if (nextSceneIndex == 0) LoadFirstLevel();
    else SceneManager.LoadScene(nextSceneIndex);
  }

  private void LoadFirstLevel() {
    SceneManager.LoadScene(0);
    Timer.ResetTime();
  }

}
