namespace PeyulErp.Settings{
    public class MongoDbSettings {
        public string ConnectionString {get; set;}
        public string DatabaseName { get; set; }
        public string ProductsCollectionName { get; set; }
        public string ProductCategoriesCollectionName { get; set; }
        public string TransactionsCollectionName { get; set; }
        public string StocksCollectionName { get; set; }
        public string PuchasesCollectionName { get; set; }
        public string SupplierCollectionName { get; set; }
        public string ExpensesCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string UserPasswordCollectionName { get; set; }
    }
}