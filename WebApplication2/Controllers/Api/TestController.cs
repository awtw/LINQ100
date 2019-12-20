using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication2.Controllers.Api
{
    public class TestController : ApiController
    {
        public TestController()
        {

        }
        string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [Route("t")]
        public IHttpActionResult GetProduct()
        {

            using (IDbConnection cn = new SqlConnection(ConnectionString))
            {
                var sql = @"SELECT TOP 100 * FROM Person.Person";
                cn.Open();

                var personList = cn.Query<Persons>(sql).AsEnumerable();
                var sql2 = @"select TOP 100 * from person.personphone";
                var personPhoneList = cn.Query<PersonPhone>(sql2).AsEnumerable();

                var sql3 = @"select TOP 10 * from person.personphone";
                var personListForTop10 = cn.Query<PersonPhone>(sql3).AsEnumerable();



                var linqTest01 = personList.Where(x => x.BusinessEntityID < 5);
                var linqTest02 = personList.Where(x => x.BusinessEntityID == 5);
                var linqTest03 = personList.Where(x => x.BusinessEntityID == 5 && x.PersonType == "EM");
                //var linqTest04 因需要輸出foreach，故沒再api上操作
                var linqTest05 = personList.Where((x, index) => x.BusinessEntityID > index);

                var linqTest6 = personList.Select(n => n.BusinessEntityID + 123);
                var linqTest7 = personList.Select(n => n.EmailPromotion);
                string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
                var linqTest8 = personListForTop10.Select((p, index) => strings[index]);
                var linqTest9 = personList.Select(p => new
                {
                    Upper = p.PersonType.ToUpper(),
                    Lower = p.PersonType.ToLower()
                });

                var linqTest10 = personList
                    .Where(x => x.BusinessEntityID > 0)
                    .Select(n => new
                    {
                        Name = n.BusinessEntityID % 2,
                        Type = n.EmailPromotion
                    });

                var linqTest11 = personList
                    .Where(x => x.BusinessEntityID > 0)
                    .Select(n => new
                    {
                        Name = n.BusinessEntityID,
                        Type = n.EmailPromotion
                    });

                var linqTest12 = personList
                    .Where(x => x.BusinessEntityID > 0)
                    .Select((num, index) => new
                    {
                        Num = num.PersonType,
                        Index = num.BusinessEntityID + 1 != index
                    });

                var linqTest13 = personList
                   .Where(x => x.BusinessEntityID > 0)
                   .Where(x => x.PersonType == "EM")
                   .Select((num, index) => new
                   {
                       Num = num.PersonType,
                       Index = num.BusinessEntityID + 1 != index
                   });

                var linqTest14 = personList.SelectMany(a => personPhoneList, (a, b) => new { a, b })
                    .Where(@t => @t.a.BusinessEntityID < @t.b.PhoneNumberTypeID);

                var linqTest15 = personList.SelectMany(a => personPhoneList, (a, b) => new { a, b })
                    .Where(@t => @t.a.BusinessEntityID > @t.b.PhoneNumberTypeID)
                    .Select(t => new
                    {
                        PersonId = t.a.BusinessEntityID,
                        PhoneNumber = t.b.PhoneNumber,
                        Type = t.a.PersonType + t.b.PhoneNumberTypeID
                    });

                var linqTest16 = personList.SelectMany(a => personPhoneList, (a, b) => new { a, b })
                     .Where(@t => @t.a.BusinessEntityID > @t.b.PhoneNumberTypeID && @t.a.PersonType == "EM")
                     .Select(t => new
                     {
                         PersonId = t.a.BusinessEntityID,
                         PhoneNumber = t.b.PhoneNumber,
                         Type = t.a.PersonType + t.b.PhoneNumberTypeID,
                         PersonSelect = t.a.PersonType
                     });

                //linqTest17-19 Amost the same, so skip
                int[] numbers = new int[] { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
                int linqTest99Index = 0;
                int linqTest100Index = 0;

                var linqTest20 = numbers.Take(3);
                var linqTest21 =
                        personList
                        .SelectMany(phone => personPhoneList,
                        (person, phone) => new { person, phone })
                    .Take(3);
                var linqTest22 = numbers.Skip(4);

                var linqTest23 = personList
                    .SelectMany(phone => personPhoneList,
                    (person, phone) => new { person, phone })
                    .Select(t => new
                    {
                        personID = t.person.BusinessEntityID,
                        peronsType = t.person.PersonType,
                        phone = t.phone.PhoneNumber
                    })
                    .Skip(3);
                //First numbers less than 6
                var linqTest24 = numbers.TakeWhile(n => n < 6);
                //First numbers not less than their position
                var linqTest25 = numbers.TakeWhile((n, index) => n >= index);
                //All elements starting from first element divisible by 3:
                var linqTest26 = numbers.SkipWhile(n => n % 3 != 0);
                //"All elements starting from first element less than its position:
                var linqTest27 = numbers.SkipWhile((n, index) => n >= index);

                string[] words = { "cherry", "apple", "blueberry" };

                //orderby to sort a list of words alphabetically
                var linqTest28 = words.OrderBy(word => word);

                var linqTest29 = words.OrderBy(word => word.Length);
                var linqTest30 = personList.OrderBy(pers => pers.BusinessEntityID);

                string[] words2 = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

                var linqTest31 = words2.OrderBy(a => a, StringComparer.OrdinalIgnoreCase);
                //Descending (usual method will be ascending order)
                double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };
                var linqTest32 = doubles.OrderByDescending(d => d);
                var linqTest33 = personList.OrderByDescending(d => d.PersonType);
                var linqTest34 = words2.OrderByDescending(a => a, StringComparer.OrdinalIgnoreCase);

                string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
                var linqTest35 = digits.OrderBy(digit => digit.Length).ThenBy(digit => digits);
                var linqTest36 = words2.OrderBy(a => a.Length).ThenBy(a => a, StringComparer.OrdinalIgnoreCase);
                var linqTest37 = personList.OrderBy(p => p.EmailPromotion).ThenByDescending(p => p.BusinessEntityID);

                var linqTest38 = words2.OrderBy(a => a.Length).ThenByDescending(a => a, StringComparer.OrdinalIgnoreCase);

                var linqTest39 = digits.Where(dig => dig[1] == 'i').Reverse();




                int[] factorsOf300 = { 2, 2, 3, 5, 5 };
                //remove duplicate elements
                var linqTest46 = factorsOf300.Distinct();

                var linqTest47 = personList.Select(p => p.BusinessEntityID).Distinct();

                int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
                int[] numbersB = { 1, 3, 5, 7, 8 };
                //This sample uses Union to create one sequence that contains the unique values from both arrays.
                // The duplicate elements will not exist
                var linqTest48 = numbersA.Union(numbersB);

                //var personID = personList.Select(p => p.BusinessEntityID);
                //var phoneNumber = personPhoneList.Select(p => p.PhoneNumberTypeID);
                var personID = personList.Select(p => p.PersonType);
                var phoneNumber = personPhoneList.Select(p => p.PhoneNumber);

                var linqTest49 = personID.Union(phoneNumber);

                var linqTest50 = numbersA.Intersect(numbersB);
                var linqTest51 = personID.Intersect(phoneNumber);
                //This sample uses Except to create a sequence that contains the values from numbersAthat are not also in numbersB.
                var linqTest52 = numbersA.Except(numbersB);
                var linqTest53 = personID.Except(phoneNumber);

                var linqTest54 = doubles.OrderByDescending(p => p).ToArray();
                var linqTest55 = words.ToList();

                var scoreRecords = new[] {
                    new {Name = "Alice", Score = 50},
                    new {Name = "Bob"  , Score = 40},
                    new {Name = "Cathy", Score = 45}
                };

                var linqTest56 = scoreRecords.ToDictionary(sr => sr.Name);

                object[] numbersZ = { null, 1.0, "two", 3, "four", 5, "six", 7.0 };
                //This sample uses OfType to return only the elements of the array that are of type double.
                var linqTest57 = numbersZ.OfType<double>();

                var linqTest58 = personList.Select(p => p.PersonType).First();
                var linqTest59 = digits.First(s => s[0] == 't');

                int[] numberForLinq61 = { };
                var linqTest61 = numberForLinq61.FirstOrDefault();
                var linqTest62 = personList.FirstOrDefault(p => p.BusinessEntityID == 25);

                int[] numberOf63 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                var linqTest64 = numberOf63.Where(num => num > 5).ElementAt(1);

                // If Combine method still exist ? Doesn't work !
                //var linqTest98 = personList.Combine(a => personPhoneList, (a, b) => )
                var linqTest65 = Enumerable.Range(100, 50).Select(n => new { Number = n, OddEven = n % 2 == 1 ? "Odd" : "Even" });
                var linqTest66 = Enumerable.Repeat(7, 10);

                string[] wordsForLinq67 = { "believe", "relief", "receipt", "field" };
                // return true or false
                var linqTest67 = wordsForLinq67.Any(w => w.Contains("ei"));
                var linqTest69 = personList
                    .GroupBy(p => p.EmailPromotion)
                    .Where(pg => pg.Any(p => p.PersonType == "EM"))
                    .Select(s => new
                    {
                        ID = s.Key,
                        Name = s
                    });

                int[] numbersForLinq70 = { 1, 11, 3, 19, 41, 65, 19 };
                // return true or false
                var linqTest70 = numbersForLinq70.All(n => n % 2 == 1);

                var linqTest72 = personList.All(p => p.PersonType == "EM");

                int[] primeFactorsOf300linqTest73 = { 2, 2, 3, 5, 5 };

                var linqTest73 = primeFactorsOf300linqTest73.Distinct().Count();

                int[] numberslintTest74 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
                var linqTest74 = numberslintTest74.Count(n => n % 2 == 1);
                var linqTest76 = personList.Select(p => new { p.BusinessEntityID, type = p.PersonType.Count() });
                var linqTest77 = personList.GroupBy(p => p.PersonType).Select(p => new { Type = p.Key, Count = p.Count() });

                int[] numberslinqTest78 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
                var linqTest78 = numberslinqTest78.Sum();

                string[] wordslinqTest79 = { "cherry", "apple", "blueberry" };

                var linqTest79 = wordslinqTest79.Sum(w => w.Length);
                var linqTest80 = personList
                    .GroupBy(p => p.PersonType)
                    .Select(n => new { name = n.Key, Sum = n.Sum(p => p.EmailPromotion) });
                int[] numberslinqTest81 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                var linqTest81 = numberslinqTest81.Min();

                string[] wordslinqTest82 = { "cherry", "apple", "blueberry" };

                var linqTest82 = wordslinqTest82.Min(w => w.Length);

                var linqTest83 = personList.GroupBy(p => p.PersonType)
                    .Select(p => new { name = p.Key, Min = p.Min(n => n.EmailPromotion) });

                int[] numberslinqTest85 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
                var linqTest85 = numberslinqTest85.Max();

                string[] wordslinqTest86 = { "cherry", "apple", "blueberry" };
                var linqTest86 = wordslinqTest86.Max(w => w.Length);

                var linqTest87 = personList.GroupBy(p => p.PersonType).Select(n => new { name = n.Key, Max = n.Max(p => p.EmailPromotion) });

                int[] numberslinqTest89 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
                var linqTest89 = numberslinqTest89.Average();
                
                string[] wordslinqTest90 = { "cherry", "apple", "blueberry" };
                var linqTest90 = wordslinqTest90.Average(w => w.Length);

                var linqTest91 = personList.GroupBy(p => p.PersonType).Select(n => new { name = n.Key, Average = n.Average(s => s.BusinessEntityID) });

                double[] doubleslinqTest92 = { 1.7, 2.3, 1.9, 4.1, 2.9 };

                var linqTest92 = doubleslinqTest92.Aggregate((runningProduct, nextFactor) => runningProduct * nextFactor);

                

                double startBalance = 100.0;

                int[] attemptedWithdrawals = { 20, 10, 40, 50, 10, 70, 30 };

                var linqTest93 = attemptedWithdrawals.Aggregate(startBalance, (balance, nextWidthdrawal) => ((nextWidthdrawal <= balance) ? (balance - nextWidthdrawal) : balance));

                int[] numbersAlinqTest94 = { 0, 2, 4, 5, 6, 8, 9 };
                int[] numbersBlinqTest94 = { 1, 3, 5, 7, 8 };


                var linqTest94 = numbersAlinqTest94.Concat(numbersBlinqTest94);
                var linqTest95 = personID.Concat(phoneNumber);

                var wordsAlinqTest96 = new string[] { "cherry", "apple", "blueberry" };
                var wordsBlinqTest96 = new string[] { "cherry", "apple", "blueberry" };

                var linqTest96 = wordsAlinqTest96.SequenceEqual(wordsBlinqTest96);

                var wordsAlinqTest97 = new string[] { "cherry", "apple", "blueberry" };
                var wordsBlinqTest97 = new string[] { "apple", "blueberry", "cherry" };

                var linqTest97 = wordsAlinqTest97.SequenceEqual(wordsBlinqTest97);

                var linqTest99 = numbers.Select(num => ++linqTest99Index);
                var linqTest100 = (numbers.Select(num => ++linqTest100Index)).ToList();

                var lowNumbers = numbers.Where(num => num <= 3);
                var linqTest101 = lowNumbers.Where(p => p % 2 == 0);

                //扁平化一維儲存
                var linqTest102 = personList.Join(personPhoneList, phone => phone.BusinessEntityID, person => person.BusinessEntityID, (p,l)=> new {   ID = p.BusinessEntityID,
                      person = p.PersonType,
                      phone = l.PhoneNumber
                });

                //二維方式儲存
                var linqTest103 = personList.GroupJoin(personPhoneList, per => per.BusinessEntityID, phon => phon.BusinessEntityID, (per, phon) => new { person = per.BusinessEntityID, phone = phon });



                // can't understand the question ask 
                //  string[] categories = new string[]{
                //  "Beverages",
                //  "Condiments",
                //  "Vegetables",
                //  "Dairy Products",
                //  "Seafood" };
                //var linqTest104 = categories
                //      .GroupJoin(personList, cat => cat, prod => prod., (cat, ps) => new { cat, ps })

                var linqTest105 = personList
                    .GroupJoin(
                    personPhoneList,
                    person => person.BusinessEntityID,
                    phone => phone.BusinessEntityID,
                    (person, phoneEnum) => new
                    {
                        person,
                        phone = phoneEnum
                    })
                    .SelectMany(
                       t => t.phone.DefaultIfEmpty(),
                       (t, p) => new { t, p }
                    )
                    .OrderBy(t => t.p.BusinessEntityID)
                    .Select(
                        t => new
                        {
                            NameId = t.t.person.BusinessEntityID,
                            phone = t.p.PhoneNumber
                        }
                    );

                var linqTest10502 = personList.GroupJoin(
                    personPhoneList,
                    person => person.BusinessEntityID,
                    phone => phone.BusinessEntityID,
                    (person, phone) =>
                    new
                    {
                        person.BusinessEntityID,
                        PhoneNumber = string.Join(",", phone.Select(x => x.PhoneNumber))
                    });

                var linqTest106 = personList.GroupJoin(
                    personPhoneList,
                    person => person.BusinessEntityID,
                    phone => phone.BusinessEntityID,
                    (person, phone) =>
                    new
                    {
                        NameID = person.BusinessEntityID,
                        PhoneNumber = string.Join(",", phone.Select(x => x.PhoneNumber)),
                        ID = person.PersonType == "EM" ? person.EmailPromotion : person.BusinessEntityID,
                        IDTest = person.PersonType == "EM" ? person.EmailPromotion.ToString() : phone.Select(x => x.PhoneNumber).ToString()
                    });

                // 107 the same with 106
                //var linqTest107 = personList






                return Ok(linqTest96);

            }
        }

        class Persons
        {
            public int BusinessEntityID { get; set; }
            public string PersonType { get; set; }
            public int EmailPromotion { get; set; }
        }

        class PersonPhone
        {
            public int BusinessEntityID { get; set; }
            public string PhoneNumber { get; set; }
            public int PhoneNumberTypeID { get; set; }
            public DateTime ModifiedDate { get; set; }
        }
    }
}
