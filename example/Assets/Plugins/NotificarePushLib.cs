using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public struct SKDownload {

}

public struct NotificareProduct {
	public string productName;
	public string productDescription;
	public string type;
	// Not doing SKProduct, not relevant for Android.
	public string application;
	public string identifier;
	// Stores variable?
	// Downloads variable?
	public string date;
	public string priceLocale;
	public string price;
	public string currency;
	public bool active;
	public bool purchased;
}

public class NotificarePushLib:Singleton<NotificarePushLib> {
	
	private delegate string StringCallback(string str);

	protected NotificarePushLib() {}

	[DllImport ("__Internal")]
	private static extern void launch();

	[DllImport ("__Internal")]
	private static extern void registerForNotifications();

	[DllImport ("__Internal")]
	private static extern void registerUserNotifications();

	[DllImport ("__Internal")]
	private static extern bool checkRemoteNotifications();

#warning How to create key-value objects like dictionaries in C#? Is this even going to work at all? Should probably happen in AppDelegate anyway.
	[DllImport ("__Internal")]
	private static extern void handleOptions(string optionsJSON);
	 
	[DllImport ("__Internal")]
	private static extern void registerDevice(string token, StringCallback callback);
	
	[DllImport ("__Internal")]
	private static extern void registerDevice(string token, string userID, string username, StringCallback callback);

	[DllImport ("__Internal")]
	private static extern void unregisterDevice();

	[DllImport ("__Internal")]
	private static extern void updateBadge(int badge);

	[DllImport ("__Internal")]
	private static extern void openNotification(string notificationJSON);

	[DllImport ("__Internal")]
	private static extern void logOpenNotification(string notificationJSON);

	[DllImport ("__Internal")]
	private static extern void getNotification(string notificationID, StringCallback callback);

	[DllImport ("__Internal")]
	private static extern void clearNotification(string notification);

	[DllImport ("__Internal")]
	private static extern void startLocationUpdates();

	[DllImport ("__Internal")]
	private static extern bool checkLocationUpdates();

	[DllImport ("__Internal")]
	private static extern void stopLocationUpdates();

	[DllImport ("__Internal")]
	private static extern void openUserPreferences();
	
	[DllImport ("__Internal")]
	private static extern void fetchProducts(StringCallback productsCallback, StringCallback errorCallback);
	
	[DllImport ("__Internal")]
	private static extern void fetchPurchasedProducts(StringCallback productsCallback, StringCallback errorCallback);
	
	[DllImport ("__Internal")]
	private static extern void fetchProduct(string productIdentifier, StringCallback productsCallback, StringCallback errorCallback);

	[DllImport ("__Internal")]
	private static extern void buyProduct(string productJSON);

	[DllImport ("__Internal")]
	private static extern void pauseDownloads(string downloadsJSON);

	[DllImport ("__Internal")]
	private static extern void cancelDownloads(string downloadsJSON);

	[DllImport ("__Internal")]
	private static extern void resumeDownloads(string downloadsJSON);

	[DllImport ("__internal")]
	private static extern string contentPathForProduct(string productIdentifier); 
}
