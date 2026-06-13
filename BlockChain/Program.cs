using BlockChain.Models;
using BlockChain.Service;
using System.Reflection;

var displayService = new DisplayService();
var blockChainService = new BlockChainService(1);
var walletAlice = new Wallet(new CryptoService());
var walletBob = new Wallet(new CryptoService());

var transaction = TransactionService.CreateTransaction(walletAlice.PublicKey, walletBob.PublicKey, 10, walletAlice.PrivateKey);

blockChainService.AddBlock(new List<Transaction> { transaction });

displayService.DisplayChain(blockChainService.Chain);