using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using frauddetect.common.user.manager;
using MongoDB.Bson;
using frauddetect.common.user;

namespace frauddetect.unittests.common.user
{
    [TestClass]
    public class usertests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserManager_Initialize_MongoCSInNull()
        {
            new UserManager().Initialize(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserManager_Initialize_MongoCSInEmpty()
        {
            new UserManager().Initialize(string.Empty);
        }

        [TestMethod]
        public void UserManager_Initialize_Valid()
        {
            new UserManager().Initialize(@"mongodb://192.168.56.101:27017");
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void UserManager_Insert_Valid()
        {
            UserManager userManager = new UserManager();
            userManager.Initialize(@"mongodb://192.168.56.101:27017");

            User user = new User() { FirstName = "insertuser", SSN = "111 111 111" };
            user.ID = userManager.Insert(user);

            Assert.IsTrue(user != null);
            Assert.IsTrue(user.ID != null);
        }

        [TestMethod]
        public void UserManager_Update_Valid()
        {
            UserManager userManager = new UserManager();
            userManager.Initialize(@"mongodb://192.168.56.101:27017");

            User user = new User() { FirstName = "updateuser", SSN = "111 111 112" };
            user.ID = userManager.Insert(user);

            userManager.Update(user);
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void UserManager_Delete_Valid()
        {
            UserManager userManager = new UserManager();
            userManager.Initialize(@"mongodb://192.168.56.101:27017");

            User user = new User() { FirstName = "deleteuser", SSN = "111 111 113" };
            user.ID = userManager.Insert(user);

            userManager.Delete(user);

            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void UserManager_GetById_Valid()
        {
            UserManager userManager = new UserManager();
            userManager.Initialize(@"mongodb://192.168.56.101:27017");

            User user = new User() { FirstName = "getiduser", SSN = "111 111 114" };
            user.ID = userManager.Insert(user);

            User tmpUser = userManager.GetById(user.ID);
            Assert.IsTrue(tmpUser != null);
        }
    }
}
