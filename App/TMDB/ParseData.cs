
#region MovieDetails

public class MovieDetails
{
    public bool adult { get; set; }
    public string backdrop_path { get; set; }
    public int[] genre_ids { get; set; }
    public int id { get; set; }
    public string original_language { get; set; }
    public string original_title { get; set; }
    public string overview { get; set; }
    public double popularity { get; set; }
    public string poster_path { get; set; }
    public string release_date { get; set; }
    public string title { get; set; }
    public bool video { get; set; }
    public double vote_average { get; set; }
    public int vote_count { get; set; }
}

public class MoviesDetails
{
    public int page { get; set; }
    public List<MovieDetails> results { get; set; }
    public int total_pages { get; set; }
    public int total_results { get; set; }
}

#endregion



#region Trailers

public class Trailer
{
    public string iso_639_1 { get; set; }
    public string iso_3166_1 { get; set; }
    public string name { get; set; }
    public string key { get; set; }
    public string site { get; set; }
    public int size { get; set; }
    public string type { get; set; }
    public bool official { get; set; }
    public DateTime published_at { get; set; }
    public string id { get; set; }
}

public class MovieTrailer
{
    public int id { get; set; }
    public List<Trailer> results { get; set; }
}

#endregion



#region MovieImages

    public class Backdrop
    {
        public double aspect_ratio { get; set; }
        public int height { get; set; }
        public string iso_639_1 { get; set; }
        public string file_path { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public int width { get; set; }
    }

    public class Logo
    {
        public double aspect_ratio { get; set; }
        public int height { get; set; }
        public string iso_639_1 { get; set; }
        public string file_path { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public int width { get; set; }
    }

    public class Poster
    {
        public double aspect_ratio { get; set; }
        public int height { get; set; }
        public string iso_639_1 { get; set; }
        public string file_path { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public int width { get; set; }
    }

    public class MovieImages
    {
        public List<Backdrop> backdrops { get; set; }
        public int id { get; set; }
        public List<Logo> logos { get; set; }
        public List<Poster> posters { get; set; }
    }

#endregion

#region Credits

public class Cast
{
    public bool? adult { get; set; }
    public int? gender { get; set; }
    public int? id { get; set; }
    public string known_for_department { get; set; }
    public string name { get; set; }
    public string original_name { get; set; }
    public double? popularity { get; set; }
    public string profile_path { get; set; }
    public int? cast_id { get; set; }
    public string character { get; set; }
    public string credit_id { get; set; }
    public int? order { get; set; }
}

public class Crew
{
    public bool? adult { get; set; }
    public int? gender { get; set; }
    public int? id { get; set; }
    public string known_for_department { get; set; }
    public string name { get; set; }
    public string original_name { get; set; }
    public double? popularity { get; set; }
    public string profile_path { get; set; }
    public string credit_id { get; set; }
    public string department { get; set; }
    public string job { get; set; }
}

public class Credits
{
    public int? id { get; set; }
    public List<Cast> cast { get; set; }
    public List<Crew> crew { get; set; }
}

#endregion

public class MovieData {
    public MovieDetails? MovieDetails { get; set; }
    public string? MovieTrailer { get; set; }
    public string[]? MovieImages { get; set; }
    public Dictionary<string, string[]>? Credits { get; set; }
}

public class DiscoverMoviews
{
    public int? page { get; set; }
    public List<MovieDetails> results { get; set; }
    public int? total_pages { get; set; }
    public int? total_results { get; set; }
}

public class Genre
{
    public int? id { get; set; }
    public string name { get; set; }
}

public class Genres
{
    public List<Genre> genres { get; set; }
}