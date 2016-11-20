using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class HandShakeTest : MonoBehaviour
{
	public GameObject fighter1;
	public GameObject fighter2;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	protected Node ST_PickupRightWeapon(GameObject participant) {
		Val<string> gestureName = Val.V (() => "PICKUPRIGHT");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(1500));
	}

	protected Node ST_Fight(GameObject participant) {
		Val<string> gestureName = Val.V (() => "FIGHT");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(300));
	}

	protected Node ST_RPunch(GameObject participant) {
		Val<string> gestureName = Val.V (() => "RIGHT PUNCH");
		Val<bool> startBool = Val.V (() => false);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(650));
	}

	protected Node ST_FaceHitted(GameObject participant) {
		Val<string> gestureName = Val.V (() => "FIGHT HITTED");
		Val<bool> startBool = Val.V (() => false);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(1500));
	}

	protected Node ST_HeavyHitted(GameObject participant) {
		Val<string> gestureName = Val.V (() => "HEAVY HIT");
		Val<bool> startBool = Val.V (() => false);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(2000));
	}

	protected Node ST_UseWeapon(GameObject participant) {
		Val<string> gestureName = Val.V (() => "USE WEAPON");
		Val<bool> startBool = Val.V (() => false);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(3000));
	}

	protected Node ST_HandShake(GameObject p1, GameObject p2) {
		Val<string> gestureName = Val.V (() => "HAND SHAKE");
		Val<bool> startBool = Val.V (() => false);
		return new Sequence (p1.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), p2.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(3000));
	}

	protected Node BuildTreeRoot()
	{
		Node root = new DecoratorLoop(new Sequence(this.ST_HandShake(fighter1, fighter2)));
		return root;
	}
}
