using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;


public class ChaseBehaviorTree : MonoBehaviour {

	public GameObject heroAgent;
	public GameObject policeman;
	public Transform destination;

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
		
	protected Node ST_Chase(GameObject participant1, GameObject participant2, Transform target){
		Vector3 runnerPosition = new Vector3 ();

		return new Sequence (new SequenceParallel (participant1.GetComponent<BehaviorMecanim> ().Node_GoTo (target.position, 20f),
			new DecoratorLoop (
				new Sequence (
					participant2.GetComponent<BehaviorMecanim> ().Node_GoTo (target.position, 15f)))
		));
	}

	protected Node BuildTreeRoot()
	{
		return ST_Chase (heroAgent, policeman, destination);

	}

}
