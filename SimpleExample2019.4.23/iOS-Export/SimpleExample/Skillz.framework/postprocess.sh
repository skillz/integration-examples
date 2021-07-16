/usr/bin/perl -x "$0"
exit

#!perl start here
use strict;

$ENV{PATH} = $ENV{PATH} . ':/usr/libexec';

my $BUILT_PRODUCTS_DIR = $ENV{'BUILT_PRODUCTS_DIR'};
my $INFOPLIST_PATH = $ENV{'INFOPLIST_PATH'};
my $CODE_SIGN_ENTITLEMENTS = $ENV{'CODE_SIGN_ENTITLEMENTS'};

# escape some troublesome path characters
$BUILT_PRODUCTS_DIR =~ s!'!\'!g;
$BUILT_PRODUCTS_DIR =~ s!"!\"!g;
$BUILT_PRODUCTS_DIR =~ s! !\ !g;

$INFOPLIST_PATH =~ s!'!\'!g;
$INFOPLIST_PATH =~ s!"!\"!g;
$INFOPLIST_PATH =~ s! !\ !g;

print "Running Skillz SDK post processing script" . "\n";

print "Built Products Path: " . $BUILT_PRODUCTS_DIR . "\n";
print "Info.plist Path: " . $INFOPLIST_PATH . "\n";

# Get platform version, warn the developer if it is below the suggested Platform Version

my $dtPlatformVersion = `PlistBuddy -c \'Print DTPlatformVersion\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
if( $dtPlatformVersion < 12 ) {
   print "warning: Skillz suggests Platform Version 12.0 or above. Your Platform Version is: " . $dtPlatformVersion ;
}

# Add Plist value to properly inform user of Location Request for iOS 8

my $locationInUse = `PlistBuddy -c \'Print NSLocationWhenInUseUsageDescription\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

