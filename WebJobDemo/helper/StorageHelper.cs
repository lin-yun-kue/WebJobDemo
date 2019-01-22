using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebJobDemo.helper
{
    public class TableStorageHelper
    {
        public string StorageConnectionString { get; set; }
        public string ErrorMessage { get; set; }

        private CloudStorageAccount _storageAccount;

        private CloudTableClient _tableClient;

        public TableStorageHelper(string connectionString)
        {
            StorageConnectionString = connectionString;

            _storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();
        }

        public (bool status, T entity) Insert<T>(string tableName, T entity) where T : ITableEntity
        {
            var result = default(TableResult);
            try
            {
                var table = _tableClient.GetTableReference(tableName);
                table.CreateIfNotExists();
                var insertOperation = TableOperation.Insert(entity);
                result = table.Execute(insertOperation);
            }
            catch (Exception e)
            {
                return (false, (T)result?.Result);
            }
            return (true, (T)result.Result);
        }

        public (bool status, IEnumerable<T> data) Query<T>(string tableName, Expression<Func<T, bool>> filter) where T : ITableEntity, new()
        {
            var result = default(IEnumerable<T>);
            try
            {
                var table = _tableClient.GetTableReference(tableName);
                result = table.CreateQuery<T>().Where(filter);
            }
            catch (Exception e)
            {
                return (false, result);
            }

            return (true, result);
        }

        public (bool status, T entity) Query<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            var result = default(T);
            try
            {
                var table = _tableClient.GetTableReference(tableName);

                // method 1
                //var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                //var entity = table.Execute(retrieveOperation);
                //result = (T)entity.Result;

                // method 2
                var data = table.CreateQuery<T>().Where(x => x.PartitionKey == partitionKey && x.RowKey == rowKey);
                result = data.FirstOrDefault();
            }
            catch (Exception e)
            {
                return (false, result);
            }
            return (true, result);
        }

        public (bool status, T entity) Update<T>(string tableName, T entity) where T : ITableEntity
        {
            var result = default(TableResult);
            try
            {
                var table = _tableClient.GetTableReference(tableName);
                var insertOperation = TableOperation.Replace(entity);
                result = table.Execute(insertOperation);
            }
            catch (Exception e)
            {
                return (false, (T)result?.Result);
            }
            return (true, (T)result.Result);
        }

        public bool Delte<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            try
            {
                var table = _tableClient.GetTableReference(tableName);
                var entity = this.Query<T>(tableName, partitionKey, rowKey).entity;
                var deleteOperation = TableOperation.Delete(entity);
                table.Execute(deleteOperation);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
