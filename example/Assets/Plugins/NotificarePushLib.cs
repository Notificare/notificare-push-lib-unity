using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

public struct SKDownload {
	public string contentIdentfier;
	public int contentLength;
	public string contentVersion;
	public string transactionIdentifier;
	public string state;
	public float progress;
	public string error;
	public string contentURL;
}

public struct SKPayment {
	public string productIdentifier;
	public int quantity;
	public string applicationUsername;
}

public struct SKPaymentTransaction {
	public string error;
	public SKPayment payment;
	public string transactionState;
	public string transactionIdentifier;
	public string transactionDate;
	public List<SKDownload> downloads;
}

public struct Notification {

}

public struct NotificareApplication {
	public string name;
	public string category;
	public string appStoreId;
	public Dictionary<string, string> regionConfig;
	public string androidPackageName;
	public Dictionary<string, bool> services;
	// public List<object> actionCategories; // Might implement this at a later time
}

public struct NotificareAction {
	string actionType;
	string actionLabel;
	string actionTarget;
	bool actionKeyboard;
	bool actionCamera;
}

public struct NotificareAttachment {
	string attachmentURI;
	string attachmentMimeType;
}

public struct NotificareContent {
	string type;
	string data;
	Dictionary<string, object> dataDictionary;
}

public struct NotificareSegment {
	string segmentLabel;
	string segmentId;
	bool selected;
}

public struct NotificareNotification {
	public string notificationID;
	public NotificareApplication application;
	public string notificationType;
	public string notificationTime;
	public string notificationMessage;
	public string notificationLatitude;
	public string notificationLongitude;
	public string notificationDistance;
	public List<NotificareContent> notificationContent;
	public List<NotificareAction> notificationActions;
	public List<NotificareAttachment> notificationAttachments;
	public List<string> notificationTags;
	public List<NotificareSegment> notificationSegments;
	public Dictionary<string, object> notificationExtra;
	public Dictionary<string, object> notificationInfo;
	public int displayMessage;
}

public struct NotificareProduct {
	public string productName;
	public string productDescription;
	public string type;
	// Not doing SKProduct, not relevant for Android.
	public string application;
	public string identifier;
	// Stores variable?
	public List<SKDownload> downloads;
	public string date;
	public string priceLocale;
	public string price;
	public string currency;
	public bool active;
	public bool purchased;
}

public class NotificareMonoBehaviour:MonoBehaviour {
	public virtual bool ShouldHandleNotification(Notification notification) {return false;}
	public virtual void DidUpdateBadge(int badge) {}
	public virtual void WillOpenNotification(NotificareNotification notification) {}
	public virtual void DidOpenNotification(NotificareNotification notification) {}
	public virtual void DidCloseNotification(NotificareNotification notification) {}
	public virtual void DidFailToOpenNotification(NotificareNotification notification) {}
	public virtual void DidLoadStore(List<NotificareProduct> products) {}
	public virtual void DidFailToLoadStore(string error) {}
	public virtual void DidCompleteProductTransaction(SKPaymentTransaction transaction) {}
	public virtual void DidRestoreProductTransaction(SKPaymentTransaction transaction) {}
	public virtual void DidFailProductTransaction(SKPaymentTransaction transaction, string error) {}
	public virtual void DidStartDownloadContent(SKPaymentTransaction transaction) {}
	public virtual void DidPauseDownloadContent(SKDownload download) {}
	public virtual void DidCancelDownloadContent(SKDownload download) {}
	public virtual void DidReceiveProgressDownloadContent(SKDownload download) {}
	public virtual void DidFailDownloadContent(SKDownload download) {}
	public virtual void DidFinishDownloadContent(SKDownload download) {}
	public virtual void OnReady(NotificareApplication application) {}

	public NotificarePushLib notificarePushLib;
}

public delegate void MessageCallback(string message);
public delegate void InfoCallback(Dictionary<string, object> info);
public delegate void NotificationCallback(Notification notification);
public delegate void ProductsCallback(List<NotificareProduct> products);
public delegate void ProductCallback(NotificareProduct product);

public class NotificarePushLib:Singleton<NotificarePushLib> {

	private delegate void BasicCallback(string json);

	private delegate string DelegateCallback(string json);
	private Dictionary<string, DelegateCallback> delegateCallbacks;

