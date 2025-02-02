using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;


namespace MindfulnessApp.Configuration
{
	public static class AppSecrets
	{
		// API Keys, Keys stored seperatly for privacy
		public static string OpenAIApiKey => GetSecret("OPENAI_API_KEY");
		public static string AnthropicApiKey => GetSecret("ANTHROPIC_API_KEY");

		// Blockchain Configuration
		public static string ModePrivateKey => GetSecret("MODE_PRIVATE_KEY");
		public static string ModeContractAddress => GetSecret("MODE_CONTRACT_ADDRESS");

		// API Endpoints
		public static string OpenAIEndpoint => GetSecret("OPENAI_ENDPOINT", "https://api.openai.com/v1/");
		public static string AnthropicEndpoint => GetSecret("ANTHROPIC_ENDPOINT", "https://api.anthropic.com/v1/");

		// Helper method to get environment variables with optional default value
		private static string GetSecret(string key, string defaultValue = "")
		{
			var value = Environment.GetEnvironmentVariable(key);
			if (string.IsNullOrEmpty(value))
			{
				// For development, try to load from local secrets file
				value = LoadFromSecretsFile(key);
			}
			return !string.IsNullOrEmpty(value) ? value : defaultValue; ;
		}

		// Helper method to load from local secrets file (development only)
		private static string LoadFromSecretsFile(string key)
		{
			try
			{
				// 🔹 Using absolute path 
				string secretsPath = @"C:\Users\trifo\source\repos\AppData\Roaming\secrets.json";
				

				if (!File.Exists(secretsPath))
				{
					Console.WriteLine($"Warning: Secrets file not found at {secretsPath}");
					return "";
				}

				var jsonContent = File.ReadAllText(secretsPath);
				var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

				return secrets != null && secrets.ContainsKey(key) ? secrets[key] : "";
			}
			catch
			{
				Console.WriteLine($"Error loading secrets file");
				return "";
			}
		}
	}
}