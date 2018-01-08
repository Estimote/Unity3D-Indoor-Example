#import "EILIndoorSDK.h"
#import <EstimoteSDK/ESTConfig.h>
#import <Foundation/Foundation.h>

@interface UnityBridge : NSObject <EILIndoorLocationManagerDelegate>

@property (nonatomic, readonly) EILIndoorLocationManager *indoorManager;

@property (nonatomic, readonly) bool isInsideLocation;
@property (nonatomic, readonly) double lastKnownX;
@property (nonatomic, readonly) double lastKnownY;

@end

@implementation UnityBridge

- (id)init {
    self = [super init];

    _indoorManager = [EILIndoorLocationManager new];
    _indoorManager.delegate = self;

    return self;
}

-    (void)indoorLocationManager:(EILIndoorLocationManager *)manager
didFailToUpdatePositionWithError:(NSError *)error {
    NSLog(@"failed to update position: %@", error);

    if (error.code == EILIndoorPositionOutsideLocationError) {
        _isInsideLocation = false;
    }
}

- (void)indoorLocationManager:(EILIndoorLocationManager *)manager
            didUpdatePosition:(EILOrientedPoint *)position
                 withAccuracy:(EILPositionAccuracy)positionAccuracy
                   inLocation:(EILLocation *)location {
    NSString *accuracy;
    switch (positionAccuracy) {
        case EILPositionAccuracyVeryHigh: accuracy = @"+/- 1.00m"; break;
        case EILPositionAccuracyHigh:     accuracy = @"+/- 1.62m"; break;
        case EILPositionAccuracyMedium:   accuracy = @"+/- 2.62m"; break;
        case EILPositionAccuracyLow:      accuracy = @"+/- 4.24m"; break;
        case EILPositionAccuracyVeryLow:  accuracy = @"+/- ? :-("; break;
        case EILPositionAccuracyUnknown:  accuracy = @"unknown"; break;
    }
    NSLog(@"x: %5.2f, y: %5.2f, orientation: %3.0f, accuracy: %@",
          position.x, position.y, position.orientation, accuracy);

    _isInsideLocation = true;
    _lastKnownX = position.x;
    _lastKnownY = position.y;
}

@end

static UnityBridge *unityBridge = nil;

extern "C" {

    void _Start(const char *appId, const char *appToken, const char *locationId) {
        if (unityBridge == nil) {
            unityBridge = [UnityBridge new];
        } else {
            [unityBridge.indoorManager stopPositionUpdates];
        }

        [ESTConfig setupAppID:[NSString stringWithUTF8String:appId]
                  andAppToken:[NSString stringWithUTF8String:appToken]];

        EILRequestFetchLocation *fetchLocationRequest = [[EILRequestFetchLocation alloc]
                                                         initWithLocationIdentifier:[NSString stringWithUTF8String:locationId]];
        [fetchLocationRequest sendRequestWithCompletion:^(EILLocation *location, NSError *error) {
            if (location != nil) {
                [unityBridge.indoorManager startPositionUpdatesForLocation:location];
            } else {
                NSLog(@"can't fetch location: %@", error);
            }
        }];
    }

    bool _IsInsideLocation() {
        return unityBridge.isInsideLocation;
    }

    double _GetX() {
        return unityBridge.lastKnownX;
    }

    double _GetY() {
        return unityBridge.lastKnownY;
    }

}
