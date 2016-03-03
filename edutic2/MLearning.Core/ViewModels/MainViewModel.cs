using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Core.Classes;
using Core.DownloadCache;
using Core.Push;
using Core.Repositories;
using Core.Session;
using Core.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using MLearning.Core.Configuration;
using MLearning.Core.Entities;
using MLearning.Core.Services;
using MLearningDB;
using Newtonsoft.Json;
using Referee.Core.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace MLearning.Core.ViewModels
{
	 
	public partial class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged {
		public ObservableDictionary( ) : base( ) { }
		public ObservableDictionary(int capacity) : base(capacity) { }
		public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
		public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
		public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
		public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		public new TValue this[TKey key] {
			get {
				return base[key];
			}
			set {
				TValue oldValue;
				bool exist = base.TryGetValue(key, out oldValue);
				var oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
				base[key] = value;
				var newItem = new KeyValuePair<TKey, TValue>(key, value);
				if (exist) {
					this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, base.Keys.ToList( ).IndexOf(key)));
				} else {
					this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, base.Keys.ToList( ).IndexOf(key)));
					this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				}
			}
		}

		public new void Add(TKey key, TValue value) {
			if (!base.ContainsKey(key)) {
				var item = new KeyValuePair<TKey, TValue>(key, value);
				base.Add(key, value);
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, base.Keys.ToList( ).IndexOf(key)));
				this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			}
		}

		public new bool Remove(TKey key) {
			TValue value;
			if (base.TryGetValue(key, out value)) {
				var item = new KeyValuePair<TKey, TValue>(key, base[key]);
				bool result = base.Remove(key);
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, base.Keys.ToList( ).IndexOf(key)));
				this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				return result;
			}
			return false;
		}

		public new void Clear( ) {
			base.Clear( );
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (this.CollectionChanged != null) {
				this.CollectionChanged(this, e);
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this, e);
			}
		}
	}

    public class MainViewModel :MvxViewModel
    {

        #region Constructor

        private IncrementalDownload _locover_manager;
        private IncrementalDownload _user_image_manager;
        private IncrementalDownload _post_image_manager;
        private IncrementalDownload _comment_image_manager;
        private IMLearningService _mLearningService;

        public MainViewModel(IMLearningService mlearningService)
        {
            _mLearningService = mlearningService;
            _locover_manager = new IncrementalDownload();
            _user_image_manager = new IncrementalDownload();
            _post_image_manager= new IncrementalDownload();
            _comment_image_manager = new IncrementalDownload(); 

            //async call
            LoadUserInfo();

            UserID = SessionService.GetUserId();

            CircleID = Constants.NoSelection;
            CurrentLOIDSelected = Constants.NoSelection;
            LoadCircles();


            NotificationHandler.Properties.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Global_PropertyChanged);

            PropertyChanged += MainViewModel_PropertyChanged;
        }

        void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "_TmpQuizzesList" && (sender as MainViewModel)._TmpQuizzesList!=null)
            {
                FilterQuizzes(CurrentLOIDSelected);
            }
        }

        private void Global_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RefreshPosts" && (sender as GlobalInfo).RefreshPosts)
            {
                LoadPostsInCircle(CircleID);
            }
            else if (e.PropertyName == "RefreshUsers" && (sender as GlobalInfo).RefreshUsers)
            {
                LoadUsersInCircle(CircleID);
            }
            else if (e.PropertyName == "RefreshCircles" && (sender as GlobalInfo).RefreshCircles)
            {
                LoadCircles();
            }
            else if (e.PropertyName == "RefreshLO" && (sender as GlobalInfo).RefreshLO)
            {
                LoadLearningObjects(CircleID);
            }
            else if (e.PropertyName == "RefreshQuizzes" && (sender as GlobalInfo).RefreshQuizzes)
            {
                LoadQuizzes(CircleID);
            }
            else if (e.PropertyName == "RefreshLOComments" && (sender as GlobalInfo).RefreshLOComments)
            {
                LoadLOComments(CurrentLOIDSelected);
            }
          
        }


        #endregion


        #region Wrappers
        //----------------------------------------------------------------Wrapper Classes

        public class lo_by_circle_wrapper : MvxNotifyPropertyChanged
        { 
            public lo_by_circle lo {get;set;}

            byte[] _cover_bytes;
            public byte[] cover_bytes
            {
                get { return _cover_bytes; }
                set { _cover_bytes = value; RaisePropertyChanged("cover_bytes"); }
            }


            byte[] _background_bytes;
            public byte[] background_bytes
            {
                get { return _background_bytes; }
                set { _background_bytes = value; RaisePropertyChanged("background_bytes"); }
            }




        }

        public class user_by_circle_wrapper : MvxNotifyPropertyChanged
        {
            public user_by_circle user { get; set; }


            byte[] _userImage;
            public byte[] userImage
            {
                get { return _userImage; }
                set { _userImage = value; RaisePropertyChanged("userImage"); }
            }


        }

        public class post_with_username_wrapper : MvxNotifyPropertyChanged
        {
            public post_with_username post { get; set; }


            byte[] _userImage;
            public byte[] userImage
            {
                get { return _userImage; }
                set { _userImage = value; RaisePropertyChanged("userImage"); }
            }


        }

        public class lo_comment_with_username_wrapper : MvxNotifyPropertyChanged
        {

            public lo_comment_with_username lo_comment { get; set; }

            byte[] _userImage;
            public byte[] userImage
            {
                get { return _userImage; }
                set { _userImage = value; RaisePropertyChanged("userImage"); }
            }
        
        }


        
        #endregion

     
        #region Properties

        // ---------------------------------------------------------------Properties---------------------------------------------------------------
        ObservableCollection<user_by_circle_wrapper> _tmpUsersList;

        ObservableCollection<user_by_circle_wrapper> _usersList;
        public ObservableCollection<user_by_circle_wrapper> UsersList
        {
            get { return _usersList; }
            set { _usersList = value; RaisePropertyChanged("UsersList"); }
        }

		ObservableDictionary<int, IList<Page> > _contentByUnit;
		public ObservableDictionary<int, IList<Page> > ContentByUnit
		{
			get { return _contentByUnit; }
			set { _contentByUnit = value; RaisePropertyChanged("ContentByUnit"); }
		}



        ObservableCollection<circle_by_user> _tmpCirclesList;

        ObservableCollection<circle_by_user> _circlesList;
        public ObservableCollection<circle_by_user> CirclesList
        {
            get { return _circlesList; }
            set { _circlesList = value; RaisePropertyChanged("CirclesList"); }
        }



        ObservableCollection<post_with_username_wrapper> _postsList;
        public ObservableCollection<post_with_username_wrapper> PostsList
        {
            get { return _postsList; }
            set { _postsList = value; RaisePropertyChanged("PostsList"); }
        }




        ObservableCollection<lo_by_circle_wrapper> _learningObjectsList;
        public ObservableCollection<lo_by_circle_wrapper> LearningOjectsList
        {
            get { return _learningObjectsList; }
            set { _learningObjectsList = value; RaisePropertyChanged("LearningOjectsList"); }

            
        }



        ObservableCollection<ObservableCollection<lo_by_circle_wrapper>> _circlesLearningObject;
        public ObservableCollection<ObservableCollection<lo_by_circle_wrapper>> CirclesLearningObjects
        {
            get { return _circlesLearningObject; }
            set { _circlesLearningObject = value; RaisePropertyChanged("CirclesLearningObjects"); }
        }




        ObservableCollection<lo_comment_with_username_wrapper> _loCommentsList;
        public ObservableCollection<lo_comment_with_username_wrapper> LOCommentsList
        {
            get { return _loCommentsList; }
            set { _loCommentsList = value; RaisePropertyChanged("LOCommentsList"); }
        }


        ObservableCollection<tag_by_circle> _circleTags;
        public ObservableCollection<tag_by_circle> CircleTags
        {
            get { return _circleTags; }
            set { _circleTags = value; RaisePropertyChanged("CircleTags"); }
        }


       // ObservableCollection<quiz_by_circle> _tmpQuizzesList;


        ObservableCollection<quiz_by_circle> _tmpQuizzesList;
        public ObservableCollection<quiz_by_circle> _TmpQuizzesList

        {
            get { return _tmpQuizzesList; }
            set { _tmpQuizzesList = value; RaisePropertyChanged("_TmpQuizzesList"); }
        }



        ObservableCollection<quiz_by_circle> _quizzesList;
        public ObservableCollection<quiz_by_circle> QuizzesList
        {
            get { return _quizzesList; }
            set { _quizzesList = value; RaisePropertyChanged("QuizzesList"); UpdatePendingAndDone(_quizzesList); }
        }



        ObservableCollection<quiz_by_circle> _pendingQuizzesList;
        public ObservableCollection<quiz_by_circle> PendingQuizzesList
        {
            get { return _pendingQuizzesList; }
            set { _pendingQuizzesList = value; RaisePropertyChanged("PendingQuizzesList"); }
        }




        ObservableCollection<quiz_by_circle> _completedQuizzesList;
        public ObservableCollection<quiz_by_circle> CompletedQuizzesList
        {
            get { return _completedQuizzesList; }
            set { _completedQuizzesList = value; RaisePropertyChanged("CompletedQuizzesList"); }
        }


        

   

        //Note: When NULL the DBM sets integer fields to 0
        private void UpdatePendingAndDone(ObservableCollection<quiz_by_circle> _quizzesList)
        {
            //Comparison to 0, means that User_id field is null in the DB
            var pending = _quizzesList.Where(q => q.User_id == 0).ToList();
            var completed = _quizzesList.Where(q => q.User_id == UserID).ToList();

            PendingQuizzesList = new ObservableCollection<quiz_by_circle>(pending);
            CompletedQuizzesList = new ObservableCollection<quiz_by_circle>(completed);
        }





        string _usersFilterString;
        public string UsersFilterString
        {
            get { return _usersFilterString; }
            set { _usersFilterString = value; RaisePropertyChanged("UsersFilterString"); FilterUsersList(_usersFilterString); }
        }

        private void FilterUsersList(string usersFilterString)
        {
            if (usersFilterString == string.Empty)
            {
                UsersList = _tmpUsersList;
            }
            else
            {
                usersFilterString = usersFilterString.ToLower();
                var list = _tmpUsersList.Select(u => u).Where(u => u.user.username.StartsWith(usersFilterString)).ToList();
                UsersList = new ObservableCollection<user_by_circle_wrapper>(list);
                    
            }
        }



        string _circleFilterString;
        public string CircleFilterString
        {
            get { return _circleFilterString; }
            set { _circleFilterString = value; RaisePropertyChanged("CircleFilterString"); FilterCircleList(_circleFilterString); }
        }

        private void FilterCircleList(string circleFilterString)
        {
            if (circleFilterString == string.Empty)
            {
                CirclesList = _tmpCirclesList;
            }
            else
            {
                circleFilterString = circleFilterString.ToLower();

                var list = _tmpCirclesList.Select(c => c).Where(c => c.name.ToLower().StartsWith(circleFilterString)).ToList();
                var tagsList = new List<circle_by_user>();
                foreach (var tag in CircleTags)
                {
                    if (tag.name.StartsWith(circleFilterString))
                    {
                        var circleMatch = _tmpCirclesList.Where(c => c.id == tag.Circle_id).ToList().FirstOrDefault();
                        tagsList.Add(circleMatch);
                    }
                }

                var unionlist = list.Union(tagsList);

                CirclesList = new ObservableCollection<circle_by_user>(unionlist);
            }
        }








        string _userFirstName;
        public string UserFirstName
        {
            get { return _userFirstName; }
            set { _userFirstName = value; RaisePropertyChanged("UserFirstName"); }
        }



        string _userLastName;
        public string UserLastName
        {
            get { return _userLastName; }
            set { _userLastName = value; RaisePropertyChanged("UserLastName"); }
        }



        byte[] _userImage;
        public byte[] UserImage
        {
            get { return _userImage; }
            set { _userImage = value; RaisePropertyChanged("UserImage"); }
        }


        //background of the current lo
        private byte[] _backgroundimage;

        public byte[] BackgroundImage
        {
            get { return _backgroundimage; }
            set { _backgroundimage = value; RaisePropertyChanged("BackgroundImage"); }
        }
        




        

        


        string _newPost;
        public string NewPost
        {
            get { return _newPost; }
            set { _newPost = value; RaisePropertyChanged("NewPost"); }
        }


        string _newLOComment;
        public string NewLOComment
        {
            get { return _newLOComment; }
            set { _newLOComment = value; RaisePropertyChanged("NewLOComment"); }
        }





        int _userID;
        public int UserID
        {
            get { return _userID; }
            set { _userID = value; RaisePropertyChanged("UserID"); }
        }


        int _circleID;
        public int CircleID
        {
            get { return _circleID; }
            set { _circleID = value; RaisePropertyChanged("CircleID"); }
        }





        bool _connectionOK;
        public bool ConnectionOK
        {
            get { return _connectionOK; }
            set { _connectionOK = value; RaisePropertyChanged("ConnectionOK"); }
        }




        bool _operationOK;
        public bool OperationOK
        {
            get { return _operationOK; }
            set { _operationOK = value; RaisePropertyChanged("OperationOK"); }
        }

        int _currentIndexDisplaying;
        public int CurrentIndexDisplaying
        {
            get { return _currentIndexDisplaying; }
            set { _currentIndexDisplaying = value; RaisePropertyChanged("CurrentIndexDisplaying");  }
        }


        int _currentLOIDSelected;
        public int CurrentLOIDSelected
        {
            get { return _currentLOIDSelected; }
            set { _currentLOIDSelected = value; RaisePropertyChanged("CurrentLOIDSelected"); }
        }




        bool _loadCirclePosts;
        public bool LoadCirclePosts
        {
            get { return _loadCirclePosts; }
            set { _loadCirclePosts = value; RaisePropertyChanged("LoadCirclePosts"); }
        }

        
        bool _isLoggingOut;
        public bool IsLoggingOut {
	        get { return _isLoggingOut; }
	        set { _isLoggingOut = value; RaisePropertyChanged ("IsLoggingOut"); }
        }

        #endregion



        #region Commands
        // ---------------------------------------------------------------Commands---------------------------------------------------------------

        MvxCommand<circle_by_user> _selectCircleCommand;
        public System.Windows.Input.ICommand SelectCircleCommand
        {
            get
            {
                _selectCircleCommand = _selectCircleCommand ?? new MvxCommand<circle_by_user>(async (circle)=>await DoSelectCircleCommand(circle));
                return _selectCircleCommand;
            }
        }

        async Task DoSelectCircleCommand(circle_by_user circle)
        {


            CircleID = circle.id;

            try
            {
                var watch = Stopwatch.StartNew();
                await LoadCircleTags(CircleID);
                await LoadPostsInCircle(CircleID);
                await LoadUsersInCircle(CircleID);
                await LoadLearningObjects(CircleID);
                await LoadQuizzes(CircleID);

                watch.Stop();

                Mvx.Trace("Time elapsed: " + watch.ElapsedMilliseconds);
            }
            catch (WebException e)
            {
              
                ConnectionOK = false;
            }
            catch (HttpRequestException e)
            {
                
                ConnectionOK = false;
            }
            catch (MobileServiceInvalidOperationException e)
            {
                Mvx.Trace("MobileServiceInvalidOperationException " + e.Message);
                OperationOK = false;
            }
        }

       


       
        MvxCommand<lo_by_circle_wrapper> _selectLOCommand;
        public System.Windows.Input.ICommand SelectLOCommand
        {
            get
            {
                _selectLOCommand = _selectLOCommand ?? new MvxCommand<lo_by_circle_wrapper>(DoSelectLOCommand);
                return _selectLOCommand;
            }
        }

        async void DoSelectLOCommand(lo_by_circle_wrapper learningobj)
        {
            try
            {
                CurrentLOIDSelected = learningobj.lo.id;
                LoadLOComments(CurrentLOIDSelected);


                FilterQuizzes(CurrentLOIDSelected);

               // 
                //reset the value of background image
                BackgroundImage = learningobj.background_bytes;
            }
            catch (WebException e)
            {
                
                ConnectionOK = false;
            }
            catch (HttpRequestException e)
            {
                
                ConnectionOK = false;
            }
            catch (MobileServiceInvalidOperationException e)
            {
                Mvx.Trace("MobileServiceInvalidOperationException " + e.Message);
                OperationOK = false;
            }
        }


        //Open - Download LO
        MvxCommand<lo_by_circle_wrapper> _openLOCommand;
        public System.Windows.Input.ICommand OpenLOCommand
        {
            get
            {
                _openLOCommand = _openLOCommand ?? new MvxCommand<lo_by_circle_wrapper>(DoOpenLOCommand);
                return _openLOCommand;
            }
        }

      

		//Open - Download LO
		MvxCommand<lo_by_circle_wrapper> _DoOpenFirstSlidePageCommand;
		public System.Windows.Input.ICommand OpenFirstSlidePageCommand
		{
			get
			{
				_DoOpenFirstSlidePageCommand = _DoOpenFirstSlidePageCommand ?? new MvxCommand<lo_by_circle_wrapper>(DoOpenFirstSlidePage);
				return _DoOpenFirstSlidePageCommand;
			}
		}


        async void DoOpenLOCommand(lo_by_circle_wrapper learningobj)
        {
            //await  _mLearningService.OpenLearningObject(learningobj.lo.id,learningobj.lo.url_package,UserID);

            try
            {
                List<lo_by_circle> list = LearningOjectsList.Select(lo => lo.lo).ToList();



                //await BlockDownload.TryLoadByteVector<MLearning.Core.ViewModels.MainViewModel.lo_by_circle_wrapper>(LearningOjectsList.ToList(),
                //    (pos, bytes) => { },
                //    (lo) => { return lo.lo.url_background; }
                //    );


                //Download all the data of the selected LO

                foreach (var item in LearningOjectsList)
                {
                    if (item.lo.id == learningobj.lo.id)
                    {
                        await FetchLOData(item.lo.id, true);
                    }
                    else
                    {
                        await FetchLOData(item.lo.id, false);
                    }



                }


                string serialized = JsonConvert.SerializeObject(list);
                ShowViewModel<LOViewModel>(new { lo_id = learningobj.lo.id, serialized_los_in_circle = serialized });
            }
            catch (WebException e)
            {
               
                ConnectionOK = false;
            }
            catch (HttpRequestException e)
            {

                ConnectionOK = false;
            }
            catch (MobileServiceInvalidOperationException e)
            {
                Mvx.Trace("MobileServiceInvalidOperationException " + e.Message);
                OperationOK = false;
            }
            
           


           
        }
 

		async void DoOpenFirstSlidePage(lo_by_circle_wrapper learningobj)
		{
			//await  _mLearningService.OpenLearningObject(learningobj.lo.id,learningobj.lo.url_package,UserID);

			try
			{
 

				//Download all the data of the selected LO

				foreach (var item in LearningOjectsList)
				{
					if (item.lo.id == learningobj.lo.id)
					{
						var list = await _mLearningService.GetPagesByLO(learningobj.lo.id);

						//Page result = await this._mLearningService.GetFirstSlidePageByLOSection(item.lo.id); 
						if (list != null) {
							Debug.WriteLine("indexList", "result...: " + list.Count);


							var indexTmp = new Dictionary<int, IList<Page> > ();
							foreach(var page in list) {
								IList<Page> resultTmp;
								if ( ! indexTmp.TryGetValue((int)page.LOsection_id, out resultTmp ) ) {
									var ls= new List<Page>();
									ls.Add(page);
		
									indexTmp [ (int)page.LOsection_id ] =  ls ;
								}
								else  {
									resultTmp.Add(page);
								}
							}
							 
							ContentByUnit = new ObservableDictionary<int, IList<Page> >( indexTmp );


						}
					}
				} 
			}
			catch (WebException e)
			{

				ConnectionOK = false;
			}
			catch (HttpRequestException e)
			{

				ConnectionOK = false;
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Mvx.Trace("MobileServiceInvalidOperationException " + e.Message);
				OperationOK = false;
			}





		}


	/*	List<LOsection> _losection;
		public List<LOsection> LOsection
		{
			get { return _losection; }
			set { _losection = value; RaisePropertyChanged("LOsection"); }
		}
*/
		ObservableCollection<LOsection> _losectionList;
		public ObservableCollection<LOsection> LOsectionList
		{
			get { return _losectionList; }
			set { _losectionList = value; RaisePropertyChanged("LOsectionList"); }


		}




		MvxCommand<lo_by_circle_wrapper> _openLOSectionListCommand;
		public System.Windows.Input.ICommand OpenLOSectionListCommand
		{
			get
			{
				_openLOSectionListCommand = _openLOSectionListCommand ?? new MvxCommand<lo_by_circle_wrapper>(DoOpenLOSectionListCommandCommand);
				return _openLOSectionListCommand;
			}
		} 

		async void DoOpenLOSectionListCommandCommand(lo_by_circle_wrapper learningobj)
		{
			var listas = await _mLearningService.GetSectionsByLO (learningobj.lo.id);
			 
			if (listas != null) {
				//NewsCollection=new ObservableCollection<string>(await service.LoadNews ());  
			
				this.LOsectionList = new ObservableCollection<LOsection>(  listas );
				 
			}

		}
			

        //Download pages and tags info
        async Task FetchLOData(int lo_id,bool images)
        {
            int LOID = lo_id;

            var list = await _mLearningService.GetPagesByLO(LOID);

            

            if (images)
            {
                IncrementalDownload _manager = new IncrementalDownload();
                await _manager.TryLoadByteVector<Page>(0, list
                     , (pos, bytes) =>
                     {
                     }
                     , (page) =>
                     {
                         return page.url_img;
                     });

            }
            


            var tags = await _mLearningService.GetTagsByLO(LOID);


            //Download xmls

            //foreach (var item in list)
            //{

            //    await _mLearningService.OpenLOPage(item.url_xml);
            //}
        }


        //Create a post about a Circle
        MvxCommand _postCommand;
        public System.Windows.Input.ICommand PostCommand
        {
            get
            {
                _postCommand = _postCommand ?? new MvxCommand(DoPostCommand);
                return _postCommand;
            }
        }

        async void DoPostCommand()
        {

            if(CircleID!=Constants.NoSelection)
            {

                try
                {
                    Post toAdd = new Post { text = NewPost, user_id = UserID, circle_id = CircleID, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };
                    await _mLearningService.CreateObject<Post>(toAdd, p => p.id);
                    NewPost = "";

                    post_with_username puser = new post_with_username(toAdd);
                    puser.username = SessionService.GetUsername();

                    PostsList.Add(new post_with_username_wrapper { post = puser, userImage = UserImage });
                }
                catch (WebException e)
                {
                    ConnectionOK = false;
                }

                
            }
            else
            {
                //Notify Invalid Operation

                OperationOK = false;
            }
            
        }


        //Testing purposes
        MvxCommand _navigateToRegistrationCommand;
        public System.Windows.Input.ICommand NavigateToRegistrationCommand
        {
            get
            {
                _navigateToRegistrationCommand = _navigateToRegistrationCommand ?? new MvxCommand(DoNavigateToRegistrationCommand);
                return _navigateToRegistrationCommand;
            }
        }

        void DoNavigateToRegistrationCommand()
        {
            ShowViewModel<RegistrationViewModel>();
        }


        MvxCommand<lo_by_circle_wrapper> _likeLOCommand;
        public System.Windows.Input.ICommand LikeLOCommand
        {
            get
            {
                _likeLOCommand = _likeLOCommand ?? new MvxCommand<lo_by_circle_wrapper>(DoLikeLOCommand);
                return _likeLOCommand;
            }
        }

        void DoLikeLOCommand(lo_by_circle_wrapper learningobj)
        {

            _mLearningService.LikeOrDislikeLO(learningobj.lo.id,UserID);

        }

         

        MvxCommand _createLOCommentCommand;
        public System.Windows.Input.ICommand CreateLOCommentCommand
        {
            get
            {
                _createLOCommentCommand = _createLOCommentCommand ?? new MvxCommand(DoCreateLOCommentCommand);
                return _createLOCommentCommand;
            }
        }

        void DoCreateLOCommentCommand()
        {
            var comment = new LOComment {lo_id= CurrentLOIDSelected,user_id=UserID,text=NewLOComment,created_at=DateTime.UtcNow,updated_at=DateTime.UtcNow };
            var comment_with_username = new lo_comment_with_username { text = NewLOComment, username = SessionService.GetUsername(),created_at = DateTime.UtcNow,updated_at = DateTime.Now,name = UserFirstName+" "+UserLastName };
            LOCommentsList.Add(new lo_comment_with_username_wrapper { lo_comment = comment_with_username, userImage = UserImage });

            _mLearningService.CreateObject<LOComment>(comment, com => com.id);

            NewLOComment = "";

            ForceTableUpdate();
        }



        MvxCommand _backToCirclePostsCommand;
        public System.Windows.Input.ICommand BackToCirclePostsCommand
        {
            get
            {
                _backToCirclePostsCommand = _backToCirclePostsCommand ?? new MvxCommand(DoBackToCirclePostsCommand);
                return _backToCirclePostsCommand;
            }
        }

        void DoBackToCirclePostsCommand()
        {
            QuizzesList = _TmpQuizzesList;
            LoadCirclePosts = true;
        }



        MvxCommand _refreshCommentsCommand;
        public System.Windows.Input.ICommand RefreshCommentsCommand
        {
            get
            {
                _refreshCommentsCommand = _refreshCommentsCommand ?? new MvxCommand(DoRefreshCommentsCommand);
                return _refreshCommentsCommand;
            }
        }

        void DoRefreshCommentsCommand()
        {
            LoadPostsInCircle(CircleID);

            if (CurrentLOIDSelected != Constants.NoSelection)
                LoadLOComments(CurrentLOIDSelected);
        }


        MvxCommand _logoutCommand;
        public System.Windows.Input.ICommand LogoutCommand
        {
            get
            {
                _logoutCommand = _logoutCommand ?? new MvxCommand(DoLogoutCommand);
                return _logoutCommand;
            }
        }

        async void DoLogoutCommand()
        {
            SessionService.LogOut();

            IsLoggingOut = true;

            await _mLearningService.LogoutUser(UserID);

            ShowViewModel<LoginViewModel>();
            
        }


        #endregion


        #region Helper Functions
        async void LoadCircles()
        {
            var list = await _mLearningService.GetCirclesByUser(UserID);

            CirclesList = new ObservableCollection<circle_by_user>(list);


            CirclesLearningObjects  = new ObservableCollection<ObservableCollection<lo_by_circle_wrapper>>();
            foreach (var item in CirclesList)
            {
                CirclesLearningObjects.Add(new ObservableCollection<lo_by_circle_wrapper>());
            }

            _tmpCirclesList = CirclesList;

            //Default: load first circle

            var firstCircle = CirclesList.FirstOrDefault();

            if (firstCircle != null)
                await DoSelectCircleCommand(firstCircle);
        
        }

        async Task LoadCircleTags(int circle_id)
        {
            List<tag_by_circle> list = await _mLearningService.GetTagsByCircle(circle_id);

            CircleTags = new ObservableCollection<tag_by_circle>(list);
        }


        async private void LoadUserInfo()
        {
            SharedPreferences prefs = Constants.GetSharedPreferences(Constants.PreferencesFileName);

            UserFirstName = prefs.GetString(Constants.UserFirstNameKey);
            UserLastName = prefs.GetString(Constants.UserLastNameKey);
            string image_url = prefs.GetString(Constants.UserImageUrlKey);

            CacheService cache = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);

            if (!string.IsNullOrEmpty(image_url))
            {
                var tuple = await cache.tryGetResource(image_url);
                UserImage = tuple.Item1;
            }
            
            
        }


        async Task LoadUsersInCircle(int circle_id)
        {
            var list = await _mLearningService.GetUsersInCircle(circle_id);
            var userList = new List<user_by_circle_wrapper>();
            

            foreach (var item in list)
            {
                userList.Add(new user_by_circle_wrapper { user = item });

            }

            UsersList = new ObservableCollection<user_by_circle_wrapper>(userList);
            _tmpUsersList = UsersList;

            _user_image_manager.Clear();
           await   UpdateUserImages(0); 
        }

        async Task LoadPostsInCircle(int circle_id)
        {

            var list = await _mLearningService.GetPostByCircle(circle_id);

            PostsList = new ObservableCollection<post_with_username_wrapper>();

            foreach (var item in list)
            {
                PostsList.Add(new post_with_username_wrapper { post = item });
            }

            _post_image_manager.Clear();
           await  UpdateUserPostImages(0); 
        }


        
     

        async Task LoadLearningObjects(int circle_id)
        {

           

            if (LearningOjectsList != null)
                LearningOjectsList.Clear();

            var list = await _mLearningService.GetLOByCircleAndUser(circle_id,UserID);

            int circleIndex = CirclesList.IndexOf(CirclesList.Where(c => c.id == circle_id).First());

            var circleLOList = new List<lo_by_circle_wrapper>();
           

           

           
            foreach (var item in list)
            {
                circleLOList.Add(new lo_by_circle_wrapper { lo = item });
            }

            CirclesLearningObjects[circleIndex] = new ObservableCollection<lo_by_circle_wrapper>(circleLOList);

             LearningOjectsList = CirclesLearningObjects[circleIndex] ;


             await BlockDownload.TryPutBytesInVector<MLearning.Core.ViewModels.MainViewModel.lo_by_circle_wrapper>(LearningOjectsList.ToList(),
                   (pos, bytes) => { if (pos < LearningOjectsList.Count) LearningOjectsList[pos].background_bytes = bytes; },
                   (lo) => { return lo.lo.url_background; }
                  );


            _locover_manager.Clear();

            CurrentIndexDisplaying = 0;

            await UpdateLOImages(CurrentIndexDisplaying, CirclesLearningObjects[CirclesList.IndexOf(CirclesList.Where(c => c.id == CircleID).First())]);
           
        }


        async void LoadLOComments(int lo_id)
        {

            var list = await _mLearningService.GetLOComments(lo_id);

            LOCommentsList = new ObservableCollection<lo_comment_with_username_wrapper>();

            foreach (var item in list)
            {
                LOCommentsList.Add(new lo_comment_with_username_wrapper { lo_comment = item });
            }

            //LoadImages

            _comment_image_manager.Clear();

            await UpdateLOCommentImages(0);

            
        }

        private void FilterQuizzes(int lo_id)
        {
            if (lo_id != Constants.NoSelection && _TmpQuizzesList != null)
            {
                var list = _TmpQuizzesList.Where(q => q.LearningObject_id == lo_id).ToList();

                QuizzesList = new ObservableCollection<quiz_by_circle>(list);
            }
        }


        async Task LoadQuizzes(int circle_id)
        {
            var list = await _mLearningService.GetQuizzesByCircle(circle_id);

            QuizzesList = new ObservableCollection<quiz_by_circle>(list);

            _TmpQuizzesList = QuizzesList;
        }

        #endregion


        async Task UpdateLOImages(int index,ObservableCollection<lo_by_circle_wrapper> list)
        {

            //_locover_manager.TryLoadByteVector<lo_by_circle_wrapper>(index, list.ToList()
            //    , (pos, bytes) =>
            //    {
            //        list[pos].cover_bytes = bytes;
            //    }
            //    , (lo) =>
            //    {
            //        return lo.lo.url_cover;
            //    });

           await BlockDownload.TryPutBytesInVector<lo_by_circle_wrapper>(list.ToList(), (pos, bytes) =>
              {
                  if(pos<list.Count)
                  list[pos].cover_bytes = bytes;
              },
               (lo) => { return lo.lo.url_cover; });

            

           
        }

        async Task UpdateUserImages(int index)
        {

           //await _user_image_manager.TryLoadByteVector<user_by_circle_wrapper>(index, UsersList.ToList()
           //     , (pos, bytes) =>
           //     {
           //         UsersList[pos].userImage= bytes;
           //     }
           //     , (usr) =>
           //     {
           //         return usr.user.image_url;
           //     });

            await BlockDownload.TryPutBytesInVector<user_by_circle_wrapper>( UsersList.ToList()
                , (pos, bytes) =>
                {
                    if(pos<UsersList.Count)
                        UsersList[pos].userImage = bytes;
                }
                , (usr) =>
                {
                    return usr.user.image_url;
                });


        }

        async Task UpdateUserPostImages(int index)
        {

            //_post_image_manager.TryLoadByteVector<post_with_username_wrapper>(index, PostsList.ToList()
            //    , (pos, bytes) =>
            //    {
            //        PostsList[pos].userImage = bytes;
            //    }
            //    , (post) =>
            //    {
            //        return post.post.image_url;
            //    });

           await BlockDownload.TryPutBytesInVector<post_with_username_wrapper>(PostsList.ToList() , (pos, bytes) =>
                {
                    if(pos<PostsList.Count)
                        PostsList[pos].userImage = bytes;
                }
                , (post) =>
                {
                    return post.post.image_url;
                });


        }

        async Task UpdateLOCommentImages(int index)
        {

            await _comment_image_manager.TryLoadByteVector<lo_comment_with_username_wrapper>(index, LOCommentsList.ToList()
                , (pos, bytes) =>
                {
                    if(pos<LOCommentsList.Count)
                        LOCommentsList[pos].userImage = bytes;
                }
                , (locomment) =>
                {
                    return locomment.lo_comment.image_url;
                });


        }

        async Task ForceTableUpdate()
        {
            var repo = Mvx.Resolve<IRepositoryService>();
            await repo.TryGetTableUpdates();
        
        }






    }
}
