using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.common.user.manager
{
    public class UserCreditDetailManager
    {
        #region Private Variable

        private const string UserDb = "UserDB";
        private const string CollectionName = "UserCreditDetails";

        private MongoCollection<UserCreditDetail> UserCreditDetailsCollection;

        private bool Initialized;

        #endregion

        #region Public functions

        public void Initialize(string mongoDB)
        {
            if (string.IsNullOrWhiteSpace(mongoDB)) { throw new ArgumentException("Mongo database connection string is empty."); }

            UserCreditDetailsCollection = new MongoClient(mongoDB).GetServer().GetDatabase(UserDb).GetCollection<UserCreditDetail>(CollectionName);
            Initialized = true;
        }

        public void Insert(UserCreditDetail userCreditDetail)
        {
            IsInitialized();

            if(userCreditDetail == null) { throw new ArgumentNullException("User Credit detail is null."); }
            if(string.IsNullOrWhiteSpace(userCreditDetail.PrimaryUserId)) { throw new ArgumentException("Primary user id is blank."); }

            long count = UserCreditDetailsCollection.Find(Query<UserCreditDetail>.EQ(u => u.Account, userCreditDetail.Account)).Count();
            if (count > 0) { throw new Exception("Account already exists."); }

            WriteConcernResult result = UserCreditDetailsCollection.Insert(userCreditDetail);
            if (result != null && !result.Ok) { throw new Exception("Failed to insert user credit detail record."); }

            return;
        }

        public UserCreditDetail GetByAccount(string account)
        {
            IsInitialized();

            if (string.IsNullOrWhiteSpace(account)) { throw new ArgumentNullException("Account number is blank."); }

            List<UserCreditDetail> userCreditDetails = UserCreditDetailsCollection.Find(Query<UserCreditDetail>.EQ(u => u.Account, account)).SetFields(Fields.Exclude("_id")).ToList();
            if (userCreditDetails == null || userCreditDetails.Count == 0) { return null; }

            return userCreditDetails[0];
        }

        #endregion

        #region Private functions

        private void IsInitialized()
        {
            if (!Initialized) { throw new Exception("User Credit Details manager isn't initialized."); }
        }

        #endregion
    }
}
