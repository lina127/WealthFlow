﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WealthFlow.Models;

namespace WealthFlow.Controllers
{
    public class MainController : BaseController
    {
        private WealthflowContext _dbContext;
        public MainController(WealthflowContext context) : base(context)
        {
            _dbContext = context;
        }

        // Transaction
        public IActionResult Transactions() 
        {
            if (IsSessionValid(out User? user))
            {
                List<Card> cards = _dbContext.Card.Where(o => o.UserId == user.UserId).ToList();
                
                List<Transaction> transactions = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId).ToList();

                List<Category> categories = _dbContext.Category.ToList();

                DataDTO dataDTO = new(user, cards, transactions, categories);
                return View(dataDTO);
            }
            return RedirectToAction("Index", "User");
        }

        public void AddTransaction(string csv, int cardId)
        {
            Card card = _dbContext.Card.Where(o => o.CardId == cardId).FirstOrDefault();
            if (card.Bank.ToLower() == "td" && card.Type.ToLower() == "debit")
            {
                List<string> skipList = new();
                skipList.Add("TD VISA PREAUTH PYMT");
                skipList.Add("HB033 TFR-TO C/C");
                string[] rows = csv.Split("\r\n");
                foreach (var r in rows)
                {
                    string[] columns = r.Split(",");
                    DateTime date = DateTime.Parse(columns[0]);
                    string merchant = columns[1].Trim();
                    decimal amount = 0;
                    if (columns[2] != null && columns[2] != "" && Convert.ToDecimal(columns[2]) > 0)
                    {
                        amount = Convert.ToDecimal(columns[2]) * -1;
                    }
                    else
                    {
                        amount = Convert.ToDecimal(columns[3]);
                    }
                    Transaction transaction = new Transaction();
                    transaction.Date = date;
                    transaction.Merchant = merchant;
                    transaction.Amount = amount;
                    transaction.CardId = cardId;
                    transaction.CategoryId = 3; // Others
                    transaction.Note = "";

                    _dbContext.Transaction.Add(transaction);
                    _dbContext.SaveChanges();
                }
            }
        }

        public void UpdateTransaction(int transactionId, string note, int categoryId)
        {
            Transaction transaction = _dbContext.Transaction.Where(o => o.TransactionId == transactionId).FirstOrDefault();
            transaction.Note = note;
            transaction.CategoryId = categoryId;
            _dbContext.Update(transaction);
            _dbContext.SaveChanges();
        }

        // Category
        public IActionResult Category()
        {
            List<Category> category = _dbContext.Category.OrderBy(o => o.Type).ThenBy(o => o.Name).ToList();
            return View(category);
        }

        [HttpPost]
        public void AddNewCategory(string name, string type)
        {
            Category category = new Category();
            category.Name = name;
            category.Type = type;
            _dbContext.Add(category);
            _dbContext.SaveChanges();
        }

        [HttpPost]
        public void RenameCategory(int categoryId, string newName, string newType)
        {
            Category category = _dbContext.Category.Where(o => o.CategoryId == categoryId).FirstOrDefault();
            category.Name = newName;
            category.Type = newType;
            _dbContext.Update(category);
            _dbContext.SaveChanges();
        }

        

    }
}
