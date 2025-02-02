using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindfulnessApp.Configuration;

namespace MindfulnessApp.Services
{
	public class ModeBlockchainService
	{
		private readonly Web3 _web3;
		private readonly Contract _contract;
		private readonly bool _isTestnet;

		public ModeBlockchainService(string privateKey, string contractAddress, bool useTestnet = true)
		{
			_isTestnet = useTestnet;
			var networkUrl = useTestnet ? ModeNetworkConfig.TestnetRPC : ModeNetworkConfig.MainnetRPC;
			var chainId = useTestnet ? ModeNetworkConfig.TestnetChainId : ModeNetworkConfig.MainnetChainId;

			if (string.IsNullOrEmpty(privateKey))
			{
				throw new ArgumentException("Private key is missing or invalid. Ensure it's loaded correctly.");
			}
			var account = new Account(privateKey);
			_web3 = new Web3(account, networkUrl);

			// Configure chain settings
			_web3.Eth.TransactionManager.DefaultGasPrice = Web3.Convert.ToWei(1, UnitConversion.EthUnit.Gwei);
			

			var abi = GetContractABI(); // Load your contract ABI
			_contract = _web3.Eth.GetContract(abi, contractAddress);
		}

		private string GetContractABI()
		{
			// This is a basic ABI for the store response function
			return @"[
            {
                ""inputs"": [
                    {
                        ""internalType"": ""string"",
                        ""name"": ""query"",
                        ""type"": ""string""
                    },
                    {
                        ""internalType"": ""string"",
                        ""name"": ""response"",
                        ""type"": ""string""
                    }
                ],
                ""name"": ""storeResponse"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }
        ]";
		}

		public async Task<string> StoreResponse(string query, string response)
		{
			try
			{
				Console.WriteLine($"🟡 Storing response on blockchain...");
				var storeFunction = _contract.GetFunction("storeResponse");

				var txReceipt = await storeFunction.SendTransactionAndWaitForReceiptAsync(
					_web3.TransactionManager.Account.Address,
					new HexBigInteger(UnitConversion.Convert.ToWei(5, UnitConversion.EthUnit.Gwei)),
					new HexBigInteger(200000),
					null,
					query, response
				);

				Console.WriteLine($"🟢 Blockchain TxHash: {txReceipt.TransactionHash}");
				return txReceipt.TransactionHash;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ BLOCKCHAIN ERROR: {ex.GetType()} - {ex.Message}");
				throw;
			}
		}
		public async Task<bool> ValidateConnection()
		{
			try
			{
				var blockNumber = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
				var networkVersion = await _web3.Net.Version.SendRequestAsync();
				var expectedChainId = _isTestnet ? ModeNetworkConfig.TestnetChainId.ToString() :
												 ModeNetworkConfig.MainnetChainId.ToString();

				return networkVersion == expectedChainId;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<decimal> GetModeBalance(string address)
		{
			var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
			return Web3.Convert.FromWei(balance.Value);
		}

		public string GetExplorerUrl(string txHash)
		{
			var baseUrl = _isTestnet ? ModeNetworkConfig.TestnetExplorer : ModeNetworkConfig.MainnetExplorer;
			return $"{baseUrl}/tx/{txHash}";
		}
	}
}
