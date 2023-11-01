using Core.Services;
using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Core.Model
{
    public class AssignmentFilesModel //: INotifyPropertyChanged
    {
        //        public delegate void HandleDelete(bool Deleted);

        //        public event HandleDelete DidDeleted;
        //        public event PropertyChangedEventHandler PropertyChanged;

        //        private string filePath;
        //        private string pathOffline;

        //        public int Id { get; set; }
        //        public string fileName { get; set; }
        //        public int assignmentId { get; set; }
        //        public bool selected { get; set; }

        //        public string FilePath
        //        {
        //            get { return this.filePath; }
        //            set
        //            {
        //                if (this.filePath != value)
        //                {
        //                    this.filePath = value;
        //                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
        //                    Photo = EvaluarPath(filePath);
        //                }
        //            }
        //        }

        //        public string PathOffline
        //        {
        //            get { return this.pathOffline; }
        //            set
        //            {
        //                if (this.pathOffline != value)
        //                {
        //                    this.pathOffline = value;
        //                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PathOffline)));
        //                    Photo = EvaluarPath(pathOffline);
        //                }
        //            }
        //        }

        //        private ImageSource photo;

        //        public ImageSource Photo
        //        {
        //            get { return this.photo; }
        //            set
        //            {
        //                if (this.photo != value)
        //                {
        //                    this.photo = value;
        //                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Photo)));
        //                }
        //            }
        //        }

        //        public string fileOnString { get; set; }

        //        private ImageSource EvaluarPath(string path)
        //        {
        //            if (this.fileName.ToLower().Contains("png") || this.fileName.ToLower().Contains("jpg") ||
        //                this.fileName.ToLower().Contains("jpeg"))
        //            {
        //                if (!string.IsNullOrWhiteSpace(path))
        //                {
        //                    if (path.Contains("blob"))
        //                    {
        //                        var i = path.Replace("\\", @"/");
        //                        return ImageSource.FromUri(new Uri(i));
        //                    }
        //                    else if (path.Substring(0, 1) == "h" && path.Substring(0, 5) != "https")
        //                    {
        //                        var r = path.Replace("http", "https");

        //                        return ImageSource.FromUri(new Uri(r));
        //                    }
        //                    else
        //                    {
        //                        var imagen =
        //                            Xamarin.Forms.ImageSource.FromStream(() =>
        //                                new MemoryStream(Convert.FromBase64String(path)));

        //                        return imagen;
        //                    }
        //                }

        //                return $"{fileName.Split('.').LastOrDefault()}.png";
        //            }
        //            else
        //            {
        //                return $"{fileName.Split('.').LastOrDefault()}.png";
        //            }
        //        }

        //        public string mime { get; set; }
        //        public int width { get; set; }
        //        public int height { get; set; }
        //        public DateTime uploadDate { get; set; }
        //        public int? position { get; set; }

        //        private bool edit;

        //        public bool isEdited
        //        {
        //            get { return this.edit; }
        //            set
        //            {
        //                if (this.edit != value)
        //                {
        //                    this.edit = value;
        //                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isEdited)));
        //                }
        //            }
        //        }

        //        //#region COMANDOS
        //        //private async void Descargar()
        //        //{
        //        //    try
        //        //    {
        //        //        var red = await Connection.SeeConnection();

        //        //        if (red)
        //        //        {
        //        //            var cameraService = new DescargarViewModel();
        //        //            await cameraService.DescargarArchivo(this.FilePath, this.fileName);
        //        //            //var s = await cameraService.GuardarImagen(this);

        //        //            //if (s != string.Empty)
        //        //            //{
        //        //            await App.Current.MainPage.DisplayAlert("Archivo Descargado", $"El archivo se ha guardado en: ", "OK");
        //        //            //};
        //        //        }
        //        //        else
        //        //        {
        //        //            await App.Current.MainPage.DisplayAlert(
        //        //            Views.LangResources.AppResource.Atention,
        //        //            Views.LangResources.AppResource.AlertNoOffline,
        //        //            Views.LangResources.AppResource.Acept);
        //        //            return;
        //        //        }
        //        //    }
        //        //    catch (Exception e)
        //        //    {
        //        //        await App.Current.MainPage.DisplayAlert(
        //        //            "Error",
        //        //            $"{e.Message}",
        //        //            "Aceptar");
        //        //        return;
        //        //    }
        //        //}

        //        //public ICommand VerImagenCommand
        //        //{
        //        //    get
        //        //    {
        //        //        return new RelayCommand(VerImagen);
        //        //    }
        //        //}
        //        //private void VerImagen()
        //        //{
        //        //    if (this.fileName.ToLower().Contains("png") || this.fileName.ToLower().Contains("jpg") || this.fileName.ToLower().Contains("jpeg"))
        //        //    {
        //        //        var pg = new ViewImagePage(this, this.fileName);

        //        //        App.Current.MainPage.Navigation.PushAsync(pg);
        //        //    }
        //        //}

        //        //private void EditarImagen()
        //        //{
        //        //    var pg = new EditImagePage(this, this.fileName);

        //        //    pg.DidUpdated += Pg_DidUpdated;

        //        //    App.Current.MainPage.Navigation.PushAsync(pg);
        //        //}

        //        //private void Pg_DidUpdated(string file, int id, bool? edit = false)
        //        //{
        //        //    if (edit.Value)
        //        //    {
        //        //        this.isEdited = true;
        //        //    }
        //        //    if (id > 0)
        //        //    {
        //        //        this.Id = id;
        //        //    }

        //        //    this.FilePath = file;
        //        //    this.fileOnString = file;
        //        //}
        //        //public string Name { get; set; }

        //        //private async void EliminarFile()
        //        //{
        //        //    try
        //        //    {
        //        //        var red = await Connection.SeeConnection();

        //        //        if (red)
        //        //        {
        //        //            this.Photo = null;
        //        //            this.Name = this.fileName;
        //        //            var s = await ServiceWeb<AssignmentFilesModel>.Agregar("Assignment/DeleteFileFromAssignment", this, Identidad.Token);
        //        //            DidDeleted(true);
        //        //        }
        //        //        else
        //        //        {
        //        //            await App.Current.MainPage.DisplayAlert(
        //        //            Views.LangResources.AppResource.Atention,
        //        //            Views.LangResources.AppResource.AlertNoOffline,
        //        //            Views.LangResources.AppResource.Acept);
        //        //            return;
        //        //        }
        //        //    }
        //        //    catch (Exception e)
        //        //    {
        //        //        await App.Current.MainPage.DisplayAlert(
        //        //            "Error",
        //        //            $"{e.Message}",
        //        //            "Aceptar");
        //        //        return;
        //        //    }
        //        //}

        //        //public ICommand FileCommand
        //        //{
        //        //    get
        //        //    {
        //        //        return new RelayCommand(File);
        //        //    }
        //        //}

        //        //private async void File()
        //        //{
        //        //    try
        //        //    {
        //        //        var pg = new SubMenuPopupPage(fileName.Split('.').LastOrDefault());

        //        //        pg.MenuType += Pg_MenuType;

        //        //        await App.Current.MainPage.Navigation.PushPopupAsync(pg);
        //        //    }
        //        //    catch (Exception e)
        //        //    {
        //        //        await App.Current.MainPage.DisplayAlert(
        //        //            "Error",
        //        //            $"{e.Message}",
        //        //            "Aceptar");
        //        //        return;
        //        //    }
        //        //}

        //        //private void Pg_MenuType(int type)
        //        //{
        //        //    switch (type)
        //        //    {
        //        //        case 1:
        //        //            EditarImagen();
        //        //            break;
        //        //        case 2:
        //        //            Descargar();
        //        //            break;
        //        //        case 3:
        //        //            EliminarFile();
        //        //            break;
        //        //    }
        //        //}

        //        //#endregion
    }
}
