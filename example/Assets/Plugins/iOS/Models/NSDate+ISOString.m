//
//  NSDate+ISOString.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 23-10-2015.
//
//

#import "NSDate+ISOString.h"

@implementation NSDate (ISOString)

- (NSString *)isoString {
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    dateFormatter.dateFormat = @"yyyy-MM-dd'T'HH:mm:ssZZZZZ";
    
    return [dateFormatter stringFromDate:self];
}

@end
