using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WealthFlow.Models;

namespace WealthFlow
{
    public class DataDTO
    {
        public User User { get; set; }
        public List<Card> Cards { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Category> Categories { get; set; }
        public DataDTO(User user, List<Card> cards)
        {
            User = user;
            Cards = cards;
        }

        public DataDTO(User user, List<Card> cards, List<Transaction> transactions)
        {
            User = user;
            Cards = cards;
            Transactions = transactions;
        }

        public DataDTO(User user, List<Card> cards, List<Transaction> transactions, List<Category> categories)
        {
            User = user;
            Cards = cards;
            Transactions = transactions;
            Categories = categories;
        }


    }
}
