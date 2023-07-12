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
        Task<IList<Transaction>> GetByDateRange(DateTime startDate, DateTime endDate);
        Task<SalesSummary> GetSalesSummary(DateTime startDate, DateTime endDate);
        Task<IDictionary<int, double>> GetMonthlySalesAggregation();
        Task<IDictionary<int, double>> GetMonthlyProfitsAggregation();
    }
}