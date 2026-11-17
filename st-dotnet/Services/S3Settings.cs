namespace st_dotnet.Services
{
    public class S3Settings
    {
        public const string SectionName = "S3";

        public string AccessKeyId { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string BucketName { get; set; } = "oscar-catari-s3-dev";

        public string PublicUrl { get; set; } = "https://oscar-catari-s3-dev.s3.eu-central-1.amazonaws.com/";

        public string? ServiceUrl { get; set; }
    }
}
