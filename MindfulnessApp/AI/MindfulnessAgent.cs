using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindfulnessApp.Configuration;

namespace MindfulnessApp.AI
{
	public class MindfulnessAgent
	{
		private readonly AIAgent _aiAgent;

		private readonly string _basePrompt = @"
You are a compassionate mindfulness guide with expertise in meditation, stress management, 
and emotional wellbeing. Provide gentle, supportive guidance while maintaining a calm and 
understanding tone. Focus on practical, actionable advice that users can implement immediately. 
When appropriate, include simple breathing or mindfulness exercises.

Key guidelines:
- Maintain a warm, supportive tone
- Offer practical, actionable advice
- Include brief mindfulness exercises when relevant
- Acknowledge emotions without judgment
- Encourage self-compassion
- Suggest simple, achievable steps
- Remind users to be present in the moment
";

		
		public MindfulnessAgent(string openAIApiKey)
		{
			string apiKey = AppSecrets.AnthropicApiKey; // Get key from AppSecrets

			if (string.IsNullOrEmpty(apiKey))
			{
				throw new Exception("Anthropic API Key is missing. Ensure it is set in AppSecrets.");
			}

			_aiAgent = new AIAgent(apiKey);
		}

		public async Task<string> ProcessQuery(string query)
		{
			var enhancedPrompt = $"{_basePrompt}\nUser Query: {query}\n\nProvide mindful guidance:";

			var response = await _aiAgent.ProcessQuery(enhancedPrompt);

			if (response == null || string.IsNullOrEmpty(response.Content))
			{
				throw new Exception("Received an empty response from the AI.");
			}

			return response.Content;
		}

		public string GetTopicPrompt(string topic)
		{
			return topic.ToLower() switch
			{
				"stress relief" => "I'm feeling stressed and need some guidance on managing it mindfully.",
				"meditation" => "Can you guide me through a simple meditation practice?",
				"breathing" => "Can you teach me some breathing exercises for relaxation?",
				_ => throw new ArgumentException("Unknown topic")
			};
		}
	}
}
