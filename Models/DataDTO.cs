using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WealthFlow
{
    public class DataDTO
    {
        public User User { get; set; }
        public List<Card> Cards { get; set; }
        public List<Transaction> Transactions { get; set; }
        public DataDTO(User user, List<Card> cards, List<Transaction> transactions)
        {
            User = user;
            Cards = cards;
            Transactions = transactions;
        }
    }
}
