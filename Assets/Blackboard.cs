using UnityEngine;
using System.Collections;

namespace TreeSharpPlus
{
	public class Blackboard{

		public int currentArc;
		public bool seePolice;
		public int ReportedSuccess; //0 fail, 1 processing, 2 success
		public bool meetAcquaintance;
		public bool seeFighter;
		public bool fighterEscaped;
		public bool hasArrived;
		public bool secondReport;

		public Blackboard()
		{
			currentArc = 0;
			seePolice = false;
			ReportedSuccess = 0;
			meetAcquaintance = false;
			seeFighter = false;
			fighterEscaped = false;
			hasArrived = false;
			secondReport = false;
		}
	}
}