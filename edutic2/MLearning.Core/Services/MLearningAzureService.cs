//#define WEB
using Core.DownloadCache;
using Core.Entities.json;
using Core.Repositories;
using Core.Session;

using MLearning.Core.Entities;
using MLearningDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MLearning.Core.Configuration;
using System.IO;
using AzureBlobUploader;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace MLearning.Core.Services
{
    public class MLearningAzureService : IMLearningService
    {


        private IRepositoryService _repositoryService;


        public MLearningAzureService(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }


        public IRepositoryService repositoryService()
        {
            return _repositoryService;
        }

        // Serverside: if login OK, sets field is_online to TRUE
        public async Task<LoginOperationResult<T>> ValidateLogin<T>(T account, Expression<Func<T, bool>> validation, Func<T, int> getID, Func<T, int> getType)
        {
            var list = await _repositoryService.SearchForAsync<T>(validation, new Dictionary<string, string> { { "login", "1" } }, false);



            LoginOperationResult<T> result = new LoginOperationResult<T>();

            if (list.Count() == 0)
            {
                //User doesnt exist
                result.id = -1;
                result.successful = false;


            }
            else
            {
                T element = list[0];
                result.id = getID(element);
                result.successful = true;
                //Login Customer
                result.userType = getType(element);
                result.account = element;

            }



            return result;
        }

        public async Task<LoginOperationResult<user_consumer>> ValidateConsumerLogin(string username, string password)
        {
            var list = await _repositoryService.SearchForAsync<user_consumer>(u => u.username == username && u.password == password,u=>u.updated_at,u=>u.id,false);
            LoginOperationResult<user_consumer> result = new LoginOperationResult<user_consumer>();
            if (list.Count() == 0)
            {
                //User doesnt exist
                result.id = -1;
                result.successful = false;


            }
            else
            {
                var element = list[0];
                result.id = element.id;
                result.successful = true;

                result.account = element;

               

            }

            return result;

        }
        public async Task<OperationResult> CreateAccount<T>(T account, Func<T, int> getID,UserType type)
        {

        


                 OperationResult result = new OperationResult();

                 int itype = (int)type;
                 await _repositoryService.InsertAsync<T>(account, new Dictionary<string, string> { {"type",itype.ToString()}});

                result.successful = true;
                result.id = getID(account);
          

            return result;
        }


        public async Task<bool> CheckIfExists<T>(Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate,Func<T,int> getID) where T : new()
        {
            var list = await _repositoryService.SearchForAsync<T>(predicate,getLastUpdate,getID, true);

            return list.Count > 0;
        }

        public async Task<bool> CheckIfExistsNoLocale<T>(Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate, Func<T, int> getID) where T : new()
        {
            var list = await _repositoryService.SearchForAsync<T>(predicate, getLastUpdate, getID, false);

            return list.Count > 0;
        }

        public async Task<int> CreateCircle(int ownerid,string name,int type)
        {
            
            Circle c = new Circle { owner_id = ownerid, name = name, type = type, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };

            await _repositoryService.InsertAsync<Circle>(c);

            return c.id;
        }

        public async Task<int> CreateCircle(Circle circle)
        {

            circle.created_at = DateTime.UtcNow;
            circle.updated_at = DateTime.UtcNow;

            await _repositoryService.InsertAsync<Circle>(circle);

            return circle.id;
        }
    
        public async Task<int> CreateObject<T>(T obj, Func<T,int> getId)
        {
            await _repositoryService.InsertAsync<T>(obj);

            return getId(obj);
        }

        public async  Task PublishLearningObjectToCircle(int circleid, int loid)
        {
            Circle_has_LO circleLO = new Circle_has_LO { circle_id = circleid, lo_id = loid, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };

            await _repositoryService.InsertAsync<Circle_has_LO>(circleLO);
        }

        public async Task<List<circle_by_user>> GetCirclesByUser(int userid)
        {
			int a = 0;
            #if (WEB)
                        return await _repositoryService.SearchForAsync<circle_by_user>(c => c.User_id==userid, c => c.updated_at, c => c.id, false);
            #else
                        return await _repositoryService.SearchForAsync<circle_by_user>(c => c.User_id == userid, c => c.updated_at, c => c.id, true);
            #endif
        }

        public async Task<List<Circle>> GetCirclesByOwner(int user_id)
        {
             #if (WEB)
                          return await _repositoryService.SearchForAsync<Circle>(c => c.owner_id == user_id, c => c.updated_at, c => c.id, false);
            #else
				return await _repositoryService.SearchForAsync<Circle>(c => c.owner_id == user_id, c => c.updated_at, c => c.id, true);
            #endif
        }

        public async Task<List<available_lo_by_user>> GetAvailableLOByUser(int userid)
        {
             #if (WEB)
                  return await _repositoryService.SearchForAsync<available_lo_by_user>(a=>a.User_id==userid,a=>a.updated_at,a=>a.id,false);
            #else
                  return await _repositoryService.SearchForAsync<available_lo_by_user>(a=>a.User_id==userid,a=>a.updated_at,a=>a.id,true);
            #endif
        }

        public async Task<List<lo_by_circle>> GetLOByCircleAndUser(int circleid,int userid)
        {
            #if (WEB)
               return await _repositoryService.SearchForAsync<lo_by_circle>(l => l.Circle_id == circleid && l.User_id==userid, l => l.updated_at, l => l.id, false);
            #else
               return await _repositoryService.SearchForAsync<lo_by_circle>(l => l.Circle_id == circleid && l.User_id==userid, l => l.updated_at, l => l.id, false);
            #endif
        }

        public async Task<List<lo_in_circle>> GetLOByCircle(int circleid)
        {
            #if (WEB)
            var list =  await _repositoryService.SearchForAsync<lo_in_circle>(l => l.Circle_id == circleid, l => l.updated_at, l => l.id, false);
            #else
			var list =  await _repositoryService.SearchForAsync<lo_in_circle>(l => l.Circle_id == circleid, l => l.updated_at, l => l.id, false);
            #endif

            return list.GroupBy(l => l.title).Select(g => g.First()).ToList();
        }

        public async Task<List<LearningObject>> GetLOByUserOwner(int user_id)
        {
            #if (WEB)
            var pubList = await _repositoryService.SearchForAsync<Publisher>(p => p.User_id == user_id,new Dictionary<string,string>(),false);
            var pubObj = pubList.FirstOrDefault();
            return await _repositoryService.SearchForAsync<LearningObject>(lo => lo.Publisher_id == pubObj.id, lo => lo.updated_at, lo => lo.id, false);

            #else
              var pubList = await _repositoryService.SearchForAsync<Publisher>(p => p.User_id == user_id,new Dictionary<string,string>(),true);
                var pubObj = pubList.FirstOrDefault();
                return await _repositoryService.SearchForAsync<LearningObject>(lo => lo.Publisher_id == pubObj.id, lo => lo.updated_at, lo => lo.id, true);
            #endif
        
        }

        public async Task<List<LearningObject>> GetPublicLOs()
        {
            #if (WEB)
                        
            return await _repositoryService.SearchForAsync<LearningObject>(lo => lo.type ==(int) LOType.Public, lo => lo.updated_at, lo => lo.id, false);
#else
            return await _repositoryService.SearchForAsync<LearningObject>(lo => lo.type == (int)LOType.Public, lo => lo.updated_at, lo => lo.id, true);
#endif

        }


        public async Task<List<lo_by_owner>> GetLOsbyOwner()
        {
            return await _repositoryService.SearchForAsync<lo_by_owner>(lo => true, new Dictionary<string, string>(), false);
        }

        public async Task<List<lo_by_owner>> GetLOsbyOwner(int user_id)
        {
            return await _repositoryService.SearchForAsync<lo_by_owner>(lo => lo.user_id == user_id , new Dictionary<string, string>(), false);
        }

        public async Task DownloadLOPage(string url_package)
        {

            //Register download
           // No need, because everytime a User is added to a circle ,a tuple in UserLO is created for every LO in that Circle
           // _repositoryService.InsertAsync<UserLO>(new UserLO { User_id = userid, LearningObject_id = lo_id,created_at=DateTime.UtcNow,updated_at=DateTime.UtcNow});

            //Download package
            CacheService cache = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);
             var request = (HttpWebRequest)WebRequest.Create(url_package);
             await cache.makeRequest(request, (ss,localpath) => { }, (err) => { throw err; }, true);


           
        }


        public async Task<List<user_by_circle>> GetUsersInCircle(int circleid)
        {
            #if (WEB)
            return await _repositoryService.SearchForAsync<user_by_circle>(u => u.Circle_id == circleid,c=>c.updated_at,c=>c.id,false);

            #else
             return await _repositoryService.SearchForAsync<user_by_circle>(u => u.Circle_id == circleid,c=>c.updated_at,c=>c.id,true);

            #endif
        }

        public async Task<List<post_with_username>> GetPostByCircle(int circleid)
        {
              #if (WEB)
               return await _repositoryService.SearchForAsync<post_with_username>(p => p.circle_id == circleid, p => p.updated_at, p => p.id, false);

            #else
             return await _repositoryService.SearchForAsync<post_with_username>(p => p.circle_id == circleid, p => p.updated_at, p => p.id, true);
            #endif
        }





        public async Task AddUserToCircle(int user_id, int circle_id)
        {

            //Backend Logic: Register every LO in Circle with the new user
            //               Doesn't insert if duplicated
            CircleUser cuser = new CircleUser { User_id = user_id, Circle_id = circle_id, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };


           await _repositoryService.InsertAsync<CircleUser>(cuser);
        }

        public async Task RemoveUserFromCircle(int user_id, int circle_id)
        {
            try
            {
                var cus = await _repositoryService.SearchForAsync<CircleUser>(cu => cu.Circle_id == circle_id && cu.User_id == user_id , new Dictionary<string, string>(), false);
                var target = cus.FirstOrDefault();
                await _repositoryService.DeleteAsync<CircleUser>(target);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public async Task<List<Circle>> GetCircles(string startsWith)
        {
            List<Circle> list;
            if (string.IsNullOrEmpty(startsWith))
            {
                list = await _repositoryService.GetFirstNAsync<Circle>(20);
            }
            else
            {
                list =  await _repositoryService.SearchForAsync<Circle>(c => c.name.StartsWith(startsWith) && c.type==(int)CircleType.Public, c => c.updated_at, c => c.id, false);
            }

            return list;
        }

        public async Task<List<Circle>> GetCircles()
        {
            return await _repositoryService.GetAllAsync<Circle>();
        }


        public async  Task<List<Institution>> GetInstitutions()
        {
            return await _repositoryService.GetAllAsync<Institution>();
        }


      


        public async Task<List<publisher_by_institution>> GetPublishersByInstitution(int inst_id)
        {
#if (WEB)
            return await _repositoryService.SearchForAsync<publisher_by_institution>(p=>p.institution_id==inst_id,p=>p.updated_at,p=>p.id,false);

            #else
               return await _repositoryService.SearchForAsync<publisher_by_institution>(p=>p.institution_id==inst_id,p=>p.updated_at,p=>p.id,true);
#endif
        }


        async public Task UpdateObject<T>(T obj)
        {
            await _repositoryService.UpdateAsync<T>(obj);
            
        }

        async public Task DeleteObject<T>(T obj)
        {
            await _repositoryService.DeleteAsync<T>(obj);
        }


        async public Task<T> GetObjectWithId<T>(int id)
        {
            return await _repositoryService.GetByIdAsync<T>(id);
        }


        async public Task LikeOrDislikeLO(int lo_id, int user_id)
        {

            var list =  await _repositoryService.SearchForAsync<UserLO>(usr => usr.User_id == user_id && usr.LearningObject_id == lo_id,usr=>usr.updated_at,usr=>usr.id,false);



            if (list.Count > 0)
            {

                UserLO first = list.FirstOrDefault();

                first.like = !first.like;

                await _repositoryService.UpdateAsync<UserLO>(first);
            }
        }


        async public Task<List<lo_comment_with_username>> GetLOComments(int lo_id)
        {
#if (WEB)
            return await _repositoryService.SearchForAsync<lo_comment_with_username>(lo => lo.lo_id == lo_id, lo => lo.updated_at, lo => lo.id, false);
            #else
               return await _repositoryService.SearchForAsync<lo_comment_with_username>(lo => lo.lo_id == lo_id, lo => lo.updated_at, lo => lo.id, true);
#endif
        }


        async public Task LogoutUser(int user_id)
        {
            User user = await _repositoryService.GetByIdAsync<User>(user_id);

            user.is_online = 0;

            await _repositoryService.UpdateAsync<User>(user);
        }


        async public Task<List<tag_by_circle>> GetTagsByCircle(int circle_id)
        {

#if(WEB)
            return await _repositoryService.SearchForAsync<tag_by_circle>(t => t.Circle_id == circle_id,t=>t.updated_at,t=>t.id,false);
            #else
                return await _repositoryService.SearchForAsync<tag_by_circle>(t => t.Circle_id == circle_id,t=>t.updated_at,t=>t.id,true);
#endif
        }

        async public Task<List<tag_by_page>> GetTagsByLO(int lo_id)
        {
#if(WEB)
            return await _repositoryService.SearchForAsync<tag_by_page>(t => t.lo_id == lo_id, t => t.updated_at, t => t.id, false);
#else
                return await _repositoryService.SearchForAsync<tag_by_page>(t => t.lo_id == lo_id, t => t.updated_at, t => t.id, true);
#endif
        }

        async public Task<List<tag_by_lo>> GetLOTags(int lo_id)
        {
            return await _repositoryService.SearchForAsync<tag_by_lo>(t => t.lo_id == lo_id, t => t.updated_at, t => t.id, false);
        }

        async public Task<List<tag_by_page>> GetTagsByPage(int page_id)
        {

#if(WEB)
            return await _repositoryService.SearchForAsync<tag_by_page>(t => t.page_id==page_id, t => t.updated_at, t => t.id, false);
#else
                return await _repositoryService.SearchForAsync<tag_by_page>(t => t.page_id==page_id, t => t.updated_at, t => t.id, true);
#endif
        }

        async public Task<List<consumer_by_circle>> GetConsumersByCircle(int circle_id)
        {

            #if(WEB)
            return await _repositoryService.SearchForAsync<consumer_by_circle>(c => c.Circle_id == circle_id, c => c.updated_at, c => c.id, false);
            #else
              return await _repositoryService.SearchForAsync<consumer_by_circle>(c => c.Circle_id == circle_id, c => c.updated_at, c => c.id, true);
#endif
        }


        async public Task<List<quiz_by_circle>> GetQuizzesByCircle(int circle_id)
        {

            #if(WEB)
            return await _repositoryService.SearchForAsync<quiz_by_circle>(q => q.circle_id == circle_id, q => q.updated_at, q => q.id, false);
            #else
             return await _repositoryService.SearchForAsync<quiz_by_circle>(q => q.circle_id == circle_id, q => q.updated_at, q => q.id, true);
#endif
        }




        async public Task<int> CreateInstitution(Institution inst, Head head_info, User user_credentials)
        {
            //Create Head account

            OperationResult r = await CreateAccount<User>(user_credentials, u => u.id, UserType.Head);

            int user_id = r.id;

            var headList = await _repositoryService.SearchForAsync<Head>(h => h.User_id == user_id, h => h.updated_at, h => h.id, false);


            var old_head = headList.FirstOrDefault();

            old_head.title = head_info.title;

            await _repositoryService.UpdateAsync<Head>(old_head);

            //Create institution
            inst.updated_at = DateTime.UtcNow;
            inst.created_at = DateTime.UtcNow;

           int inst_id = await CreateObject<Institution>(inst, i => i.id);     
            //Relationship
           await RegisterUserToInstitution(r.id, inst_id);
           return user_id;
        }


      
       async public Task<int> GetHeadInstitutionID(int UserID)
        {
            var institutionsList = await _repositoryService.SearchForAsync<head_by_institution>(h => h.id == UserID,h=>h.updated_at,h=>h.id,false);

           //WArning: Assuming there's only one Instituton per Head User

            if (institutionsList.Count > 0)
            {
                return institutionsList.FirstOrDefault().institution_id;
            }
            else
            {
                throw new InvalidOperationException();
            }
       }
       async public Task<List<head_by_institution>> GetHeads()
       {
           return await _repositoryService.GetAllAsync<head_by_institution>();
       }
        async public Task<List<consumer_by_institution>> GetConsumersByInstitution(int inst_id)
        {

#if (WEB)
            return await _repositoryService.SearchForAsync<consumer_by_institution>(c => c.institution_id == inst_id, c => c.updated_at, c => c.id, false);
#else
               return await _repositoryService.SearchForAsync<consumer_by_institution>(c => c.institution_id == inst_id, c => c.updated_at, c => c.id, true);
#endif
        }

        async public Task<List<circle_by_owner>> GetCirclesByInstitution(int inst_id)
        {
            return await _repositoryService.SearchForAsync<circle_by_owner>(c => c.institution_id == inst_id,new Dictionary<string,string>(), false);
        }

        async public Task<OperationResult> CreateAndRegisterPublisher(User account, Publisher publisher, int institution_id)
        {
            OperationResult op = await CreateAccount<User>(account, u => u.id, UserType.Publisher);

            int user_id = op.id;
       //     Publisher old_publisher = await _repositoryService.GetByIdAsync<Publisher>(user_id);

           var publisherList =  await _repositoryService.SearchForAsync<Publisher>(p => p.User_id == user_id,p=>p.updated_at,p=>p.id,false);

           var old_publisher = publisherList.FirstOrDefault();

            old_publisher.city = publisher.city;
            old_publisher.country = publisher.country;
            old_publisher.region = publisher.region;
            old_publisher.telephone = publisher.telephone;
            old_publisher.title = publisher.title;

            await _repositoryService.UpdateAsync<Publisher>(old_publisher);

            if (institution_id != Constants.NoInstitution)
            {
               await RegisterUserToInstitution(user_id, institution_id);
            }
            

            return op;

        }

        async public Task<OperationResult> CreateAndRegisterConsumer(User account, int institution_id)
        {
            OperationResult op = await CreateAccount<User>(account, u => u.id, UserType.Consumer);
            int user_id = op.id;

            if (institution_id != Constants.NoInstitution)
            {
                Institution_has_User institution = await GetInstitutionByUser(user_id);
                if (institution == null)
                    await RegisterUserToInstitution(user_id, institution_id);
            }
            

            return op;
        }


        public async Task RegisterUserToInstitution(int user_id, int institution_id)
        {
            Institution_has_User institutionHead = new Institution_has_User { institution_id = institution_id, user_id = user_id, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };
            await CreateObject<Institution_has_User>(institutionHead, i => i.id);
        }


        public async Task UnSubscribeConsumerFromCircle(int user_id, int circle_id)
        {

            //Backend Logic: Unregister Lo's that were available for the user
           var relationList = await _repositoryService.SearchForAsync<CircleUser>(cu => cu.User_id == user_id && cu.Circle_id == circle_id, cu => cu.updated_at, cu => cu.id, false);

           if (relationList.Count > 0)
           {
               var toDelete = relationList.FirstOrDefault();
               await _repositoryService.DeleteAsync<CircleUser>(toDelete);
           }
        }

        public async Task<int> GetPublisherInstitutionID(int user_id)
        {
            var institutionsList = await _repositoryService.SearchForAsync<publisher_by_institution>(h => h.id == user_id, h => h.updated_at, h => h.id, false);

            //WArning: Assuming there's only one Instituton per Head User

            if (institutionsList.Count > 0)
            {
                return institutionsList.FirstOrDefault().institution_id;
            }
            else
            {
                throw new InvalidOperationException();
            }
        
        }

        public async Task<List<Page>> GetPagesByLO(int lo_id)
        {

#if (WEB)
            return await _repositoryService.SearchForAsync<Page>(p => p.lo_id == lo_id, p => p.updated_at, p => p.id, false);
#else
            return await _repositoryService.SearchForAsync<Page>(p => p.lo_id == lo_id, p => p.updated_at, p => p.id, true);
#endif
        }

		public async Task<List<LOsection>> GetSectionsByLO(int lo_id)
		{
			int b = 0;		 
			#if (WEB)
			return await _repositoryService.SearchForAsync<LOsection>(s => s.LO_id == lo_id, s => s.updated_at, s => s.id, false);
			#else
			return await _repositoryService.SearchForAsync<LOsection>(s => s.LO_id == lo_id, s => s.updated_at, s => s.id, true);
			#endif
 
		}






        public async Task<int> TryCreateUser(string socialID, int idInstitution)
        {
            //Backend logic: sets username and user image_url
            User myuser = new User { social_id = socialID };

            //await _repositoryService.InsertAsync<User>(myuser);

           //var result = await CreateAccount<User>(myuser, u => u.id, UserType.Consumer);
            var result = await CreateAndRegisterConsumer(myuser, idInstitution);
           return result.id;
        }


        public async Task<string> UploadResource(Stream s,string filename)
        {

            string toReturn = string.Empty;
            

            //    s.CopyTo(ms);
                s.Position = 0;

             
              toReturn = await AzureUploader.uploadFile(s, filename);
             

            

            return toReturn;
        
        }


        public async Task<List<Tag>> GetAllTags()
        {
            return await _repositoryService.GetAllAsync<Tag>();
        }


        
        public async Task AddTagToPage(int tag_id, int page_id)
        {
            await _repositoryService.InsertAsync<PageTag>(new PageTag { page_id = page_id, tag_id = tag_id, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
        }

        public async Task AddTagToLO(int tag_id, int lo_id)
        {
            await _repositoryService.InsertAsync<LearningObjectTag>(new LearningObjectTag { lo_id = lo_id, tag_id = tag_id, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
        }

        public async Task DeleteTagFromPage(int tag_id, int page_id)
        {

            var list = await _repositoryService.SearchForAsync<PageTag>(pt => pt.tag_id == tag_id && pt.page_id == page_id,pt=>pt.updated_at,pt=>pt.id,false);

           await _repositoryService.DeleteAsync<PageTag>(list.FirstOrDefault());

        }

        public async Task DeleteTagFromLO(int tag_id, int lo_id)
        {

            var list = await _repositoryService.SearchForAsync<LearningObjectTag>(lot => lot.tag_id == tag_id && lot.lo_id == lo_id, pt => pt.updated_at, pt => pt.id, false);

            await _repositoryService.DeleteAsync<LearningObjectTag>(list.FirstOrDefault());

        }

        public async Task<List<Question>> GetQuestionsByQuiz(int quiz_id)
        {
            return await _repositoryService.SearchForAsync<Question>(q => q.Quiz_id == quiz_id, q => q.updated_at, q => q.id, false);
        }

        public async Task<List<QuestionOption>> GetOptionsByQuestion(int question_id)
        {
            return await _repositoryService.SearchForAsync<QuestionOption>(q => q.Question_id == question_id, q => q.updated_at, q => q.id, false);

        }


        public async Task SaveUserAnswer(int user_id, int question_id, string ans)
        {
            //If answer for question already exists logic in azure
            await _repositoryService.InsertAsync<UserQuestion>(new UserQuestion { User_id = user_id, Question_id = question_id, answer = ans, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow});
        }

        public async Task<List<Quiz>> GetQuizzesByLO(int lo_id)
        {
            return await _repositoryService.SearchForAsync<Quiz>(q => q.LearningObject_id==lo_id, q => q.updated_at, q => q.id, false);
        }

      
        public async Task<List<Page>> GetPagesByLOSection(int sec_id)
        {
			#if (WEB)
			return await _repositoryService.SearchForAsync<Page>(s => s.LOsection_id == sec_id, s => s.updated_at, s => s.id, false);
			#else
			return await _repositoryService.SearchForAsync<Page>(s => s.LOsection_id == sec_id, s => s.updated_at, s => s.id, true);
			#endif
        }

		public async Task<Page> GetFirstSlidePageByLOSection(int sec_id)
		{
			#if (WEB)
			return await _repositoryService.SearchForFirstAsync<Page>(s => s.LOsection_id == sec_id, s => s.updated_at, s => s.id, false);
			#else
			return await _repositoryService.SearchForFirstAsync<Page>(s => s.LOsection_id == sec_id, s => s.updated_at, s => s.id, false);
			#endif
		}

        public async Task<Institution_has_User> GetInstitutionByUser(int user_id)
        {
            var institution = await _repositoryService.SearchForAsync<Institution_has_User>(c => c.user_id == user_id, new Dictionary<string, string>(), false);
            return institution.FirstOrDefault();
        }
        public async Task<MobileServiceUser> LoginProvider(int provider, JObject access_token)
        {
            return await _repositoryService.LoginProvider(provider, access_token);
        }
        public void Logout()
        {
            _repositoryService.Logout();
        }
        public async Task<Circle_has_LO> GetCircleLOByIdLO(int id_lo)
        {
            var circles  = await _repositoryService.SearchForAsync<Circle_has_LO>(c => c.lo_id == id_lo, new Dictionary<string, string>(), false);
            return circles.FirstOrDefault();
        }
        public async Task<CircleUser> GetCircleUser(int idUser, int idCircle)
        {
            var circleUser = await _repositoryService.SearchForAsync<CircleUser>(c => c.User_id == idUser && c.Circle_id == idCircle, new Dictionary<string, string>(), false);
            return circleUser.FirstOrDefault();
        }
    }
}
