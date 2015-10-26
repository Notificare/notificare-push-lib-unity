//
//  NotificareProduct+Dictionary.h
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 23-10-2015.
//
//

#import "NotificareProduct.h"

@interface NotificareProduct (Dictionary)

- (instancetype)initWithDictionary:(NSDictionary *)info;
- (NSDictionary *)toDictionary;

@end
