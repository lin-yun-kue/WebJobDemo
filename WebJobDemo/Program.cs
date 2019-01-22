using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobDemo.helper;

namespace WebJobDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectString = ConfigurationManager.AppSettings["AzureStorage"];
            var storageHelper = new TableStorageHelper(connectString);

            var rand = new Random();
            var temp = rand.Next(1, 13);
            //insert
            var member = new Member("Hong", temp.ToString())
            {
                Date = DateTime.UtcNow.AddHours(8)
            };
            var result = storageHelper.Insert("KyleTest", member);
        }
    }

    public class Member : TableEntity
    {
        public Member(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public Member()
        {

        }

        public string Address { get; set; }

        public string Phone { get; set; }

        public bool? Sex { get; set; }

        public DateTime Date { get; set; }
    }
}
