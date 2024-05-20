public enum TopicType {
    EnglishCinema,
    TeaCeremony,
    Review,
    NightChillChat,
    WeekMovies
}

public class GroupTopics {
    Dictionary<TopicType, object> TestTopics = new() {
        {TopicType.EnglishCinema, 5},
        {TopicType.TeaCeremony, 6},
        {TopicType.Review, 3},
        {TopicType.NightChillChat, 2},
        {TopicType.WeekMovies, 4}
    };
    
    Dictionary<TopicType, object> Topics = new() {
        {TopicType.EnglishCinema, 82},
        {TopicType.TeaCeremony, 80},
        {TopicType.Review, 77},
        {TopicType.NightChillChat, 2},
        {TopicType.WeekMovies, 6}
    };
    
    public TopicType GetTopicType(int id) {
        if (Mode.IsDev) {
            foreach (var topic in TestTopics) {
                if (topic.Value is int topicId && topicId == id) {
                    return topic.Key;
                }
            }
        }
        else {
            foreach (var topic in Topics) {
                if (topic.Value is int topicId && topicId == id) {
                    return topic.Key;
                }
            }
        }
        
        
        return TopicType.NightChillChat;
    }
    
}