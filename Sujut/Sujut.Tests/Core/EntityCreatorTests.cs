using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sujut.Core;

namespace Sujut.Tests.Core
{
    [TestFixture]
    public class EntityCreatorTests
    {
        [Test]
        public void DebtCalculationsFromJson_AddsAllSubEntities()
        {
            // Arrange
            const string json = "[{\"Id\":12,\"Name\":\"Testing\",\"Description\":null,\"Currency\":\"EUR\",\"LastActivityTime\":\"2013-05-27T15:02:43\",\"Phase\":3,\"CreatorId\":0,\"Participants\":[{\"Id\":1,\"Email\":\"nicklas.forss@iki.fi\",\"Firstname\":\"Nicklas\",\"Lastname\":\"Forss\",\"PaymentInstructions\":\"Skicka brevduva. Två gånger.\",\"DoneAddingExpenses\":true,\"HasPaid\":false},{\"Id\":30,\"Email\":\"harry@holkeri.fi\",\"Firstname\":\"Harry\",\"Lastname\":\"Holkeri\",\"PaymentInstructions\":null,\"DoneAddingExpenses\":true,\"HasPaid\":true},{\"Id\":31,\"Email\":\"mama@mia.fi\",\"Firstname\":\"Mama\",\"Lastname\":\"Mia\",\"PaymentInstructions\":null,\"DoneAddingExpenses\":true,\"HasPaid\":true}],\"Expenses\":[{\"Amount\":123.00000,\"Description\":\"adadad\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":1,\"UsersInDebtIds\":[30,31,1]},{\"Amount\":432.00000,\"Description\":\"adssd\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":30,\"UsersInDebtIds\":[30,31]},{\"Amount\":42.00000,\"Description\":\"dsaffds\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":30,\"UsersInDebtIds\":[30,31,1]},{\"Amount\":212.00000,\"Description\":\"gfgfdg\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":31,\"UsersInDebtIds\":[30,31,1]}],\"Debts\":[{\"Amount\":129.67000,\"DebtorId\":31,\"CreditorId\":30},{\"Amount\":2.67000,\"DebtorId\":1,\"CreditorId\":30},{\"Amount\":129.67000,\"DebtorId\":31,\"CreditorId\":30},{\"Amount\":2.67000,\"DebtorId\":1,\"CreditorId\":30}]}," +
                                 "{\"Id\":11,\"Name\":\"Blabla\",\"Description\":\"Jee\",\"Currency\":\"EUR\",\"LastActivityTime\":\"2013-05-20T21:59:21\",\"Phase\":2,\"CreatorId\":0,\"Participants\":[{\"Id\":1,\"Email\":\"nicklas.forss@iki.fi\",\"Firstname\":\"Nicklas\",\"Lastname\":\"Forss\",\"PaymentInstructions\":\"Skicka brevduva. Två gånger.\",\"DoneAddingExpenses\":false,\"HasPaid\":false}],\"Expenses\":[{\"Amount\":12.10000,\"Description\":\"adadas\",\"AddedTime\":\"2013-05-20T21:48:57\",\"PayerId\":1,\"UsersInDebtIds\":[1]},{\"Amount\":12.10000,\"Description\":\"aads\",\"AddedTime\":\"2013-05-20T21:59:21\",\"PayerId\":1,\"UsersInDebtIds\":[1]}],\"Debts\":[]}]";

            // Act
            var result = EntityCreator.DebtCalculationsFromJson(json);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void DebtCalculationFromJson_AddsAllSubEntities()
        {
            // Arrange
            const int id = 30;
            const string name = "Some calc";
            const string desc = "Real great calc";
            const string currency = "EUR";
            const OldDebtCalculationPhase phase = OldDebtCalculationPhase.CollectingExpenses;
            const string time = "2013-05-27T14:06:35";
            const long creatorId = 43;

            string json = "{\"Id\":" + id + ",\"Name\":\""+ name + "\",\"Description\":\"" + desc + "\",\"Currency\":\"" + currency + 
                "\",\"LastActivityTime\":\""+ time + "\",\"Phase\":" + (int)phase + ",\"CreatorId\":" + creatorId + ",\"Participants\":[{\"Id\":1,\"Email\":\"nicklas.forss@iki.fi\",\"Firstname\":\"Nicklas\",\"Lastname\":\"Forss\",\"PaymentInstructions\":\"Skicka brevduva. Två gånger.\",\"DoneAddingExpenses\":true,\"HasPaid\":false},{\"Id\":30,\"Email\":\"harry@holkeri.fi\",\"Firstname\":\"Harry\",\"Lastname\":\"Holkeri\",\"PaymentInstructions\":null,\"DoneAddingExpenses\":true,\"HasPaid\":true},{\"Id\":31,\"Email\":\"mama@mia.fi\",\"Firstname\":\"Mama\",\"Lastname\":\"Mia\",\"PaymentInstructions\":null,\"DoneAddingExpenses\":true,\"HasPaid\":true}],\"Expenses\":[{\"Amount\":123.00000,\"Description\":\"adadad\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":1,\"UsersInDebtIds\":[30,31,1]},{\"Amount\":432.00000,\"Description\":\"adssd\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":30,\"UsersInDebtIds\":[30,31]},{\"Amount\":42.00000,\"Description\":\"dsaffds\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":30,\"UsersInDebtIds\":[30,31,1]},{\"Amount\":212.00000,\"Description\":\"gfgfdg\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":31,\"UsersInDebtIds\":[30,31,1]}],\"Debts\":[{\"Amount\":129.67000,\"DebtorId\":31,\"CreditorId\":30},{\"Amount\":2.67000,\"DebtorId\":1,\"CreditorId\":30},{\"Amount\":129.67000,\"DebtorId\":31,\"CreditorId\":30},{\"Amount\":2.67000,\"DebtorId\":1,\"CreditorId\":30}]}";

            // Act
            var result = EntityCreator.DebtCalculationFromJson(json);

            // Assert
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(desc, result.Description);
            Assert.AreEqual(currency, result.Currency);
            Assert.AreEqual(phase, result.Phase);
            Assert.AreEqual(new DateTime(2013, 05, 27, 14, 06, 35), result.LastActivityTime);
            Assert.AreEqual(creatorId, result.CreatorId);
            Assert.AreEqual(3, result.Participants.Count());
            Assert.AreEqual(4, result.Expenses.Count());
            Assert.AreEqual(4, result.Debts.Count());
        }

        [Test]
        public void ParticipantsFromJson_AddsAllSubEntities()
        {
            // Arrange
            const string json = "[{\"Id\":1,\"Email\":\"nicklas.forss@iki.fi\",\"Firstname\":\"Nicklas\",\"Lastname\":\"Forss\",\"PaymentInstructions\":\"Skicka brevduva. Två gånger.\",\"DoneAddingExpenses\":true,\"HasPaid\":false}," +
                                "{\"Id\":30,\"Email\":\"harry@holkeri.fi\",\"Firstname\":\"Harry\",\"Lastname\":\"Holkeri\",\"PaymentInstructions\":null,\"DoneAddingExpenses\":true,\"HasPaid\":true}," +
                                "{\"Id\":31,\"Email\":\"mama@mia.fi\",\"Firstname\":\"Mama\",\"Lastname\":\"Mia\",\"PaymentInstructions\":null,\"DoneAddingExpenses\":true,\"HasPaid\":true}]";

            // Act
            var result = EntityCreator.ParticipantsFromJson(json);

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public void ParticipantFromJson_AddsAllSubEntities()
        {
            // Arrange
            const int id = 30;
            const string firstName = "Harry";
            const string lastName = "Holkeri";
            const string email = "harry@holkeri.fi";
            const string paymentInst = "As fast as possible";
            const string doneAdding = "true";
            const string hasPaid = "true";

            var json = "{\"Id\":" + id + ",\"Email\":\""+ email + "\",\"Firstname\":\"" + firstName + "\",\"Lastname\":\"" + lastName + 
                "\",\"PaymentInstructions\":\"" + paymentInst +"\",\"DoneAddingExpenses\":" + doneAdding + ",\"HasPaid\":"+ hasPaid +"}";

            // Act
            var result = EntityCreator.ParticipantFromJson(json);

            // Assert
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(firstName, result.Firstname);
            Assert.AreEqual(lastName, result.Lastname);
            Assert.AreEqual(email, result.Email);
            Assert.AreEqual(paymentInst, result.PaymentInstructions);
            Assert.IsTrue(result.DoneAddingExpenses);
            Assert.IsTrue(result.HasPaid);
        }

        [Test]
        public void ExpensesFromJson_AddsAllSubEntities()
        {
            // Arrange
            const string json = "[{\"Amount\":123.00000,\"Description\":\"adadad\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":1,\"UsersInDebtIds\":[30,31,1]}," +
                                "{\"Amount\":432.00000,\"Description\":\"adssd\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":30,\"UsersInDebtIds\":[30,31]}," +
                                "{\"Amount\":42.00000,\"Description\":\"dsaffds\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":30,\"UsersInDebtIds\":[30,31,1]}," +
                                "{\"Amount\":212.00000,\"Description\":\"gfgfdg\",\"AddedTime\":\"2013-05-27T14:06:35\",\"PayerId\":31,\"UsersInDebtIds\":[30,31,1]}]";

            // Act
            var result = EntityCreator.ExpensesFromJson(json);

            // Assert
            Assert.AreEqual(4, result.Count());
       }

        [Test]
        public void ExpenseFromJson_AddsAllSubEntities()
        {
            // Arrange
            const string amount = "123.02";
            const string desc = "something";
            const string time = "2013-05-27T14:06:35";
            const long payerId = 30;
            const string usersInDebt = "[30,31,1]";

            string json = "{\"Amount\":"+ amount +",\"Description\":\""+ desc +"\",\"AddedTime\":\""+ time +
                            "\",\"PayerId\":"+ payerId +",\"UsersInDebtIds\":" + usersInDebt +"}";

            // Act
            var result = EntityCreator.ExpenseFromJson(json);

            // Assert
            Assert.AreEqual(123.02m, result.Amount);
            Assert.AreEqual(desc, result.Description);
            Assert.AreEqual(new DateTime(2013, 05, 27, 14, 06, 35), result.AddedTime);
            Assert.AreEqual(payerId, result.PayerId);
            Assert.AreEqual(3, result.UsersInDebtIds.Count());
            Assert.IsTrue(result.UsersInDebtIds.Contains(30));
            Assert.IsTrue(result.UsersInDebtIds.Contains(31));
            Assert.IsTrue(result.UsersInDebtIds.Contains(1));
        }

        [Test]
        public void DebtsFromJson_AddsAllSubEntities()
        {
            // Arrange
            const string json = "[{\"Amount\":139.67000,\"DebtorId\":31,\"CreditorId\":30}," +
                                "{\"Amount\":2.67000,\"DebtorId\":1,\"CreditorId\":30}," +
                                "{\"Amount\":127.67000,\"DebtorId\":31,\"CreditorId\":30}," +
                                "{\"Amount\":0.67000,\"DebtorId\":1,\"CreditorId\":30}]";

            // Act
            var result = EntityCreator.DebtsFromJson(json);

            // Assert
            Assert.AreEqual(4, result.Count());
        }

        [Test]
        public void DebtFromJson_AddsAllSubEntities()
        {
            // Arrange
            const string amount = "123.02";
            const long debtorId = 30;
            const long creditorId = 33;

            string json = "{\"Amount\":"+ amount +",\"DebtorId\":"+ debtorId +",\"CreditorId\":"+ creditorId +"}";

            // Act
            var result = EntityCreator.DebtFromJson(json);

            // Assert
            Assert.AreEqual(123.02m, result.Amount);
            Assert.AreEqual(creditorId, result.CreditorId);
            Assert.AreEqual(debtorId, result.DebtorId);
        }
    }
}
