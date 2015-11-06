using UnityEngine;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using RemoteNotification = UnityEngine.iOS.RemoteNotification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using LitJson;
using AOT;

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

public struct Aps {
	public string alert;
	public int badge;
	public string sound;
}

public struct Notification {
	public Aps aps;
	public string id;
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
	public double notificationLatitude;
	public double notificationLongitude;
	public double notificationDistance;
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
	public NotificarePushLib notificarePushLib;

	public virtual void OnInit() {}

	public virtual bool ShouldHandleNotification(Notification notification) {return true;}
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

	public virtual void DidRegisterForRemoteNotificationsWithDeviceToken(byte[] deviceToken) {}
	public virtual void DidReceiveRemoteNotification(Notification notification) {}

	public virtual void DidRegisterDevice(Dictionary<string, object> registration) {}
	public virtual void DidFailToRegisterDevice(string error) {}
	public virtual void DidGetNotification(Notification notification) {}
	public virtual void DidFailToGetNotification(string error) {}
	public virtual void DidFetchProducts(List<NotificareProduct> products) {}
	public virtual void DidFailToFetchProducts(string error) {}
	public virtual void DidFetchPurchasedProducts(List<NotificareProduct> products) {}
	public virtual void DidFailToFetchPurchasedProducts(string error) {}
	public virtual void DidFetchProduct(NotificareProduct product) {}
	public virtual void DidFailToFetchProduct(string error) {}
}

public class NotificarePushLib:Singleton<NotificarePushLib> {

	private class JsonNotificationWriter {
		public static void Main()
		{
			StringBuilder sb = new StringBuilder();
			JsonWriter writer = new JsonWriter(sb);
			
			writer.WriteArrayStart();
			writer.Write(1);
			writer.Write(2);
			writer.Write(3);
			
			writer.WriteObjectStart();
			writer.WritePropertyName("color");
			writer.Write("blue");
			writer.WriteObjectEnd();
			
			writer.WriteArrayEnd();
			
			Console.WriteLine(sb.ToString());
		}
	}

	private struct ErrorInfo {
		public string error;
	}

	private struct NotificareApplicationInfo {
		public NotificareApplication application;
	}

	private struct BadgeInfo {
		public int badge;
	}

	private struct NotificationInfo {
		public Notification notification;
	}

	private struct NotificationsInfo {
		public List<Notification> notifications;
	}

	private struct NotificareNotificationInfo {
		public NotificareNotification notification;
	}

	private struct ProductInfo {
		public NotificareProduct product;
	}

	private struct ProductsInfo {
		public List<NotificareProduct> products;
	}

	private struct TransactionInfo {
		public SKPaymentTransaction transaction;
	}

	private struct FailedTransactionInfo {
		public SKPaymentTransaction transaction;
		public string error;
	}

	private struct DownloadInfo {
		public SKDownload download;
	}

	private struct DownloadsInfo {
		public List<SKDownload> downloads;
	}

	private struct RegistrationInfo {
		public Dictionary<string, object> registration;
	}

	private delegate void BasicCallback(string json);

	private delegate string DelegateCallback(string json);
	private Dictionary<string, DelegateCallback> delegateCallbacks;

	private static List<NotificareMonoBehaviour> Subscribers;
	public List<NotificareMonoBehaviour> subscribers;

