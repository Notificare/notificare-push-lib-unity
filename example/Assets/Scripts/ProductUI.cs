using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ProductUI : MonoBehaviour {

	public Text productLabel;
	public Text buyButtonLabel;

	public NotificarePushLib notificarePushLib;

	private NotificareProduct product;
	public NotificareProduct Product {
		get {
			return product;
		}
		set {
			product = value;

			productLabel.text = product.productName;
			buyButtonLabel.text = product.priceLocale;
		}
	}

	void BuyProduct() {
		Debug.Log("Does this work?");
		Debug.Log("Attempting to buy Product: " + product.productName);

		notificarePushLib.BuyProduct(product);
	}
}
