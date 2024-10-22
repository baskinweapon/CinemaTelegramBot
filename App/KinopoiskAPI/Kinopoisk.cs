// using System.Text.RegularExpressions;
// using App;
// using App.KinopoiskAPI;
// using Newtonsoft.Json;
// using Telegram.Bot;
// using Telegram.Bot.Types;
// using Telegram.Bot.Types.Enums;
// using Telegram.Bot.Types.ReplyMarkups;
// // using VideoLibrary;
//
// using YoutubeExplode;
//
// // public struct Movie {
// //     public string Title;
// //     public string Description;
// //     public string Poster;
// //     public string[] Images;
// //     public string[] Trailers;
// //     public string Rating;
// //     public string Year;
// //     public string Country;
// //     public string Genre;
// //     public string Producer;
// //     public string[] Actors;
// //     public string Duration;
// // }
//
// public class Kinopoisk {
//     private int _maxImages = 5;
//     private readonly int maxActors = 7;
//     private HttpClient sharedClient = new() {
//         BaseAddress = new Uri("https://kinopoiskapiunofficial.tech"),
//     };
//     
//     public async void SendToWeekMovies(Message message) {
//         var movie = await GetFilmInfo(message.Text);
//         
//         if (movie == null) {
//             await TelegramProvider.Instance.bot.SendTextMessageAsync(message.Chat.Id, "🤨 Я не смог найти фильм 🤨", message.MessageThreadId);
//         } else {
//             SendAboutFilm(movie.Value, message.Chat.Id, message.MessageThreadId);
//             
//             DataBase.Instance.AddWeekCinemaMovie(movie.Value);
//             DataBase.Instance.AddWeekCinemaUser(DataBase.Instance.GetOrNewUser(message));
//         }
//         
//         DataBase.Instance.AddChatIDMessageIDDictionary(message.Chat.Id, message.MessageId);
//     }
//     
//     private async void SendAboutFilm(Movie movie, long chatId, int? messageThreadId = null) {
//         var message = ($"<strong>{movie.Title}</strong>" + "\n\n" + movie.Description + "\n");
//         var addition = 
//             $"<strong>Жанр:</strong> {movie.Genre}\n" +
//             $"<strong>Режиссер:</strong> {movie.Producer}\n" +
//             $"<strong>Cтрана:</strong> {movie.Country}\n" + 
//             $"<strong>Год:</strong> {movie.Year}\n" +
//             $"<strong>Рейтинг:</strong> {movie.Rating}\n" +
//             $"<strong>Актеры:</strong> {string.Join(", ", movie.Actors)}\n" +
//             $"<strong>Длительность:</strong> {movie.Duration}\n";
//         
//         var youtube = new YoutubeClient();
//         if (movie.Trailers.Length > 0 && movie.Trailers[0] != "") {
//             Message ms;
//             foreach (var trailer in movie.Trailers) {
//                 
//                 YoutubeExplode.Videos.Video video;
//                 try {
//                     video = await youtube.Videos.GetAsync(trailer);
//
//                     var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
//                     var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();
//                     if (muxedStreams.Any()) {
//                         var streamInfo = muxedStreams.First();
//                         using var httpClient = new HttpClient();
//                         var stream = await httpClient.GetStreamAsync(streamInfo.Url);
//
//                         ms = await TelegramProvider.Instance.bot.SendVideoAsync(
//                             chatId: chatId,
//                             video: new InputFileStream(stream),
//                             caption: message,
//                             replyToMessageId: messageThreadId,
//                             parseMode: ParseMode.Html,
//                             supportsStreaming: true,
//                             replyMarkup: new InlineKeyboardMarkup(TelegramProvider.Instance.additionButton)
//                         );
//
//                         DataBase.Instance.AddMovieMessageIdDictionary(ms.MessageId, new[] { message, addition });
//                         break;
//                     }
//                 }
//                 catch (Exception e) {
//                     try {
//                         video = await youtube.Videos.GetAsync("https://www.youtube.com/watch?v=RqJVa0fl01w");
//
//                         var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
//                         var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();
//                         if (muxedStreams.Any()) {
//                             var streamInfo = muxedStreams.First();
//                             using var httpClient = new HttpClient();
//                             var stream = await httpClient.GetStreamAsync(streamInfo.Url);
//                             
//                             ms = await TelegramProvider.Instance.bot.SendVideoAsync(
//                                 chatId: chatId,
//                                 video: new InputFileStream(stream),
//                                 caption: message,
//                                 replyToMessageId: messageThreadId,
//                                 parseMode: ParseMode.Html,
//                                 supportsStreaming: true,
//                                 replyMarkup: new InlineKeyboardMarkup(TelegramProvider.Instance.additionButton)
//                             );
//
//                             DataBase.Instance.AddMovieMessageIdDictionary(ms.MessageId, new[] { message, addition });
//                         }
//                     }
//                     catch (Exception ex) {
//                         Console.WriteLine("have links but dont parse");
//                         ms = await TelegramProvider.Instance.bot.SendTextMessageAsync(
//                             chatId: chatId,
//                             replyToMessageId: messageThreadId,
//                             text: message,
//                             parseMode: ParseMode.Html);
//                         
//                         DataBase.Instance.AddMovieMessageIdDictionary(ms.MessageId, new[] { message, addition });
//                     }
//                     break;
//                 }
//             }
//         } else {
//             YoutubeExplode.Videos.Video video;
//             try {
//                 video = await youtube.Videos.GetAsync("https://www.youtube.com/watch?v=RqJVa0fl01w");
//
//                 var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
//                 var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();
//                 if (muxedStreams.Any()) {
//                     var streamInfo = muxedStreams.First();
//                     using var httpClient = new HttpClient();
//                     var stream = await httpClient.GetStreamAsync(streamInfo.Url);
//
//                     var ms = await TelegramProvider.Instance.bot.SendVideoAsync(
//                         chatId: chatId,
//                         video: new InputFileStream(stream),
//                         caption: message,
//                         replyToMessageId: messageThreadId,
//                         parseMode: ParseMode.Html,
//                         supportsStreaming: true,
//                         replyMarkup: new InlineKeyboardMarkup(TelegramProvider.Instance.additionButton)
//                     );
//
//                     DataBase.Instance.AddMovieMessageIdDictionary(ms.MessageId, new[] { message, addition });
//                 }
//             }
//             catch (Exception e) {
//                 var ms = await TelegramProvider.Instance.bot.SendTextMessageAsync(
//                     chatId: chatId,
//                     replyToMessageId: messageThreadId,
//                     text: message,
//                     parseMode: ParseMode.Html);
//                     
//                 DataBase.Instance.AddMovieMessageIdDictionary(ms.MessageId, new[] { message, addition });
//                 Console.WriteLine("No links and dont parse");
//             }
//         }
//         
//         
//         await TelegramProvider.Instance.SendMediaGroupImages(movie.Images, chatId, messageThreadId);
//     }
//
//     private async Task<Movie?> GetFilmInfo(string name) {
//         var search = await GetFilm(name);
//         if (search == null || search.Description == null) {
//             Console.WriteLine("Not found any movies");
//             return null;
//         }
//
//         var filmID = GetFilm(search.FilmId);
//         if (filmID == null) {
//             Console.WriteLine("Not found any movies");
//             return null;
//         }
//
//         var result = filmID.Result;
//         
//         var movie = new Movie();
//         movie.Trailers = new string[1];
//         movie.Images = new string[_maxImages];
//
//         // movie.Title = result.NameRu;
//         // movie.Description = result.Description;
//         // movie.Duration = result.FilmLength.ToString();
//         // movie.Actors = await GetActors(search.FilmId);
//         // movie.Producer = await GetProducer(search.FilmId);
//         // movie.Genre = result.Genres[0].genre;
//         // movie.Country = result.Countries[0].country;
//         // movie.Poster = await GetPoster(search.FilmId);
//         // movie.Year = result.Year.ToString();
//         // movie.Rating = result.RatingKinopoisk.ToString();
//
//         var trailers = await GetTrailers(search.FilmId);
//         movie.Trailers = trailers.ToArray();
//         var images = await GetImages(search.FilmId);
//         movie.Images = images.ToArray();
//         
//         return movie;
//     }
//
//     private async Task<string[]> GetActors(int id) {
//         var client = new HttpClient();
//         string url = sharedClient.BaseAddress + "api/v1/staff?filmId=" + id;
//         client.DefaultRequestHeaders.Add("X-API-KEY", "dbca33db-8a14-4014-a82a-cd552431339e");
//         
//         HttpResponseMessage response = await client.GetAsync(url);
//         response.EnsureSuccessStatusCode();
//         string responseBody = await response.Content.ReadAsStringAsync();
//         
//         List<StaffResponse>? res = JsonConvert.DeserializeObject<List<StaffResponse>>(responseBody);
//
//
//         var actors = new List<string>();
//         for (int i = 0; i < res.Count; i++) {
//             var staff = res[i];
//             if (i >= maxActors) return actors.ToArray();
//             actors.Add(staff.NameRu);
//         }
//         foreach (var staff in res) {
//             if (staff.ProfessionKey == ProfessionKey.ACTOR) {
//                 actors.Add(staff.NameRu);
//             }
//         }
//
//         return actors.ToArray();
//     }
//     
//     private async Task<string> GetProducer(int id) {
//         var client = new HttpClient();
//         string url = sharedClient.BaseAddress + "api/v1/staff?filmId=" + id;
//         client.DefaultRequestHeaders.Add("X-API-KEY", "dbca33db-8a14-4014-a82a-cd552431339e");
//         
//         HttpResponseMessage response = await client.GetAsync(url);
//         response.EnsureSuccessStatusCode();
//         string responseBody = await response.Content.ReadAsStringAsync();
//         
//         List<StaffResponse>? res = JsonConvert.DeserializeObject<List<StaffResponse>>(responseBody);
//         
//         foreach (var staff in res) {
//             if (staff.ProfessionKey == ProfessionKey.DIRECTOR) {
//                 return staff.NameRu;
//             }
//         }
//         return null;
//     }
//
//     private async Task<string> GetPoster(int id) {
//         var client = new HttpClient();
//         string url = sharedClient.BaseAddress + "api/v2.2/films/" + id + "/images";
//         client.DefaultRequestHeaders.Add("X-API-KEY", "dbca33db-8a14-4014-a82a-cd552431339e");
//         client.DefaultRequestHeaders.Add("TYPE", "POSTER");
//
//         HttpResponseMessage response = await client.GetAsync(url);
//         response.EnsureSuccessStatusCode();
//         string responseBody = await response.Content.ReadAsStringAsync();
//         
//         ImageResponse? res = JsonConvert.DeserializeObject<ImageResponse>(responseBody);
//         
//         if (res is { Items: not null }) {
//             foreach (var item in res.Items) {
//                 return item.ImageUrl;
//             }
//         }
//         return null;
//     }
//
//     private async Task<List<string>> GetImages(int id) {
//         var client = new HttpClient();
//         string url = sharedClient.BaseAddress + "api/v2.2/films/" + id + "/images";
//         client.DefaultRequestHeaders.Add("X-API-KEY", "dbca33db-8a14-4014-a82a-cd552431339e");
//         client.DefaultRequestHeaders.Add("TYPE", "SCREENSHOT");
//
//         HttpResponseMessage response = await client.GetAsync(url);
//         response.EnsureSuccessStatusCode();
//         string responseBody = await response.Content.ReadAsStringAsync();
//         
//         ImageResponse? res = JsonConvert.DeserializeObject<ImageResponse>(responseBody);
//
//         var images = new List<string>();
//         if (res is { Items: not null }) {
//             for (int i = 0; i < res.Items.Count; i++) {
//                 var image = res.Items[i];
//                 if (i >= _maxImages) return images;
//                 images.Add(image.PreviewUrl);
//             }
//             return images;
//         }
//         return null;
//     }
//
//     private async Task<List<string>> GetTrailers(int id) {
//         var client = new HttpClient();
//         string url = sharedClient.BaseAddress + "api/v2.2/films/" + id + "/videos";
//         client.DefaultRequestHeaders.Add("X-API-KEY", "dbca33db-8a14-4014-a82a-cd552431339e");
//
//         HttpResponseMessage response = await client.GetAsync(url);
//         response.EnsureSuccessStatusCode();
//         string responseBody = await response.Content.ReadAsStringAsync();
//         
//         VideoResponse? res = JsonConvert.DeserializeObject<VideoResponse>(responseBody);
//
//         var trailersUrl = new List<string>();
//         
//         if (res != null) {
//             foreach (var item in res.Items) {
//                 if (item.Site == SiteType.YOUTUBE) {
//                     if (item.Url.Contains("https://www.youtube.com/watch?v=")) {
//                         trailersUrl.Add(item.Url);
//                     } else {
//                         var uri = "https://www.youtube.com/watch?v=" + item.Url.Split("/").Last();
//                         trailersUrl.Add(uri);
//                     }
//                 }
//             }
//             return trailersUrl;
//         }
//         return null;
//     }
//     
//     private async Task<MovieDetails> GetFilm(int id) {
//         var client = new HttpClient();
//         string url = sharedClient.BaseAddress + "api/v2.2/films/" + id;
//         client.DefaultRequestHeaders.Add("X-API-KEY", "dbca33db-8a14-4014-a82a-cd552431339e");
//
//         HttpResponseMessage response = await client.GetAsync(url);
//         response.EnsureSuccessStatusCode();
//         string responseBody = await response.Content.ReadAsStringAsync();
//         
//         MovieDetails? res = JsonConvert.DeserializeObject<MovieDetails>(responseBody);
//
//         if (res != null) 
//             return res;
//         return null;
//     }
//
//     private async Task<Film?> GetFilm(string name) {
//         string url = sharedClient.BaseAddress + "api/v2.1/films/search-by-keyword?keyword=" + name + "&page=1";
//         sharedClient.DefaultRequestHeaders.Add("X-API-KEY", "dbca33db-8a14-4014-a82a-cd552431339e");
//
//         HttpResponseMessage response = await sharedClient.GetAsync(url);
//         response.EnsureSuccessStatusCode();
//         string responseBody = await response.Content.ReadAsStringAsync();
//         
//         KinopoiskResponse? res = JsonConvert.DeserializeObject<KinopoiskResponse>(responseBody);
//
//         // Example of accessing data
//         foreach (var film in res.Films)
//             return film;
//         return null;
//     }
//     
//     string ScrubHtml(string value) {
//         var pattern = new Regex("&#171");
//         var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
//         var step2 = Regex.Replace(step1, @"\s{2,}", " ");
//         var step3 = pattern.Replace(step2, "");
//         var pattern2 = new Regex("(?:&#171|&laquo;)");
//         var step4 = pattern2.Replace(step3, "");
//         return step4.Replace("&raquo;","");
//     }
// }
//
