//
//  SKPaymentTransaction+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 23-10-2015.
//
//

#import "SKPaymentTransaction+Dictionary.h"

#import "SKPayment+Dictionary.h"
#import "SKDownload+Dictionary.h"
#import "NSDate+ISOString.h"

@implementation SKPaymentTransaction (Dictionary)

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"error":                    self.error                  ? self.error.description            : [NSNull null],
                           @"payment":                  self.payment                ? [self.payment toDictionary]       : [NSNull null],
                           @"transactionState":         [self getTransactionStateObject],
                           @"transactionIdentifier":    self.transactionIdentifier  ? self.transactionIdentifier        : [NSNull null],
                           @"transactionDate":          self.transactionDate        ? [self.transactionDate isoString]  : [NSNull null],
                           @"downloads":                [self getDictionaryDownloads]};
    
    return info;
}

- (id)getTransactionStateObject {
    switch (self.transactionState) {
        case SKPaymentTransactionStatePurchasing:
            return @"SKPaymentTransactionStatePurchasing";
            break;
            
        case SKPaymentTransactionStatePurchased:
            return @"SKPaymentTransactionStatePurchased";
            break;
            
        case SKPaymentTransactionStateFailed:
            return @"SKPaymentTransactionStateFailed";
            break;
            
        case SKPaymentTransactionStateRestored:
            return @"SKPaymentTransactionStateRestored";
            break;
            
        case SKPaymentTransactionStateDeferred:
            return @"SKPaymentTransactionStateDeffered";
            break;
            
        default:
            return [NSNull null];
            break;
    }
}

- (NSArray *)getDictionaryDownloads {
    NSMutableArray *dictionaryDownloads = [NSMutableArray array];
    
    for (SKDownload *download in self.downloads) {
        [dictionaryDownloads addObject:[download toDictionary]];
    }
    
    return [dictionaryDownloads copy];
}

@end
