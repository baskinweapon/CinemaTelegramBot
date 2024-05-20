public class Original {
    public string height { get; set; }
    public string width { get; set; }
    public string size { get; set; }
    public string url { get; set; }
    public string mp4_size { get; set; }
    public string mp4 { get; set; }
    public string webp_size { get; set; }
    public string webp { get; set; }
    public string frames { get; set; }
    public string hash { get; set; }

}
public class Fixed_height {
    public string height { get; set; }
    public string width { get; set; }
    public string size { get; set; }
    public string url { get; set; }
    public string mp4_size { get; set; }
    public string mp4 { get; set; }
    public string webp_size { get; set; }
    public string webp { get; set; }

}
public class Fixed_height_downsampled {
    public string height { get; set; }
    public string width { get; set; }
    public string size { get; set; }
    public string url { get; set; }
    public string webp_size { get; set; }
    public string webp { get; set; }

}
public class Fixed_height_small {
    public string height { get; set; }
    public string width { get; set; }
    public string size { get; set; }
    public string url { get; set; }
    public string mp4_size { get; set; }
    public string mp4 { get; set; }
    public string webp_size { get; set; }
    public string webp { get; set; }

}
public class Fixed_width {
    public string height { get; set; }
    public string width { get; set; }
    public string size { get; set; }
    public string url { get; set; }
    public string mp4_size { get; set; }
    public string mp4 { get; set; }
    public string webp_size { get; set; }
    public string webp { get; set; }

}
public class Fixed_width_downsampled {
    public string height { get; set; }
    public string width { get; set; }
    public string size { get; set; }
    public string url { get; set; }
    public string webp_size { get; set; }
    public string webp { get; set; }

}
public class Fixed_width_small {
    public string height { get; set; }
    public string width { get; set; }
    public string size { get; set; }
    public string url { get; set; }
    public string mp4_size { get; set; }
    public string mp4 { get; set; }
    public string webp_size { get; set; }
    public string webp { get; set; }

}
public class Images {
    public Original original { get; set; } 
    public Fixed_height fixed_height { get; set; } 
    public Fixed_height_downsampled fixed_height_downsampled { get; set; } 
    public Fixed_height_small fixed_height_small { get; set; } 
    public Fixed_width fixed_width { get; set; } 
    public Fixed_width_downsampled fixed_width_downsampled { get; set; } 
    public Fixed_width_small fixed_width_small { get; set; } 

}
public class User {
    public string avatar_url { get; set; }
    public string banner_image { get; set; }
    public string banner_url { get; set; }
    public string profile_url { get; set; }
    public string username { get; set; }
    public string display_name { get; set; }
    public string description { get; set; }
    public string instagram_url { get; set; }
    public string website_url { get; set; }
    public bool is_verified { get; set; }

}
public class Onload {
    public string url { get; set; }

}
public class Onclick {
    public string url { get; set; }

}
public class Onsent {
    public string url { get; set; }

}
public class Analytics {
    public Onload onload { get; set; } 
    public Onclick onclick { get; set; } 
    public Onsent onsent { get; set; } 

}
public class Data {
    public string type { get; set; }
    public string id { get; set; }
    public string url { get; set; }
    public string slug { get; set; }
    public string bitly_gif_url { get; set; }
    public string bitly_url { get; set; }
    public string embed_url { get; set; }
    public string username { get; set; }
    public string source { get; set; }
    public string title { get; set; }
    public string rating { get; set; }
    public string content_url { get; set; }
    public string source_tld { get; set; }
    public string source_post_url { get; set; }
    public int is_sticker { get; set; }
    public DateTime import_datetime { get; set; }
    public string trending_datetime { get; set; }
    public Images images { get; set; } 
    public User user { get; set; } 
    public string analytics_response_payload { get; set; }

}
public class Pagination {
    public int total_count { get; set; }
    public int count { get; set; }
    public int offset { get; set; }

}
public class Meta {
    public int status { get; set; }
    public string msg { get; set; }
    public string response_id { get; set; }

}
public class Application {
    public IList<Data> data { get; set; }
    public Pagination pagination { get; set; } 
    public Meta meta { get; set; } 

}