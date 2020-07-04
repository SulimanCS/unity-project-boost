using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

  // TODO: remove from inspector later
  [SerializeField] Vector3 movementVector;

  [SerializeField] [Range(0, 1)] float movementFactor;

  Vector3 startingPos;
  // Start is called before the first frame update
  void Start() {
    startingPos = transform.position;
  }

  // Update is called once per frame
  void Update() {
    Vector3 offset = movementVector * movementFactor;
    transform.position = startingPos + offset;
  }
}
