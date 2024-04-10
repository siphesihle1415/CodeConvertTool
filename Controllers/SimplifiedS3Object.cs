
namespace CodeConverterTool.Controllers
{
    internal class SimplifiedS3Object
    {
        public string ETag { get; internal set; }
        public string Key { get; internal set; }
        public DateTime LastModified { get; internal set; }
        public long Size { get; internal set; }
        public Type Type { get; internal set; }
    }
}