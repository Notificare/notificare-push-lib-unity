using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : NotificareMonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public override void OnInit() {
		if (notificarePushLib != null) {
			notificarePushLib.Launch();
		}
	}

	public override void OnReady(NotificareApplication application) {
		Debug.Log("OnReady");
		Debug.Log(application.services);

		notificarePushLib.RegisterForNotifications();
	}

	public override void DidRegisterForRemoteNotificationsWithDeviceToken(byte[] deviceToken) {
		Debug.Log("DidRegisterForRemoteNotificationsWithDeviceToken");

		notificarePushLib.RegisterDevice(deviceToken);
	}

	public override void DidRegisterDevice(Dictionary<string, object> registration) {
		Debug.Log("DidRegisterDevice");

		if (notificarePushLib.CheckLocationUpdates()) {
			Debug.Log("Authorized");
		}
		else {
			Debug.Log("Not Authorized");
		}

		notificarePushLib.StartLocationUpdates();
	}

	public override void DidFailToRegisterDevice(string error) {
		Debug.Log("DidFailToRegisterDevice: " + error);
	}
}
