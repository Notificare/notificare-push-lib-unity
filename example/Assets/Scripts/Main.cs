using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

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
		Debug.Log("NotificarePushLib for Unity is Ready:");
		Debug.Log(application);

		// Is this a synchronous call?
		NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);

		if (NotificationServices.registrationError != null) {
			Debug.LogError(NotificationServices.registrationError);

			// Cannot continue
			return;
		}

		if (NotificationServices.deviceToken == null) {
			Debug.LogError("Device Token is empty.");

			// Cannot continue
			return;
		}

		InfoCallback registrationCallback = delegate(Dictionary<string, object> registration) {
			Debug.Log("Device successfully registered with Notificare:");
			Debug.Log(registration);

			if (notificarePushLib.CheckLocationUpdates()) {
				notificarePushLib.StartLocationUpdates();
			}
		};

		MessageCallback errorCallback = delegate(string message) {
			Debug.LogError("Device failed to register with Notificare: " + message);
		};

		notificarePushLib.RegisterDevice(NotificationServices.deviceToken, registrationCallback, errorCallback);
	}
}
