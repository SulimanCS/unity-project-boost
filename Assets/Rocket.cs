using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

  [SerializeField] float rcsThrust = 100f;
  [SerializeField] float mainThrust = 100f;

  Rigidbody rigitBody;
  AudioSource audioSource;

  // Start is called before the first frame update
  void Start() {
    rigitBody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update() {
    Thrust();
    Rotate();
  }

  private void Rotate()
  {
    rigitBody.freezeRotation = true; // take manual control of rotation

    float rotationThisFrame = rcsThrust * Time.deltaTime;

    if (Input.GetKey(KeyCode.A))
    {
      transform.Rotate(Vector3.forward * rotationThisFrame);
    }
    else if (Input.GetKey(KeyCode.D))
    {
      transform.Rotate(-Vector3.forward * rotationThisFrame);
    }

    rigitBody.freezeRotation = false; // resume physics control of rotation
  }

  private void Thrust()
  {
    if (Input.GetKey(KeyCode.Space))
    {
      rigitBody.AddRelativeForce(Vector3.up * mainThrust);
      if (!audioSource.isPlaying) audioSource.Play();
    }
    else
    {
      audioSource.Stop();
    }
  }
}
