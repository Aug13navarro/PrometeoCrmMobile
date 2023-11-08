using System;

namespace Core.Model
{
    public class AttachFile
    {
        public int Id { get; set; }
        public int OpportunityOrderNoteId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int? Orientation { get; set; }
        public string MineType { get; set; }
        public string FileIconName => SetIcon(FileName);

        public string SetIcon(string fileName)
        {
            if (fileName.ToLower().Contains("doc") || fileName.ToLower().Contains("docx"))
            {
                return "docx.png";
            }
            else if (fileName.ToLower().Contains("jpg"))
            {
                return "jpg.png";
            }
            else if (fileName.ToLower().Contains("pdf"))
            {
                return "pdf.png";
            }
            else if (fileName.ToLower().Contains("png"))
            {
                return "png.png";
            }
            else if (fileName.ToLower().Contains("xlsx") || fileName.ToLower().Contains("xls"))
            {
                return "xlsx.png";
            }

            return "";
        }
    }
}
