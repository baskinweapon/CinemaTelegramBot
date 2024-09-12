using App;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class TelegramProvider {
    private static TelegramProvider _instance;
    
    public static TelegramProvider Instance => _instance ??= new TelegramProvider();

    private const string Token = "6573556229:AAEjGUau1Fr0X5eFlRjpY89v1sdr9iLmNqc";

    public TelegramBotClient bot;
    
    public InlineKeyboardButton descriptionButton = new ("Описание");
    public InlineKeyboardButton additionButton = new ("Дополнительно");
    
    // Keyboard markup
    private InlineKeyboardMarkup inline;
    
    async Task CallbackQueryHandlerAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken) {
        if (callbackQuery.Data == "movie_description") {
            // if (Mode.IsDev) {
            //     await bot.EditMessageTextAsync(
            //      callbackQuery.Message.Chat.Id,
            //      callbackQuery.Message.MessageId,
            //      DataBase.Instance.GetMovieMessageIdDictionary(callbackQuery.Message.MessageId),
            //          cancellationToken: cancellationToken,
            //          replyMarkup: new InlineKeyboardMarkup(additionButton),
            //          parseMode: ParseMode.Html);
            // }
            // else {
                await bot.EditMessageCaptionAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    DataBase.Instance.GetMovieMessageIdDictionary(callbackQuery.Message.MessageId),
                    cancellationToken: cancellationToken,
                    replyMarkup: new InlineKeyboardMarkup(additionButton),
                    parseMode: ParseMode.Html);
            // }
        }

        if (callbackQuery.Data == "movie_additional") {
            // if (Mode.IsDev) {
            //     await bot.EditMessageTextAsync(
            //      callbackQuery.Message.Chat.Id,
            //      callbackQuery.Message.MessageId,
            //      DataBase.Instance.GetMovieMessageIdDictionaryAdditional(callbackQuery.Message.MessageId),
            //      cancellationToken: cancellationToken,
            //      replyMarkup: new InlineKeyboardMarkup(descriptionButton),
            //      parseMode: ParseMode.Html);
            // }
            // else {
                await bot.EditMessageCaptionAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    DataBase.Instance.GetMovieMessageIdDictionaryAdditional(callbackQuery.Message.MessageId),
                    cancellationToken: cancellationToken,
                    replyMarkup: new InlineKeyboardMarkup(descriptionButton),
                    parseMode: ParseMode.Html);
            }
        // }
        await bot.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"Computing...", cancellationToken: cancellationToken);
    }

    // Main Update handler
    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        if (update.Type == UpdateType.PollAnswer) {
            var win = update.PollAnswer.OptionIds.Aggregate((i1, i2) => i1 > i2 ? i1 : i2);
            if (update.PollAnswer.OptionIds is not null) {
                DataBase.Instance.GetLastPollId().Poll.Options[win].VoterCount++;
                DataBase.Instance.GetLastPollId().Poll.TotalVoterCount++;
            }
        }

        if (update.CallbackQuery is { } callbackQuery) 
            await CallbackQueryHandlerAsync(callbackQuery, cancellationToken);
        
        new AnswerFacade().SendAnswer(update);
    }
    
    //send images
    public async Task SendMediaGroupImages(string[] path, long chatId, int? messageThreadId = null) {
        var media = new List<IAlbumInputMedia>();
        for (int i = 0; i < path.Length; i++) {
            if (path[i].Length == 0 || path[i] == "") continue;
            media.Add(new InputMediaPhoto(new InputFileUrl(path[i])));    
        }
        
        await bot.SendMediaGroupAsync(
            chatId: chatId,
            media: media.ToArray(),
            replyToMessageId: messageThreadId
        );
    }
    
    public async Task Init() {
        bot = new TelegramBotClient($"{Token}");
        
        descriptionButton.Text = "Описание";
        descriptionButton.CallbackData = "movie_description";
        
        additionButton.Text = "Дополнительно";
        additionButton.CallbackData = "movie_additional";
        
        inline = new InlineKeyboardMarkup(new[] {
            descriptionButton, additionButton
        });
        
        using CancellationTokenSource cts = new ();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new () {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };
        
        bot.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await bot.GetMeAsync(cancellationToken: cts.Token);
        
        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();
        
        // Send cancellation request to stop bot
        await cts.CancelAsync();
    }
    
    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}