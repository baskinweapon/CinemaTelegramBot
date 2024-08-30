using System.Diagnostics;
using App;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

public class Mode {
    public static bool IsDev { get; set; }
}

public enum Version {
    Debug,
    Release,
    TestFlight,
    Production
}

class Aplication {
    
    // settings
    public static readonly bool TestFlight = true;

    public static void Update() {
        var loop2Task = Task.Run(function: async () => {
            while (true) {
                Console.WriteLine("Checking week cinema");
                Console.WriteLine("Current time: " + DateTime.Now);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday && DateTime.Now.Hour >= 16) {
                    Console.WriteLine("Sending week pool");
                    
                    var message = await TelegramProvider.Instance.bot.SendPollAsync(
                        DataBase.Instance.GetMainChatId(), 
                        "Фильмы выбраны. Начинаем голосование!", 
                        DataBase.Instance.GetWeekCinemaMovies().Select(movie => movie.Title).ToArray(),
                        isAnonymous: false,
                        allowsMultipleAnswers: false,
                        cancellationToken: new CancellationTokenSource().Token);
                    DataBase.Instance.SetLastPollId(message);
                    
                }
                
                await Task.Delay((int)10000);
                
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour >= 14) {
                    await TelegramProvider.Instance.bot.StopPollAsync(
                        DataBase.Instance.GetMainChatId(),
                        DataBase.Instance.GetLastPollId().MessageId,
                        cancellationToken: new CancellationTokenSource().Token);
                    var poll = DataBase.Instance.GetLastPollId().Poll;
                
                    if (poll != null) {
                        var win = poll.Options.Aggregate((i1, i2) => i1.VoterCount > i2.VoterCount ? i1 : i2);
                        string str = $"Голосование окончено. Участников {poll.TotalVoterCount} \n +" +
                                     $"{poll.Options.Length} вариантов ответа \n" +
                                     $"Победитель: {win.Text}";

                                     await TelegramProvider.Instance.bot.SendTextMessageAsync(
                            DataBase.Instance.GetMainChatId(),
                            str,
                            cancellationToken: new CancellationTokenSource().Token);
                    }
                }
                
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) {
                    Console.WriteLine("Clearing week cinema");
                    
                    DataBase.Instance.ClearWeekCinemaUsers();
                    DataBase.Instance.ClearWeekCinemaMovies();

                    var dict = DataBase.Instance.GetChatIDMessageIDDictionary();
                    foreach (var (key, value) in dict) {
                        foreach (var messageId in value) {
                            await TelegramProvider.Instance.bot.DeleteMessageAsync(key, messageId);
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
        if (args[0] == "prod") {
            Mode.IsDev = false;
            Console.WriteLine("Prod mode");
        } else {
            Mode.IsDev = true;
            Console.WriteLine("Dev mode");
        }
        
        DataBase.Instance.Load();
        
        // var teaCeremony = new TeaCeremony();
        // teaCeremony.GetTea();
        string str = $"Dev Testing CI/CD";

        await TelegramProvider.Instance.bot.SendTextMessageAsync(
            DataBase.Instance.GetMainChatId(),
            str,
            cancellationToken: new CancellationTokenSource().Token);
        
        
        Update();
        
        
        await TelegramProvider.Instance.Init();
    }
}

