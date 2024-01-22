using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Domain.Entities
{
    public class FileUpload
    {
        [Key]
        public int Id { get; set; }
        public int FileId { get; set; }
        public int FileType { get; set; }
        public string? FileName { get; set; }
        public string? FileExtenstion { get; set; }
        public byte[]? FileData { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }


    }
}
