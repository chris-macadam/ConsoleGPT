using OpenAI_API;
using OpenAI_API.Chat;
using System;
using System.ComponentModel.DataAnnotations;

namespace ConsoleGPT
{
    internal class Program
    {
        public static bool IsRunning { get; private set; } = true;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Welcome to ConsoleGPT!");
            Console.WriteLine();

            Console.WriteLine("Validating API key...");

            APIAuthentication authentication = GetValidAuthentication();

            Console.WriteLine("Validated!");
            Console.WriteLine();

            OpenAIAPI openAI = new OpenAIAPI(authentication);
            Conversation conversation = openAI.Chat.CreateConversation();

            string greeting = "Hello! What can I help you with today?";
            conversation.AppendExampleChatbotOutput(greeting);
            Console.WriteLine(greeting);
            Console.WriteLine();

            do
            {
                string? userInput = GetUserInput();
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    continue;
                }

                Console.WriteLine();

                if (userInput.Trim().ToLower() == "quit" || userInput.Trim().ToLower() == "exit")
                {
                    IsRunning = false;
                    return;
                }

                conversation.AppendUserInput(userInput);
                await conversation.StreamResponseFromChatbotAsync(Console.Write);

                Console.WriteLine();
                Console.WriteLine();
            }
            while (IsRunning == true);
        }

        private static APIAuthentication GetValidAuthentication()
        {
            const string ApiKeyFileName = ".openai";

            APIAuthentication authentication = APIAuthentication.LoadFromPath(filename: ApiKeyFileName);

            while (authentication == null || authentication.ValidateAPIKey().Result == false)
            {
                Console.WriteLine("API key not found or invalid! Please enter your API key: ");
                string? apiKey = GetUserInput();
                Console.WriteLine();

                File.WriteAllText(ApiKeyFileName, $"OPENAI_API_KEY={apiKey}");
                authentication = APIAuthentication.LoadFromPath(ApiKeyFileName);
            }

            return authentication;
        }

        private static string? GetUserInput()
        {
            Console.Write(">> ");
            return Console.ReadLine();
        }
    }
}