	public List<NotificareMonoBehaviour> subscribers;

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

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				shouldHandleNotification |= subscriber.ShouldHandleNotification(notificationInfo.notification);
			}
		}

		var shouldHandleNotificationInfo = new {shouldHandleNotification = shouldHandleNotification};
		string shouldHandleNotificationJSON = JsonConvert.SerializeObject(shouldHandleNotificationInfo);

		return shouldHandleNotificationJSON;
	}

	string DidUpdateBadge(string badgeJSON) {
		var definition = new {badge = 0};
		var badgeInfo = JsonConvert.DeserializeAnonymousType(badgeJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidUpdateBadge(badgeInfo.badge);
			}
		}

		return null;
	}

	string WillOpenNotification(string notificationJSON) {
		var definition = new {notification = new NotificareNotification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.WillOpenNotification(notificationInfo.notification);
			}
		}

		return null;
	}

	string DidOpenNotification(string notificationJSON) {
		var definition = new {notification = new NotificareNotification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidOpenNotification(notificationInfo.notification);
			}
		}

		return null;
	}

	string DidCloseNotification(string notificationJSON) {
		var definition = new {notification = new NotificareNotification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidCloseNotification(notificationInfo.notification);
			}
		}

		return null;
	}

	string DidFailToOpenNotification(string notificationJSON) {
		var definition = new {notification = new NotificareNotification()};
		var notificationInfo = JsonConvert.DeserializeAnonymousType(notificationJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToOpenNotification(notificationInfo.notification);
			}
		}

		return null;
	}

	string DidLoadStore(string productsJSON) {
		var definition = new {products = new List<NotificareProduct>()};
		var productsInfo = JsonConvert.DeserializeAnonymousType(productsJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidLoadStore(productsInfo.products);
			}
		}

		return null;
	}

	string DidFailToLoadStore(string errorJSON) {
		var definition = new {error = ""};
		var errorInfo = JsonConvert.DeserializeAnonymousType(errorJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToLoadStore(errorInfo.error);
			}
		}

		return null;
	}

	string DidCompleteProductTransaction(string transactionJSON) {
		var definition = new {transaction = new SKPaymentTransaction()};
		var transactionInfo = JsonConvert.DeserializeAnonymousType(transactionJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			subscriber.DidCompleteProductTransaction(transactionInfo.transaction);
		}

		return null;
	}

	string DidRestoreProductTransaction(string transactionJSON) {
		var definition = new {transaction = new SKPaymentTransaction()};
		var transactionInfo = JsonConvert.DeserializeAnonymousType(transactionJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidRestoreProductTransaction(transactionInfo.transaction);
			}
		}
		
		return null;
	}

	string DidFailProductTransaction(string failedTransactionJSON) {
		var definition = new {transaction = new SKPaymentTransaction(), error = ""};
		var failedTransactionInfo = JsonConvert.DeserializeAnonymousType(failedTransactionJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidFailProductTransaction(failedTransactionInfo.transaction, failedTransactionInfo.error);
			}
		}

		return null;
	}

	string DidStartDownloadContent(string transactionJSON) {
		var definition = new {transaction = new SKPaymentTransaction()};
		var transactionInfo = JsonConvert.DeserializeAnonymousType(transactionJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidStartDownloadContent(transactionInfo.transaction);
			}
		}

		return null;
	}

	string DidPauseDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidPauseDownloadContent(downloadInfo.download);
			}
		}

		return null;
	}

	string DidCancelDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidCancelDownloadContent(downloadInfo.download);
			}
		}

		return null;
	}

	string DidReceiveProgressDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidReceiveProgressDownloadContent(downloadInfo.download);
			}
		}

		return null;
	}

	string DidFailDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidFailDownloadContent(downloadInfo.download);
			}
		}

		return null;
	}

	string DidFinishDownloadContent(string downloadJSON) {
		var definition = new {download = new SKDownload()};
		var downloadInfo = JsonConvert.DeserializeAnonymousType(downloadJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidFinishDownloadContent(downloadInfo.download);
			}
		}

		return null;
	}

	string OnReady(string applicationJSON) {
		var definition = new {application = new NotificareApplication()};
		var applicationInfo = JsonConvert.DeserializeAnonymousType(applicationJSON, definition);

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.OnReady(applicationInfo.application);
			}
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

	public void RegisterDevice(byte[] byteToken, InfoCallback registrationCallback, MessageCallback errorCallback) {
#warning Not sure if this will create a proper char array to generate NSData from
		string stringToken = Encoding.UTF8.GetString(byteToken);

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

		_registerDevice(stringToken, infoBasicCallback, errorBasicCallback);
	}

	public void RegisterDeviceWithUser(byte[] byteToken, string userID, string userName, InfoCallback registrationCallback, MessageCallback errorCallback) {
#warning Not sure if this will create a proper char array to generate NSData from
		string stringToken = Encoding.UTF8.GetString(byteToken);

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

		_registerDevice(stringToken, userID, userName, infoBasicCallback, errorBasicCallback);
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


