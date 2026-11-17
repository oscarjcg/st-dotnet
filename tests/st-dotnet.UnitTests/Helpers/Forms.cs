using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace st_dotnet.UnitTests.Helpers;

public static class Forms
{
    public static IFormFile File(string name, string fileName = "image.png", string content = "fake-png")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, name, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };
    }

    public static IFormCollection Create(IDictionary<string, string>? fields = null, params IFormFile[] files)
    {
        var values = (fields ?? new Dictionary<string, string>())
            .ToDictionary(kv => kv.Key, kv => new StringValues(kv.Value));
        var fileCollection = new FormFileCollection();
        fileCollection.AddRange(files);
        return new FormCollection(values, fileCollection);
    }
}
