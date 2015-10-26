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
    NSDictionary *info = @{@"error":                    self.error ? self.error.description : nil,
                           @"payment":                  self.payment ? [self.payment toDictionary] : nil,
                           @"transactionState":         [self getTransactionStateString],
                           @"transactionIdentifier":    self.transactionIdentifier,
                           @"transactionDate":          [self.transactionDate isoString],
                           @"downloads":                [self getDictionaryDownloads]};
    
    return info;
}

- (NSString *)getTransactionStateString {
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
            return nil;
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
