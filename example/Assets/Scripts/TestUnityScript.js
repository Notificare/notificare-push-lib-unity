#pragma strict

class TestUnityScript extends NotificareMonoBehaviour {
	public function Start () {

	}

	public function Update () {

	}
	
	public override function OnReady(application:NotificareApplication) {
		Debug.Log("UnityScript received OnReady");
	}
}