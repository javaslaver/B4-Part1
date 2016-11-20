using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class HeroBehaviorTree : MonoBehaviour
{
	public GameObject heroAgent;
	public GameObject policeman;
	public GameObject speaker;
	public GameObject fighter;
	public GameObject fighter2;

	public Transform goaltransform;
	public Transform forntDoor;
	public Transform Destination;
	public Transform FirstStep;
	public Transform SecondStep;
	public Transform AnotherSecondStep;
	public Transform endStep;

	private BehaviorAgent behaviorAgent;

	enum CurrentArc {none, seeThePolice, noReportConfirmed, ReportConfirmed, greetAcquaintance, seeFighter, hasNoClues, findFighter, seekFighter};

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

	bool checkSee(GameObject participant1, GameObject participant2){
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

	protected Node ST_Greeting(GameObject participant){
		Val<string> gestureName = Val.V (() => "WAVE");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(100));
	}

	protected Node ST_ApproachAndWait(GameObject participant, Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
	}

	protected Node ST_Control()
	{
		Val<string> gestureName = Val.V (() => "USER INPUT");
		Val<bool> startBool = Val.V (() => false);
		return new Sequence (heroAgent.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(800));
	}

	protected Node ST_HandShake(GameObject p1, GameObject p2) {
		Val<string> gestureName = Val.V (() => "HAND SHAKE");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (p1.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), 
			p2.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), 
			new LeafWait(1500));
	}

	protected Node ST_Yawn(GameObject agent)
	{
		Val<string> gestureName = Val.V (() => "YAWN");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (agent.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(500));
	}

	protected Node ST_Fight(GameObject participant) {
		Val<string> gestureName = Val.V (() => "FIGHT");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(300));
	}
		
	protected Node ST_StepBack(GameObject participant) {
		Val<string> gestureName = Val.V (() => "STEPBACK");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(800));
	}

	protected Node ST_StepBackTwice(GameObject participant) {
		return new Sequence (ST_StepBack(heroAgent), ST_StepBack(heroAgent));
	}

	protected Node ST_HeroAwayFromPolice() {
		return new Sequence (ST_StepBack(heroAgent), ST_StepBack(heroAgent), ST_StepBack(heroAgent));
	}

	protected Node ST_KeepBack(GameObject participant) {
		Val<string> gestureName = Val.V (() => "KEEP BACK");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(3500));
	}

	protected Node ST_Idle(GameObject participant) {
		Val<string> gestureName = Val.V (() => "IDLE");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(100));
	}

	protected Node ST_InHardShip(GameObject participant) {
		Val<string> gestureName = Val.V (() => "DONT KNOW WHAT TO DO");
		Val<bool> startBool = Val.V (() => true);
		return new Sequence (participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation(gestureName, startBool), new LeafWait(100));
	}

	protected Node randomResPonse(Blackboard blackboard)
	{
		return new Sequence( ST_Idle(policeman), ST_Idle(heroAgent), new MyRandom (
			new Sequence (ST_KeepBack(policeman), ST_HeroAwayFromPolice()),
			new Sequence(
				new LeafInvoke(
					() => (blackboard.ReportedSuccess = 1)	
				),
				ST_HandShake(policeman, heroAgent), ST_Idle(policeman), ST_Idle(heroAgent), 
				ST_StepBackTwice(heroAgent),
				new LeafInvoke(
					() => (blackboard.ReportedSuccess = 2)
				)
			)
			)
		);
	}

	protected Node selectNotReportYet(Blackboard blackboard){
		Func <bool> NotReportYet = () => (blackboard.currentArc == (int)CurrentArc.none);
		return new SelectorParallel (new DecoratorInvert(new DecoratorLoop (new LeafAssert (NotReportYet))), ST_Yawn(policeman));
	}

	protected Node selectReportPolice(Blackboard blackboard){
		Func<bool> act = () => (blackboard.currentArc == (int)CurrentArc.seeThePolice);

		return new SelectorParallel (new DecoratorInvert(new DecoratorLoop (new LeafAssert (act))), randomResPonse(blackboard));
	}

	protected Node selectPoliceWork(Blackboard blackboard){
		Func <bool> HasReport = () => (blackboard.currentArc == (int)CurrentArc.ReportConfirmed);
		return new SelectorParallel (new DecoratorInvert (new DecoratorLoop (new LeafAssert (HasReport))),
			new Sequence (
				ST_ApproachAndWait (policeman, goaltransform)
			));
	}

	protected Node selectMeetAcquaintance(Blackboard blackboard){
		Func <bool> meetAcquaintance = () => (blackboard.currentArc == (int)CurrentArc.greetAcquaintance);

		return new SelectorParallel (new DecoratorInvert (new DecoratorLoop (new LeafAssert (meetAcquaintance))),
			new Sequence (
				ST_Greeting(heroAgent),
				ST_StepBack(heroAgent)
			));
	}


	protected Node selectFighterEscape(Blackboard blackboard){
		Func <bool> meetAcquaintance = () => (blackboard.currentArc == (int)CurrentArc.seeFighter);

		return new SelectorParallel (new DecoratorInvert (new DecoratorLoop (new LeafAssert (meetAcquaintance))),
			new Sequence (
				new LeafInvoke(
					()=> (blackboard.fighterEscaped = true)
				)

			));	
	}

	protected Node selectPolicemanHasNoClues(Blackboard blackboard){
		Func <bool> HasNoClues = () => (blackboard.currentArc == (int)CurrentArc.hasNoClues);
		Func<bool> seeEachOther = () => (checkSee(policeman, fighter2));

		return new SelectorParallel (new DecoratorInvert (new DecoratorLoop (new LeafAssert (HasNoClues))),
			new Sequence (
				ST_ApproachAndWait (policeman, goaltransform),
				new Sequence(
					new DecoratorInvert(new DecoratorLoop(new LeafAssert (seeEachOther))), 
					new LeafInvoke(
						()=> (blackboard.hasArrived = true)
					),
					ST_InHardShip(policeman))
			));
	}

	protected Node selectFindTheFighter(Blackboard blackboard){
		Func <bool> findFighter = () => (blackboard.currentArc == (int)CurrentArc.findFighter);
		return new SelectorParallel (new DecoratorInvert (new DecoratorLoop (new LeafAssert (findFighter))),
			new Sequence (
				ST_Dying (heroAgent), new DecoratorLoop(new Sequence(ST_Dead(heroAgent)))
			));	
	}

	protected Node selectPolicemanFindFighter(Blackboard blackboard){
		Func <bool> seekFighter = () => (blackboard.currentArc == (int)CurrentArc.seekFighter);
		return new SelectorParallel (new DecoratorInvert (new DecoratorLoop (new LeafAssert (seekFighter))),
			new Sequence (
				new LeafInvoke(
					()=>(print("okay"))
				),
				ST_ApproachAndWait(policeman, Destination), ST_Dying (fighter), new DecoratorLoop(new Sequence(ST_Dead(fighter)))
			));	
	}

	protected Node Story(Blackboard blackboard){
		return new DecoratorLoop(
			new DecoratorForceStatus(RunStatus.Success, 
				new Sequence(selectReportPolice(blackboard),
					selectNotReportYet(blackboard),
					selectPoliceWork(blackboard),
					selectMeetAcquaintance(blackboard),
					selectFighterEscape(blackboard),
					selectPolicemanHasNoClues(blackboard),
					selectFindTheFighter(blackboard),
					selectPolicemanFindFighter(blackboard)
					)));
	}

	protected Node SetSeeAcquaintance(Blackboard blackboard){

		return new LeafInvoke(
			()=> (blackboard.meetAcquaintance = true)
		);
	}
		
	protected Node SetSeePolice(Blackboard blackboard){

		return new LeafInvoke(
			()=> (blackboard.seePolice = true)
		);
	}

	protected Node SetNotSeePolice(Blackboard blackboard){
		return new LeafInvoke(
			()=> (blackboard.seePolice = false)
		);
	}

	protected Node SetNotSeeAcquaintance(Blackboard blackboard){
		return new LeafInvoke(
			()=> (
				blackboard.meetAcquaintance = false
			)
		);
	}

	protected Node SetSeeFighter(Blackboard blackboard){

		return new LeafInvoke(
			()=> (blackboard.seeFighter = true)
		);
	}

	protected Node SetNotSeeFighter(Blackboard blackboard){
		return new LeafInvoke(
			()=> (
				blackboard.seeFighter = false
			)
		);
	}

	protected Node SetSecondSeePoliceman(Blackboard blackboard){
		return new LeafInvoke(
			()=> (
				blackboard.secondReport = true
			)
		);
	}

	protected Node MonitorCheckSeeAcquaintance(Blackboard blackboard){
		Func<bool> seeEachOther = () => (checkSee(heroAgent, speaker));

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (seeEachOther), SetSeeAcquaintance(blackboard)));	
	}
		
	protected Node MonitorCheckSee(Blackboard blackboard){
		Func<bool> seeEachOther = () => (checkSee(heroAgent, policeman));
			
		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (seeEachOther), SetSeePolice(blackboard)));
	}

	protected Node MonitorCheckNotSee(Blackboard blackboard){
		Func<bool> notSeeEachOther = () => (!checkSee(heroAgent, policeman));

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (notSeeEachOther), SetNotSeePolice(blackboard)));
	}

	protected Node MonitorCheckNotSeeAcquaintance(Blackboard blackboard){
		Func<bool> notSeeEachOther = () => (!checkSee(heroAgent, speaker));

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (notSeeEachOther), SetNotSeeAcquaintance(blackboard)));
	}
		
	protected Node MonitorCheckNotSeeFighter(Blackboard blackboard){
		Func<bool> notSeeEachOther = () => (!checkSee(heroAgent, fighter));

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (notSeeEachOther), SetNotSeeFighter(blackboard)));
	}

	protected Node MonitorCheckSeeFighter(Blackboard blackboard){
		Func<bool> seeEachOther = () => (checkSee(heroAgent, fighter));

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (seeEachOther), SetSeeFighter(blackboard)));
	}
		
	protected Node MonitorCheckSecondSeePoliceman(Blackboard blackboard){
		Func<bool> seeEachOther = () => (checkSee(heroAgent, policeman) && blackboard.hasArrived == true);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (seeEachOther), SetSecondSeePoliceman(blackboard)));
	}


	protected Node MonitorUserInput(Blackboard blackboard){
		return new DecoratorLoop(
			new Sequence (MonitorCheckSee(blackboard), 
						MonitorCheckNotSee(blackboard), 
						MonitorCheckSeeAcquaintance(blackboard),
						MonitorCheckNotSeeAcquaintance(blackboard),
						MonitorCheckSeeFighter(blackboard),
						MonitorCheckNotSeeFighter(blackboard),
						MonitorCheckSecondSeePoliceman(blackboard)));
	}

	protected Node SetArc(Blackboard blackboard)
	{
		return new LeafInvoke (
			() => (blackboard.currentArc = (int)CurrentArc.seeThePolice)
		);
	}

	protected Node uncheckReportPolice(Blackboard blackboard){
		Func<bool> notSeePolice = () => ((blackboard.seePolice == false && blackboard.ReportedSuccess == 0) && blackboard.meetAcquaintance == false);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence(new LeafAssert (notSeePolice), SetUnckeckArc(blackboard)));
	}

	protected Node SetUnckeckArc(Blackboard blackboard)
	{
		return new LeafInvoke (
			() => (blackboard.currentArc = (int)CurrentArc.none)
		);
	}

	protected Node checkReportPolice(Blackboard blackboard){
		Func<bool> act2 = () => (blackboard.seePolice == true && blackboard.ReportedSuccess == 0);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence(new LeafAssert (act2), SetArc(blackboard)));
	}

	protected Node SetReportSuccess(Blackboard blackboard){
		return new LeafInvoke (
			() => (blackboard.currentArc = (int)CurrentArc.ReportConfirmed)
		);
	}

	protected Node SetHasNoClues(Blackboard blackboard){
		return new LeafInvoke (
			() => (blackboard.currentArc = (int)CurrentArc.hasNoClues)
		);
	}

	protected Node checkReportSuccessWithoutFighter(Blackboard blackboard){
		Func<bool> reportSuccess = () => (blackboard.ReportedSuccess == 2 && blackboard.fighterEscaped == true && blackboard.secondReport == false);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (reportSuccess), SetHasNoClues(blackboard)));
	}

	protected Node checkReportSuccess(Blackboard blackboard){
		Func<bool> reportSuccess = () => (blackboard.ReportedSuccess == 2 && blackboard.fighterEscaped == false);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (reportSuccess), SetReportSuccess(blackboard)));	
	}

	protected Node SetMeetAcquaintance(Blackboard blackboard){
		return new LeafInvoke (
			() => (blackboard.currentArc = (int)CurrentArc.greetAcquaintance)
		);
	}

	protected Node checkMeetAcquaintance(Blackboard blackboard){
		Func<bool> meetAcquaintance = () => (blackboard.meetAcquaintance == true);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (meetAcquaintance), SetMeetAcquaintance(blackboard)));	
	}

	protected Node SetSeeTheFighter(Blackboard blackboard){
		return new LeafInvoke (
			() => (blackboard.currentArc = (int)CurrentArc.seeFighter)
		);
	}

	protected Node checkSeeFighter(Blackboard blackboard){
		Func<bool> seeFighter = () => (blackboard.seeFighter == true && blackboard.fighterEscaped == false);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (seeFighter), SetSeeTheFighter(blackboard)));	
	}

	protected Node SetFindTheFighter(Blackboard blackboard){
		return new LeafInvoke (
			() => (blackboard.currentArc = (int)CurrentArc.findFighter)
		);
	}

	protected Node SetSeekTheFighter(Blackboard blackboard){
		return new LeafInvoke (
			() => (blackboard.currentArc = (int)CurrentArc.seekFighter)
		);
	}

	protected Node checkSecondReport(Blackboard blackboard){
		Func<bool> SecondReport = () => (blackboard.secondReport == true && blackboard.ReportedSuccess == 2 && blackboard.fighterEscaped == true && blackboard.hasArrived == true);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (SecondReport),
			SetSeekTheFighter(blackboard)));
	}

	protected Node checkFindFighter(Blackboard blackboard){
		Func<bool> findFighter = () => (blackboard.seeFighter == true && blackboard.ReportedSuccess == 2 && blackboard.fighterEscaped == true);

		return new DecoratorForceStatus(RunStatus.Success, new Sequence (new LeafAssert (findFighter), SetFindTheFighter(blackboard)));	
	}

	protected Node MonitorStoryState(Blackboard blackboard){
		return new DecoratorLoop(new Sequence(checkReportPolice(blackboard), 
							uncheckReportPolice(blackboard), 
							checkReportSuccess(blackboard),
							checkMeetAcquaintance(blackboard),
							checkSeeFighter(blackboard),
							checkReportSuccessWithoutFighter(blackboard),
							checkFindFighter(blackboard),
							checkSecondReport(blackboard)
							));
	}

	protected Node InteractiveBehaviorTree(Blackboard blackboard)
	{
		return new SequenceParallel (Story(blackboard), MonitorUserInput(blackboard), MonitorStoryState(blackboard));
	}

	protected Node BuildTreeRoot()
	{
		Node WaveToAcquence = new Sequence (ST_ApproachAndWait (heroAgent, FirstStep), ST_StepBack(heroAgent), ST_Greeting(heroAgent), new LeafWait(10000));

		Node ReportPolice = new Sequence( ST_Idle(policeman), ST_Idle(heroAgent), ST_HandShake(policeman, heroAgent), ST_Idle(policeman), ST_Idle(heroAgent), 
			ST_StepBack(heroAgent)
		);

		Node policeWork = new Sequence (
			ST_ApproachAndWait (policeman, goaltransform),
			new DecoratorLoop(
				new Sequence()
			)
		);

		Node policeWorkWithoutFighter = new Sequence (
			ST_ApproachAndWait (policeman, goaltransform),
			new DecoratorLoop(
				new Sequence(ST_InHardShip(policeman))
			)
		);

		Node oneEnd = new Sequence (ST_ApproachAndWait (heroAgent, SecondStep), ReportPolice, new SequenceParallel(policeWork, ST_ApproachAndWait(heroAgent, Destination)));

		Node theOtherEnd = new Sequence (ST_ApproachAndWait (heroAgent, AnotherSecondStep), ST_ApproachAndWait (heroAgent, SecondStep), ReportPolice, new SequenceParallel(policeWorkWithoutFighter, ST_ApproachAndWait(heroAgent, goaltransform)));

		Node twoSplitStroy = new MyRandom (oneEnd, theOtherEnd);

		Node root = new Sequence (WaveToAcquence, twoSplitStroy);

		return root;

	}
}