
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using IOs.Implementation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(CustomMasterDetailPageRenderer))]
namespace IOs.Implementation
{
    public class CustomMasterDetailPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            CreateNativeToolbarItems();
        }
        private void CreateNativeToolbarItems()
        {
            if (this.NavigationController == null)
                return;
            if (this.NavigationController.TopViewController == null)
                return;
            if (this.NavigationController.TopViewController.NavigationItem == null)
                return;
            if (this.NavigationController.TopViewController.NavigationItem.RightBarButtonItems == null)
                return;
            var rightList = new List<UIBarButtonItem>();
            foreach (var item in this.NavigationController.TopViewController.NavigationItem.RightBarButtonItems)
            {
                if (string.IsNullOrEmpty(item.Title))
                {
                    continue;
                }
                if (item.Title.ToLower() == "search")
                {
                    var newItem = new UIBarButtonItem(UIBarButtonSystemItem.Search)
                    {
                        Action = item.Action,
                        Target = item.Target
                    };
                    rightList.Add(newItem);
                }
                else
                {
                    rightList.Add(item);
                }
            }
            if (rightList.Count > 0)
            {
                this.NavigationController.TopViewController.NavigationItem.RightBarButtonItems = rightList.ToArray();
            }
        }
    }
}
