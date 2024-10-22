using App;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;
using File = System.IO.File;

public static class FindCinemaBehavior {
    
     public static async void SendAboutFilm(MovieData movie, long chatId, string userName, int? messageThreadId = null) {
        var message = ($"<strong>{movie.MovieDetails.original_title}.</strong>\n<i>Sponsored by @{userName}</i>" + "\n\n" + movie.MovieDetails.overview +
                       "\n");
        
        var addition = GenerateAdditionalInfo(movie);
        
        Message ms;
        if (movie.MovieImages == null || movie.MovieImages.Length == 0) {
            ms = await TelegramProvider.Instance.bot.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                replyToMessageId: messageThreadId
            );
            DataBase.Instance.AddMovieMessageIdDictionary(ms.MessageId, new[] { message, addition });
            return;
        }
        
        ms = await TelegramProvider.Instance.bot.SendPhotoAsync(
            chatId: chatId,
            photo: new InputFileUrl(movie.MovieImages[0]),
            caption: message,
            replyToMessageId: messageThreadId,
            parseMode: ParseMode.Html,
            replyMarkup: new InlineKeyboardMarkup(TelegramProvider.Instance.additionButton)
        );
        
        DataBase.Instance.AddMovieMessageIdDictionary(ms.MessageId, new[] { message, addition });
        
        // additioanl images and trailer
        var arr = new string[movie.MovieImages.Length - 1];
        Array.Copy(movie.MovieImages, 1, arr, 0, movie.MovieImages.Length - 1);
        if (arr.Length > 0)
            await TelegramProvider.Instance.SendMediaGroupImages(arr, chatId, messageThreadId);
        
        if (string.IsNullOrEmpty(movie.MovieTrailer)) return;
        var path = await DownloadVideo(movie.MovieTrailer);
        
        await using FileStream fs = File.OpenRead(path);
        await TelegramProvider.Instance.bot.SendVideoAsync(
            chatId: chatId,
            video: new InputFileStream(fs),
            replyToMessageId: messageThreadId,
            parseMode: ParseMode.Html,
            supportsStreaming: true
        );


        var response = await new ChatAI().SendLastMessage(movie.MovieDetails.original_title);
        await TelegramProvider.Instance.bot.SendTextMessageAsync(
            chatId: chatId,
            text: response + "\n\n" + "🎥🍿 <strong>---NEXT---</strong>. 🍿🎥",
            replyToMessageId: messageThreadId,
            parseMode: ParseMode.Html
        );
    }

     private static string GenerateAdditionalInfo(MovieData movie) {
         var director = movie.Credits["Director"].First();
         var actors = movie.Credits["Cast"].Take(3).ToArray();
         var date = DateTime.Parse(movie.MovieDetails.release_date).ToString("dd MMMM yyyy");
         var addition =
             $"<strong>Director:</strong> {director}\n" +
             $"<strong>Year:</strong> {date}\n" +
             $"<strong>Rating:</strong> {(int)movie.MovieDetails.popularity} / 100\n" +
             $"<strong>Cast:</strong> {string.Join(", ", actors)}\n";

         return addition;
     }
     
     private static async Task<string> DownloadVideo(string url) {
         var path = $"./video.mp4";
         var youtube = new YoutubeClient();
         var video = await youtube.Videos.GetAsync(url);
         // var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
         // var streamInfo = streamManifest
         //     .GetVideoOnlyStreams().Where(s => s.Container == Container.Mp4).GetWithHighestVideoQuality();
         
         await youtube.Videos.DownloadAsync(url, path);
         return path;
     }
}
