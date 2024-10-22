using System.Text.Json;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using File = System.IO.File;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace App;

public class Data {
    public DateTime lastNightCinemaDate;
    public TimeSpan durationBetweenNightCinema;
    public int sizeOfNightCinemaSessions;
    public Dictionary<int, string[]> movieMessageIdDictionary = new();
    public List<User> users = new();
    public List<MovieData> movies = new();
    
    public List<User> weekCinemaUsers = new();
    public List<MovieData> weekCinemaMovies = new();
    
    public Dictionary<long, List<int>> chatIDMessageIDDictionary = new();
    public Message? lastPollId;
    public ChatId mainChatId;
}


public class DataBase {
    
    private static DataBase _instance;
    
    public static DataBase Instance => _instance ??= new DataBase();
    private Data content = new();

    public List<User> GetWeekCinemaUsers() => content.weekCinemaUsers;
    public List<MovieData> GetWeekCinemaMovies() => content.weekCinemaMovies;
    
    public Message? GetLastPollId() => content.lastPollId;
    public void SetLastPollId(Message? id) {
        content.lastPollId = id;
        Save();
    }
    
    
    public void SetLastNightCinemaDate(DateTime date) {
        content.lastNightCinemaDate = date;
        Save();
    }
    
    public DateTime GetLastNightCinemaDate() => content.lastNightCinemaDate;
    
    public void AddChatIDMessageIDDictionary(long chatId, int messageId) {
        if (content.chatIDMessageIDDictionary.ContainsKey(chatId)) {
            content.chatIDMessageIDDictionary[chatId].Add(messageId);
            Save();
            return;
        }
        content.chatIDMessageIDDictionary.Add(chatId, new List<int> {messageId});
        Save();
    }
    
    public Dictionary<long, List<int>> GetChatIDMessageIDDictionary() => content.chatIDMessageIDDictionary; 

    public List<int> GetAllMessages() {
        var messages = new List<int>();
        for (int i = 0; i < content.chatIDMessageIDDictionary.Count; i++) {
            content.chatIDMessageIDDictionary.TryGetValue(content.chatIDMessageIDDictionary.Keys.ElementAt(i), out var value);
            if (value != null) messages = (List<int>)messages.Concat(value);
        }
        return messages;
    }
    
    public void ClearChatIDMessageIDDictionary() {
        content.chatIDMessageIDDictionary.Clear();
        Save();
    }
    
    public ChatId GetMainChatId() => content.mainChatId;
    public void SetMainChatId(ChatId chatId) {
        content.mainChatId = chatId;
        Save();
    }
    
    public void ClearWeekCinemaUsers() {
        content.weekCinemaUsers.Clear();
        Save();
    }
    
    public void ClearWeekCinemaMovies() {
        content.weekCinemaMovies.Clear();
        Save();
    }
    
    public void AddWeekCinemaUser(User user) {
        content.weekCinemaUsers.Add(user);
        Save();
    }
    
    public void AddWeekCinemaMovie(MovieData movie) {
        content.weekCinemaMovies.Add(movie);
        Save();
    }
    
    public void AddMovieMessageIdDictionary(int messageId, string[] message) {
        content.movieMessageIdDictionary.Add(messageId, message);
        Save();
    }
    
    public string GetMovieMessageIdDictionary(int messageId) => content.movieMessageIdDictionary.TryGetValue(messageId, out var value) ? value[0] : "Not found";
    public string GetMovieMessageIdDictionaryAdditional(int messageId) => content.movieMessageIdDictionary.TryGetValue(messageId, out var value) ? value[1] : "Not found";
    
    
    public void AddMovie(MovieData movie) {
        content.movies.Add(movie);
        Save();
    }

    public User GetOrNewUser(Message message) {
        if (content.users.All(user => user.Id == message.From.Id)) {
            return content.users.Find(user => user.Id == message.From.Id);
        }
        
        var user = new User(
            message.From.Id,
            message.From.FirstName,
            message.From.Username,
            DateTime.Now,
            message.Text ?? ""
        );
        content.users.Add(user);
        Save();
        return user;
        
    }
    
    public void NewUser(Message message) {
        if (content.users.All(user => user.Id != message.From.Id)) {
            content.users.Add(new App.User(
                message.From.Id,
                message.From.FirstName,
                message.From.Username,
                DateTime.Now,
                message.Text ?? ""
            ));
            Save();
        }
    }
    
    public void NewUser(User user) {
        content.users.Add(user);
    }
    
    public void Load() {
        if (File.Exists(GetSavesPath())) {
            using (StreamReader file = File.OpenText(GetSavesPath()))
            {
                JsonSerializer serializer = new JsonSerializer();
                content = (Data)serializer.Deserialize(file, typeof(Data))! ?? new Data();
            }
        } else {
            content = new Data();
        }
    }
    
    private void Save() {
        content.durationBetweenNightCinema = DateTime.Now - content.lastNightCinemaDate;
        string json = JsonConvert.SerializeObject(content, Formatting.Indented);
        using (StreamWriter file = File.CreateText(GetSavesPath()))
        {
            JsonSerializer serializer = new JsonSerializer();
            var options = new JsonSerializerOptions { WriteIndented = true };
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(file, content);
        }
    }


    // public void SetLastPollId(Poll updatePoll) {
    //     content.lastPollId.Poll = updatePoll;
    //     Save();
    // }

    private string GetSavesPath() {
        // Относительный путь к файлу
        string relativePath = "saves.json";
        return relativePath;
        // Относительный путь для подъема на 3 директории вверх и перехода в папку Saves
        string relativeDir = Path.Combine("..", "..", "..");
        
        Console.WriteLine("Full path = " + Path.GetFullPath(relativeDir));
        
        // Комбинируем текущий каталог с относительным путем
        return Path.Combine(Path.GetFullPath(relativeDir), relativePath);
    }
}