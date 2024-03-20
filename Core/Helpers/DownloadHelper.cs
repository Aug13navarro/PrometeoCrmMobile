using Core.Services.Contracts;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core.Helpers
{
    public class DownloadHelper
    {
        public async Task DescargarArchivo(string url, string nombreArchivo)
        {
            using (var httpClient = new HttpClient())
            {
                // Descargar el archivo como un arreglo de bytes
                var archivoBytes = await httpClient.GetByteArrayAsync(url);

                // Obtener la ruta de almacenamiento dependiendo de la plataforma
                var rutaArchivo = ObtenerRutaArchivo(nombreArchivo);

                // Guardar el archivo en la ruta correspondiente de manera asíncrona
                await File.WriteAllBytesAsync(rutaArchivo, archivoBytes);

                // Aquí puedes realizar cualquier otra acción con el archivo descargado
            }
        }

        private string ObtenerRutaArchivo(string nombreArchivo)
        {
            var interfaces = DependencyService.Get<IAlmacenamientoExterno>();
            return interfaces.ObtenerRutaAlmacenamientoExterno(nombreArchivo);

            //pegar en el archivo ios
            //    string nombreArchivo = "archivotipoword.pdf";
            //string rutaDirectorioDescargas = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string rutaArchivoCompleta = Path.Combine(rutaDirectorioDescargas, nombreArchivo);

        }
    }
}
