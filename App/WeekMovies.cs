using System.Text.RegularExpressions;
using App;
using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


public struct Movie {
    public string Title;
    public string Description;
    public string Poster;
    public string[] Images;
    public string[] Trailers;
    public string Rating;
    public string Year;
    public string Country;
    public string Genre;
    public string Producer;
    public string[] Actors;
    public string Duration;
}

public class WeekMovies {
    private readonly int _maxImages = 6;
    
    private HtmlWeb web = new HtmlWeb();

    private const string site_url = "https://www.film.ru/";
    
    
    private async Task<Movie?> GetFilmInfo(string name) {
        var site_url = "https://www.film.ru/";
        var url = "https://www.film.ru/search/result?text=" + name + "&type=all";
        
        var doc = web.Load(url);

        var firstFindPage = doc.DocumentNode.SelectSingleNode("//*[@id='movies_list']/a[1]");
        if (firstFindPage == null) {
            Console.WriteLine("Not found any movies");
            return null;
        }
        var main = web.Load(site_url + firstFindPage.Attributes["href"].Value);
        
        var movie = new Movie();
        movie.Trailers = new string[1];
        movie.Images = new string[_maxImages];

        movie.Title = GetTitle(main);
        movie.Description = GetDescription(main);
        movie.Duration = GetDuration(main);
        movie.Actors = GetActors(main);
        movie.Producer = GetProducer(main);
        movie.Genre = GetGenre(main);
        movie.Country = GetCountry(main);
        movie.Year = GetYear(main);
        movie.Rating = GetRating(main);
        
        movie.Trailers = GetTrailers(firstFindPage, movie.Trailers.Length);
        movie.Images = GetImages(firstFindPage, movie.Images.Length);
        
        return movie;
    }
    
    private string GetRating(HtmlDocument str) {
        var rating = str.DocumentNode.SelectSingleNode("//*[@id='block-system-main']/div/div[1]/div[1]/div[3]/div[2]/div[2]/div[1]/div");
        return rating == null ? "" : rating.InnerText;
    }
    
    private string GetYear(HtmlDocument str) {
        var year = str.DocumentNode.SelectSingleNode("//*[@id='block-system-main']/div/div[1]/div[1]/div[3]/div[2]/div[1]/a[1]/text()");
        return year == null ? "" : year.InnerText;
    }
    
    private string GetCountry(HtmlDocument str) {
        var country = str.DocumentNode.SelectNodes("//*[@id='block-system-main']/div/div[1]/div[1]/div[3]/div[2]/div[1]/a");
        return country == null ? "" : country.Last().InnerText;
    }
    
    private string GetGenre(HtmlDocument str) {
        var genre = str.DocumentNode.SelectSingleNode("//*[@id='block-system-main']/div/div[1]/div[1]/div[3]/div[2]/div[1]/a[2]/text()");
        return genre == null ? "" : genre.InnerText;
    }
    
    private string GetProducer(HtmlDocument str) {
        var producer = str.DocumentNode.SelectSingleNode("//*[@id='block-system-main']/div/div[1]/div[1]/div[3]/div[2]/div[5]/div[2]/a");
        return producer == null ? "" : producer.InnerText;
    }
    
    private string[] GetActors(HtmlDocument str) {
        var actors = new List<string>();
        var nodes = str.DocumentNode.SelectNodes("//*[@id='block-system-main']/div/div[4]/div[2]/div[2]/a/strong");
        foreach (var node in nodes) {
            actors.Add(node.InnerText.Trim());
        }
        return actors.ToArray();
    }
    
    private string GetDuration(HtmlDocument str) {
        var duration = str.DocumentNode.SelectSingleNode("//*[@id='block-system-main']/div/div[1]/div[1]/div[3]/div[2]/div[4]/div[2]");
        return duration == null ? "" : duration.InnerText;
    }
    
    private string GetDescription(HtmlDocument str) {
        var description = str.DocumentNode.SelectSingleNode("//*[@id='block-system-main']/div/div[4]/div[2]/div[3]");
        
        return description.InnerText;
    }
    
    private string GetTitle(HtmlDocument str) {
        var title = str.DocumentNode.SelectSingleNode("//*[@id='block-system-main']/div/div[1]/div[1]/div[3]/div[2]/h1");
        return title == null ? "" : title.InnerText;
    }
    
