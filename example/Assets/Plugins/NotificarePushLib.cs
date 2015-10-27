using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

public struct SKDownload {

}

public struct SKPaymentTransaction {

}

public struct Notification {

}

public struct NotificareApplication {

}

public struct NotificareNotification {

}

public struct NotificareProduct {
	public string productName;
	public string productDescription;
	public string type;
	// Not doing SKProduct, not relevant for Android.
	public string application;
	public string identifier;
	// Stores variable?
	//public List<SKDownload> downloads;
	public string date;
	public string priceLocale;
	public string price;
	public string currency;
	public bool active;
	public bool purchased;
}

public interface INotificarePushLibSubscriber {
	bool ShouldHandleNotification(Notification notification);
	void DidUpdateBadge(int badge);
	void WillOpenNotification(NotificareNotification notification);
	void DidOpenNotification(NotificareNotification notification);
	void DidCloseNotification(NotificareNotification notification);
	void DidFailToOpenNotification(NotificareNotification notification);
	void DidLoadStore(List<NotificareProduct> products);
	void DidFailToLoadStore(string error);
	void DidCompleteProductTransaction(SKPaymentTransaction transaction);
	void DidRestoreProductTransaction(SKPaymentTransaction transaction);
	void DidFailProductTransaction(SKPaymentTransaction transaction, string error);
	void DidStartDownloadContent(SKPaymentTransaction transaction);
	void DidPauseDownloadContent(SKDownload download);
	void DidCancelDownloadContent(SKDownload download);
	void DidReceiveProgressDownloadContent(SKDownload download);
	void DidFailDownloadContent(SKDownload download);
	void DidFinishDownloadContent(SKDownload download);
	void OnReady(NotificareApplication application);
}

public class NotificarePushLib:Singleton<NotificarePushLib> {

	private delegate void BasicCallback(string json);

	private delegate string DelegateCallback(string json);
	private Dictionary<string, DelegateCallback> delegateCallbacks;

	public delegate void MessageCallback(string message);
	public delegate void InfoCallback(Dictionary<string, object> info);
	public delegate void NotificationCallback(Notification notification);
	public delegate void ProductsCallback(List<NotificareProduct> products);
	public delegate void ProductCallback(NotificareProduct product);

	public List<INotificarePushLibSubscriber> subscribers;

	protected NotificarePushLib() {}

	[DllImport ("__Internal")]
	private static extern void _launch();

	[DllImport ("__Internal")]
	private static extern void _registerForNotifications();

	[DllImport ("__Internal")]
	private static extern void _registerUserNotifications();

	[DllImport ("__Internal")]
	private static extern bool _checkRemoteNotifications();

#warning Is this even going to work at all? Should probably happen in AppDelegate anyway.
	[DllImport ("__Internal")]
	private static extern void _handleOptions(string optionsJSON);
	 
	[DllImport ("__Internal")]
	private static extern void _registerDevice(string token, BasicCallback registrationBasicCallback, BasicCallback errorBasicCallback);
	
	[DllImport ("__Internal")]
	private static extern void _registerDevice(string token, string userID, string username, BasicCallback registrationBasicCallback, BasicCallback errorBasicCallback);

	[DllImport ("__Internal")]
	private static extern void _unregisterDevice();

	[DllImport ("__Internal")]
	private static extern void _updateBadge(int badge);

	[DllImport ("__Internal")]
	private static extern void _openNotification(string notificationJSON);

	[DllImport ("__Internal")]
	private static extern void _logOpenNotification(string notificationJSON);

	[DllImport ("__Internal")]
	private static extern void _getNotification(string notificationID, BasicCallback notificationBasicCallback, BasicCallback errorBasicCallback);

	[DllImport ("__Internal")]
	private static extern void _clearNotification(string notification);

	[DllImport ("__Internal")]
	private static extern void _startLocationUpdates();

	[DllImport ("__Internal")]
	private static extern bool _checkLocationUpdates();

