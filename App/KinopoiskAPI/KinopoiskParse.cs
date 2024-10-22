// namespace App.KinopoiskAPI; 
// using System;
// using System.Collections.Generic;
//
// public class Film
// {
//     public int FilmId { get; set; }
//     public string NameRu { get; set; }
//     public string NameEn { get; set; }
//     public string Type { get; set; }
//     public string Year { get; set; }
//     public string? Description { get; set; }
//     public string FilmLength { get; set; }
//     public List<Country> Countries { get; set; }
//     public List<Genre> Genres { get; set; }
//     public string Rating { get; set; }
//     public int RatingVoteCount { get; set; }
//     public string PosterUrl { get; set; }
//     public string PosterUrlPreview { get; set; }
// }
//
// public class KinopoiskResponse
// {
//     public string Keyword { get; set; }
//     public int PagesCount { get; set; }
//     public List<Film?> Films { get; set; }
//     public int SearchFilmsCountResult { get; set; }
// }
//
// public class MovieDetails
// {
//     public int? KinopoiskId { get; set; }
//     public string KinopoiskHDId { get; set; }
//     public string ImdbId { get; set; }
//     public string NameRu { get; set; }
//     public string NameEn { get; set; }
//     public string NameOriginal { get; set; }
//     public string PosterUrl { get; set; }
//     public string PosterUrlPreview { get; set; }
//     public string CoverUrl { get; set; }
//     public string LogoUrl { get; set; }
//     public int? ReviewsCount { get; set; }
//     public double? RatingGoodReview { get; set; }
//     public int? RatingGoodReviewVoteCount { get; set; }
//     public double? RatingKinopoisk { get; set; }
//     public int? RatingKinopoiskVoteCount { get; set; }
//     public double? RatingImdb { get; set; }
//     public int? RatingImdbVoteCount { get; set; }
//     public double? RatingFilmCritics { get; set; }
//     public int? RatingFilmCriticsVoteCount { get; set; }
//     public double? RatingAwait { get; set; }
//     public int? RatingAwaitCount { get; set; }
//     public double? RatingRfCritics { get; set; }
//     public int? RatingRfCriticsVoteCount { get; set; }
//     public string WebUrl { get; set; }
//     public int? Year { get; set; }
//     public int? FilmLength { get; set; }
//     public string Slogan { get; set; }
//     public string Description { get; set; }
//     public string ShortDescription { get; set; }
//     public string EditorAnnotation { get; set; }
//     public bool IsTicketsAvailable { get; set; }
//     public string ProductionStatus { get; set; }
//     public string Type { get; set; }
//     public string RatingMpaa { get; set; }
//     public string RatingAgeLimits { get; set; }
//     public bool HasImax { get; set; }
//     public bool Has3D { get; set; }
//     public DateTime LastSync { get; set; }
//     public List<Country> Countries { get; set; }
//     public List<Genre> Genres { get; set; }
//     public int? StartYear { get; set; }
//     public int? EndYear { get; set; }
//     public bool Serial { get; set; }
//     public bool ShortFilm { get; set; }
//     public bool Completed { get; set; }
// }
//
// public class Country
// {
//     public string country { get; set; }
// }
//
// public class Genre
// {
//     public string genre { get; set; }
// }
//
// public class VideoResponse
// {
//     public int Total { get; set; }
//     public List<VideoItem> Items { get; set; }
// }
//
// public class VideoItem
// {
//     public string Url { get; set; }
//     public string Name { get; set; }
//     public SiteType Site { get; set; }
// }
//
// public enum SiteType
// {
//     YOUTUBE,
//     KINOPOISK_WIDGET,
//     YANDEX_DISK,
//     UNKNOWN
// }
//
// public class ImageResponse
// {
//     public int Total { get; set; }
//     public int TotalPages { get; set; }
//     public List<ImageItem> Items { get; set; }
// }
//
// public class ImageItem
// {
//     public string ImageUrl { get; set; }
//     public string PreviewUrl { get; set; }
// }
//
// public class StaffResponse
// {
//     public int StaffId { get; set; }
//     public string NameRu { get; set; }
//     public string NameEn { get; set; }
//     public string Description { get; set; }
//     public string PosterUrl { get; set; }
//     public string ProfessionText { get; set; }
//     public ProfessionKey ProfessionKey { get; set; }
// }
//
// public enum ProfessionKey
// {
//     WRITER, OPERATOR, EDITOR, COMPOSER, PRODUCER_USSR, TRANSLATOR,
//     DIRECTOR, DESIGN, PRODUCER, ACTOR, VOICE_DIRECTOR, UNKNOWN
// }
//
