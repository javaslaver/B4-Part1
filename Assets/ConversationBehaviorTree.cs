using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class ConversationBehaviorTree : MonoBehaviour {

	public GameObject speaker1;
	public GameObject speaker2;
	public GameObject speaker3;
	public GameObject heroAgent;

	private BehaviorAgent behaviorAgent;

	// Use this for initialization
	void Start () {
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update () {
	
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

	protected Node ST_Greeting(GameObject participant){
		Val<string> gestureName = Val.V (() => "WAVE");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(100));
	}

	protected Node ST_Roaring(GameObject participant){
		Val<string> gestureName = Val.V (() => "PICKUPRIGHT");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(100));
	}

	protected Node ST_Conversation(GameObject participant1, GameObject participant2, GameObject participant3){
		return new MyRandom (ST_FocusOn (participant1, participant2, participant3),
			ST_FocusOn (participant1, participant3, participant2),
			ST_FocusOn (participant2, participant3, participant1),
			ST_FocusOn (participant3, participant2, participant1),
			ST_FocusOn (participant2, participant1, participant3)
		);
	}

	Val<Vector3> FacePosition(GameObject participant){
		Vector3 facePosition = new Vector3(participant.transform.position.x, participant.transform.position.y+1.0f, participant.transform.position.z);
		return facePosition;
	}

	/*
	protected Node ST_FocusOn(GameObject participant1, GameObject participant2, GameObject participant3){
		return new Sequence (participant2.GetComponent<BehaviorMecanim> ().Node_HeadLook (FacePosition(participant1)),
			participant3.GetComponent<BehaviorMecanim>().Node_HeadLook(FacePosition(participant2)),
			participant1.GetComponent<BehaviorMecanim>().Node_HeadLook(FacePosition(participant3))
		);
	}
	*/

	protected Node ST_FocusOn(GameObject participant1, GameObject participant2, GameObject participant3){
		return new Sequence (participant2.GetComponent<BehaviorMecanim> ().ST_TurnToFace (participant1.transform.position),
			participant3.GetComponent<BehaviorMecanim>().ST_TurnToFace(participant1.transform.position),
			participant1.GetComponent<BehaviorMecanim>().ST_TurnToFace(participant2.transform.position),
			ST_Roaring(participant1),
			ST_Roaring(participant2)
		);
	}

	protected Node ST_SayHello(GameObject participant1, GameObject participant2, GameObject participant3, GameObject participant4){
		return new Sequence (participant1.GetComponent<BehaviorMecanim> ().ST_TurnToFace (participant4.transform.position),
			participant2.GetComponent<BehaviorMecanim> ().ST_TurnToFace (participant4.transform.position),
			participant3.GetComponent<BehaviorMecanim> ().ST_TurnToFace (participant4.transform.position),
			ST_Greeting(participant1));
	}

	protected Node BuildTreeRoot()
	{
		Func<bool> SeeAcquaintance = () => (!checkSee (speaker1, heroAgent));
		Func<bool> NotSeeAcquaintance = () => (checkSee (speaker1, heroAgent));
	
		Node doConversation =  new Sequence (new DecoratorInvert(
			new DecoratorLoop (new LeafAssert (NotSeeAcquaintance))), new LeafWait(1000), ST_Conversation (speaker1, speaker2, speaker3));

		Node doSayHello =  new Sequence (new DecoratorInvert(
			new DecoratorLoop (new LeafAssert (SeeAcquaintance))), new LeafWait(1000), ST_SayHello (speaker1, speaker2, speaker3, heroAgent));

		Node root = new DecoratorLoop(new SequenceParallel (doConversation, doSayHello));

		return root;

	}

}
