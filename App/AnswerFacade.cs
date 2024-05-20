using App;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = App.User;

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
    private string _warningMessage = "В этом чате можно отправлять только одно сообщение раз в неделю.<strong> Порядок и разумность превыше всего </strong>";
    
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
                    // future feature
                    
                    // var teaCeremony = new TeaCeremony();
                    // var messages = teaCeremony.GetBlog();
                    //
                    // foreach (var msg in messages) {
                    //     await TelegramProvider.Instance.bot.SendTextMessageAsync(
                    //         message.Chat.Id, msg, message.MessageThreadId, ParseMode.Html);
                    // }
                    break;
                case TopicType.NightChillChat:
                    break;
            }
        }
    }
    
    private async void SendToWeekMovies(Message message) {
        
        if (Mode.IsDev) {}
        else {
            // if (DataBase.Instance.GetWeekCinemaUsers().Find(x => x.Id == message.From.Id) != null) {
            //     await TelegramProvider.Instance.bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            //     await TelegramProvider.Instance.bot.SendTextMessageAsync(message.Chat.Id, _warningMessage, message.MessageThreadId, ParseMode.Html);
            //     return;
            // }
        }
        
        await TelegramProvider.Instance.bot.SendTextMessageAsync(
            message.Chat.Id, $"В поисках твоего фильма: {message.Text.Trim()}.", message.MessageThreadId, ParseMode.Html);
        
        var stickerId = new InputFileId(StickerPack.GetRandomSticker());
        var l = await TelegramProvider.Instance.bot.SendStickerAsync(message.Chat.Id, stickerId, message.MessageThreadId);
        DeleteSticker(l);
        
        var kinoPoisk = new Kinopoisk();
        kinoPoisk.SendToWeekMovies(message);
        // var weekMovies = new WeekMovies();
        // weekMovies.SendToWeekMovies(message);
    }
    
    private async void DeleteSticker(Message message) {
        await Task.Delay(2 * 1000);
        await TelegramProvider.Instance.bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
    }
    
}