using Microsoft.AspNetCore.Mvc;
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

                List<Category> categories = _dbContext.Category.Where(o => o.UserId == user.UserId).ToList();

                DataDTO dataDTO = new(user, cards, transactions, categories);
                return View(dataDTO);
            }
            return RedirectToAction("Index", "User");
        }

        public void AddTransaction(string csv, int cardId)
        {
            User user = GetCurrentUser();
            if(user is not null)
            {
                List<Category> categories = _dbContext.Category.Where(o => o.UserId == user.UserId).ToList();
                List<Keyword> keywords = _dbContext.Keyword.Where(o => o.Category.UserId == user.UserId).ToList();
                List<ExcludeKeyword> excludeKeywords = _dbContext.ExcludeKeyword.Where(o => o.Card.UserId == user.UserId).ToList();

                Card card = _dbContext.Card.Where(o => o.CardId == cardId).FirstOrDefault();
                List<string> rows = new();

                if (card.Bank.ToLower() == "cibc")
                {
                    rows = csv.Split("\n").ToList();
                }
                else if (card.Bank.ToLower() == "td")
                {
                    rows = csv.Split("\r\n").ToList();
                }
                foreach (var r in rows)
                {
                    string[] columns = r.Split(",");
                    DateTime date = DateTime.Parse(columns[0]);
                    string merchant = columns[1].Trim();
                    decimal amount = 0;

                    if (card.Bank.ToLower() == "td" && card.Type.ToLower() == "debit")
                    {
                        if (columns[0] == "")
                            continue;
                        date = DateTime.ParseExact(columns[0], "MM/dd/yyyy", null);
                        merchant = columns[1].Trim();
                        if (columns[2] != null && columns[2] != "" && Convert.ToDecimal(columns[2]) > 0)
                        {
                            amount = Convert.ToDecimal(columns[2]) * -1;
                        }
                        else
                        {
                            amount = Convert.ToDecimal(columns[3]);
                        }
                    }
                    else if (card.Bank.ToLower() == "td" && card.Type.ToLower() == "credit")
                    {
                        if (columns[0] == "")
                            continue;
                        date = DateTime.Parse(columns[0]);
                        merchant = columns[1].Trim();
                        if (columns[2] != null && columns[2] != "" && Convert.ToDecimal(columns[2]) > 0)
                        {
                            amount = Convert.ToDecimal(columns[2]) * -1;
                        }
                        else
                        {
                            amount = Convert.ToDecimal(columns[3]);
                        }
                    }
                    else if (card.Bank.ToLower() == "cibc" && card.Type.ToLower() == "debit")
                    {
                        if (columns[0] == "")
                            continue;
                        date = DateTime.Parse(columns[0]);
                        merchant = columns[1].Trim();
                        if (columns[2] != null && columns[2] != "" && Convert.ToDecimal(columns[2]) > 0)
                        {
                            amount = Convert.ToDecimal(columns[2]) * -1;
                        }
                        else
                        {
                            amount = Convert.ToDecimal(columns[3]);
                        }
                    }
                    else if (card.Bank.ToLower() == "cibc" && card.Type.ToLower() == "credit")
                    {
                        if (columns[0] == "")
                            continue;
                        date = DateTime.Parse(columns[0]);
                        if (columns[1].Contains("\""))
                        {
                            merchant = (columns[1] + columns[2]).Trim();
                            if (columns[3] != null && columns[3] != "" && Convert.ToDecimal(columns[3]) > 0)
                            {
                                amount = Convert.ToDecimal(columns[3]) * -1;
                            }
                            else if (columns[4] != null && columns[4] != "" && Convert.ToDecimal(columns[4]) > 0)
                            {
                                amount = Convert.ToDecimal(columns[4]);
                            }
                        }
                        else
                        {
                            merchant = columns[1].Trim();
                            amount = Convert.ToDecimal(columns[3]);
                        }

                    }
                    // Exclude => skip to next item
                    if (excludeKeywords.Any(o => merchant.ToLower().Contains(o.Name.ToLower())))
                        continue;

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
            if (IsSessionValid(out User? user))
            {
                List<Category> category = _dbContext.Category.Where(o => o.UserId == user.UserId).OrderBy(o => o.Type).ThenBy(o => o.Name).ToList();
                return View(category);
            }
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public void AddNewCategory(string name, string type)
        {
            User currentUser = GetCurrentUser();
            Category category = new Category();
            category.Name = name;
            category.Type = type;
            category.UserId = currentUser.UserId;
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

        // Keyword
        public IActionResult Keyword()
        {
            if (IsSessionValid(out User? user))
            {
                List<Keyword> keyword = _dbContext.Keyword.Where(o => o.Category.UserId == user.UserId).OrderBy(o => o.Category.Name).ThenBy(o => o.Name).ToList();
                List<Category> category = _dbContext.Category.Where(o => o.UserId == user.UserId).OrderBy(o => o.Type).ThenBy(o => o.Name).ToList();
                List<ExcludeKeyword> excludeKeyword = _dbContext.ExcludeKeyword.Where(o => o.Card.UserId == user.UserId).OrderBy(o => o.Name).ToList();
                List<Card> card = _dbContext.Card.Where(o => o.UserId == user.UserId).ToList();
                DataDTO dataDTO = new DataDTO(keyword, category, card, excludeKeyword);
                return View(dataDTO);
            }
            return RedirectToAction("Index", "User");
        }

        public void AddNewKeyword(string name, int categoryId)
        {
            Keyword keyword = new Keyword();
            keyword.Name = name;
            keyword.CategoryId = categoryId;
            _dbContext.Add(keyword);
            _dbContext.SaveChanges();
        }

        public void RenameKeyword(int keywordId, string newName)
        {
            Keyword keyword = _dbContext.Keyword.Where(o => o.KeywordId == keywordId).FirstOrDefault();
            keyword.Name = newName;
            _dbContext.Update(keyword);
            _dbContext.SaveChanges();
        }

        public void DeleteKeyword(int keywordId)
        {
            Keyword keyword = _dbContext.Keyword.Where(o => o.KeywordId == keywordId).FirstOrDefault();
            _dbContext.Remove(keyword);
            _dbContext.SaveChanges();
        }

        // Exclude Keyword
        public void AddExclueKeyword(string excludeKeywordName, int cardId)
        {
            ExcludeKeyword excludeKeyword = new ExcludeKeyword();
            excludeKeyword.Name = excludeKeywordName;
            excludeKeyword.CardId = cardId;
            _dbContext.Add(excludeKeyword);
            _dbContext.SaveChanges();
        }

        public void RenameExcludeKeyword(int excludeKeywordId, string newName)
        {
            ExcludeKeyword keyword = _dbContext.ExcludeKeyword.Where(o => o.ExcludeKeywordId == excludeKeywordId).FirstOrDefault();
            keyword.Name = newName;
            _dbContext.Update(keyword);
            _dbContext.SaveChanges();
        }

        public void DeleteExcludeKeyword(int excludeKeywordId)
        {
            ExcludeKeyword keyword = _dbContext.ExcludeKeyword.Where(o => o.ExcludeKeywordId == excludeKeywordId).FirstOrDefault();
            _dbContext.Remove(keyword);
            _dbContext.SaveChanges();
        }
    }
}
