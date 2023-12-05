using Core.Model;
using Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UI.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;

namespace UI.Pages
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, NoHistory = false)]
    public partial class CreateOrderPage : MvxContentPage<CreateOrderViewModel>
    {
        public CreateOrderPage()
        {
            InitializeComponent();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.ShowEditProductPopup += OnShowEditProductPopup;
            ViewModel.ShowAddressPopup += OnShowCustomerAddressPopup;
            ViewModel.ShowConfirmPopup += OnShowConfirmPopup;
        }

        private async void OnShowConfirmPopup(object sender, OrderNote e)
        {
            var popup = new ConfirmOrderNotePopupPage();
            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);

                if (args.confirmed && args.notConfirmed == false)
                {
                    await ViewModel.ConfirmaOrderNote(e);
                }
            };

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void OnShowCustomerAddressPopup(object sender, List<CustomerAddress> addresses)
        {
            var popup = new CustomerAddressPopup(addresses);

            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);

                if (!string.IsNullOrWhiteSpace(args))
                {
                    editorAddress.Text = args;
                }
            };

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void OnShowEditProductPopup(object sender, Product product)
        {
            var popup = new SelectProductPopup(product, true);

            popup.OkTapped += async (s, args) =>
            {
                await PopupNavigation.Instance.PopAsync(false);
                ViewModel.FinishEditProduct(args);
            };

            popup.Dismissed += (s, args) => ViewModel.CancelEditProduct();

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private void Entry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            try
            {
                var idioma = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                ViewModel.ResetTotal(ViewModel.Order.products);

                if (string.IsNullOrWhiteSpace(lblOrderDiscount.Text) || ViewModel.OrderDiscount == 0)
                {
                }

                if (ViewModel.OrderDiscount > 0)
                {
                    ViewModel.ValorDescuento = ViewModel.Total * ViewModel.OrderDiscount / 100;

                    ViewModel.ActualizarTotal(ViewModel.Order.products);
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        private void TapInfo_Tapped(object sender, EventArgs e)
        {
            btnProductos.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblProductos.TextColor = Xamarin.Forms.Color.White;

            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblAdjuntos.TextColor = Xamarin.Forms.Color.White;

            btnInfo.BackgroundColor = Xamarin.Forms.Color.White;
            lblInfo.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = true;
            ViewModel.StackProductos = false;
            ViewModel.StackAdjunto = false;
        }

        private void TapProducts_Tapped(object sender, EventArgs e)
        {
            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblAdjuntos.TextColor = Xamarin.Forms.Color.White;

            btnInfo.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblInfo.TextColor = Xamarin.Forms.Color.White;

            btnProductos.BackgroundColor = Xamarin.Forms.Color.White;
            lblProductos.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = false;
            ViewModel.StackProductos = true;
            ViewModel.StackAdjunto = false;
        }

        private void TapAdjuntos_Tapped(object sender, EventArgs e)
        {
            btnInfo.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblInfo.TextColor = Xamarin.Forms.Color.White;

            btnProductos.BackgroundColor = Xamarin.Forms.Color.FromHex("#2B0048");
            lblProductos.TextColor = Xamarin.Forms.Color.White;

            btnAdjunto.BackgroundColor = Xamarin.Forms.Color.White;
            lblAdjuntos.TextColor = Xamarin.Forms.Color.FromHex("#2B0048");

            ViewModel.StackInfo = false;
            ViewModel.StackProductos = false;
            ViewModel.StackAdjunto = true;
        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            cmbStatusOrderNote.Focus();
        }

        private void cmbStatusOrderNote_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (Picker)sender;

            ViewModel.Order.StatusOrderNote = cmb.SelectedItem as StatusOrderNote;
            ViewModel.Order.OrderStatus = (cmb.SelectedItem as StatusOrderNote).Id;
        }

        private async void ImageButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                string action = await DisplayActionSheet(null, null, null, LangResources.AppResources.Camera, LangResources.AppResources.Galery);

                if (action != null && action == LangResources.AppResources.Galery)
                {
                    var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    if (status == PermissionStatus.Granted)
                    {
                        var files = PickAndShow(default).ContinueWith(
                            (task) => { });
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            LangResources.AppResources.Attention,
                            LangResources.AppResources.NoPermissionsToFiles,
                            LangResources.AppResources.Accept);
                        return;
                    }
                }
                else
                if (action != null && action == LangResources.AppResources.Camera)
                {
                    var cameraOK = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera);
                    var storageOK = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);

                    var FilePath = string.Empty;
                    var FileResource = string.Empty;

                    var file = await TakePhoto(cameraOK, storageOK) as MediaFile;

                    if (file != null)
                    {
                        var Photo = ImageSource.FromStream(() => file.GetStream());

                        if (Photo != null)
                        {
                            FilePath = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}-{DateTime.Now.Hour}{DateTime.Now.Minute}.jpg";
                            FileResource = Convert.ToBase64String(ImageSourceToByteArray(Photo));

                            ViewModel.AddPictureToOrderNote(FilePath, FileResource);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", $"{exception.Message}", "aceptar"); return;
            }
        }

        private async Task<object> TakePhoto(Plugin.Permissions.Abstractions.PermissionStatus a, Plugin.Permissions.Abstractions.PermissionStatus b)
        {
            if (a == Plugin.Permissions.Abstractions.PermissionStatus.Granted
                && b == Plugin.Permissions.Abstractions.PermissionStatus.Granted
                && CrossMedia.Current.IsCameraAvailable
                && CrossMedia.Current.IsTakePhotoSupported)
            {
                var options = new StoreCameraMediaOptions
                {
                    DefaultCamera = CameraDevice.Rear, // Doesn't always work on Android, depends on Device
                    AllowCropping = true, // UWP & iOS only,
                    PhotoSize = PhotoSize.Medium, // if Custom, you can set CustomPhotoSize = percentage_value 
                    CompressionQuality = 90,
                    Directory = "Prometeo",
                    Name = $"{Guid.NewGuid()}.jpg",
                    SaveToAlbum = false,
                };
                var file = await CrossMedia.Current.TakePhotoAsync(options);

                if (file == null)
                {
                    return null;
                }

                return file;
            }
            else
            {
                return new Exception("No se pudo acceder a la camara, revise de tener los permisos activados");
            }
        }

        async Task PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickMultipleAsync(options); //obtengo lista de imagenes 

                if (result != null && result.Any())
                {
                    ViewModel.AddFileToOrderNote(result);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public byte[] ImageSourceToByteArray(ImageSource source)
        {
            StreamImageSource streamImageSource = (StreamImageSource)source;
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
            Task<Stream> task = streamImageSource.Stream(cancellationToken);
            Stream stream = task.Result;

            byte[] b;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                b = ms.ToArray();
            }

            return b;
        }
    }
}