    private string[] GetTrailers(HtmlNode str, int size) {
        var trailers = new string[size];
        
        var frames = web.Load(site_url + str.Attributes["href"].Value + "/trailers");
        for (int i = 0; i < size; i++) {
            var node = frames.DocumentNode.SelectSingleNode($"//*[@id='block-system-main']/div/div[2]/div[2]/div[2]/video-js/source");
            if (node != null)
                trailers[i] = node.Attributes["src"].Value;
            else
                trailers[i] = "";
        }
        return trailers;
    }

    private string[] GetImages(HtmlNode str, int size) {
        var images = new string[size];
        
        var frames = web.Load(site_url + str.Attributes["href"].Value + "/frames");
        for (int i = 0; i < size; i++) {
            var node = frames.DocumentNode.SelectSingleNode($"//*[@id='block-system-main']/div/div/div[2]/div[2]/div[2]/a[{i + 1}]");
            if (node != null)
                images[i] = node.Attributes["data-src"].Value;
        }
        return images;
    }
    
    private async Task<Movie?> GenerateFilm(string name) {
        return await GetFilmInfo(name);
    }
    
     public async void SendToWeekMovies(Message message) {
         var weekMovies = new WeekMovies();
        var movie = await weekMovies.GenerateFilm(message.Text);
        
        if (movie == null) {
            await TelegramProvider.Instance.bot.SendTextMessageAsync(message.Chat.Id, "Фильм не найден", message.MessageThreadId);
        } else {
            SendAboutFilm(movie.Value, message.Chat.Id, message.MessageThreadId);
            
            DataBase.Instance.AddWeekCinemaMovie(movie.Value);
            DataBase.Instance.AddWeekCinemaUser(DataBase.Instance.GetOrNewUser(message));
        }
        
        DataBase.Instance.AddChatIDMessageIDDictionary(message.Chat.Id, message.MessageId);
    }
     
     string ScrubHtml(string value) {
         var pattern = new Regex("&#171");
         var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
         var step2 = Regex.Replace(step1, @"\s{2,}", " ");
         var step3 = pattern.Replace(step2, "");
         var pattern2 = new Regex("(?:&#171|&laquo;)");
         var step4 = pattern2.Replace(step3, "");
         return step4.Replace("&raquo;","");
     }
     
    private async void SendAboutFilm(Movie movie, long chatId, int? messageThreadId = null) {
        var message = ($"<strong>{movie.Title}</strong>" + "\n" + movie.Description + "\n");
        var addition = 
            $"<strong>Жанр:</strong> {movie.Genre}\n" +
            $"<strong>Режиссер:</strong> {movie.Producer}\n" +
            $"<strong>Cтрана:</strong> {movie.Country}\n" + 
            $"<strong>Год:</strong> {movie.Year}\n" +
            $"<strong>Рейтинг:</strong> {movie.Rating}\n" +
            $"<strong>Актеры:</strong> {string.Join(", ", movie.Actors)}\n" +
            $"<strong>Длительность:</strong> {movie.Duration}\n";

        addition = ScrubHtml(addition);
        message = ScrubHtml(message);

        if (movie.Trailers.Length > 0 && movie.Trailers[0] != "") {
            using var httpClient = new HttpClient();
            await using var s = await httpClient.GetStreamAsync(movie.Trailers[0]);
        }
        
        if (movie.Trailers.Length > 0 && movie.Trailers[0] != "") {
            Message ms;
            if (Mode.IsDev) {
                ms = await TelegramProvider.Instance.bot.SendTextMessageAsync(
                    chatId: chatId,
                    text: message,
                    replyToMessageId: messageThreadId,
                    parseMode: ParseMode.Html,
                    replyMarkup: new InlineKeyboardMarkup(TelegramProvider.Instance.additionButton)
                );
            } else {
                using var httpClient = new HttpClient();
                await using var s = await httpClient.GetStreamAsync(movie.Trailers[0]);
                
                ms = await TelegramProvider.Instance.bot.SendVideoAsync(
                    chatId: chatId,
                    video: new InputFileStream(s),
                    caption: message,
                    replyToMessageId: messageThreadId,
                    parseMode: ParseMode.Html,
                    supportsStreaming: true,
                    replyMarkup: new InlineKeyboardMarkup(TelegramProvider.Instance.additionButton)
                );
            }
            
            DataBase.Instance.AddMovieMessageIdDictionary(ms.MessageId, new []{ message, addition });
        } else {
            await TelegramProvider.Instance.bot.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                replyToMessageId: messageThreadId,
                parseMode: ParseMode.Html
            );
        }
        
        if (Mode.IsDev) {}
        else 
            await TelegramProvider.Instance.SendMediaGroupImages(movie.Images, chatId, messageThreadId);
    }
}