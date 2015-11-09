using UnityEngine;
using System;
using System.Collections.Generic;

public class Store : NotificareMonoBehaviour {

	private List<NotificareProduct> products;
	private List<NotificareProduct> purchasedProducts;
	private List<SKDownload> downloads;

	public GameObject productUIPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void DidLoadStore(List<NotificareProduct> products) {
		this.products = products;

		Debug.Log("Loaded Store");

		// remove existing productUIs
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}

		foreach (NotificareProduct product in products) {
			GameObject productUIGameObject = Instantiate(productUIPrefab);

			ProductUI productUI = productUIGameObject.GetComponent<ProductUI>();

			productUI.Product = product;
			productUI.notificarePushLib = notificarePushLib;

			productUI.transform.parent = transform;
		}
	}

	public virtual void DidFailToLoadStore(string error) {
		Debug.Log("Failed to Load Store: " + error);
	}

	public virtual void DidCompleteProductTransaction(SKPaymentTransaction transaction) {

	}

	public virtual void DidRestoreProductTransaction(SKPaymentTransaction transaction) {

	}

	public virtual void DidFailProductTransaction(SKPaymentTransaction transaction, string error) {
		Debug.Log("Product Transaction Failed: " + error);
	}

	public virtual void DidStartDownloadContent(SKPaymentTransaction transaction) {

	}

	public virtual void DidPauseDownloadContent(SKDownload download) {

	}

	public virtual void DidCancelDownloadContent(SKDownload download) {

	}

	public virtual void DidReceiveProgressDownloadContent(SKDownload download) {

	}

	public virtual void DidFailDownloadContent(SKDownload download) {

	}

	public virtual void DidFinishDownloadContent(SKDownload download) {

	}

	public virtual void DidFetchProducts(List<NotificareProduct> products) {
		this.products = products;
	}

	public virtual void DidFailToFetchProducts(string error) {
		Debug.Log("Failed to Fetch Products: " + error);
	}

	public virtual void DidFetchPurchasedProducts(List<NotificareProduct> products) {
		this.purchasedProducts = purchasedProducts;
	}
	
	public virtual void DidFailToFetchPurchasedProducts(string error) {
		Debug.Log("Failed to Fetch Purchased Products: " + error);
	}
	
	public virtual void DidFetchProduct(NotificareProduct product) {

	}

	public virtual void DidFailToFetchProduct(string error) {
		Debug.Log("Failed to Fetch Product: " + error);
	}
}
