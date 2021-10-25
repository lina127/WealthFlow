﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace WealthFlow.Models
{
    public partial class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Merchant { get; set; }
        public decimal Amount { get; set; }
        public int CardId { get; set; }
        public int? CategoryId { get; set; }
        public string Note { get; set; }

        public virtual Card Card { get; set; }
        public virtual Category Category { get; set; }
    }
}