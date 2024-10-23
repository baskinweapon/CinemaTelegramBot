using RestSharp;

class TMDBAPI
{
    private static readonly string apiKey = Keys.TMDB; // Замените на ваш ключ
    private static readonly string baseUrl = "https://api.themoviedb.org/3";
    private static readonly string imageBaseUrl = "https://image.tmdb.org/t/p/w500";

    public async Task<MovieData?> SearchMovies(string args) {
        var data = await SearchMovieByName(args);
        return data;
    }

    private static async Task<MovieData?> SearchMovieByName(string movieName)
    {
        var client = new RestClient(baseUrl);

        // Запрос для поиска фильма по имени
        var searchRequest = new RestRequest("/search/movie")
            .AddParameter("api_key", apiKey)
            .AddParameter("query", movieName);

        var searchResponse = await client.ExecuteAsync<MoviesDetails>(searchRequest);

        if (searchResponse.IsSuccessful)
        {
            var result = searchResponse.Data;

            if (result != null && result.results.Count > 0) {
                var movie = result.results[0];

                Console.WriteLine($"Название: {movie.original_title}");
                Console.WriteLine($"Дата выхода: {movie.release_date}");
                Console.WriteLine($"Описание: {movie.overview}");
                Console.WriteLine($"genres: {string.Join(", ", movie.genre_ids)}");
                
                // get data
                var data = new MovieData();
                data.MovieTrailer = await GetMovieTrailer(movie.id);
                data.MovieDetails = await GetMovieDetails(movie.id);
                if (data.MovieDetails != null)
                    data.MovieDetails.genre_ids = movie.genre_ids;
                data.MovieImages = await GetMovieImages(movie.id);
                data.Credits = await GetCredits(movie.id);

                return data;
            }
            else {
                Console.WriteLine("Фильмы не найдены.");
                return null;
            }
        }
        else {
            Console.WriteLine($"Ошибка: {searchResponse.StatusCode}, {searchResponse.Content}");
            return null;
        }
    }

    public static async Task<Genres?> GetAllGenres() {
        var client = new RestClient(baseUrl);

        // Запрос для получения списка жанров
        var genresRequest = new RestRequest("/genre/movie/list")
            .AddParameter("api_key", apiKey);

        var genresResponse = await client.ExecuteAsync<Genres>(genresRequest);

        if (genresResponse.IsSuccessful) {
            var genres = genresResponse.Data;

            if (genres != null && genres.genres.Count > 0) {
                return genres;
            }
            else
            {
                Console.WriteLine("Жанры не найдены.");
                return null;
            }
        }
        else
        {
            Console.WriteLine($"Ошибка при получении жанров: {genresResponse.StatusCode}, {genresResponse.Content}");
            return null;
        }
    }
    
    public static async Task<string> GetRandomMovies(int? genreId = null) {
        var client = new RestClient(baseUrl);
        
        var random = new Random();
        var randomRequest = new RestRequest($"/discover/movie")
            .AddParameter("api_key", apiKey).AddParameter("page", random.Next(1, 100));
        if (genreId != null) {
            randomRequest = new RestRequest($"/discover/movie")
                .AddParameter("api_key", apiKey).AddParameter("page", random.Next(1, 10)).AddParameter("with_genres", genreId.Value);
        }
        
        
        var randomResponse = await client.ExecuteAsync<DiscoverMoviews>(randomRequest);
        
        if (randomResponse.IsSuccessful) {
            var result = randomResponse.Data;
            
            if (result != null && result.results.Count > 0) {
                Console.WriteLine("results.Count: " + result.results.Count);
                var r = random.Next(0, result.results.Count);
                var movie = result.results[r];
                
                return movie.original_title;
            } else {
                Console.WriteLine("Фильмы не найдены.");
                return "";
            }
        } else {
            Console.WriteLine($"Ошибка: {randomResponse.StatusCode}, {randomResponse.Content}");
            return "";
        }
    }

