using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SplitBillsapi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SplitBillsapi.Helpers
{
    public class Helper
    {
        private IConfiguration _configuration;
        string apiKey = string.Empty;
        public Helper(IConfiguration configuration)
        {
            _configuration = configuration;
            apiKey = configuration["externalAPIKey"];
        }
             
        public async Task<CreateUpdateBillRequest> Process(CreateUpdateBillRequest createBillRequest)
        {
            var baseCurrency = string.Empty;
            float baseRate = 0;
            foreach (Participant participant in createBillRequest.Participants)
            {
                if (participant.TransactionType.Equals("Paid"))
                {
                    baseCurrency = participant.Currency;
                    break;
                }
            }
            List<Currency> currencies = await CreateCurrencyConverterAPI();
            foreach (Currency currency in currencies)
            {
                if (baseCurrency.Equals(currency.Name))
                {
                    baseRate = currency.Rate;
                }
            }
            foreach (Participant participant in createBillRequest.Participants)
            {
                foreach (Currency currency in currencies)
                {
                    if (participant.Currency.Equals(currency.Name))
                    {
                        participant.Rate = currency.Rate / baseRate;
                    }
                }
            }
            return createBillRequest;
        }

        private async Task<List<Currency>> CreateCurrencyConverterAPI()
        {
            //Console.WriteLine
            List<Currency> currencies = new List<Currency>();
            using (var client = new HttpClient())
            {
                var url = new Uri($"http://data.fixer.io/api/latest?access_key=" + apiKey);
                var response = await client.GetAsync(url);
                string json;
                using (var content = response.Content)
                {
                    json = await content.ReadAsStringAsync();
                }
                JsonTextReader reader = new JsonTextReader(new StringReader(json));
                Currency c = new Currency();
                bool readValue = false;
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        //Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                        switch (reader.Value)
                        {
                            case "MXN":
                                c = new Currency();
                                c.Name = reader.Value.ToString();
                                readValue = true;
                                break;
                            case "CAD":
                                c = new Currency();
                                c.Name = reader.Value.ToString();
                                readValue = true;
                                break;
                            case "USD":
                                c = new Currency();
                                c.Name = reader.Value.ToString();
                                readValue = true;
                                break;
                            default:
                                if (readValue)
                                {
                                    c.Rate = float.Parse(reader.Value.ToString());
                                    readValue = false;
                                    currencies.Add(c);
                                }
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Token: {0}", reader.TokenType);
                    }
                }

            }
            return currencies;
        }

        public DataTable GetParticipantTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SignInName", typeof(string));
            dataTable.Columns.Add("Rate", typeof(float));
            dataTable.Columns.Add("TransactionType", typeof(string));
            return dataTable;
        }

        public DataTable GetMemberTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SignInName", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Currency", typeof(string));
            return dataTable;
        }
    }
}
