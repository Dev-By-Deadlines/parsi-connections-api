namespace Connecions.Api.Utils;

public static class GameConstants
{
    public static string SessionCookieName = "game-session";
    public static readonly Dictionary<int, string> CategoryIndexToEmojiMap = new()
    {
        {0, "🟩"},
        {1, "🟦"},
        {2, "🟪"},
        {3, "🟧"}
    };
}
