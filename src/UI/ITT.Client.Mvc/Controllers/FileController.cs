using ITT.Client.Mvc.ApiClients;
using ITT.Client.Mvc.Dtos.Requests;
using ITT.Client.Mvc.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ITT.Client.Mvc.Controllers
{
    public class FileController : Controller
    {
        private readonly IDocumentApiClient documentApiClient;
        private readonly ITextSummaryApiClient textSummaryApiClient;

        public FileController(IDocumentApiClient DocumentApiClient, ITextSummaryApiClient TextSummaryApiClient)
        {
            
            documentApiClient = DocumentApiClient;
            textSummaryApiClient = TextSummaryApiClient;
        }


        public IActionResult FileUpload()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<FileUploadResponse>> UploadFile(IFormFile file)
        {
            try
            {
                var fileUploadRequest = new FileUploadRequest
                {
                    FileId = 1,
                    FileType = 1,
                    FileName = file.FileName,
                    FileExtension = Path.GetExtension(file.FileName)?.TrimStart('.'),
                    FileContentType = file.ContentType,
                    FileData = await ConvertFileToBytesAsync(file),
                    DateCreated = DateTimeOffset.Now

                };

                var result = await documentApiClient.UploadFile(fileUploadRequest);

            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<List<FileUploadResponse>>> UploadFiles(List<IFormFile> files)
        {
            try
            {
                List<FileUploadRequest> fileUploadRequests = new();
                foreach (var file in files)
                {
                    var fileUploadRequest = new FileUploadRequest
                    {
                        FileId = 1,
                        FileType = 1,
                        FileName = file.FileName,
                        FileExtension = Path.GetExtension(file.FileName)?.TrimStart('.'),
                        FileContentType = file.ContentType,
                        FileData = await ConvertFileToBytesAsync(file),
                        DateCreated = DateTimeOffset.Now

                    };

                    fileUploadRequests.Add(fileUploadRequest);

                }

                var result = await documentApiClient.UploadFiles(fileUploadRequests);
            }
            catch (Exception)
            {

                throw;
            }
            

            return View();
        }

        [HttpPost]
        public async Task<ActionResult<TextSummaryResponse>> SummarizeText()
        {
            try
            {
                var textSummaryRequest = new TextSummaryRequest
                {
                    DocumentId = 1,
                    DcoumentTypeId = 1,
                    DocumentName = "Sample Name",
                    TextContent = "This is the content of the document that you want to summarize.",
                    TextSummary = string.Empty,

                };

                var result = await textSummaryApiClient.SummarizeText(textSummaryRequest);

                return View(result.Data);

            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<string> ConvertToBase64(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
        }

        private async Task<byte[]> ConvertFileToBytesAsync(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


    }
}
