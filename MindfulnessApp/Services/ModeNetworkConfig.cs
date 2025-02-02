using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindfulnessApp.Services
{
	public class ModeNetworkConfig
	{
		// MODE Testnet Configuration
		public static readonly string TestnetRPC = "https://sepolia.mode.network";
		public static readonly int TestnetChainId = 919;
		public static readonly string TestnetExplorer = "https://sepolia.explorer.mode.network";

		// MODE Mainnet Configuration
		public static readonly string MainnetRPC = "https://mainnet.mode.network";
		public static readonly int MainnetChainId = 34443;
		public static readonly string MainnetExplorer = "https://explorer.mode.network";
	}
}
