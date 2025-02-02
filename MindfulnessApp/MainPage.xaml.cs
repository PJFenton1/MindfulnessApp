using MindfulnessApp.Services;
using MindfulnessApp.AI;
using MindfulnessApp.Configuration;
using System;
using System.IO;

namespace MindfulnessApp
{
	public partial class MainPage : ContentPage
	{
		private readonly MindfulnessAgent _mindfulness;
		private readonly ModeBlockchainService _blockchain;

		public MainPage()
		{
			InitializeComponent();

			// 🔹 Step 1: Check if API Key is loaded correctly
			string apiKey = AppSecrets.AnthropicApiKey;
			

			// 🔹 Write API Key to a file for debugging
			string debugFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "debug_log.txt");
			File.WriteAllText(debugFilePath, $"🔑 API Key Loaded: '{apiKey}'");

			// 🔹 Display an alert so we know it executed
			DisplayAlert("Debug", $"API Key written to debug_log.txt", "OK");

			// ✅ Proceed only if API key is valid
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				DisplayAlert("Error", "API Key is missing! Check debug_log.txt", "OK");
				return;
			}

			_mindfulness = new MindfulnessAgent(AppSecrets.AnthropicApiKey);
			_blockchain = new ModeBlockchainService(
				privateKey: AppSecrets.ModePrivateKey,
				contractAddress: AppSecrets.ModeContractAddress
			);
		}

		private void OnTopicClicked(object sender, EventArgs e)
		{
			if (sender is Button button)
			{
				QueryEntry.Text = _mindfulness.GetTopicPrompt(button.Text);
			}
		}

		private async void OnSubmitClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(QueryEntry.Text))
			{
				await DisplayAlert("Gentle Reminder", "Please share what's on your mind.", "OK");
				return;
			}

			try
			{
				Console.WriteLine("Submitting request to Blockhead No More...");
				LoadingIndicator.IsVisible = true;
				LoadingIndicator.IsRunning = true;
				SubmitButton.IsEnabled = false;

				// Guidance
				var response = await _mindfulness.ProcessQuery(QueryEntry.Text);
				Console.WriteLine($"Guidance Received: {response}");

				ResponseLabel.Text = response;
				ResponseFrame.IsVisible = true;

				// Store response on blockchain
				Console.WriteLine("Storing response on Mode Blockchain...");
				var txHash = await _blockchain.StoreResponse(QueryEntry.Text, response);
				Console.WriteLine($"Mode Blockchain Transaction Hash: {txHash}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ ERROR: {ex.GetType()} - {ex.Message}");
				Console.WriteLine($"📌 STACK TRACE: {ex.StackTrace}");
				await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
			}
			finally
			{
				LoadingIndicator.IsVisible = false;
				LoadingIndicator.IsRunning = false;
				SubmitButton.IsEnabled = true;
			}
		}
	}
}