if (!length($locationInUse)) {
   `PlistBuddy -c \'Add :NSLocationWhenInUseUsageDescription string \"Due to legal requirements we require your location in games that can be played for cash.\"\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}

my $locationAlways = `PlistBuddy -c \'Print NSLocationAlwaysUsageDescription\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

if (!length($locationAlways)) {
`PlistBuddy -c \'Add :NSLocationAlwaysUsageDescription string \"Due to legal requirements we require your location in games that can be played for cash.\"\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}


# Add Plist value to properly inform user of camera roll usage for iOS 11

my $photoLibraryUsage = `PlistBuddy -c \'Print NSPhotoLibraryAddUsageDescription\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

if (!length($photoLibraryUsage)) {
   `PlistBuddy -c \'Add :NSPhotoLibraryAddUsageDescription string \"This allows you to save photos to your camera roll.\"\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}

my $contactsInUse = `PlistBuddy -c \'Print NSContactsUsageDescription\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

if (!length($contactsInUse)) {
    `PlistBuddy -c \'Add :NSContactsUsageDescription string \"Your contacts will be uploaded to the Skillz servers so you can easily find friends to chat and play.\"\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}

my $photosInUse = `PlistBuddy -c \'Print NSPhotoLibraryUsageDescription\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

if (!length($photosInUse)) {
`PlistBuddy -c \'Add :NSPhotoLibraryUsageDescription string \"Skillz uses access to your photo album to customize your profile picture.\"\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}

my $cameraInInUse = `PlistBuddy -c \'Print NSCameraUsageDescription\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

if (!length($photosInUse)) {
`PlistBuddy -c \'Add :NSCameraUsageDescription string \"Skillz can access your camera to customize your profile picture.\"\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}

# Add Plist value to respect view controller status bar appearance
`PlistBuddy -c \'Delete :UIViewControllerBasedStatusBarAppearance\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
`PlistBuddy -c \'Add :UIViewControllerBasedStatusBarAppearance bool YES\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

# Add Plist value to require full screen
`PlistBuddy -c \'Delete :UIRequiresFullScreen\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
`PlistBuddy -c \'Add :UIRequiresFullScreen bool YES\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

# Add Plist value to use Light theme
my $UIUserInterfaceStyleExists = `PlistBuddy -c \'Print UIUserInterfaceStyle\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
if (length($UIUserInterfaceStyleExists)) {
    print "Note: Due to the Skillz theming engine, we currently do not support the automatic Dark Mode UI adjustments made by iOS, and are updating your Info.plist to reflect this.\n";
    `PlistBuddy -c \'Delete :UIUserInterfaceStyle\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}
`PlistBuddy -c \'Add :UIUserInterfaceStyle string Light\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

# Add Custom URL Scheme unique to your game.
my $bundleId = `PlistBuddy -c \"Print CFBundleIdentifier\" "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
$bundleId =~ s/\R//g;
my $customURLScheme = "skillz.gamelinks." . $bundleId;
print "Custom url scheme: $customURLScheme \n";

my $customPaymentsURLScheme = $bundleId . ".skillz.gamelinks.payments";
print "Custom payments url scheme: $customPaymentsURLScheme \n";

my $bundleTypes = `PlistBuddy -c \'Print CFBundleURLTypes\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

if (!length($bundleTypes)) {
    print "CFBundleURLTypes does not yet exist, create it.\n";
    `PlistBuddy -c \'Add :CFBundleURLTypes array\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleURLTypes: dict\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \"Add :CFBundleURLTypes:0:CFBundleURLName string ${customURLScheme}\" \"${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}\"`;
    `PlistBuddy -c \"Add :CFBundleURLTypes:0:CFBundleURLName string ${customPaymentsURLScheme}\" \"${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}\"`;
    `PlistBuddy -c \'Add :CFBundleURLTypes:0:CFBundleURLSchemes array\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \"Add :CFBundleURLTypes:0:CFBundleURLSchemes: string "${customURLScheme}"\" \"${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}\"`;
     `PlistBuddy -c \"Add :CFBundleURLTypes:0:CFBundleURLSchemes: string "${customPaymentsURLScheme}"\" \"${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}\"`;
} else {
    sub add_scheme_if_needed
    {
        my $scheme = $_[0];
        print "CFBundleURLTypes exists, check if we should add Skillz scheme: $scheme. \n";
        # Only add our custom scheme if it does not yet exist
        my $customScheme = `grep "$scheme" "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

        if (!length($customScheme)) {
            print "Skillz URL scheme $scheme not yet set \n";
            my $temporaryPlistDirectory = `dirname "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
            $temporaryPlistDirectory =~ s/\R//g;
            my $tempPlistName = "/urlScheme.plist";
            my $temporaryPlistPath = $temporaryPlistDirectory . $tempPlistName;

            unlink "$temporaryPlistPath";

            my $fileContents = "
                        <array>
                            <dict>
                                <key>CFBundleURLName</key>
                                    <string>${scheme}</string>
                                <key>CFBundleURLSchemes</key>
                                <array>
                                    <string>${scheme}</string>
                                </array>
                            </dict>
                        </array>";

            open(my $fh, '>', "$temporaryPlistPath");
            print "$fh" . "$fileContents" . "\n";
            print $fh $fileContents;
            close $fh;

            `PlistBuddy -c \'Merge "$temporaryPlistPath" :CFBundleURLTypes:\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

            unlink "$temporaryPlistPath";

        } else {
            print "Skillz URL scheme $scheme already set \n";
        }
    }
    add_scheme_if_needed($customURLScheme);
    add_scheme_if_needed($customPaymentsURLScheme);
}

# Add fetch background mode
if (system("PlistBuddy -c \'Print :UIBackgroundModes\' " . "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH")) {
    print "Adding background modes";

    `PlistBuddy -c \'Add :UIBackgroundModes array\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}

if (`PlistBuddy -c \'Print :UIBackgroundModes\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"` !~ /fetch/) {
    print "Adding fetch background mode";
    `PlistBuddy -c \'Add :UIBackgroundModes: string fetch\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;
}

# Set up localization.
my $localizations = `PlistBuddy -c \'Print CFBundleLocalizations:\' "$BUILT_PRODUCTS_DIR/$INFOPLIST_PATH"`;

if (!length($localizations)) {

    `PlistBuddy -c \'Add :CFBundleLocalizations array\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:0 string en\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:1 string de\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:2 string es\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:3 string fr\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:4 string it\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:5 string ja\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:6 string pt\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:7 string ru\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
    `PlistBuddy -c \'Add :CFBundleLocalizations:8 string zh-Hans\' "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;
}

my $skillzDict =  `PlistBuddy -c \'Print :skillzSDK:environment\'  "${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"`;

if ($skillzDict =~ /Production/i && $ENV{'CONFIGURATION'} =~ /Debug/i) {
  print "Debug Configuration is used for  Production build";
  exit 1;
}

my $entitlements = `PlistBuddy -c \'Print :com.apple.developer.associated-domains\' "${CODE_SIGN_ENTITLEMENTS}"`;

if (!length($entitlements)) {
     `PlistBuddy -c \"Add :com.apple.developer.associated-domains array\" $CODE_SIGN_ENTITLEMENTS`;

    `PlistBuddy -c \"Add :com.apple.developer.associated-domains:0 string \'applinks:dl.skillz.com\'\" "${CODE_SIGN_ENTITLEMENTS}"`;
 
    `PlistBuddy -c \"Add :com.apple.developer.associated-domains:0 string \'webcredentials:skillz.com\'\" "${CODE_SIGN_ENTITLEMENTS}"`;
}

my $codeSignIdentity =  $ENV{'EXPANDED_CODE_SIGN_IDENTITY'};
my $dylib = "$BUILT_PRODUCTS_DIR/" . $ENV{'FRAMEWORKS_FOLDER_PATH'} . "/Skillz.framework/Skillz";
my $signingPath = "$BUILT_PRODUCTS_DIR/" . $ENV{'FRAMEWORKS_FOLDER_PATH'} . "/Skillz.framework";
my $shouldSign = ($codeSignIdentity ne "") && (defined $codeSignIdentity) && -e "$signingPath";

print "Code sign identity: $codeSignIdentity \n";
print "Signing Path: $signingPath \n";

if ( $ENV{'DEPLOYMENT_LOCATION'} eq "YES") {
    my $fileResult = `file "$dylib"`;
    if (index($fileResult, "i386") != -1) {
        print "Exporting for release, remove unused archs. \n";
        my $tempfile = `mktemp -t skillz`;
        `/usr/bin/lipo -output "$tempfile" -remove i386 -remove x86_64 "$dylib"`;
        `unlink "$dylib"`;
        `mv "$tempfile" "$dylib"`;
        print "Arch i386 found, removed \n";
    } else {
        print "Arch i386 not found \n";
    }
    print "Archiving or building for release, remove self \n";
    `rm "$0"`;

    if ($shouldSign) {
        print "Signing Skillz \n";
        `/usr/bin/codesign --force --verbose --sign "$codeSignIdentity" --preserve-metadata=identifier,entitlements,resource-rules "$signingPath"`;
    } else {
        print "No identity or no framework, not signing \n";
    }
} else {
    if ($shouldSign) {
        print "Signing Skillz \n";
        `/usr/bin/codesign --force --verbose --sign "$codeSignIdentity" --preserve-metadata=identifier,entitlements,resource-rules "$signingPath"`;
    } else {
        print "No identity or no framework, not signing \n";
    }
}
