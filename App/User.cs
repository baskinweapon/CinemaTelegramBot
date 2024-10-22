namespace App; 

public class User {
    public long Id { get; set; }
    public string Name { get; set; }
    public string? TelegramUsername { get; set; }
    public string lastMessage { get; set; }
    public DateTime? lastMessageDate { get; set; }
    public MovieData lastMovie { get; set; }
    public DateTime firstMessageDate { get; set; }
    
    public User(long id, string name, string? telegramUsername, DateTime firstMessageDate, string lastMessage = default, DateTime? lastMessageDate = default, MovieData lastMovie = default) {
        Id = id;
        Name = name;
        TelegramUsername = telegramUsername;
        this.lastMessage = lastMessage;
        this.lastMessageDate = lastMessageDate;
        this.lastMovie = lastMovie;
        this.firstMessageDate = firstMessageDate;
    }
    
}