    public static async Task<string[]?> GetMovieImages(int movieId)
    {
        var client = new RestClient(baseUrl);

        // Запрос для получения изображений фильма по ID
        var imagesRequest = new RestRequest($"/movie/{movieId}/images")
            .AddParameter("api_key", apiKey);
        
        var imagesResponse = await client.ExecuteAsync<MovieImages>(imagesRequest);
        
        if (imagesResponse.IsSuccessful) {
            var imageArr = new List<string>();
            
            var images = imagesResponse.Data;

            Console.WriteLine("Изображения фильма:");
            
            // Выводим постеры
            if (images.posters.Count > 0) {
                Console.WriteLine("Постеры:");
                for (var index = 0; index < images.posters.Count; index++) {
                    var poster = images.posters[index];
                    imageArr.Add($"{imageBaseUrl}{poster.file_path}"); 
                    break;
                }
            }
            else {
                Console.WriteLine("Постеры не найдены.");
            }

            // Выводим фоны
            if (images.backdrops.Count > 0) {
                // images.backdrops.Sort((a, b) => (a.height + a.width).CompareTo(b.height + b.width));
                
                for (var index = images.backdrops.Count - 1; index >= 0; index--) {
                    
                    var backdrop = images.backdrops[index];
                    
                    if (imageArr.Count <= 5 && !imageArr.Contains(backdrop.file_path)) {
                        imageArr.Add($"{imageBaseUrl}{backdrop.file_path}");
                    } else {
                        break;
                    }
                }
            } else {
                Console.WriteLine("Фоны не найдены.");
            }

            // Выводим логотипы (если нужны)
            // if (images.logos.Count > 0)
            // {
            //     Console.WriteLine("Логотипы:");
            //     foreach (var logo in images.logos)
            //     {
            //         Console.WriteLine($"- {imageBaseUrl}{logo.file_path} (Размер: {logo.width}x{logo.height})");
            //     }
            // }
            // else
            // {
            //     Console.WriteLine("Логотипы не найдены.");
            // }
            
            if (imageArr.Count > 0) {
                return imageArr.ToArray();
            } else {
                return null;
            }
        }
        else
        {
            Console.WriteLine($"Ошибка при получении изображений: {imagesResponse.StatusCode}, {imagesResponse.Content}");
            return null;
        }
    }

    public static async Task<string?> GetMovieTrailer(int movieId)
    {
        var client = new RestClient(baseUrl);

        // Запрос для получения видео по ID фильма
        var videoRequest = new RestRequest($"/movie/{movieId}/videos")
            .AddParameter("api_key", apiKey);
        
        var videoResponse = await client.ExecuteAsync<MovieTrailer>(videoRequest);
        if (videoResponse.IsSuccessful)
        {
            var videoResult = videoResponse.Data;

            if (videoResult != null && videoResult.results.Count > 0)
            {
                var trailer = videoResult.results.Find(v => v.type == "Trailer" && v.site == "YouTube");
                
                if (trailer != null) {
                    Console.WriteLine($"Трейлер: https://www.youtube.com/watch?v={trailer.key}");
                    return $"https://www.youtube.com/watch?v={trailer.key}";
                }
                else {
                    Console.WriteLine("Трейлер не найден.");
                    return null;
                }
            }
            else {
                Console.WriteLine("Видео не найдено.");
                return null;
            }
        }
        else {
            Console.WriteLine($"Ошибка: {videoResponse.StatusCode}");
            return null;
        }
        
        return null;
    }

    private static async Task<MovieDetails?> GetMovieDetails(int movieId)
    {
        var client = new RestClient(baseUrl);

        // Запрос для получения полной информации о фильме по ID
        var detailsRequest = new RestRequest($"/movie/{movieId}")
            .AddParameter("api_key", apiKey);

        var detailsResponse = await client.ExecuteAsync<MovieDetails>(detailsRequest);
        
        if (detailsResponse.IsSuccessful)
        {
            var movie = detailsResponse.Data;
            return movie;
        }
        else {
            Console.WriteLine($"Ошибка при получении деталей: {detailsResponse.StatusCode}, {detailsResponse.Content}");
            return null;
        }
    }
    
    private static async Task<Dictionary<string, string[]>?> GetCredits(int movieId)
    {
        var client = new RestClient(baseUrl);

        // Запрос для получения информации о создателях фильма по ID
        var creditsRequest = new RestRequest($"/movie/{movieId}/credits")
            .AddParameter("api_key", apiKey);

        var creditsResponse = await client.ExecuteAsync<Credits>(creditsRequest);
        
        if (creditsResponse.IsSuccessful)
        {
            var dict = new Dictionary<string, string[]>();
            
            var credits = creditsResponse.Data;
        
            if (credits != null)
            {
                // Console.WriteLine("Режиссеры:");
                foreach (var crew in credits.crew)
                {
                    if (crew.job == "Director")
                    {
                        if (dict.ContainsKey("Director")) {
                            var arr = dict["Director"];
                            var secondArr = new string[arr.Length + 1];
                            arr.CopyTo(secondArr, 0);
                            secondArr[arr.Length] = crew.name;
                            dict["Director"] = secondArr;
                        } else
                            dict.Add("Director", new string[] {crew.name});
                        // Console.WriteLine($"- {crew.name}");
                    }
                }
        
                // Console.WriteLine("Актеры:");
                foreach (var cast in credits.cast)
                {
                    if (dict.ContainsKey("Cast")) {
                        var arr = dict["Cast"];
                        var secondArr = new string[arr.Length + 1];
                        arr.CopyTo(secondArr, 0);
                        secondArr[arr.Length] = cast.name;
                        dict["Cast"] = secondArr;
                    } else
                        dict.Add("Cast", new string[] {cast.name});
                    // Console.WriteLine($"- {cast.name}");
                }
                
                if (dict.Count > 0) {
                    return dict;
                } else {
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Информация о создателях не найдена.");
                return null;
            }
        }
        else
        {
            Console.WriteLine($"Ошибка при получении информации о создателях: {creditsResponse.StatusCode}, {creditsResponse.Content}");
            return null;
        }
    }  
}





