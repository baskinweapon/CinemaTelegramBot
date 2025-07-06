using System.Text;
using App;
using Telegram.Bot;
using Telegram.Bot.Types;

public class Mode {
    public static bool IsDev { get; set; }
}

class Aplication {
    
    // main loop
    public static void Update() {
        var loop2Task = Task.Run(function: async () => {
            while (true) {
                Console.WriteLine("Checking week cinema");
                Console.WriteLine("Current time: " + DateTime.Now);
                
                if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday && DateTime.Now.Hour >= 16 && DataBase.Instance.GetLastPollId() == null) {
                    Console.WriteLine("Sending week pool");
                    
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\ud83c\udfac Movies Selected!");
                    sb.Append("\ud83d\udc69\u200d\ud83c\udfa4Vote for the film you want to watch this week.");
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append("\ud83d\udd14 Reminder: Vote only for movies suggested by others.");
                    sb.Append(Environment.NewLine);
                    sb.Append("\ud83d\udd52 Poll closes: Friday at 14:00.");
                    string s = sb.ToString();

                    var arr = new List<InputPollOption>();
                    foreach (var movie in DataBase.Instance.GetWeekCinemaMovies()
                                 .Select(movie => movie.MovieDetails.original_title)) {
                        arr.Add(new InputPollOption {
                            Text = movie,
                        });
                    }
                    
                    var message = await TelegramProvider.Instance.bot.SendPoll(
                        DataBase.Instance.GetMainChatId(),
                        s,
                        arr.ToArray(),
                        isAnonymous: false,
                        allowsMultipleAnswers: false,
                        cancellationToken: new CancellationTokenSource().Token);
                    DataBase.Instance.SetLastPollId(message);
                }
                
                await Task.Delay((int)10000);
                
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour >= 14 && DataBase.Instance.GetLastPollId() != null) {
                    await TelegramProvider.Instance.bot.StopPoll(
                        DataBase.Instance.GetMainChatId(),
                        DataBase.Instance.GetLastPollId().MessageId,
                        cancellationToken: new CancellationTokenSource().Token);
                    var poll = DataBase.Instance.GetLastPollId().Poll;
                
                    if (poll != null) {
                        Console.WriteLine("Poll ended");
                        var win = poll.Options.Aggregate((i1, i2) => i1.VoterCount > i2.VoterCount ? i1 : i2);
                        string str = $"🎥 Vote closed.\n🏄‍ Participants: {poll.TotalVoterCount} \n" +
                                     $"📽️ Movies: {poll.Options.Length} \n" +
                                     $"🐣 Winner: {win.Text} 🐓" + 
                                     "\n\nLooking forward to seeing you at movie night today at 19:00🧗";

                                     await TelegramProvider.Instance.bot.SendMessage(
                            DataBase.Instance.GetMainChatId(),
                            str,
                            cancellationToken: new CancellationTokenSource().Token);
                    }
                    
                    DataBase.Instance.SetLastPollId(null);
                }
                
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) {
                    Console.WriteLine("Clearing week cinema");
                    
                    DataBase.Instance.ClearWeekCinemaUsers();
                    DataBase.Instance.ClearWeekCinemaMovies();

                    var dict = DataBase.Instance.GetChatIDMessageIDDictionary();
                    foreach (var (key, value) in dict) {
                        foreach (var messageId in value) {
                            await TelegramProvider.Instance.bot.DeleteMessage(key, messageId);
                        }
                    }
                    DataBase.Instance.ClearChatIDMessageIDDictionary();
                }
                
                await Task.Delay((int)8.64e+7);
            }
        });
    }
    
    
    // main
    public static async Task Main(String[] args) {
        // Mode.IsDev = true;
        Console.WriteLine("Starting application...");
        DataBase.Instance.Load();
        Update();
        
        await TelegramProvider.Instance.Init();
    }
}

