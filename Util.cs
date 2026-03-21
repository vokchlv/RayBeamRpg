namespace IdleGame;

public class Util
{
    public static string BuildPath(string folder, string name)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, name);
    }
}