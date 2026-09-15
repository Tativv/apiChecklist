using Microsoft.Extensions.Options;

namespace HotelChecklist.Api.Common.Adapters.FileStorage;

public sealed class LocalFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    private readonly string _rootPath = options.Value.RootPath;

    public async Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var relativeDir = Path.Combine(now.Year.ToString(), now.Month.ToString("00"));
        var absoluteDir = Path.Combine(_rootPath, relativeDir);
        Directory.CreateDirectory(absoluteDir);

        var sanitizedFileName = SanitizeFileName(fileName);
        var storedFileName = $"{Guid.NewGuid()}-{sanitizedFileName}";
        var relativePath = Path.Combine(relativeDir, storedFileName).Replace('\\', '/');
        var absolutePath = Path.Combine(_rootPath, relativePath);

        await using var fileStream = File.Create(absolutePath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return relativePath;
    }

    public Task<Stream> ReadAsync(string filePath, CancellationToken cancellationToken)
    {
        var absolutePath = Path.Combine(_rootPath, filePath);

        if (!File.Exists(absolutePath))
            throw new FileNotFoundException("Evidence file not found.", filePath);

        Stream stream = File.OpenRead(absolutePath);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken)
    {
        var absolutePath = Path.Combine(_rootPath, filePath);

        if (File.Exists(absolutePath))
            File.Delete(absolutePath);

        return Task.CompletedTask;
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
    }
}
