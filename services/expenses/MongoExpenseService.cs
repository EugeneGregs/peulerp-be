using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PeyulErp.Models;
using PeyulErp.Settings;

namespace PeyulErp.Services
{
    public class MongoExpenseService : IExpenseService
    {
        private readonly MongoDbSettings _settings;
        private readonly FilterDefinitionBuilder<Expense> _filter = Builders<Expense>.Filter;
        private readonly IMongoCollection<Expense> _expensesCollection;

        public MongoExpenseService(IOptions<MongoDbSettings> settings, IMongoClient mongoClient) {
            _settings = settings.Value;
            var database = mongoClient.GetDatabase(_settings.DatabaseName);
            _expensesCollection = database.GetCollection<Expense>(_settings.ExpensesCollectionName);
        }
        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            var newExpense = expense with { Id = Guid.NewGuid(), CreateDate = DateTime.UtcNow };
            return await _expensesCollection.InsertOneAsync(newExpense).ContinueWith(_ => newExpense);
        }

        public async Task<bool> DeleteExpenseAsync(Guid Id)
        {
            var existingExpense = (await _expensesCollection.FindAsync(_filter.Eq(p => p.Id, Id))).FirstOrDefault();

            if (existingExpense is null)
                return false;

            await _expensesCollection.DeleteOneAsync(_filter.Eq(p => p.Id, Id));
            return true;
        }

        public async Task<IList<Expense>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return null;

            if(startDate == endDate)
                endDate = startDate.AddDays(1);

            var filter = _filter.And(_filter.Gte(p => p.ExpenseDate, startDate.Date), _filter.Lte(p => p.ExpenseDate, endDate.Date));
            return await _expensesCollection.Find(filter).ToListAsync();
        }

        public async Task<IList<Expense>> GetByPaymentType(PaymentType paymentType)
        {
            var dateFilter = _filter.And(_filter.Gte(p => p.ExpenseDate, DateTime.UtcNow.Date), _filter.Lt(p => p.ExpenseDate, DateTime.UtcNow.Date.AddDays(1)));
            var paymentTypeFilter = _filter.Eq(p => (int)p.PaymentType, (int)paymentType);

            var filter = dateFilter & paymentTypeFilter;

            var expenses = await _expensesCollection.FindAsync(filter).Result.ToListAsync();

            return expenses;
        }

        public async Task<Expense> GetExpenseAsync(Guid Id)
        {
            return (await _expensesCollection.FindAsync(_filter.Eq(p => p.Id, Id))).FirstOrDefault();
        }

        public async Task<IEnumerable<Expense>> GetExpensesAsync()
        {
            return (await _expensesCollection.FindAsync(new BsonDocument())).ToList();
        }

        public async Task<ExpenseSummary> GetExpenseSummary(DateTime startDateTime, DateTime endDateTime)
        {
            var startDate = startDateTime.Date;
            var endDate = endDateTime.Date;

            if(startDate > endDate)
                return null;

            if(startDate == endDate)
                endDate = startDate.AddDays(1);

            var filter = _filter.And(_filter.Gte(p => p.ExpenseDate, startDate.Date), _filter.Lte(p => p.ExpenseDate, endDate.Date));

            var expenses = (await _expensesCollection.FindAsync(filter)).ToList();
            var ExpenseSummary = new ExpenseSummary();

            ExpenseSummary.TotalExpenses = expenses.Sum(e => e.Amount);

            ExpenseSummary.MonthlyExpenseSummary = new Dictionary<int, double>();
            ExpenseSummary.MonthlyExpenseSummary = await GetMonthlyAggregation();

            return ExpenseSummary;
        }

        public async Task<IDictionary<int, double>> GetMonthlyAggregation()
        {
            var filter = _filter.Gte(e => e.ExpenseDate, new DateTime(DateTime.Now.Year, 1, 1));

            var thisYearExpenses = await _expensesCollection.FindAsync(filter);

            return thisYearExpenses.ToList().GroupBy(e => e.ExpenseDate.Month).ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
        }

        public async Task<Expense> UpdateExpenseAsync(Expense expense)
        {
            var existingExpense = await _expensesCollection.FindAsync(_filter.Eq(p => p.Id, expense.Id));

            if (existingExpense is null)
                return null;

            var newExpense =
                expense with 
                {
                    UpdateDate = DateTime.UtcNow,
                    Name = expense.Name,
                    Description = expense.Description,
                    Amount = expense.Amount,
                    ExpenseDate = expense.ExpenseDate,
                    ExpenseType = expense.ExpenseType,
                    PaymentType = expense.PaymentType
                };

            return await _expensesCollection.ReplaceOneAsync(_filter.Eq(p => p.Id, expense.Id), expense).ContinueWith(_ => newExpense);
        }
    }
}