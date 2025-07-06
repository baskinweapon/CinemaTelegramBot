using OpenAI.Assistants;
using OpenAI.Chat;

public class ChatAI {

    public void Init() {
        ChatClient client = new(model: "gpt-4o-mini", apiKey: Keys.ChatGPTToken);

        ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
    }
    
    public async Task<string> SendLastMessage(string movieName) {
        ChatClient client = new(model: "gpt-4o-mini", apiKey: Keys.ChatGPTToken);
        
        ChatCompletion completion = await client.CompleteChatAsync($"Верни два предложения где ты хвалишь меня за отличный выбор фильма, добавляй emoji по смыслу. фильм - {movieName}. не пиги ничего другого кроме двух предложений. на английском языке c молодежным сленгом.");

        Console.WriteLine($"{completion.Content[0].Text}");
        return completion.Content[0].Text;
    }

    public async Task<string> GetRandomMovie() {
        ChatClient client = new(model: "gpt-4o-mini", apiKey: Keys.ChatGPTToken);
        
        ChatCompletion completion = await client.CompleteChatAsync("Get me a random movie. send only name. movie должно существовать в реальности. выбирай на свой вкус и каждый раз разное, не повторяйся");

        Console.WriteLine($"{completion.Content[0].Text}");
        return completion.Content[0].Text;
    }
    
   

}