	[DllImport ("__Internal")]
	private static extern void _stopLocationUpdates();

	[DllImport ("__Internal")]
	private static extern void _openUserPreferences();

	[DllImport ("__Internal")]
	private static extern int _myBadge();
	
	[DllImport ("__Internal")]
	private static extern void _fetchProducts(BasicCallback productsBasicCallback, BasicCallback errorBasicCallback);
	
	[DllImport ("__Internal")]
	private static extern void _fetchPurchasedProducts(BasicCallback productsBasicCallback, BasicCallback errorBasicCallback);
	
	[DllImport ("__Internal")]
	private static extern void _fetchProduct(string productIdentifier, BasicCallback productBasicCallback, BasicCallback errorBasicCallback);

	[DllImport ("__Internal")]
	private static extern void _buyProduct(string productJSON);

	[DllImport ("__Internal")]
	private static extern void _pauseDownloads(string downloadsJSON);

	[DllImport ("__Internal")]
	private static extern void _cancelDownloads(string downloadsJSON);

	[DllImport ("__Internal")]
	private static extern void _resumeDownloads(string downloadsJSON);

	[DllImport ("__internal")]
	private static extern string _contentPathForProduct(string productIdentifier);

	[DllImport ("__internal")]
	private static extern void _registerDelegateCallback(string delegateMethod, DelegateCallback callback);

	[DllImport ("__internal")]
	private static extern void _unregisterDelegateCallback(string delegateMethod);

	string ShouldHandleNotification(string notificationJSON) {
		var definition = new {notification = new Notification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		bool shouldHandleNotification = false;

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			shouldHandleNotification |= subscriber.ShouldHandleNotification(notificationInfo.notification);
		}

		var shouldHandleNotificationInfo = new {shouldHandleNotification = shouldHandleNotification};
		string shouldHandleNotificationJSON = JsonConvert.SerializeObject(shouldHandleNotificationInfo);

		return shouldHandleNotificationJSON;
	}

	string DidUpdateBadge(string badgeJSON) {
		var definition = new {badge = 0};
		var badgeInfo = JsonConvert.DeserializeAnonymousType(badgeJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidUpdateBadge(badgeInfo.badge);
		}

		return null;
	}

	string WillOpenNotification(string notificationJSON) {
		var definition = new {notification = new NotificareNotification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.WillOpenNotification(notificationInfo.notification);
		}

		return null;
	}

	string DidOpenNotification(string notificationJSON) {
		var definition = new {notification = new NotificareNotification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidOpenNotification(notificationInfo.notification);
		}

		return null;
	}

	string DidCloseNotification(string notificationJSON) {
		var definition = new {notification = new NotificareNotification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidCloseNotification(notificationInfo.notification);
		}

		return null;
	}

	string DidFailToOpenNotification(string notificationJSON) {
		var definition = new {notification = new NotificareNotification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidFailToOpenNotification(notificationInfo.notification);
		}

		return null;
	}

	string DidLoadStore(string productsJSON) {
		var definition = new {products = new List<NotificareProduct>()};
		var productsInfo = JsonConvert.DeserializeAnonymousType(productsJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidLoadStore(productsInfo.products);
		}

		return null;
	}

	string DidFailToLoadStore(string errorJSON) {
		var definition = new {error = ""};
		var errorInfo = JsonConvert.DeserializeAnonymousType(errorJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidFailToLoadStore(errorInfo.error);
		}

		return null;
	}

	string DidCompleteProductTransaction(string transactionJSON) {
		var definition = new {transaction = new SKPaymentTransaction()};
		var transactionInfo = JsonConvert.DeserializeAnonymousType(transactionJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidCompleteProductTransaction(transactionInfo.transaction);
		}

		return null;
	}

	string DidRestoreProductTransaction(string transactionJSON) {
		var definition = new {transaction = new SKPaymentTransaction()};
		var transactionInfo = JsonConvert.DeserializeAnonymousType(transactionJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidRestoreProductTransaction(transactionInfo.transaction);
		}
		
		return null;
	}

