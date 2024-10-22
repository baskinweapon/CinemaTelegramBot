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
        
        Console.WriteLine("Try stream video");
        if (string.IsNullOrEmpty(movie.MovieTrailer)) return;
        var path = await DownloadVideo(movie.MovieTrailer);

        Console.WriteLine("Saved video");
        await using FileStream fs = File.OpenRead(path);
        Console.WriteLine("Start stream video " + fs.Length);
        
        var response = await new ChatAI().SendLastMessage(movie.MovieDetails.original_title);
        response += "\n\n" + "🎥🍿 <strong>---NEXT---</strong>🍿🎥";
        await TelegramProvider.Instance.bot.SendVideoAsync(
            chatId: chatId,
            video: new InputFileStream(fs),
            caption: response,
            replyToMessageId: messageThreadId,
            parseMode: ParseMode.Html,
            supportsStreaming: true
        );
        
        Console.WriteLine("End stream video");
    }

     private static string GenerateAdditionalInfo(MovieData movie) { 
         var director = "";
         if (movie.Credits != null && movie.Credits.TryGetValue("Director", out var credit)) {
            director = credit.First(); 
         }
         string[] actors = new[] { "", "" };
         if (movie.Credits != null && movie.Credits.TryGetValue("Cast", out var cast)) {
                actors = cast.Take(3).ToArray(); 
         }
         
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
         if (File.Exists(path)) {
             File.Delete(path);
         }
        var youtube = new YoutubeClient();
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
         
        // Select best audio stream (highest bitrate)
         var audioStreamInfo = streamManifest
             .GetAudioStreams()
             .Where(s => s.Container == Container.Mp4)
             .GetWithHighestBitrate();

        // Select best video stream (1080p60 in this example)
         var videoStreamInfo = streamManifest
             .GetVideoStreams()
             // .Where(s => s.Container == Container.Mp4)
             .FirstOrDefault(s => s.VideoQuality.MaxHeight == 720);
         
         if (videoStreamInfo == null) {
                videoStreamInfo = streamManifest
                    .GetVideoStreams().GetWithHighestVideoQuality();
         }
         
         // Download and mux streams into a single file
         var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
         await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(path).Build());
         
         return path;
     }
}
