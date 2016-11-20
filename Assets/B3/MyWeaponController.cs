using UnityEngine;
using System.Collections;

public class MyWeaponController : MonoBehaviour {

	private Transform agentTransform;
	private Animator agentAnimator;
	private bool associated;

	// Use this for initialization
	void Start () {
		associated = false;
	}

	public void linkToAent (Transform agentInput, Animator animatorInput) {
		agentTransform = agentInput;
		agentAnimator = animatorInput;
		associated = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (associated) {
			Vector3 targetPosition = agentAnimator.GetIKPosition(AvatarIKGoal.RightHand);
			targetPosition.y = targetPosition.y - 0.05f;
			transform.Translate (targetPosition - transform.position, Space.World);
		}
	}
}
