namespace HotelChecklist.Api.Common.Adapters.FileStorage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string RootPath { get; set; } = "uploads";
}