	// For Singleton Implementation
	protected NotificarePushLib() {}


	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string ShouldHandleNotification(string notificationJSON) {
		NotificationInfo notificationInfo = JsonMapper.ToObject<NotificationInfo>(notificationJSON);
		
		bool shouldHandleNotification = true;
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				shouldHandleNotification &= subscriber.ShouldHandleNotification(notificationInfo.notification);
			}
		}
		
		var shouldHandleNotificationInfo = new {shouldHandleNotification = shouldHandleNotification};
		string shouldHandleNotificationJSON = JsonMapper.ToJson(shouldHandleNotificationInfo);
		
		return shouldHandleNotificationJSON;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidUpdateBadge(string badgeJSON) {
		BadgeInfo badgeInfo = JsonMapper.ToObject<BadgeInfo>(badgeJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidUpdateBadge(badgeInfo.badge);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string WillOpenNotification(string notificationJSON) {
		NotificareNotificationInfo notificationInfo = JsonMapper.ToObject<NotificareNotificationInfo>(notificationJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.WillOpenNotification(notificationInfo.notification);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidOpenNotification(string notificationJSON) {
		NotificareNotificationInfo notificationInfo = JsonMapper.ToObject<NotificareNotificationInfo>(notificationJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidOpenNotification(notificationInfo.notification);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidCloseNotification(string notificationJSON) {
		NotificareNotificationInfo notificationInfo = JsonMapper.ToObject<NotificareNotificationInfo>(notificationJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidCloseNotification(notificationInfo.notification);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidFailToOpenNotification(string notificationJSON) {
		NotificareNotificationInfo notificationInfo = JsonMapper.ToObject<NotificareNotificationInfo>(notificationJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToOpenNotification(notificationInfo.notification);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidLoadStore(string productsJSON) {
		ProductsInfo productsInfo = JsonMapper.ToObject<ProductsInfo>(productsJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidLoadStore(productsInfo.products);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidFailToLoadStore(string errorJSON) {
		ErrorInfo errorInfo = JsonMapper.ToObject<ErrorInfo>(errorJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToLoadStore(errorInfo.error);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidCompleteProductTransaction(string transactionJSON) {
		TransactionInfo transactionInfo = JsonMapper.ToObject<TransactionInfo>(transactionJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			subscriber.DidCompleteProductTransaction(transactionInfo.transaction);
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidRestoreProductTransaction(string transactionJSON) {
		TransactionInfo transactionInfo = JsonMapper.ToObject<TransactionInfo>(transactionJSON);
		
		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			if (subscriber != null) {
				subscriber.DidRestoreProductTransaction(transactionInfo.transaction);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidFailProductTransaction(string failedTransactionJSON) {
		FailedTransactionInfo failedTransactionInfo = JsonMapper.ToObject<FailedTransactionInfo>(failedTransactionJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailProductTransaction(failedTransactionInfo.transaction, failedTransactionInfo.error);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidStartDownloadContent(string transactionJSON) {
		TransactionInfo transactionInfo = JsonMapper.ToObject<TransactionInfo>(transactionJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidStartDownloadContent(transactionInfo.transaction);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidPauseDownloadContent(string downloadJSON) {
		DownloadInfo downloadInfo = JsonMapper.ToObject<DownloadInfo>(downloadJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidPauseDownloadContent(downloadInfo.download);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidCancelDownloadContent(string downloadJSON) {
		DownloadInfo downloadInfo = JsonMapper.ToObject<DownloadInfo>(downloadJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidCancelDownloadContent(downloadInfo.download);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidReceiveProgressDownloadContent(string downloadJSON) {
		DownloadInfo downloadInfo = JsonMapper.ToObject<DownloadInfo>(downloadJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidReceiveProgressDownloadContent(downloadInfo.download);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidFailDownloadContent(string downloadJSON) {
		DownloadInfo downloadInfo = JsonMapper.ToObject<DownloadInfo>(downloadJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailDownloadContent(downloadInfo.download);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string DidFinishDownloadContent(string downloadJSON) {
		DownloadInfo downloadInfo = JsonMapper.ToObject<DownloadInfo>(downloadJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFinishDownloadContent(downloadInfo.download);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(DelegateCallback))]
	string OnReady(string applicationJSON) {
		NotificareApplicationInfo applicationInfo = JsonMapper.ToObject<NotificareApplicationInfo>(applicationJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.OnReady(applicationInfo.application);
			}
		}
		
		return null;
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidRegisterDevice(string registrationJSON) {
		RegistrationInfo registrationInfo = JsonMapper.ToObject<RegistrationInfo>(registrationJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidRegisterDevice(registrationInfo.registration);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidFailToRegisterDevice(string errorJSON) {
		ErrorInfo errorInfo = JsonMapper.ToObject<ErrorInfo>(errorJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToRegisterDevice(errorInfo.error);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidGetNotification(string notificationJSON) {
		NotificationInfo notificationInfo = JsonMapper.ToObject<NotificationInfo>(notificationJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidGetNotification(notificationInfo.notification);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidFailToGetNotification(string errorJSON) {
		ErrorInfo errorInfo = JsonMapper.ToObject<ErrorInfo>(errorJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToGetNotification(errorInfo.error);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidFetchProducts(string productsJSON) {
		ProductsInfo productsInfo = JsonMapper.ToObject<ProductsInfo>(productsJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFetchProducts(productsInfo.products);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidFailToFetchProducts(string errorJSON) {
		ErrorInfo errorInfo = JsonMapper.ToObject<ErrorInfo>(errorJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToFetchProducts(errorInfo.error);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidFetchPurchasedProducts(string productsJSON) {
		ProductsInfo productsInfo = JsonMapper.ToObject<ProductsInfo>(productsJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFetchPurchasedProducts(productsInfo.products);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidFailToFetchPurchasedProducts(string errorJSON) {
		ErrorInfo errorInfo = JsonMapper.ToObject<ErrorInfo>(errorJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToFetchPurchasedProducts(errorInfo.error);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidFetchProduct(string productJSON) {
		ProductInfo productInfo = JsonMapper.ToObject<ProductInfo>(productJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFetchProduct(productInfo.product);
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(BasicCallback))]
	void DidFailToFetchProduct(string errorJSON) {
		ErrorInfo errorInfo = JsonMapper.ToObject<ErrorInfo>(errorJSON);
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			if (subscriber != null) {
				subscriber.DidFailToFetchProduct(errorInfo.error);
			}
		}
	}

	[DllImport ("__Internal")]
	private static extern bool _isOpen();

	[DllImport ("__Internal")]
	private static extern bool _isFixingGPS();

	[DllImport ("__Internal")]
	private static extern bool _displayMessage();

	[DllImport ("__Internal")]
	private static extern string _notificationQueue();

	[DllImport ("__Internal")]
	private static extern string _activeNotification();

	[DllImport ("__Internal")]
	private static extern void _registerDelegateCallback(string delegateMethod, DelegateCallback callback);
	
	[DllImport ("__Internal")]
	private static extern void _unregisterDelegateCallback(string delegateMethod);

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
	private static extern void _registerDevice(byte[] token, uint tokenLength, BasicCallback registrationBasicCallback, BasicCallback errorBasicCallback);
	
	[DllImport ("__Internal")]
	private static extern void _registerDevice(byte[] token, uint tokenLength, string userID, string username, BasicCallback registrationBasicCallback, BasicCallback errorBasicCallback);

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

	[DllImport ("__Internal")]
	private static extern string _contentPathForProduct(string productIdentifier);

	[DllImport ("__Internal")]
	private static extern string _sdkVersion();

	public bool IsOpen() {
		return _isOpen();
	}

	public bool IsFixingGPS() {
		return _isFixingGPS();
	}

	public bool displayMessage() {
		return _displayMessage();
	}

	public List<Notification> NotificationQueue() {
		string notificationQueueJSON = _notificationQueue();
		NotificationsInfo notificationQueueInfo = JsonMapper.ToObject<NotificationsInfo>(notificationQueueJSON);
		
		return notificationQueueInfo.notifications;
	}

	public Notification ActiveNotification() {
		string notificationJSON = _activeNotification();
		NotificationInfo notificationInfo = JsonMapper.ToObject<NotificationInfo>(notificationJSON);

		return notificationInfo.notification;
	}

	public void Launch() {
		_launch();
	}

	private static Notification UnityNotificationToNotification(RemoteNotification unityNotification) {
		Notification notification = new Notification();
		
		notification.id = (string)unityNotification.userInfo["id"];
		
		notification.aps = new Aps();
		notification.aps.alert = unityNotification.alertBody;
		notification.aps.badge = unityNotification.applicationIconBadgeNumber;
		notification.aps.sound = unityNotification.soundName;

		return notification;
	}

	private IEnumerator CheckForNotifications() {
		int lastNotificationCount = 0;
		
		while(true) {
			int notificationCount = NotificationServices.remoteNotificationCount;

			if (notificationCount > lastNotificationCount) {
				for (int i = lastNotificationCount; i < notificationCount; i++) {
					RemoteNotification unityNotification = NotificationServices.GetRemoteNotification(i);

					Notification notification = UnityNotificationToNotification(unityNotification);

					foreach (NotificareMonoBehaviour subscriber in Subscribers) {
						subscriber.DidReceiveRemoteNotification(notification);
					}
				}
			}

			lastNotificationCount = notificationCount;
			
			yield return new WaitForSeconds(1.0f);
		}
	}

	private IEnumerator WaitForDeviceToken() {
		// Maybe resort to exponential fallback?
		while (NotificationServices.deviceToken == null && NotificationServices.registrationError == null) {
			yield return new WaitForSeconds(0.001f);
		}

		if (NotificationServices.registrationError != null) {
			Debug.LogError(NotificationServices.registrationError);
			
			// Cannot continue
			yield break;
		}
		
		foreach (NotificareMonoBehaviour subscriber in Subscribers) {
			subscriber.DidRegisterForRemoteNotificationsWithDeviceToken(NotificationServices.deviceToken);

			yield return null;
		}
	}

	public void RegisterForNotifications() {
		// Currently using Notificare's registration method
		// If not working, use: "NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);"

		_registerForNotifications();

		StartCoroutine(WaitForDeviceToken());
		StartCoroutine(CheckForNotifications());
	}

	public void RegisterUserNotifications() {
		// Currently using Notificare's registration method
		// If not working, use: "NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);"

		_registerUserNotifications();

		StartCoroutine(WaitForDeviceToken());
		StartCoroutine(CheckForNotifications());
	}

	public bool CheckRemoteNotifications() {
		return _checkRemoteNotifications();
	}

	public void HandleOptions(Dictionary<string, string> options) {
		var optionsInfo = new {options = options};
		string optionsJSON = JsonMapper.ToJson(optionsInfo);

		_handleOptions(optionsJSON);
	}

	public void RegisterDevice(byte[] token) {
		BasicCallback registrationCallback = new BasicCallback(DidRegisterDevice);
		BasicCallback errorCallback = new BasicCallback(DidFailToRegisterDevice);

		_registerDevice(token, (uint)token.Length, registrationCallback, errorCallback);
	}

	public void RegisterDeviceWithUser(byte[] token, string userID, string userName) {
		BasicCallback registrationCallback = new BasicCallback(DidRegisterDevice);
		BasicCallback errorCallback = new BasicCallback(DidFailToRegisterDevice);

		_registerDevice(token, (uint)token.Length, userID, userName, registrationCallback, errorCallback);
	}

	public void UnregisterDevice() {
		_unregisterDevice();
	}

	public void UpdateBadge(int badge) {
		_updateBadge(badge);
	}

	public void OpenNotification(Notification notification) {
		var notificationInfo = new {notification = notification};
		string notificationJSON = JsonMapper.ToJson(notificationInfo);

		_openNotification(notificationJSON);
	}

	public void LogOpenNotification(Notification notification) {
		var notificationInfo = new {notification = notification};

		string notificationJSON = JsonMapper.ToJson(notificationInfo);

		_logOpenNotification(notificationJSON);
	}

	public void GetNotification(string notificationID) {
		BasicCallback notificationCallback = new BasicCallback(DidGetNotification);
		BasicCallback errorCallback = new BasicCallback(DidFailToGetNotification);

		_getNotification(notificationID, notificationCallback, errorCallback);
	}

	public void ClearNotification(string notificationID) {
		_clearNotification(notificationID);
	}

	public void StartLocationUpdates() {
		_startLocationUpdates();
	}

	public bool CheckLocationUpdates() {
		// Isn't this supposed to create a dialog box the first time?
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

	public void FetchProducts() {
		BasicCallback productsCallback = new BasicCallback(DidFetchProducts);
		BasicCallback errorCallback = new BasicCallback(DidFailToFetchProducts);

		_fetchProducts(productsCallback, errorCallback);
	}

	public void FetchPurchasedProducts() {
		BasicCallback productsCallback = new BasicCallback(DidFetchPurchasedProducts);
		BasicCallback errorCallback = new BasicCallback(DidFailToFetchPurchasedProducts);
		
		_fetchPurchasedProducts(productsCallback, errorCallback);
	}

	public void FetchProduct(string productIdentifier) {
		BasicCallback productCallback = new BasicCallback(DidFetchProduct);
		BasicCallback errorCallback = new BasicCallback(DidFailToFetchProduct);

		_fetchProduct(productIdentifier, productCallback, errorCallback);
	}

	public void BuyProduct(NotificareProduct product) {
		var productInfo = new {product = product};
		string productJSON = JsonMapper.ToJson(productInfo);

		_buyProduct(productJSON);
	}

	public void PauseDownloads(List<SKDownload> downloads) {
		var downloadsInfo = new {downloads = downloads};
		string downloadsJSON = JsonMapper.ToJson(downloadsInfo);

		_pauseDownloads(downloadsJSON);
	}

	public void CancelDownloads(List<SKDownload> downloads) {
		var downloadsInfo = new {downloads = downloads};
		string downloadsJSON = JsonMapper.ToJson(downloadsInfo);
		
		_cancelDownloads(downloadsJSON);
	}

	public void ResumeDownloads(List<SKDownload> downloads) {
		var downloadsInfo = new {downloads = downloads};
		string downloadsJSON = JsonMapper.ToJson(downloadsInfo);
		
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

		foreach (NotificareMonoBehaviour subscriber in subscribers) {
			subscriber.OnInit();
		}
	}

	void Start() {
		Subscribers = subscribers;
	}

	void Stop() {
		Subscribers = null;
	}

	void OnApplicationQuit() {
		UnregisterDelegateCallbacks();

		subscribers = null;
		Subscribers = null;
	}
}


