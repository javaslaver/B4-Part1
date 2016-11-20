using UnityEngine;
using System.Collections;

public class Doors : MonoBehaviour {

	Animator animator;

	bool doorOpen;

	void Start()
	{
		doorOpen = false;
		animator = GetComponent<Animator> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			doorOpen = true;
			DoorControl ("Open");
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (doorOpen) 
		{
			doorOpen = false;
			DoorControl ("Close");
		}
	}

	void DoorControl(string direction)
	{
		animator.SetTrigger(direction);
	}
}
