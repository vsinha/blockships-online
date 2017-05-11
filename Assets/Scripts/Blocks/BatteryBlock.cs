using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryBlock: BlockBehavior {

	public int power = 4;

 	int PowerConsumption {
		get {
			return -power;
		}
	}
}