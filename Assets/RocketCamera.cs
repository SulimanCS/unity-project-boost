using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCamera : MonoBehaviour {

  GameObject rocket;
  float originalZ = 0;

  // Start is called before the first frame update
  void Start() {
    rocket = GameObject.Find("Rocket Ship");
    originalZ = transform.position.z;
  }

  // Update is called once per frame
  void Update() {
    float rocketPositionX = rocket.transform.position.x;
    float rocketPositionY = rocket.transform.position.y;
    transform.position = new Vector3(rocketPositionX, rocketPositionY, originalZ);
  }
}