	string DidFailProductTransaction(string failedTransactionJSON) {
		var definition = new {transaction = new SKPaymentTransaction(), error = ""};
		var failedTransactionInfo = JsonConvert.DeserializeAnonymousType(failedTransactionJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidFailProductTransaction(failedTransactionInfo.transaction, failedTransactionInfo.error);
		}

		return null;
	}

	string DidStartDownloadContent(string transactionJSON) {
		var definition = new {transaction = new SKPaymentTransaction()};
		var transactionInfo = JsonConvert.DeserializeAnonymousType(transactionJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidStartDownloadContent(transactionInfo.transaction);
		}

		return null;
	}

	string DidPauseDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidPauseDownloadContent(downloadInfo.download);
		}

		return null;
	}

	string DidCancelDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidCancelDownloadContent(downloadInfo.download);
		}

		return null;
	}

	string DidReceiveProgressDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidReceiveProgressDownloadContent(downloadInfo.download);
		}

		return null;
	}

	string DidFailDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidFailDownloadContent(downloadInfo.download);
		}

		return null;
	}

	string DidFinishDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.DidFinishDownloadContent(downloadInfo.download);
		}

		return null;
	}

	string OnReady(string applicationJSON) {
		var definition = new {application = new NotificareApplication()};
		var applicationInfo = JsonConvert.DeserializeAnonymousType(applicationJSON, definition);

		foreach (INotificarePushLibSubscriber subscriber in subscribers) {
			subscriber.OnReady(applicationInfo.application);
		}

		return null;
	}

	public void Launch() {
		_launch();
	}

	public void RegisterForNotifications() {
		_registerForNotifications();
	}

	public void RegisterUserNotifications() {
		_registerUserNotifications();
	}

	public bool CheckRemoteNotifications() {
		return _checkRemoteNotifications();
	}

	public void HandleOptions(Dictionary<string, string> options) {
		var optionsInfo = new {options = options};
		var optionsJSON = JsonConvert.SerializeObject(optionsInfo);

		_handleOptions(optionsJSON);
	}

	public void RegisterDevice(string token, InfoCallback registrationCallback, MessageCallback errorCallback) {
		BasicCallback infoBasicCallback = delegate(string json) {
			var definition = new {registration = new Dictionary<string, object>()};
			var registrationInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			registrationCallback(registrationInfo.registration);
		};

		BasicCallback errorBasicCallback = delegate(string json) {
			var definition = new {error = ""};
			var errorInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			errorCallback(errorInfo.error);
		};

		_registerDevice(token, infoBasicCallback, errorBasicCallback);
	}

	public void RegisterDeviceWithUser(string token, string userID, string userName, InfoCallback registrationCallback, MessageCallback errorCallback) {
		BasicCallback infoBasicCallback = delegate(string json) {
			var definition = new {registration = new Dictionary<string, object>()};
			var registrationInfo = JsonConvert.DeserializeAnonymousType(json, definition);
			
			registrationCallback(registrationInfo.registration);
		};
		
		BasicCallback errorBasicCallback = delegate(string json) {
			var definition = new {error = ""};
			var errorInfo = JsonConvert.DeserializeAnonymousType(json, definition);
			
			errorCallback(errorInfo.error);
		};

		_registerDevice(token, userID, userName, infoBasicCallback, errorBasicCallback);
	}

	public void UnregisterDevice() {
		_unregisterDevice();
	}

	public void UpdateBadge(int badge) {
		_updateBadge(badge);
	}

	public void OpenNotification(Notification notification) {
		var notificationInfo = new {notification = notification};
		string notificationJSON = JsonConvert.SerializeObject(notificationInfo);

		_openNotification(notificationJSON);
	}

	public void LogOpenNotification(Notification notification) {
		var notificationInfo = new {notification = notification};
		string notificationJSON = JsonConvert.SerializeObject(notificationInfo);

		_logOpenNotification(notificationJSON);
	}

	public void GetNotification(string notificationID, NotificationCallback notificationCallback, MessageCallback errorCallback) {
		BasicCallback notificationBasicCallback = delegate(string json) {
			var definition = new {notification = new Notification()};
			var notificationInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			notificationCallback(notificationInfo.notification);
		};

		BasicCallback errorBasicCallback = delegate(string json) {
			var definition = new {error = ""};
			var errorInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			errorCallback(errorInfo.error);
		};

		_getNotification(notificationID, notificationBasicCallback, errorBasicCallback);
	}

	public void ClearNotification(string notificationID) {
		_clearNotification(notificationID);
	}

	public void StartLocationUpdates() {
		_startLocationUpdates();
	}

	public bool CheckLocationUpdates() {
		return _checkLocationUpdates();
	}

	public void StopLocationUpdates() {
		_stopLocationUpdates();
	}

	public void OpenUserPreferences() {
		_openUserPreferences();
	}

	public int myBadge() {
		return _myBadge();
	}

	public void FetchProducts(ProductsCallback productsCallback, MessageCallback errorCallback) {
		BasicCallback productsBasicCallback = delegate(string json) {
			var definition = new {products = new List<NotificareProduct>()};
			var productsInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			productsCallback(productsInfo.products);
		};

		BasicCallback errorBasicCallback = delegate(string json) {
			var definition = new {error = ""};
			var errorInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			errorCallback(errorInfo.error);
		};

		_fetchProducts(productsBasicCallback, errorBasicCallback);
	}

	public void FetchPurchasedProducts(ProductsCallback productsCallback, MessageCallback errorCallback) {
		BasicCallback productsBasicCallback = delegate(string json) {
			var definition = new {products = new List<NotificareProduct>()};
			var productsInfo = JsonConvert.DeserializeAnonymousType(json, definition);
			
			productsCallback(productsInfo.products);
		};
		
		BasicCallback errorBasicCallback = delegate(string json) {
			var definition = new {error = ""};
			var errorInfo = JsonConvert.DeserializeAnonymousType(json, definition);
			
			errorCallback(errorInfo.error);
		};
		
		_fetchPurchasedProducts(productsBasicCallback, errorBasicCallback);
	}

	public void FetchProduct(string productIdentifier, ProductCallback productCallback, MessageCallback errorCallback) {
		BasicCallback productBasicCallback = delegate(string json) {
			var definition = new {product = new NotificareProduct()};
			var productInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			productCallback(productInfo.product);
		};

		BasicCallback errorBasicCallback = delegate(string json) {
			var definition = new {error = ""};
			var errorInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			errorCallback(errorInfo.error);
		};

		_fetchProduct(productIdentifier, productBasicCallback, errorBasicCallback);
	}

	public void BuyProduct(NotificareProduct product) {
		var productInfo = new {product = product};
		string productJSON = JsonConvert.SerializeObject(productInfo);

		_buyProduct(productJSON);
	}

	public void PauseDownloads(List<SKDownload> downloads) {
		var downloadsInfo = new {downloads = downloads};
		string downloadsJSON = JsonConvert.SerializeObject(downloadsInfo);

		_pauseDownloads(downloadsJSON);
	}

	public void CancelDownloads(List<SKDownload> downloads) {
		var downloadsInfo = new {downloads = downloads};
		string downloadsJSON = JsonConvert.SerializeObject(downloadsInfo);
		
		_cancelDownloads(downloadsJSON);
	}

	public void ResumeDownloads(List<SKDownload> downloads) {
		var downloadsInfo = new {downloads = downloads};
		string downloadsJSON = JsonConvert.SerializeObject(downloadsInfo);
		
		_resumeDownloads(downloadsJSON);
	}

	public string ContentPathForProduct(string productIdentifier) {
		return _contentPathForProduct(productIdentifier);
	}

	private void RegisterDelegateCallback(string delegateMethod, DelegateCallback callback) {
		if (delegateCallbacks.ContainsKey(delegateMethod)) {
			delegateCallbacks[delegateMethod] = callback;
		}
		else {
			delegateCallbacks.Add(delegateMethod, callback);
		}

		_registerDelegateCallback(delegateMethod, callback);
	}

	private void UnregisterDelegateCallback(string delegateMethod) {
		delegateCallbacks.Remove(delegateMethod);
		_unregisterDelegateCallback(delegateMethod);
	}

	void RegisterDelegateCallbacks() {
		RegisterDelegateCallback("shouldHandleNotification", new DelegateCallback(ShouldHandleNotification));
		RegisterDelegateCallback("didUpdateBadge", new DelegateCallback(DidUpdateBadge));
		RegisterDelegateCallback("willOpenNotification", new DelegateCallback(WillOpenNotification));
		RegisterDelegateCallback("didOpenNotification", new DelegateCallback(DidOpenNotification));
		RegisterDelegateCallback("didCloseNotification", new DelegateCallback(DidCloseNotification));
		RegisterDelegateCallback("didFailToOpenNotification", new DelegateCallback(DidFailToOpenNotification));
		RegisterDelegateCallback("didLoadStore", new DelegateCallback(DidLoadStore));
		RegisterDelegateCallback("didFailToLoadStore", new DelegateCallback(DidFailToLoadStore));
		RegisterDelegateCallback("didCompleteProductTransaction", new DelegateCallback(DidCompleteProductTransaction));
		RegisterDelegateCallback("didRestoreProductTransaction", new DelegateCallback(DidRestoreProductTransaction));
		RegisterDelegateCallback("didFailProductTransaction", new DelegateCallback(DidFailProductTransaction));
		RegisterDelegateCallback("didStartDownloadContent", new DelegateCallback(DidStartDownloadContent));
		RegisterDelegateCallback("didPauseDownloadContent", new DelegateCallback(DidPauseDownloadContent));
		RegisterDelegateCallback("didCancelDownloadContent", new DelegateCallback(DidCancelDownloadContent));
		RegisterDelegateCallback("didReceiveProgressDownloadContent", new DelegateCallback(DidReceiveProgressDownloadContent));
		RegisterDelegateCallback("didFailDownloadContent", new DelegateCallback(DidFailDownloadContent));
		RegisterDelegateCallback("didFinishDownloadContent", new DelegateCallback(DidFinishDownloadContent));
		RegisterDelegateCallback("onReady", new DelegateCallback(OnReady));
	}

	void UnregisterDelegateCallbacks() {
		UnregisterDelegateCallback("shouldHandleNotification");
		UnregisterDelegateCallback("didUpdateBadge");
		UnregisterDelegateCallback("willOpenNotification");
		UnregisterDelegateCallback("didOpenNotification");
		UnregisterDelegateCallback("didCloseNotification");
		UnregisterDelegateCallback("didFailToOpenNotification");
		UnregisterDelegateCallback("didLoadStore");
		UnregisterDelegateCallback("didFailToLoadStore");
		UnregisterDelegateCallback("didCompleteProductTransaction");
		UnregisterDelegateCallback("didRestoreProductTransaction");
		UnregisterDelegateCallback("didFailProductTransaction");
		UnregisterDelegateCallback("didStartDownloadContent");
		UnregisterDelegateCallback("didPauseDownloadContent");
		UnregisterDelegateCallback("didCancelDownloadContent");
		UnregisterDelegateCallback("didReceiveProgressDownloadContent");
		UnregisterDelegateCallback("didFailDownloadContent");
		UnregisterDelegateCallback("didFinishDownloadContent");
		UnregisterDelegateCallback("onReady");
	}

	void Awake() {
		delegateCallbacks = new Dictionary<string, DelegateCallback>();

		RegisterDelegateCallbacks();
	}

	void OnApplicationQuit() {
		UnregisterDelegateCallbacks();
	}
}


