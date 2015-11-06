//
//  NotificareProduct+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 23-10-2015.
//
//

#import "NotificareProduct+Dictionary.h"

#import "SKDownload+Dictionary.h"

@implementation NotificareProduct (Dictionary)

- (instancetype)initWithDictionary:(NSDictionary *)info {
    self = [super init];
    
    if (self) {
        self.productName        = info[@"productName"];
        self.productDescription = info[@"productDescription"];
        self.type               = info[@"type"];
        self.application        = info[@"application"];
        self.identifier         = info[@"identifier"];
        // stores?
        // downloads, maybe restore them from paymentQueue?
        self.date               = info[@"date"];
        self.priceLocale        = info[@"priceLocale"];
        self.price              = info[@"price"];
        self.currency           = info[@"currency"];
        self.active             = [info[@"active"] boolValue];
        self.purchased          = [info[@"purchased"] boolValue];
    }
    
    return self;
}

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"productName":         self.productName         ? self.productName          : [NSNull null],
                           @"productDescription":  self.productDescription  ? self.productDescription   : [NSNull null],
                           @"type":                self.type                ? self.type                 : [NSNull null],
                           @"application":         self.application         ? self.application          : [NSNull null],
                           @"identifier":          self.identifier,
                           // stores?
                           @"downloads":           [self getDictionaryDownloads],
                           @"date":                self.date                ? self.date                 : [NSNull null],
                           @"priceLocale":         self.priceLocale         ? self.priceLocale          : [NSNull null],
                           @"price":               self.price               ? self.price                : [NSNull null],
                           @"currency":            self.currency            ?self.currency              : [NSNull null],
                           @"active":              [NSNumber numberWithBool:self.active],
                           @"purchased":           [NSNumber numberWithBool:self.purchased]};
    
    return info;
}

- (NSArray *)getDictionaryDownloads {
    NSMutableArray *dictionaryDownloads = [NSMutableArray array];
    
    for (SKDownload *download in self.downloads) {
        [dictionaryDownloads addObject:[download toDictionary]];
    }
    
    return [dictionaryDownloads copy];
}

@end
