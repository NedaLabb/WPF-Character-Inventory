using CharacterInventory.Core.Infrastructure;
using CharacterInventory.Core.Models;
using Newtonsoft.Json;

namespace CharacterInventory.Core.Managers
{
    public static class CharacterManager
    {
        public static List<Character> Characters { get; private set; } = new();

        public static void LoadCharacters()
        {
            string path = FilePaths.Characters;
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Nepavyko rasti failo.", path);
            }

            string json = File.ReadAllText(path);
            Characters = JsonConvert.DeserializeObject<List<Character>>(json) ?? new();
        }

        public static void SaveCharacters()
        {
            string path = FilePaths.Characters;

            File.WriteAllText(path, JsonConvert.SerializeObject(Characters, Formatting.Indented));
        }

    }
}
