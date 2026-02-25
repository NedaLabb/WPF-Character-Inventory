namespace CharacterInventory.Core.Infrastructure
{
    public static class FilePaths
    {
        public static string BaseDirectory = AppContext.BaseDirectory;
        public static string Characters = Path.Combine(BaseDirectory, "Files", "Characters.json");
    }
}