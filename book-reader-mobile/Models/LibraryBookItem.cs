namespace BookReader.Mobile.Models;

public sealed class LibraryBookItem
{
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string Status { get; init; }
    public required string Initial { get; init; }
    public double Progress { get; init; }
    public required Color Cover { get; init; }
}
