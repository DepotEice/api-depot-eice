using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Net;

namespace API.DepotEice.UIL.Managers
{
    public class FileManager : IFileManager
    {
        private readonly ILogger _logger;
        private readonly string _bucketName;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly Amazon.RegionEndpoint _region;

#if DEBUG
        private readonly IConfiguration _configuration;
#endif

#if DEBUG
        /// <summary>
        /// Instanciate <see cref="FileManager"/> in DEBUG containing the IConfiguration to retrieve the 
        /// appsettings.json or secrets.json values
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FileManager(ILogger<FileManager> logger, IConfiguration configuration)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _logger = logger;
            _configuration = configuration;

            _bucketName = _configuration["AWS:AWS_BUCKET_NAME"] ??
                throw new ArgumentNullException($"Cannot find AWS_BUCKET_NAME in appsettings.json or secret.json");

            _accessKey = _configuration["AWS:AWS_ACCESS_KEY"] ??
                throw new ArgumentNullException($"Cannot find AWS_ACCESS_KEY in appsettings.json or secret.json");

            _secretKey = _configuration["AWS:AWS_SECRET_KEY"] ??
                throw new ArgumentNullException($"Cannot find AWS_SECRET_KEY in appsettings.json or secret.json");

            string regionEndPoint = _configuration["AWS:AWS_REGION_ENDPOINT"] ??
                throw new ArgumentNullException($"Cannot find AWS_REGION_ENDPOINT in appsettings.json or secret.json");

            if (!Enum.TryParse(typeof(Amazon.RegionEndpoint), regionEndPoint, out object? region))
            {
                throw new ArgumentException($"Cannot parse {regionEndPoint} to Amazon.RegionEndpoint");
            }

            _region = (Amazon.RegionEndpoint?)region ?? throw new ArgumentNullException(nameof(region));
        }

#else
        /// <summary>
        /// Instanciate <see cref="FileManager"/> in RELEASE
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FileManager(ILogger<FileManager> logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;

            _bucketName = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME") ??
                throw new ArgumentNullException($"Cannot find AWS_BUCKET_NAME in the environment variables");

            _accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY") ??
                throw new ArgumentNullException($"Cannot find AWS_ACCESS_KEY in the environment variables");

            _secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY") ??
                throw new ArgumentNullException($"Cannot find AWS_SECRET_KEY in the environment variables");

            string regionEndPoint = Environment.GetEnvironmentVariable("AWS_REGION_ENDPOINT") ??
                throw new ArgumentNullException($"Cannot find AWS_REGION_ENDPOINT in the environment variables");

            string regionEndPoint = _configuration["AWS:AWS_REGION_ENDPOINT"] ??
                throw new ArgumentNullException($"Cannot find AWS_REGION_ENDPOINT in appsettings.json or secret.json");

            if (!Enum.TryParse(typeof(Amazon.RegionEndpoint), regionEndPoint, out object? region))
            {
                throw new ArgumentException($"Cannot parse {regionEndPoint} to Amazon.RegionEndpoint");
            }

            _region = (Amazon.RegionEndpoint?)region ?? throw new ArgumentNullException(nameof(region));
        }
#endif

        /// <summary>
        /// Get an object based on its key from AWS S3
        /// </summary>
        /// <param name="key">The object's key</param>
        /// <returns>
        /// A <see cref="FileModel"/> containing the stream, content type and the key (The filename is optional for 
        /// the moment)
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<FileModel?> GetObjectAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                AmazonS3Config config = new AmazonS3Config
                {
                    RegionEndpoint = _region as Amazon.RegionEndpoint
                };

                using IAmazonS3 client = new AmazonS3Client(_accessKey, _secretKey, config);

                GetObjectRequest getObjectRequest = new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = key
                };

                GetObjectResponse getObjectResponse = await client.GetObjectAsync(getObjectRequest);

                byte[] bytes = new byte[getObjectResponse.ResponseStream.Length];

                int lengthToRead = bytes.Length;
                int offset = 0;
                int result = 0;

                while (result < lengthToRead)
                {
                    result = await getObjectResponse.ResponseStream.ReadAsync(bytes, offset, lengthToRead);
                    offset += result;
                    lengthToRead -= result;
                }

                FileModel fileModel = new FileModel()
                {
                    Key = key,
                    Content = bytes,
                    ContentType = getObjectResponse.Headers.ContentType
                };

                return fileModel;
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(GetObjectAsync)}.\n" +
                    $"{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Delete an object based on its key from AWS S3.
        /// </summary>
        /// <param name="key">The object key in AWS. Mostly the file name but can be anything else</param>
        /// <returns><c>true</c> If the operation went successfully. <c>false</c> Otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<bool> DeleteObjectAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                AmazonS3Config config = new()
                {
                    RegionEndpoint = _region as Amazon.RegionEndpoint
                };

                using IAmazonS3 client = new AmazonS3Client(_accessKey, _secretKey, config);

                DeleteObjectRequest deleteObjectRequest = new()
                {
                    BucketName = _bucketName,
                    Key = key
                };

                DeleteObjectResponse deleteObjectResponse = await client.DeleteObjectAsync(deleteObjectRequest);

                if (deleteObjectResponse is null)
                {
                    _logger.LogWarning($"{DateTime.Now} - The response object returned by AWS S3 is null.");
                    return false;
                }

                return deleteObjectResponse.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(DeleteObjectAsync)}\n" +
                    $"{e.Message}\n{e.StackTrace}");

                return false;
            }
        }

        /// <summary>
        /// Upload a file to an AWS S3 bucket
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="key">The file's key in S3</param>
        /// <returns><c>true</c> If the upload is successful. <c>false</c> Otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<bool> UploadObjectAsync(IFormFile file, string key)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                AmazonS3Config config = new()
                {
                    RegionEndpoint = _region as Amazon.RegionEndpoint
                };

                using IAmazonS3 client = new AmazonS3Client(_accessKey, _secretKey, config);

                using MemoryStream memoryStream = new MemoryStream();

                file.CopyTo(memoryStream);

                PutObjectRequest putObjectRequest = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = memoryStream
                };

                PutObjectResponse putObjectResponse = await client.PutObjectAsync(putObjectRequest);

                return putObjectResponse.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(UploadObjectAsync)}\n" +
                    $"{e.Message}\n{e.StackTrace}");

                return false;
            }
        }
    }
}
