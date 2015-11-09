using UnityEngine;
using System.Collections;

public class Notifications : NotificareMonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void WillOpenNotification(NotificareNotification notification) {

	}

	public virtual void DidOpenNotification(NotificareNotification notification) {

	}

	public virtual void DidCloseNotification(NotificareNotification notification) {

	}

	public virtual void DidFailToOpenNotification(NotificareNotification notification) {

	}

	public override void DidReceiveRemoteNotification (Notification notification) {
		notificarePushLib.OpenNotification(notification);
		
		Debug.Log("Received Notification: " + notification.aps.alert);
	}
}
