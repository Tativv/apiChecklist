namespace HotelChecklist.Api.Common.Adapters.FileStorage;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken);

    Task<Stream> ReadAsync(string filePath, CancellationToken cancellationToken);

    Task DeleteAsync(string filePath, CancellationToken cancellationToken);
}
