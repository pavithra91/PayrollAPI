using Amazon.S3.Transfer;
using Amazon.S3;
using Org.BouncyCastle.Utilities.Zlib;
using PdfSharp.Pdf;

namespace PayrollAPI.Data
{
    public class S3Uploader
    {
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;

        public S3Uploader(string accessKeyId, string secretAccessKey, string bucketName, Amazon.RegionEndpoint region)
        {
            _s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
            _bucketName = bucketName;
        }

        public async Task UploadFileAsync(byte[] pdfData, string objectKey)
        {
            try
            {
                MemoryStream InputStream = new MemoryStream(pdfData);
                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(InputStream, _bucketName, objectKey);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error uploading file: {e.Message}");
            }
        }
    }
}
