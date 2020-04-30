using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelegramBotWantedCarsList;

namespace TelegramBot_UnitTests
{
    [TestClass]
    public class TestClassForTelegramBot
    {
        Form1 f1 = new Form1();
        List<CarInfo> cars = new List<CarInfo>();
        List<UserSubscribes> users = new List<UserSubscribes>();
        [TestMethod]
        public void TestGetCarsInfoTrue()
        {
            bool res = false;
            bool expectedRes = true;
            f1.getCarsInfo();
            if(f1.cars.Count > 0)
            {
                res = true;
            }

            Assert.AreEqual(expectedRes,res);
        }
        [TestMethod]
        public void TestGetCarsInfoFalse()
        {
            bool res = false;
            bool expectedRes = false;
            f1.getCarsInfo();
            if (f1.cars.Count == 0)
            {
                res = true;
            }

            Assert.AreEqual(expectedRes, res);
        }
        [TestMethod]
        public void TestGetUsersInfoTrue()
        {
            bool res = false;
            bool expectedRes = true;
            f1.getUsers();
            if (f1.users.Count > 0)
            {
                res = true;
            }

            Assert.AreEqual(expectedRes, res);
        }
        [TestMethod]
        public void TestGetUsersInfoFalse()
        {
            bool res = false;
            bool expectedRes = false;
            f1.getUsers();
            if (f1.users.Count == 0)
            {
                res = true;
            }

            Assert.AreEqual(expectedRes, res);
        }
        [TestMethod]
        public void TestCheckUsersSubscribesTrue()
        {
            f1.getUsers();
            f1.getCarsInfo();
            bool res = false;
            bool expectedRes = true;
            List<CarInfo> found = f1.checkCarForUserSubscribes();
            if(found.Count > 0)
            {
                res = true;
            }

            Assert.AreEqual(expectedRes,res);
        }
        [TestMethod]
        public void TestCheckUsersSubscribesFalse()
        {
            f1.getUsers();
            f1.getCarsInfo();
            bool res = false;
            bool expectedRes = false;
            List<CarInfo> found = f1.checkCarForUserSubscribes();
            if (found.Count == 0)
            {
                res = true;
            }

            Assert.AreEqual(expectedRes, res);
        }
        [TestMethod]
        public void TestCheckIfUserExistTrue()
        {
            f1.getUsers();
            bool res = true;
            bool expectedRes = false;
            res = f1.CheckIfUserExist("442173328");

            Assert.AreEqual(expectedRes,res);
        }
        [TestMethod]
        public void TestCheckIfUserExistFalse()
        {
            f1.getUsers();
            bool res = true;
            bool expectedRes = true;
            res = f1.CheckIfUserExist("442173328");

            Assert.AreNotEqual(expectedRes, res);
        }
        [TestMethod]
        public void TestFindCarTrue()
        {
            bool res = false;
            bool expectedRes = true;
            f1.getCarsInfo();
            List<CarInfo> foundFirst = f1.FindCar("VEHICLENUMBER", "20475ЕА");
            List<CarInfo> foundSecond = f1.FindCar("BODYNUMBER", "JTEBPAX8100221874");
            List<CarInfo> foundThird = f1.FindCar("ENGINENUMBER", "V1198CUBCM");
            if(foundFirst.Count > 0 && foundSecond.Count > 0 && foundThird.Count > 0)
            {
                res = true;
            }

            Assert.AreEqual(expectedRes,res);
        }
        [TestMethod]
        public void TestFindCarFalse()
        {
            bool res = false;
            bool expectedRes = false;
            f1.getCarsInfo();
            List<CarInfo> foundFirst = f1.FindCar("VEHICLENUMBER", "20475ЕА");
            List<CarInfo> foundSecond = f1.FindCar("BODYNUMBER", "JTEBPAX8100221874");
            List<CarInfo> foundThird = f1.FindCar("ENGINENUMBER", "V1198CUBCM");
            if (foundFirst.Count == 0 || foundSecond.Count == 0 || foundThird.Count == 0)
            {
                res = true;
            }

            Assert.AreEqual(expectedRes, res);
        }
    }
}
