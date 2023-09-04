namespace SimpleApp;

[Options("Features:Awesome")]
internal class AwesomeOptions
{
    public string EntryMessage { get; init; } = "hi";
    public string ExitMessage { get; init; } = "bye";
}