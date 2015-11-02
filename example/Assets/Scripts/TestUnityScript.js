#pragma strict

class TestUnityScript extends NotificareMonoBehaviour {
	public function Start () {

	}

	public function Update () {

	}
	
	public function OnReady(application) {
		Debug.Log("UnityScript received OnReady");
	}
}