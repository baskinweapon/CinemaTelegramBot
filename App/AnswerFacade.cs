using App;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class StickerPack {
    public static string[] Stickers => stickerPack;
    
    
    public static string GetRandomSticker() {
        var random = new Random();
        var index = random.Next(0, stickerPack.Length);
        return stickerPack[index];
    }

    private static readonly string[] sadStickerPack = {
        "CAACAgQAAxkBAAEo-stlqZ9gi_6Bs1xRReHgE1BcbRydNAACvwoAAlpUuVAMyZwN6blhwzQE",
        "CAACAgIAAxkBAAEo-s1lqZ-gw4FqIXxoHwPMG2t1V_o_dgAC8BUAAo4rQUgjhaGVO-rMIDQE",
        "CAACAgQAAxkBAAEo-s9lqZ-82DqnuRxHBSj9X-fIF5nY-AACmAgAAq2OgVNoK_SzCIEVUzQE"
    };
    
    static readonly string[] stickerPack = new string[] {
            "CAACAgIAAxkBAAEn-6VlaL3neM4pyCc0fb3bNRQnRW3fowAC9TUAAk6wMUjCbiOaPe7-xzME",
            "CAACAgIAAxkBAAEn-51laLvYnUjECMLApW4WnV0bCpScOAAC_jcAAp8oMEhNwEkLPkcjnjME",
            "CAACAgIAAxkBAAEn-6plaL6VYyqe2aHXUBRuM9FgzGo41gAC3jIAApu5MEihI08wjdm0JDME",
            "CAACAgIAAxkBAAEn-61laL7l3RO9hMbQqq7CIxMwVyL-QgACiD0AAn5jMEiQNBP-QUmiLjME",
            "CAACAgIAAxkBAAEn-69laL8D5u6LV3aeJeIO1tyLalEX8AACrhYAAj1niEj7ep07L4oSyjME",
            "CAACAgIAAxkBAAEo-sllqZ8sCilKhpBo8ffAZuOSg9wmBQACvhoAAmsKUUiCKh1F-sv_iDQE",
            "CAACAgIAAxkBAAEo-tFlqZ_O8yq0QoEJ92HASlkRPhmH4wACISsAAhWgOEg8DLi5H2TnJjQE"
        
        };
}

public class AnswerFacade {
    private string _warningMessage = "In this chat, you can only send one message per week.\n\n<strong>Order and reason above all.</strong>";
    
    public async void SendAnswer(Update update) {
        if (update.Message is not { } message)
            return;
        
        DataBase.Instance.SetMainChatId(update.Message.Chat.Id);
        DataBase.Instance.NewUser(message);
        if (message.Text is not { } messageText)
            return;
        
        // Topics Logic
        if (message.MessageThreadId is not null) {
            var topicType = new GroupTopics().GetTopicType(message.MessageThreadId.Value);
            switch (topicType) {
                case TopicType.Review:
                    break;
                case TopicType.WeekMovies:
                    SendToWeekMovies(message);
                    break;
                case TopicType.EnglishCinema:
                    break;
                case TopicType.TeaCeremony:
                    break;
                case TopicType.NightChillChat:
                    break;
            }
        }
    }
    
    private async void SendToWeekMovies(Message message) {
        if (!Mode.IsDev && DataBase.Instance.GetWeekCinemaUsers().Any(user => user.Id == message.From.Id)) {
            await TelegramProvider.Instance.bot.SendMessage(message.Chat.Id, _warningMessage, messageThreadId: message.MessageThreadId, parseMode: ParseMode.Html);
            return;
        }
        
        if (message.Text.Contains("/random")) {
            var movieName = await TMDBAPI.GetRandomMovies();
            Genre genre;
            if (message.Text.Contains("/random_")) {
                var genres = await TMDBAPI.GetAllGenres();
                
                genre = genres.genres.FirstOrDefault(genre => genre.name.ToLower() == message.Text.Split("_")[1].Split("@")[0].ToLower());
                if (genre != null) {
                    movieName = await TMDBAPI.GetRandomMovies(genre.id);
                }
            }
            
            var m = await TelegramProvider.Instance.bot.SendMessage(
                message.Chat.Id, $"🎥🍿 <strong>Find random movie</strong>. 🍿🎥", messageThreadId: message.MessageThreadId, parseMode: ParseMode.Html);
            message.Text = movieName;
            DeleteMessage(m);
        }
        else {
            var m = await TelegramProvider.Instance.bot.SendMessage(
                message.Chat.Id, $"Try to find your movie. \n 🎥🍿 <strong>{message.Text.Trim()}</strong>. 🍿🎥",messageThreadId: message.MessageThreadId, parseMode: ParseMode.Html);
            DeleteMessage(m);
        }
        
        
        var stickerId = new InputFileId(StickerPack.GetRandomSticker());
        var l = await TelegramProvider.Instance.bot.SendSticker(message.Chat.Id, stickerId, message.MessageThreadId);
        DeleteMessage(l);

        DeleteMessage(message, 100);
        
        // Find movie
        var movie = await new TMDBAPI().SearchMovies(message.Text.Trim());
        
        if (movie == null) {
            var mes = await TelegramProvider.Instance.bot.SendMessage(message.Chat.Id, $"🤨 <strong>I can't find your film {message.Text} </strong> 🤨", messageThreadId: message.MessageThreadId, parseMode: ParseMode.Html);
            DeleteMessage(mes, 10);
        } else {
            FindCinemaBehavior.SendAboutFilm(movie, message.Chat.Id, message.From.Username, message.MessageThreadId);
            
            DataBase.Instance.AddWeekCinemaMovie(movie);
            DataBase.Instance.AddWeekCinemaUser(DataBase.Instance.GetOrNewUser(message));
        }
        
        DataBase.Instance.AddChatIDMessageIDDictionary(message.Chat.Id, message.MessageId);
    }
    
    private async void DeleteMessage(Message message, int time = 2) {
        await Task.Delay(time * 1000);
        await TelegramProvider.Instance.bot.DeleteMessage(message.Chat.Id, message.MessageId);
    }


   
}