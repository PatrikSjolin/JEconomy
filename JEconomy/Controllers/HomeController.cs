using JEconomy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Globalization;

namespace JEconomy.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult AddCategory(string category)
        {
            if (category == null)
                return RedirectToAction("Index");

            ApplicationDbContext context = new ApplicationDbContext();

            string userId = User.Identity.GetUserId();
            IdentityUser user = context.Users.First(x => x.Id == userId);

            if (context.Categories.Any(x => x.IdentityUser.Id == userId && x.Name == category))
            {
                string url = Request.UrlReferrer.AbsolutePath;
                return Redirect(url);
            }

            context.Categories.Add(new Category
            {
                Id = Guid.NewGuid(),
                IdentityUser = user,
                Name = category
            });

            context.SaveChanges();

            string urlol = Request.UrlReferrer.AbsolutePath;
            return Redirect(urlol);
        }

        [HttpPost]
        public ActionResult ChangeCategory(string category, string transaction)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            string userId = User.Identity.GetUserId();
            List<Transaction> transactions = context.Transactions.Where(x => x.Place == transaction).ToList();

            foreach (Transaction trans in transactions)
            {
                trans.Category = context.Categories.FirstOrDefault(x => x.Name == category);
            }

            context.SaveChanges();

            string url = Request.UrlReferrer.AbsolutePath;
            return Redirect(url);
        }

        public ActionResult Import(string data, string bank, string save)
        {
            DateTime start = DateTime.Now;

            KeyValuePair<string, object> banks = new KeyValuePair<string, object>("bank", (object)new List<SelectListItem>
            {                    
                new SelectListItem { Value = "Handelsbanken", Text = "Handelsbanken" },
                new SelectListItem { Value = "Nordea", Text = "Nordea" },
                new SelectListItem { Value = "SEB", Text = "SEB" },
            });

            ViewData.Add(banks);

            if (data == null || !User.Identity.IsAuthenticated) return View();

            ApplicationDbContext context = new ApplicationDbContext();
            Init(context);

            string userId = User.Identity.GetUserId();
            IdentityUser user = context.Users.First(x => x.Id == userId);
            TransactionsViewModel viewModel = new TransactionsViewModel { Transactions = new List<TransactionViewModel>() };
            List<TransactionViewModel> rows = new List<TransactionViewModel>();

            List<Category> categories = context.Categories.Where(x => x.IdentityUser.Id == userId).ToList();
            List<Transaction> transactions = context.Transactions.Where(x => x.IdentityUser.Id == userId).ToList();

            if (bank == "Handelsbanken")
            {
                List<string> inputRows = data.Split(new string[] { "\t \t" }, StringSplitOptions.None).ToList();

                for (int i = 0; i < inputRows.Count - 3; i += 4)
                {
                    TransactionViewModel row;

                    string balance = inputRows[i + 4].Substring(0, inputRows[i + 4].LastIndexOf(" ")).Replace(" ", "").Replace(",", ".");

                    if (i > inputRows.Count - 7)
                    {
                        balance = inputRows[i + 4].Replace(" ", "").Replace(",", ".");
                    }

                    rows.Add(row = new TransactionViewModel
                    {
                        Balance = Convert.ToDouble(balance, CultureInfo.InvariantCulture),
                        Place = inputRows[i + 2],
                        TransactionDate = Convert.ToDateTime(inputRows[i + 1]),
                        Value = Convert.ToDouble(inputRows[i + 3].Replace(",", ".").Replace(" ", ""), CultureInfo.InvariantCulture),
                    });
                    viewModel.Transactions.Add(row);
                }
            }
            else if (bank == "Nordea")
            {
                List<string> inputRows = data.Split(new string[] { "\t   \t" }, StringSplitOptions.None).ToList();

                for (int i = 0; i < inputRows.Count; i++)
                {
                    List<string> tmpList = new List<string>();

                    tmpList = inputRows[i].Split('\t').ToList();

                    string stringdouble = tmpList[4].Replace(".", "").Replace(".", ",");
                    string stringdouble2 = tmpList[3].Replace(".", "").Replace(".", ",");

                    double balance = Convert.ToDouble(stringdouble);
                    double value = Convert.ToDouble(stringdouble2);
                    TransactionViewModel row;
                    rows.Add(row = new TransactionViewModel
                    {
                        Balance = balance,
                        Place = tmpList[1],
                        TransactionDate = Convert.ToDateTime(tmpList[0]),
                        Value = value
                    });
                    viewModel.Transactions.Add(row);
                }
            }
            else if (bank == "SEB")
            {
                List<string> inputRows = data.Split('\t').ToList();

                TransactionViewModel row;
                string val = inputRows[3] == "" ? inputRows[2] : inputRows[3];
                rows.Add(row = new TransactionViewModel
                {
                    Balance = Convert.ToDouble(inputRows[4].Split(' ')[0].Replace(".", "").Replace(",", "."), CultureInfo.InvariantCulture),
                    Place = inputRows[1].Split('/')[0],
                    TransactionDate = Convert.ToDateTime(inputRows[0]),
                    Value = Convert.ToDouble(val.Replace(".", "").Replace(",", "."), CultureInfo.InvariantCulture),
                });

                viewModel.Transactions.Add(row);

                for (int i = 4; i < inputRows.Count - 1; i += 4)
                {
                    TransactionViewModel row2;

                    val = inputRows[i + 3] == "" ? inputRows[i + 2] : inputRows[i + 3];

                    string balance = inputRows[i + 4].Split(' ')[0].Replace(".", "").Replace(",", ".");
                    string place = inputRows[i + 1].Split('/')[0];
                    string transactionDate = inputRows[i].Split(' ')[1];
                    string value = val.Replace(".", "").Replace(",", ".");

                    rows.Add(row2 = new TransactionViewModel
                    {
                        Balance = Convert.ToDouble(balance, CultureInfo.InvariantCulture),
                        Place = place,
                        TransactionDate = Convert.ToDateTime(transactionDate),
                        Value = Convert.ToDouble(value, CultureInfo.InvariantCulture),
                    });

                    viewModel.Transactions.Add(row2);
                }
            }
            else
            {
                return View();
            }

            foreach (TransactionViewModel transactionViewModel in viewModel.Transactions)
            {
                Transaction similar = transactions.FirstOrDefault(x => x.Category != null && x.Place == transactionViewModel.Place);

                Category category = similar != null ? similar.Category : null;

                Transaction transaction = transactions.FirstOrDefault(x => x.Balance == transactionViewModel.Balance &&
                                                                           x.Place == transactionViewModel.Place &&
                                                                           x.TransactionDate == transactionViewModel.TransactionDate &&
                                                                           x.Value == transactionViewModel.Value);
                if (transaction != null)
                {
                    transactionViewModel.Category = transaction.Category == null ? "Not set" : transaction.Category.Name;
                    transactionViewModel.State = "Duplicate";
                }
                else
                {
                    if (category != null)
                    {
                        transactionViewModel.Category = category.Name;
                        transactionViewModel.State = "New";
                    }
                    else
                    {
                        transactionViewModel.Category = null;
                        transactionViewModel.State = "Missing Category";
                    }
                    context.Transactions.Add(new Transaction
                    {
                        Balance = transactionViewModel.Balance,
                        Category = category,
                        Id = Guid.NewGuid(),
                        IdentityUser = user,
                        Place = transactionViewModel.Place,
                        TransactionDate = transactionViewModel.TransactionDate,
                        Value = transactionViewModel.Value
                    });
                }
            }
            context.SaveChanges();
            ViewBag.InputValues = viewModel;
            viewModel.TimeElapsed = DateTime.Now.Subtract(start);

            return View();
        }

        public ActionResult Statistics()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            string usedId = User.Identity.GetUserId();
            List<Transaction> transactions = context.Transactions.Where(x => x.IdentityUser.Id == usedId).ToList();

            List<MonthSummaryViewModel> summaries = new List<MonthSummaryViewModel>();

            foreach (Transaction transaction in transactions.OrderByDescending(x => x.TransactionDate))
            {
                string cat = transaction.Category == null ? null : transaction.Category.Name;

                string subCat = null;
                if (transaction.Category != null)
                {
                    List<string> tran = transaction.Category.Name.Split(new string[] { " - " }, StringSplitOptions.None).ToList();
                    if (tran.Count > 1)
                    {
                        cat = tran[0];
                        subCat = tran[1];
                    }
                    else
                    {
                        cat = transaction.Category.Name;
                    }
                }

                MonthSummaryViewModel month = summaries.FirstOrDefault(x => x.Year == transaction.TransactionDate.Year && x.Month == transaction.TransactionDate.Month);
                if (month != null)
                {
                    CategorySummary summary = month.CategorySummaries.FirstOrDefault(x => x.Category == (transaction.Category == null ? "Not set" : cat));
                    if (summary != null)
                    {
                        summary.Value += transaction.Value;
                        if (subCat != null)
                        {
                            if (summary.SubCategorySummaries != null)
                            {
                                SubCategorySummary subSummary = summary.SubCategorySummaries.FirstOrDefault(x => x.Category == subCat);
                                if (subSummary != null)
                                {
                                    subSummary.Value += transaction.Value;
                                }
                                else
                                {
                                    subSummary = new SubCategorySummary
                                        {
                                            Category = subCat,
                                            Value = transaction.Value
                                        };
                                    summary.SubCategorySummaries.Add(subSummary);
                                }
                            }
                            else
                            {
                                summary.SubCategorySummaries = new List<SubCategorySummary>
                                {
                                    new SubCategorySummary
                                    {
                                        Category = subCat,
                                        Value = transaction.Value
                                    }
                                };
                            }
                        }
                    }
                    else
                    {
                        month.CategorySummaries.Add(new CategorySummary
                        {
                            Value = transaction.Value,
                            Category = cat == null ? "Not set" : cat,
                            SubCategorySummaries = subCat == null ? null : new List<SubCategorySummary>
                            {
                                new SubCategorySummary()
                                {
                                    Category = subCat,
                                    Value = transaction.Value
                                }
                            }
                        });
                    }
                }
                else
                {
                    summaries.Add(new MonthSummaryViewModel()
                    {
                        CategorySummaries = new List<CategorySummary>
                        {
                            new CategorySummary
                            {
                                Value = transaction.Value,
                                Category = cat == null ? "Not set" : cat,
                                SubCategorySummaries = subCat == null ? null : new List<SubCategorySummary>
                                {
                                    new SubCategorySummary()
                                    {
                                        Category = subCat,
                                        Value = transaction.Value
                                    }
                                }
                            }
                        },
                        Month = transaction.TransactionDate.Month,
                        Year = transaction.TransactionDate.Year,
                        MonthName = Months[transaction.TransactionDate.Month]
                    });
                }
            }

            ViewBag.MonthSummaries = summaries;

            return View();
        }

        public ActionResult Categories()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            ViewBag.Categories = context.Categories.OrderBy(x => x.Name);

            return View();
        }

        public ActionResult RemoveCategory(Guid id)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            Category category = context.Categories.First(x => x.Id == id);

            List<Transaction> transactions = context.Transactions.Where(x => x.Category.Id == id).ToList();

            foreach (Transaction t in transactions)
            {
                t.Category = null;
            }
            context.Categories.Remove(category);

            context.SaveChanges();

            return Redirect("/Home/Categories");
        }

        public ActionResult RemoveTransactions()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            foreach (Transaction t in context.Transactions)
            {
                context.Transactions.Remove(t);
            }
            context.SaveChanges();
            return Redirect("/");
        }

        public Dictionary<int, string> Months
        {
            get
            {
                return new Dictionary<int, string>
                {
                { 1, "January" },
                { 2, "February" },
                { 3, "March" },
                { 4, "April" },
                { 5, "May" },
                { 6, "June" },
                { 7, "July" },
                { 8, "August" },
                { 9, "September" },
                { 10, "October" },
                { 11, "November" },
                { 12, "December" } };
            }
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                Init(context);
                DateTime start = DateTime.Now;


                string usedId = User.Identity.GetUserId();
                List<Transaction> transactions = context.Transactions.Where(x => x.IdentityUser.Id == usedId).ToList();
                TransactionsViewModel viewModel = new TransactionsViewModel { Transactions = new List<TransactionViewModel>() };

                foreach (Transaction transaction in transactions)
                {
                    viewModel.Transactions.Add(new TransactionViewModel
                    {
                        Balance = transaction.Balance,
                        Category = transaction.Category == null ? string.Empty : transaction.Category.Name,
                        Place = transaction.Place,
                        TransactionDate = transaction.TransactionDate,
                        Value = transaction.Value
                    });
                }
                viewModel.TimeElapsed = DateTime.Now.Subtract(start);
                ViewBag.InputValues = viewModel;
            }

            return View();
        }

        private void Init(ApplicationDbContext context)
        {
            KeyValuePair<string, object> categories = new KeyValuePair<string, object>("categories", (object)new List<SelectListItem>());
            string userId = User.Identity.GetUserId();
            foreach (Category category in context.Categories.Where(x => x.IdentityUser.Id == userId).OrderBy(x => x.Name))
            {
                ((List<SelectListItem>)categories.Value).Add(new SelectListItem { Text = category.Name, Value = category.Name });
            }

            ViewData.Add(categories);
        }
    }
}