using Foundation;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using System;
using System.IO;
using UI;
using UIKit;

namespace IOs
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
    
        //[Export("window")]
        //public UIWindow Window { get; set; }

        //[Export ("application:didFinishLaunchingWithOptions:")]
        public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            //return true;

            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new FormsApp());
            DownLoaded();
            return base.FinishedLaunching(application, launchOptions);
        }
        public void DownLoaded()
        {
            CrossDownloadManager.Current.PathNameForDownloadedFile = new Func<IDownloadFile, string>
                (file =>
                {

                    string fileName = (new NSUrl(file.Url, false)).LastPathComponent;
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
                });
        }

        // UISceneSession Lifecycle

        [Export ("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create ("Default Configuration", connectingSceneSession.Role);
        }

        [Export ("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions (UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }


    }
}

