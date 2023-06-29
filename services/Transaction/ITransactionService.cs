//---------------------------------------------------------------------
// <copyright file="ITransactionService.cs" company="PEUL Africa">
//   Copyright (c) PEUL Africa. All rights reserved.
// </copyright>
//--------------------------------------------------------------------/

using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface ITransactionService
    {
        Task<Transaction> GetTransaction(Guid transactionId);
        Task<IList<Transaction>> GetTransactions();
        Task<bool> DeleteTransaction(Guid transactionId);
        Task<Transaction> CreateTransaction(Transaction transaction);
        Task<bool> UpdateTransaction(Transaction transaction);

    }
}