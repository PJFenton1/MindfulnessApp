using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MindfulnessApp.AI
{
	public class AIAgent
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		private readonly string _baseUrl;

		public AIAgent(string apiKey, string baseUrl = "https://api.anthropic.com/v1/")
		{
			_apiKey = apiKey;
			_baseUrl = baseUrl;
			_httpClient = new HttpClient();
			
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
			_httpClient.DefaultRequestHeaders.Add("Anthropic-Version", "2023-06-01");  // Required for Anthropic API
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public class AgentResponse
		{
			public string Content { get; set; }
		}

		public async Task<AgentResponse> ProcessQuery(string query)
		{
			try
			{
				Console.WriteLine($"🟡 Sending AI request: {query}");

				var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}complete", new
				{
					model = "claude-2",
					prompt = query,
					max_tokens_to_sample = 300,
					temperature = 0.7
				});

				string responseText = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"🟢 API Response: {responseText}");

				if (!response.IsSuccessStatusCode)
				{
					throw new Exception($"❌ API Error {response.StatusCode}: {responseText}");
				}

				using var jsonDoc = JsonDocument.Parse(responseText);
				if (!jsonDoc.RootElement.TryGetProperty("completion", out JsonElement completionElement))
				{
					throw new Exception("❌ Invalid API response format: 'completion' field missing.");
				}

				return new AgentResponse { Content = completionElement.GetString() };
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ AI ERROR: {ex.GetType()} - {ex.Message}");
				throw;
			}
		}
		
		
	}
}
