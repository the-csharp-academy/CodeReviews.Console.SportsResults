namespace NBANotifier.Services;

internal static class Utils
{
    internal static string GetTestDataDirectory()
    {
        var projectDir = GetProjectDirectory();
        var dataDir = Path.Combine(projectDir, "TestData");
        Directory.CreateDirectory(dataDir);

        return dataDir;
    }

    internal static string GetTemplateDirectory()
    {
        var projectDir = GetProjectDirectory();
        var templateDir = Path.Combine(projectDir, "Templates");
        Directory.CreateDirectory(templateDir);

        return templateDir;
    }

    internal static string GetDatabaseConnectionString()
    {
        return
            $"Data Source={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NBANotifier", "app.db")}";
    }

    internal static string GetDatabaseDirectory()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NBANotifier");
    }

    private static string GetProjectDirectory()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
    }
}