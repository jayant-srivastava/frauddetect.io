using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.common.user.manager
{
    public class UserManager
    {
        #region Private Variable

        private const string USERDB = "UserDB";
        private const string COLLECTIONNAME = "User";

        private MongoCollection<User> UserCollection;

        #endregion

        public void Initialize(string mongoDB)
        {
            if (string.IsNullOrWhiteSpace(mongoDB))
            {
                throw new ArgumentException("Mongo database connection string is empty.");
            }

            UserCollection = new MongoClient(mongoDB).GetServer().GetDatabase(USERDB).GetCollection<User>(COLLECTIONNAME);
        }

        public ObjectId Insert(User user)
        {
            if(user == null)
            {
                throw new ArgumentNullException("User input object is null.");
            }

            if(string.IsNullOrWhiteSpace(user.SSN))
            {
                throw new ArgumentException("User SSN is empty.");
            }

            long count = UserCollection.Find(Query<User>.EQ(u => u.SSN, user.SSN)).Count();
            if (count == 1)
            {
                throw new Exception("User already exists.");
            }

            WriteConcernResult result = UserCollection.Insert(user);
            if(result != null && !result.Ok)
            {
                throw new Exception("Failed to insert user record.");
            }

            return user.ID;
        }

        public void Update(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User input object is null.");
            }

            if(user.ID == null)
            {
                throw new ArgumentException("User Id is null.");
            }

            if (string.IsNullOrWhiteSpace(user.SSN))
            {
                throw new ArgumentException("User SSN is empty.");
            }

            long count = UserCollection.Find(Query.And(Query<User>.EQ(u => u.SSN, user.SSN), Query<User>.EQ(u => u.ID, user.ID))).Count();
            if (count == 0)
            {
                throw new Exception("User doesn't exists.");
            }

            WriteConcernResult result = UserCollection.Save(user);
            if (result != null && !result.Ok)
            {
                throw new Exception("Failed to update user record.");
            }

            return;
        }

        public void Delete(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User input object is null.");
            }

            if (user.ID == null)
            {
                throw new ArgumentException("User Id is null.");
            }

            if (string.IsNullOrWhiteSpace(user.SSN))
            {
                throw new ArgumentException("User SSN is empty.");
            }

            WriteConcernResult result = UserCollection.Remove(Query.And(Query<User>.EQ(u => u.SSN, user.SSN), Query<User>.EQ(u => u.ID, user.ID)));
            if (result != null && !result.Ok)
            {
                throw new Exception("Failed to update user record.");
            }

            return;
        }

        public User GetById(ObjectId id)
        {
            if(id == null)
            {
                throw new ArgumentNullException("Id is null.");
            }

            List<User> users = UserCollection.Find(Query<User>.EQ(u => u.ID, id)).ToList();
            if(users == null || users.Count != 1)
            {
                throw new Exception("User doesn't exist.");
            }

            return users[0];
        }
    }
}
