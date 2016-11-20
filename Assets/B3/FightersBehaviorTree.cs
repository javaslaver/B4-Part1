using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class FightersBehaviorTree : MonoBehaviour
{
	public GameObject fighter1;
	public GameObject fighter2;
	public Transform escapeDestination;
	public GameObject policeman;
	public GameObject heroAgent;

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

	protected Node ST_Dying(GameObject participant)
	{
		Val<string> dying = Val.V (() => "Dying");
		Val<bool> start = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(dying, start),
			new LeafWait(3000));

	}

	protected Node ST_Dead(GameObject participant)
	{
		Val<string> dead = Val.V (() => "Dead");
		Val<bool> start = Val.V (() => false);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(dead, start),
			new LeafWait(3000));
	}

	bool checkSee(GameObject participant1, GameObject participant2) {
		float dis = (participant1.transform.position - participant2.transform.position).magnitude;
		float dot_checked = Vector3.Dot (participant1.transform.forward, participant2.transform.forward);

		if (dis > 3f) {
			return false;
		}
		if (dot_checked < 0) {
			return true;
		}

		return false;
	}

	protected Node ST_Idle(GameObject participant) {
		Val<string> gestureName = Val.V (() => "IDLE");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(100));
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

	protected Node ST_Fight(GameObject participant1, GameObject particiapnt2){
		return new Sequence (ST_Fight (participant1), 
			ST_Fight (particiapnt2),
			ST_RPunch (particiapnt2),
			ST_FaceHitted (participant1));
	}

	protected Node ST_Chase(GameObject participant1, GameObject participant2, Transform target){
		Func<bool> SeeTheFighter = () => (checkSee (fighter1, policeman));

		return new SequenceParallel (new Sequence(participant1.GetComponent<BehaviorMecanim> ().Node_GoTo (target.position, 18f)),
			new Sequence ( ST_Fight(participant2), new DecoratorInvert(new DecoratorLoop(new Sequence(new LeafAssert(SeeTheFighter), ST_RPunch (participant2)))), ST_Idle(participant2),
					participant2.GetComponent<BehaviorMecanim> ().Node_GoTo (target.position, 10f),
				ST_Fight(participant2), ST_RPunch (participant2), ST_Dying (participant1), new DecoratorLoop(new Sequence(ST_Dead(participant1)))
			))
		;
	}


	protected Node BuildTreeRoot()
	{
		Func<bool> PolicemanNotSeeTheFighter = () => (!checkSee (fighter1, policeman));

		Func<bool> SeeTheFighter = () => (checkSee (fighter1, policeman) && checkSee(fighter1, heroAgent));

		Func<bool> HeroAgentNotSeeTheFighter = () => (!checkSee (fighter1, heroAgent));

		Node doChase =  new Sequence (new DecoratorInvert(new DecoratorLoop (new LeafAssert (PolicemanNotSeeTheFighter))), new LeafWait(1000), ST_Chase(fighter1, policeman, escapeDestination)
		);

		Node fight = new Sequence (ST_Fight (fighter1), ST_Fight (fighter2),
			             ST_RPunch (fighter2), ST_FaceHitted (fighter1), ST_PickupRightWeapon (fighter1));

		Node fight_with_weapon = new DecoratorInvert(
			new DecoratorLoop(
				new Sequence(
					new DecoratorInvert(new LeafAssert(SeeTheFighter)), ST_UseWeapon(fighter1), new DecoratorInvert(new LeafAssert(SeeTheFighter)), ST_HeavyHitted(fighter2)
		)));

		Node doFight = new Sequence (fight, fight_with_weapon);

		Node doEscape = new Sequence (new DecoratorInvert(new DecoratorLoop (new LeafAssert (HeroAgentNotSeeTheFighter))),
			fighter1.GetComponent<BehaviorMecanim> ().Node_GoTo (escapeDestination.position, 18f));

		Node root = new SequenceParallel (doFight, doChase, doEscape);

		return root;
	
	}
}