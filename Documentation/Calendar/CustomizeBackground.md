User Story: Apply Purchased Wallpaper to Windows Desktop (Test Workflow)

As a user on Windows 10/11 I want a purchased wallpaper to be applied to my desktop through the native app So that the purchase-to-desktop-apply pipeline can be verified end to end

Scope

Windows 10/11 only. macOS, Mobile, and Linux are out of scope (see separate future-OS file).
In-app background apply, private chat background apply, and website/live-state triggers are out of scope for this test.
CSAM/moderation checks are assumed already passed — this test uses a pre-approved fixture wallpaper, not a fresh upload.

Preconditions

Test user account exists and can authenticate.
Native app is installed on a Windows 10 or Windows 11 test machine.
At least one moderation-approved wallpaper exists in the test catalog and is available for purchase.
Native app can reach the server (network available).

Acceptance Criteria

User can view and select a purchasable wallpaper (web or native UI).
On purchase, a server-side ownership record is created for the user and item.
Native app detects the new ownership record, whether it was running at purchase time or not.
Native app downloads the purchased wallpaper file locally.
Native app sets the downloaded file as the Windows desktop wallpaper.
Desktop wallpaper visibly changes to the purchased file.
If the native app was closed at purchase time, the wallpaper is applied automatically the next time the app launches (no manual step required from the user).

Test Steps — Happy Path

Log into the native app on the Windows test machine with the test account.
Confirm the wallpaper catalog loads and contains at least one approved test item.
Purchase the test item.
Verify the server has created an ownership record for the test account and item.
Verify the native app detects the new owned item (check app log or internal event).
Verify the native app downloads the wallpaper file to local storage.
Verify the Windows desktop wallpaper changes to the downloaded file.
Close the native app. Purchase a second test item. Relaunch the native app. Verify the pending item is applied automatically on launch without manual action.

Implementation Notes

Windows wallpaper is set via the SystemParametersInfo API (SPI_SETDESKWALLPAPER), or a .NET wrapper around it.
Pending-apply state is a flag or queue tied to the ownership record, checked by the native app on startup and on reconnect — this covers the "app wasn't running" case in step 8.
Verification of the wallpaper change can be automated by reading the registry value for the current desktop wallpaper path (HKEY_CURRENT_USER\Control Panel\Desktop\Wallpaper) after the apply step, or by checking the return value of the SystemParametersInfo call.
No UI automation is required for the desktop-apply verification step — checking the registry/API result is sufficient and simpler than screen comparison.

