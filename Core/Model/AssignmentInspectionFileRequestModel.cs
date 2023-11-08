namespace Core.Model
{
    public class AssignmentInspectionFileRequestModel
    {
        public int AssignmentId { get; set; }
        public string FileBase64 { get; set; }
        public string FileName { get; set; }
        public int FileTypeId { get; set; }
    }
}
