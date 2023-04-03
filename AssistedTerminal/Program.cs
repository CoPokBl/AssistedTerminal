using AssistedTerminal;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

const string system =
    "You are going to generate bash commands, you will generate them from the description given to you. " +
    "You must reply with the only the command you generate, do not write explanations, ever, do not say ANYTHING AT ALL except the command, " +
    "DO NOT put anything before your command, that includes a prefix like [cmd]. " +
    "If you do not want to not execute a command and instead say something to the user, prefix your message with [msg]. " +
    "If you are telling the user anything at all prefix your message with [msg]. " +
    "If you do not know how to fulfil the request or you can't then prefix your message with [msg]. " +
    "Do not make multiple commands unless it is because the commands are joined by && to make one command. " +
    "If my message starts with [feedback] then I am telling you what the output of the command was. " +
    "If my message starts with [cmd] then I am telling you what command I am executing and you don't need to do anything. ";
string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new Exception("OPENAI_API_KEY environment variable not set!");
Model model = (Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt4").ToLower() switch {
    "gpt3.5" => Model.GPT3_5_Turbo,
    "gpt4" => Model.GPT4,
    _ => throw new Exception("OPENAI_MODEL must be GPT4 or GPT3.5")
};

Bash bash = new();
OpenAIClient client = new(apiKey);

List<ChatPrompt> prompts = new() {
    new ChatPrompt("system", system)
};

async Task<string> GenerateCommand(string desc) {
    if (prompts.Count > 10) {
        prompts.RemoveAt(prompts.Count - 1);
    }
    
    prompts.Add(new ChatPrompt("user", desc));
    while (true) {
        ChatRequest chatRequest = new(prompts, model, maxTokens:100);
        ChatResponse response = await client.ChatEndpoint.GetCompletionAsync(chatRequest);
        prompts.Add(new ChatPrompt("assistant", response.FirstChoice));
        return response.FirstChoice;
    }
}

while (true) {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write(">> ");
    string cmd = Console.ReadLine()!;
    Console.ResetColor();

    if (cmd == "exit") {
        break;
    }

    if (cmd[0] == '/') {
        string feedback = bash.ExecuteCommand(cmd.Remove(0, 1));
        prompts.Add(new ChatPrompt("user", $"[cmd]{cmd}"));
        prompts.Add(new ChatPrompt("user", $"[feedback]{feedback.Shorten(1000)}"));
        continue;
    }
    
    // Use AI to generate a command
    string aiCmd = await GenerateCommand(cmd);
    if (aiCmd == "exit") {
        break;
    }

    if (aiCmd.StartsWith("[msg]")) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[MESSAGE] {aiCmd.Remove(0, 5)}");
        Console.ResetColor();
        continue;
    }
    
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($">> /{aiCmd}");
    Console.ResetColor();
    
    // Execute the command using bash
    string feedbackAi = bash.ExecuteCommand(aiCmd);
    prompts.Add(new ChatPrompt("user", $"[feedback]{feedbackAi.Shorten(1000)}"));
}

bash.Dispose();