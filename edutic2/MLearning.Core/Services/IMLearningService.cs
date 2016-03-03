using Core.Entities.json;

using MLearning.Core.Entities;
using MLearningDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MLearning.Core.Configuration;
using System.IO;
using Core.Repositories;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace MLearning.Core.Services
{

    //TODO: When creating LO, should update UserLO table 
    public interface IMLearningService
    {

        //Ex of use :validate username
        Task<bool> CheckIfExists<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate,Func<T,int> getID) where T : new();
        //Ex of use : validate username no locale
        Task<bool> CheckIfExistsNoLocale<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate, Func<T, int> getID) where T : new();

        // Recommendend
        //Validate user credentials
        Task<LoginOperationResult<T>> ValidateLogin<T>(T account, Expression<Func<T, bool>> validation, Func<T, int> getID, Func<T, int> getType);
         
        //Deprecated
        //Validate Consumer Credentials
        Task<LoginOperationResult<user_consumer>> ValidateConsumerLogin(string username, string password);

        /// <summary>
        /// Create an MLearning account
        /// </summary>
        /// <typeparam name="T">User</typeparam>
        /// <param name="account">User object</param>
        /// <param name="getID">Function that gets the object ID</param>
        /// <param name="type">Type of User: Consumer, Publisher, Admin</param>
        /// <returns></returns>
        Task<OperationResult> CreateAccount<T>(T account, Func<T, int> getID, UserType type);

        /// <summary>
        /// Create a Publisher Account and register it to the institution identified by institution_id. If no institution use Constants.NoInstitution
        /// </summary>
        /// <param name="account"></param>
        /// <param name="publisher"></param>
        /// <param name="institution_id"></param>
        /// <returns></returns>
        Task<OperationResult> CreateAndRegisterPublisher(User account, Publisher publisher,int institution_id);

        /// <summary>
        /// Create a Consumer Account and register it to the institution identified by institution_id. If no institution use Constants.NoInstitution
        /// </summary>
        /// <param name="account"></param>
        /// <param name="institution_id"></param>
        /// <returns></returns>
        Task<OperationResult> CreateAndRegisterConsumer(User account, int institution_id);

        IRepositoryService repositoryService();

        Task RegisterUserToInstitution(int user_id, int institution_id);



        /// <summary>
        /// Create a Circle (Classroom)
        /// </summary>
        /// <param name="ownerid">User ID of type: Publisher</param>
        /// <param name="name">Circle name</param>
        /// <param name="type">Type: can be CircleType.Public or CircleType.Private</param>
        /// <returns></returns>
        Task<int> CreateCircle(int ownerid,string name,int type);

        Task<int> CreateCircle(Circle circle);

        /// <summary>
        /// Registers a user in a circle
        /// </summary>
        /// <param name="userid">User ID</param>
        /// <param name="circleid">Circle ID</param>
        /// <returns></returns>
        Task AddUserToCircle(int userid, int circleid);

        Task RemoveUserFromCircle(int user_id, int circle_id);

        /// <summary>
        /// Persist an object in the database of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">object to persist</param>
        /// <param name="getId">function to get object ID</param>
        /// <returns></returns>
        Task<int> CreateObject<T>(T obj,Func<T,int> getId);
        /// <summary>
        /// Update an object in the database of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task UpdateObject<T>(T obj);


        /// <summary>
        /// Deletes an object of type T in the database 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task DeleteObject<T>(T obj);

        /// <summary>
        /// Search an object by id in table T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetObjectWithId<T>(int id);

        /// <summary>
        /// Publish Learning Object with id: loid to a Circle identified with circleid
        /// </summary>
        /// <param name="circleid"></param>
        /// <param name="loid"></param>
        /// <returns></returns>
        Task PublishLearningObjectToCircle(int circleid, int loid);


        //Warning: Using LIKE
        /// <summary>
        /// List all circles whose name start with startWith
        /// </summary>
        /// <param name="startWith"></param>
        /// <returns></returns>
        Task<List<Circle>> GetCircles(string startWith);

        Task<List<Circle>> GetCircles();

        /// <summary>
        /// List all Institutions
        /// </summary>
        /// <returns></returns>
        Task<List<Institution>> GetInstitutions();


        /// <summary>
        /// List all circles a user with userid is registered in
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<List<circle_by_user>> GetCirclesByUser(int userid);
        Task<List<Circle>> GetCirclesByOwner(int user_id);

        /// <summary>
        /// List all Learning Objects that a User with userid can access
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<List<available_lo_by_user>> GetAvailableLOByUser(int userid);


        /// <summary>
        /// List Learning objects available for a user in a Circle
        /// </summary>
        /// <param name="circleid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<List<lo_by_circle>> GetLOByCircleAndUser(int circleid,int userid);

        Task<List<lo_in_circle>> GetLOByCircle(int circleid);

        Task<List<LearningObject>> GetLOByUserOwner(int user_id);

        Task<List<LearningObject>> GetPublicLOs();

        Task<List<lo_by_owner>> GetLOsbyOwner();

        Task<List<lo_by_owner>> GetLOsbyOwner(int user_id);

        /// <summary>
        /// List all users registered in a circle
        /// </summary>
        /// <param name="circleid"></param>
        /// <returns></returns>
        Task<List<user_by_circle>> GetUsersInCircle(int circleid);


        /// <summary>
        /// List all circle's posts
        /// </summary>
        /// <param name="circleid"></param>
        /// <returns></returns>
        Task<List<post_with_username>> GetPostByCircle(int circleid);
       
        /// <summary>
        /// List all publishers in an institution
        /// </summary>
        /// <param name="inst_id"></param>
        /// <returns></returns>
        Task<List<publisher_by_institution>> GetPublishersByInstitution(int inst_id);

        Task<List<head_by_institution>> GetHeads();

        /// <summary>
        /// List all consumers registered in a circle
        /// </summary>
        /// <param name="circle_id"></param>
        /// <returns></returns>
        Task<List<consumer_by_circle>> GetConsumersByCircle(int circle_id);

        /// <summary>
        /// Make a user like or dislike the given Learning Object
        /// </summary>
        /// <param name="lo_id"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        Task LikeOrDislikeLO(int lo_id, int user_id);

        /// <summary>
        /// List all comments of the given Learning Object
        /// </summary>
        /// <param name="lo_id"></param>
        /// <returns></returns>
        Task<List<lo_comment_with_username>> GetLOComments(int lo_id);

        /// <summary>
        /// Logouts current user
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        Task LogoutUser(int user_id);

        /// <summary>
        /// List all tags of the given circle
        /// </summary>
        /// <param name="circle_id"></param>
        /// <returns></returns>
        /// 
        Task<List<tag_by_circle>> GetTagsByCircle(int circle_id);

        Task<List<tag_by_page>> GetTagsByLO(int lo_id);
        Task<List<tag_by_lo>> GetLOTags(int lo_id);

        Task<List<tag_by_page>> GetTagsByPage(int page_id);


        Task<List<Tag>> GetAllTags();


        Task AddTagToPage(int tag_id, int page_id);

        Task AddTagToLO(int tag_id, int lo_id);

        Task<List<quiz_by_circle>> GetQuizzesByCircle(int circle_id);

        Task<List<Quiz>> GetQuizzesByLO(int lo_id);

        Task<List<Question>> GetQuestionsByQuiz(int quiz_id);

        Task<List<QuestionOption>> GetOptionsByQuestion(int question_id);

        Task SaveUserAnswer(int user_id, int question_id,string answer);

        /// <summary>
        /// Create the new Head account and the related institution
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="inst_head"></param>
        /// <returns></returns>
        Task<int> CreateInstitution(Institution inst, Head head_info, User user_credentials);

        /// <summary>
        /// Returns the Institution ID of the Head User with the given UserID
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        Task<int> GetHeadInstitutionID(int UserID);

        /// <summary>
        /// List all consumers registered in a given institution
        /// </summary>
        /// <param name="inst_id"></param>
        /// <returns></returns>
        Task<List<consumer_by_institution>> GetConsumersByInstitution(int inst_id);

        Task<List<circle_by_owner>> GetCirclesByInstitution(int inst_id);

        Task UnSubscribeConsumerFromCircle(int user_id, int circle_id);

        Task<int> GetPublisherInstitutionID(int user_id);


        Task<List<Page>> GetPagesByLO(int lo_id);


        /// <summary>
        /// Try to create the user if it doesn't exist
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        Task<int> TryCreateUser(string socialID, int idInstitution);

        /// <summary>
        /// Upload image to Azure Storage
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        Task<string> UploadResource(Stream s,string filename);

        Task DeleteTagFromPage(int tag_id, int page_id);

        Task DeleteTagFromLO(int tag_id, int lo_id);

        Task<List<LOsection>> GetSectionsByLO(int lo_id);

        Task<List<Page>> GetPagesByLOSection(int sec_id);

		Task<Page> GetFirstSlidePageByLOSection (int sec_id); 

        /// <summary>
        /// return institution by user id
        /// </summary>
        /// <param name="user_id">user id</param>
        /// <returns>institution consumer</returns>
        Task<Institution_has_User> GetInstitutionByUser(int user_id);
        Task<MobileServiceUser> LoginProvider(int provider, JObject access_token);

        void Logout();

        Task<Circle_has_LO> GetCircleLOByIdLO(int id_lo);

        Task<CircleUser> GetCircleUser(int idUser, int idCircle);
    }
}
