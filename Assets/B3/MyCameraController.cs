using UnityEngine;
using System.Collections;

public class MyCameraController : MonoBehaviour {

	public GameObject heroAgent;
	public HeroBehaviorTree heroBehavior;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		updateCamera ();
	}

	void updateCamera() {
		Vector3 f = heroAgent.transform.forward;
		f.Normalize ();
		Vector3 location = heroAgent.transform.position - 1.3f * f;
		Vector3 lookLocation = heroAgent.transform.position + 1f * f;
		location.y = 2.5f;
		lookLocation.y = 1.5f;
		transform.position = location;
		transform.LookAt (lookLocation);
	}
}
