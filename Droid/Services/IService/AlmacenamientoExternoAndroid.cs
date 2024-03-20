using Core.Services.Contracts;
using PrometeoCrmMobile.Droid.Services.IService;
using System.IO;


[assembly: Xamarin.Forms.Dependency(typeof(AlmacenamientoExternoAndroid))]
namespace PrometeoCrmMobile.Droid.Services.IService
{
    public class AlmacenamientoExternoAndroid : IAlmacenamientoExterno
    {
        public string ObtenerRutaAlmacenamientoExterno(string nombreArchivo)
        {
            var rutaArchivo = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, nombreArchivo);
            return rutaArchivo;
        }
    }
}