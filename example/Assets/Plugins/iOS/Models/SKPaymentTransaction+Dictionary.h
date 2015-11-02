//
//  SKPaymentTransaction+Dictionary.h
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 23-10-2015.
//
//

#import <StoreKit/StoreKit.h>

@interface SKPaymentTransaction (Dictionary)

- (NSDictionary *)toDictionary;

@end
