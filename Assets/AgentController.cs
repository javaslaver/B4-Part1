using UnityEngine;
using System.Collections;

public class AgentController : MonoBehaviour {

	private Animator animator;


	// Use this for initialization
	void Awake () {
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if ((new Vector3 (Input.GetAxis ("Vertical"), 0, Input.GetAxis ("Horizontal"))).magnitude > 0.3f) {
			if (Input.GetAxis ("Vertical") < 0) {
				Vector3 f = (GetComponent<Transform> ()).forward;
				f.Normalize ();
				(GetComponent<Transform> ()).Translate (-0.1f * f);
				this.animator.SetTrigger ("B_StepBackTrigger");
				(GetComponent<Transform> ()).Translate (-0.5f * f);
			} else {
				this.animator.SetFloat ("WalkSpeed", 2f);
				float deltaX = Input.GetAxis ("Horizontal");
				if (deltaX <= 0.3f && deltaX >= -0.3f) {
					this.animator.SetFloat ("WalkDirection", 0);
				} else {
					this.animator.SetFloat ("WalkDirection", deltaX * 3f);
				}
			}
		} else {
			this.animator.SetFloat ("WalkSpeed", 0);
		}
	}
}
