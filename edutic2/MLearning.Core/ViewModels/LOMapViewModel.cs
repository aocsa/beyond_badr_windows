using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Core.DownloadCache;
using Core.Session;
using MLearning.Core.Classes;
using MLearning.Core.Configuration;
using MLearning.Core.Entities;
using MLearning.Core.Entities.json;
using MLearning.Core.Services;
using MLearningDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.ViewModels
{
	public class LOMapViewModel : MvxViewModel
	{


		private IMLearningService _mLearningService;


		static int pagecount = 0;


		public LOMapViewModel(IMLearningService mlearningService)
		{
			_mLearningService = mlearningService;


		}




		public int _currentCurso;
		public int _currentUnidad;
		public int _currentSection;

		string _serialized_list;

		public void Init(int lo_id,string serialized_los_in_circle, int _currentCurso, int _currentUnidad,int _currentSection)
		{
			//LoadPages(selectedLOIndex);

			this._currentCurso = _currentCurso;
			this._currentUnidad = _currentUnidad;
			this._currentSection = _currentSection;

			LOID = lo_id;

			_serialized_list = serialized_los_in_circle; 

		}


		async public Task InitLoad()
		{

			List<lo_by_circle> los_in_Circle = JsonConvert.DeserializeObject<List<lo_by_circle>>(_serialized_list);

			LOsInCircle = new ObservableCollection<lo_by_circle_wrapper>();


			int i = 0;
			foreach (var item in los_in_Circle)
			{
				bool images = item.id == LOID;
			
				if (LOID == item.id)
				{
					LOsInCircle.Add(new lo_by_circle_wrapper { lo = item,stack =new stack_wrapper { IsLoaded = false }  });
					await LoadPages2(i++, images);
				} 

				//Load images if its the selected LO
			

				//await LoadPages(i++, images);

			}



			var selectedLOIndex = LOsInCircle.IndexOf(LOsInCircle.Where(lo => lo.lo.id == LOID).First());

			//await LoadBackgroundImages();
		}



		async public Task InitLoadData()
		{
			List<lo_by_circle> los_in_Circle = JsonConvert.DeserializeObject<List<lo_by_circle>>(_serialized_list);

			LOsInCircle = new ObservableCollection<lo_by_circle_wrapper>();


		}




		async Task LoadPages(int loListIndex,bool images)
		{
			var lo_obj = LOsInCircle[loListIndex];
			var LOID = lo_obj.lo.id;

			var list = await _mLearningService.GetPagesByLO(LOID);







			var AllPagesList = new ObservableCollection<page_wrapper>();

			//Loading json content to memory
			foreach (var item in list)
			{
				Mvx.Trace("Id: " + item.id);
				LOContent locontent = null;
				try
				{
					locontent = JsonConvert.DeserializeObject<LOContent>(item.content);

				}
				catch(Exception e)
				{
					Mvx.Trace("Serialization error: "+e.Message);
				}


				AllPagesList.Add(new page_wrapper { page = item, content = locontent /*, id=pagecount*/});
				pagecount++;
			}


			var tags = await _mLearningService.GetTagsByLO(LOID);

			PageTags = new ObservableCollection<tag_by_page>(tags);

			var tlist = tags.GroupBy(t => t.name).Select(t=>t).ToList();

			var GroupedPagesList = new ObservableCollection<page_collection_wrapper>();

			for (int i = 0; i < tlist.Count;i++ )
			{
				IEnumerable<tag_by_page> group = tlist[i];
				var groupCollection = new ObservableCollection<page_wrapper>();

				foreach (var item in group)
				{
					groupCollection.Add(AllPagesList.Where(p => p.page.id == item.page_id).First());
				}

				GroupedPagesList.Add(new page_collection_wrapper { PagesList = groupCollection, TagName = group.FirstOrDefault().name });

				if (images)
					UpdatePagesImages(0, GroupedPagesList[i].PagesList);

				//  GroupedPagesList.Add(AllPagesList.Where(p => p.page.id == item.page_id).First());
			}


			LOsInCircle[loListIndex].stack.StacksList = GroupedPagesList;
			LOsInCircle[loListIndex].stack.IsLoaded = true;

			LOCurrentIndex = loListIndex;

			UpdateExtraInfo(LOCurrentIndex); 

			//Download xmls

			//foreach (var item in list)
			//{

			//    await _mLearningService.OpenLOPage(item.url_xml);
			//}


		}


		async Task LoadPages2(int loListIndex,bool images)
		{
			var lo_obj = LOsInCircle[loListIndex];
			var LOID = lo_obj.lo.id;

			var sectionList = await _mLearningService.GetSectionsByLO (LOID);

			//var allSections = new ObservableCollection<LOsection>();

			//Collection for transorming data
			var AllPagesList = new ObservableCollection<page_wrapper> (); 
			//collections of collections
			var GroupedPagesList = new ObservableCollection<page_collection_wrapper>();

			//For collection al pages 
			var pagesList = new List<Page> ();



			foreach (var item in sectionList) {
				var sectionPages = await _mLearningService.GetPagesByLOSection (item.id);
				foreach (var page in sectionPages) {
					pagesList.Add (page);
				}
				var collectionWrapper = new page_collection_wrapper ();
				collectionWrapper.TagName = item.name;

				foreach (var p in sectionPages) {
					Mvx.Trace("Id: " + p.id);
					LOContent locontent = null;
					//LOContent locontent =  new LOContent();
					try
					{
						locontent = JsonConvert.DeserializeObject<LOContent> (p.content);//(locontentstring);;
						//locontent.lopage = JsonConvert.DeserializeObject<Lopage>(p.content);
					}
					catch(Exception e)
					{
						Mvx.Trace("Serialization error: "+e.Message);
					}

					var pw1 = new page_wrapper { page = p, content = locontent /*, id=pagecount*/ };
					collectionWrapper.PagesList.Add (pw1);

					//AllPagesList.Add(pw1);
					pagecount++;
				}

				GroupedPagesList.Add (collectionWrapper);

				if (this.LOID == item.LO_id)
					if (images)
						UpdatePagesImages (0, collectionWrapper.PagesList);
			}


			//if (images)
			//	UpdatePagesImages(0, GroupedPagesList[i].PagesList);


			LOsInCircle[loListIndex].stack.StacksList = GroupedPagesList;
			LOsInCircle[loListIndex].stack.IsLoaded = true;

			LOCurrentIndex = loListIndex;

			UpdateExtraInfo(LOCurrentIndex);

		}


		async Task LoadPageImages(int loindex)
		{
			foreach (var item in LOsInCircle[loindex].stack.StacksList)
			{                
				UpdatePagesImages(0, item.PagesList);   
			}
		}


		public class page_wrapper : MvxNotifyPropertyChanged
		{
			public Page page { get; set; }

			byte[] _cover_bytes;
			public byte[] cover_bytes
			{
				get { return _cover_bytes; }
				set { _cover_bytes = value; RaisePropertyChanged("cover_bytes"); }
			}

			//public int id { get; set; }
			//public int i_id { get; set; }
			//public int j_id { get; set; }
			//public int k_id { get; set; }



			public LOContent content { get; set; }


			bool _isLoaded;
			public bool IsLoaded
			{
				get { return _isLoaded; }
				set { _isLoaded = value; RaisePropertyChanged("IsLoaded"); }
			}





		}

		public class page_collection_wrapper : MvxNotifyPropertyChanged
		{

			ObservableCollection<page_wrapper> _pagesList = new ObservableCollection<page_wrapper> ();

			public ObservableCollection<page_wrapper> PagesList
			{
				get { return _pagesList; }
				set { _pagesList = value; RaisePropertyChanged("PagesList"); }
			}


			string _tagName;
			public string TagName
			{
				get { return _tagName; }
				set { _tagName = value; RaisePropertyChanged("TagName"); }
			}

		}

		public class stack_wrapper : MvxNotifyPropertyChanged
		{

			ObservableCollection<page_collection_wrapper> _stacksList;
			public ObservableCollection<page_collection_wrapper> StacksList
			{
				get { return _stacksList; }
				set { _stacksList = value; RaisePropertyChanged("StacksList"); }
			}


			bool _isLoaded;
			public bool IsLoaded
			{
				get { return _isLoaded; }
				set { _isLoaded = value; RaisePropertyChanged("IsLoaded"); }
			}




		}

		public class lo_by_circle_wrapper : MvxNotifyPropertyChanged
		{
			public lo_by_circle lo { get; set; }



			byte[] _background_bytes;
			public byte[] background_bytes
			{
				get { return _background_bytes; }
				set { _background_bytes = value; RaisePropertyChanged("background_bytes"); }
			}




			stack_wrapper _stack;
			public stack_wrapper stack
			{
				get { return _stack; }
				set { _stack = value; RaisePropertyChanged("stack"); }
			}


			stack_wrapper _stack2;
			public stack_wrapper stack2
			{
				get { return _stack; }
				set { _stack2 = value; RaisePropertyChanged("stack2"); }
			}





		}


		int _lOID;
		public int LOID
		{
			get { return _lOID; }
			set { _lOID = value; RaisePropertyChanged("LOID"); }
		}



		page_wrapper _currentPage;
		public page_wrapper CurrentPage
		{
			get { return _currentPage; }
			set {
				_currentPage = value;
				RaisePropertyChanged("CurrentPage");
			}
		}




		int _loCurrentIndex;
		public int LOCurrentIndex
		{
			get { return _loCurrentIndex; }
			set { _loCurrentIndex = value; RaisePropertyChanged("LOCurrentIndex"); /*Testing Purposes*/}
		}

		async private Task UpdateExtraInfo(int _loCurrentIndex)
		{
			Title = LOsInCircle[_loCurrentIndex].lo.title;
			Description = LOsInCircle[_loCurrentIndex].lo.description;
			if (LOsInCircle[_loCurrentIndex].stack.StacksList[0].PagesList[0].cover_bytes==null )
			{
				LoadPageImages(_loCurrentIndex);

			}
			GroupedPagesList = LOsInCircle[_loCurrentIndex].stack.StacksList;
		}


		string _title;
		public string Title
		{
			get { return _title; }
			set { _title = value; RaisePropertyChanged("Title"); }
		}


		string _description;
		public string Description
		{
			get { return _description; }
			set { _description = value; RaisePropertyChanged("Description"); }
		}



		ObservableCollection<page_collection_wrapper> _groupedPagesList;
		public ObservableCollection<page_collection_wrapper> GroupedPagesList
		{
			get { return _groupedPagesList; }
			set { 
				_groupedPagesList = value;
				RaisePropertyChanged("GroupedPagesList"); 
			}
		}



		//Testing Purposes

		ObservableCollection<page_wrapper> _pagesList;
		public ObservableCollection<page_wrapper> PagesList
		{
			get { return _pagesList; }
			set { _pagesList = value; RaisePropertyChanged("PagesList"); }
		}




		ObservableCollection<lo_by_circle_wrapper> _losInCircle;
		public ObservableCollection<lo_by_circle_wrapper> LOsInCircle
		{
			get { return _losInCircle; }
			set {
				_losInCircle = value; 
				RaisePropertyChanged("LOsInCircle"); 
			}
		}



		ObservableCollection<tag_by_page> _pageTags;
		public ObservableCollection<tag_by_page> PageTags
		{
			get { return _pageTags; }
			set { _pageTags = value; RaisePropertyChanged("PageTags"); }
		}






		//ObservableCollection<page_wrapper> _allPagesList;
		//public ObservableCollection<page_wrapper> AllPagesList
		//{
		//    get { return _allPagesList; }
		//    set { _allPagesList = value; RaisePropertyChanged("AllPagesList"); }
		//}




		//List of GroupedPagesList







		int _currentIndexDisplaying;
		public int CurrentIndexDisplaying
		{
			get { return _currentIndexDisplaying; }
			set { _currentIndexDisplaying = value; RaisePropertyChanged("CurrentIndexDisplaying"); }
		}



		//Testing purposes


		MvxCommand<page_collection_wrapper> _selectStack;
		public System.Windows.Input.ICommand SelectStack
		{
			get
			{
				_selectStack = _selectStack ?? new MvxCommand<page_collection_wrapper>(DoSelectStack);
				return _selectStack;
			}
		}

		void DoSelectStack(page_collection_wrapper collection)
		{
			PagesList = collection.PagesList;
		}



		MvxCommand<int> _loadStackImagesCommand;
		public System.Windows.Input.ICommand LoadStackImagesCommand
		{
			get
			{
				_loadStackImagesCommand = _loadStackImagesCommand ?? new MvxCommand<int>(DoLoadStackImagesCommand);
				return _loadStackImagesCommand;
			}
		}

		void DoLoadStackImagesCommand(int loindex)
		{
			LoadPageImages(loindex);
		}




		MvxCommand _backCommand;
		public System.Windows.Input.ICommand BackCommand
		{
			get
			{
				_backCommand = _backCommand ?? new MvxCommand(DoBackCommand);
				return _backCommand;
			}
		}

		void DoBackCommand()
		{
			Close(this);
		}


		MvxCommand _homeCommand;
		public System.Windows.Input.ICommand HomeCommand
		{
			get
			{
				_homeCommand = _homeCommand ?? new MvxCommand(DoHomeCommand);
				return _homeCommand;
			}
		}

		void DoHomeCommand()
		{

		}



		MvxCommand _searchCommand;
		public System.Windows.Input.ICommand SearchCommand
		{
			get
			{
				_searchCommand = _searchCommand ?? new MvxCommand(DoSearchCommand);
				return _searchCommand;
			}
		}

		void DoSearchCommand()
		{

		}




		MvxCommand _shareCommand;
		public System.Windows.Input.ICommand ShareCommand
		{
			get
			{
				_shareCommand = _shareCommand ?? new MvxCommand(DoShareCommand);
				return _shareCommand;
			}
		}

		void DoShareCommand()
		{

		}




		MvxCommand _helpCommand;
		public System.Windows.Input.ICommand HelpCommand
		{
			get
			{
				_helpCommand = _helpCommand ?? new MvxCommand(DoHelpCommand);
				return _helpCommand;
			}
		}

		void DoHelpCommand()
		{

		}




		MvxCommand<page_wrapper> _openPageCommand;
		public System.Windows.Input.ICommand OpenPageCommand
		{
			get
			{
				_openPageCommand = _openPageCommand ?? new MvxCommand<page_wrapper>(DoOpenPageCommand);
				return _openPageCommand;
			}
		}

		async void DoOpenPageCommand(page_wrapper page)
		{ 

			if (!page.IsLoaded)
			{
				await PageParser.DownloadSetBytes(page.content); 
				page.IsLoaded = true;
			}

			//CurrentPage = page;
			addpage2cache(page);
		}


		int MaxPagesCache = 5;
		List<page_wrapper> pagelistcache = new List<page_wrapper>();

		void addpage2cache(page_wrapper page)
		{
			if (pagelistcache.Count >= MaxPagesCache)
			{
				pagelistcache[0].IsLoaded = false;
				PageParser.NullBytes(pagelistcache[0].content);
				pagelistcache.RemoveAt(0);
			}

			pagelistcache.Add(page);

		}



		async Task UpdatePagesImages(int index,ObservableCollection<page_wrapper> list)
		{


			/* IncrementalDownload _manager = new IncrementalDownload(); ;

			_manager.TryLoadByteVector<page_wrapper>(index, list.ToList()
				, (pos, bytes) =>
				{
					list[pos].cover_bytes = bytes;
				}
				, (page) =>
				{
					return page.page.url_img;
				}); */


		}

		//Not incremental
		async Task LoadBackgroundImages()
		{

			//await BlockDownload.TryLoadByteVector<lo_by_circle_wrapper>(LOsInCircle.ToList(),
			//     (pos, bytes) => { LOsInCircle[pos].background_bytes = bytes; },
			//     (lo) => {return lo.lo.url_background;}
			//     );

			await BlockDownload.TryPutBytesInVector<lo_by_circle_wrapper>(LOsInCircle.ToList(),
				(pos, bytes) => { LOsInCircle[pos].background_bytes = bytes; },
				(lo) => {return lo.lo.url_background;}
			);

		}



		async public Task<byte[]> GetBytes(string url)
		{

			CacheService cache = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);

			var bytesAndLocalPath = await cache.tryGetResource(url);

			return bytesAndLocalPath.Item1;

		}

		string locontentstring = "{\n  \"lopage\": {\n    \"loslide\": [\n      {\n        \"lotitle\": \"Mollepata-Soroypampa\",\n        \"loparagraph\": \"Recorrido Duración 8km 5horas Punto elevado 3,869msnm   Terreno Valle andino alto, puna, pastizales\",\n\t\t\"loimage\": \"https://mlearningservice.blob.core.windows.net/caminoinca/002_%20La%20ruta%20del%20Salkantay/001%20D%C3%8DA%201/001%20Mollepata-Soroypampa/1366x768.jpg\",\n        \"lotype\": 0\n},\n\t  {\n        \"lotitle\": \"La ruta del Salkantay\",\n        \"loparagraph\": \"La ruta del Salkantay se ha posicionado en los últimos años como una forma alternativa, y no menos impresionante, de acceder a Machu Picchu. Con la compañía constante del apu tutelar de los incas, esta travesía ofrece al caminante los grandes espacios de los altos Andes y toda la exuberancia de los bosques húmedos en un viaje donde historia y naturaleza se funden para mostrar la forma en que los antiguos peruanos alcanzaron un alto desarrollo cultural respetando su entorno.\",\n        \"loimage\": \"http://mlearningservice.blob.core.windows.net/images/image-063.jpg\",\n        \"lotype\": 1\n},\n      {\n        \"lotitle\": \"Soraypampa\",\n        \"loparagraph\": \"El primer día de caminata se inicia antes del amanecer en la ciudad de Cusco e incluye el recorrido por carretera asfaltada hacia el valle de Limatambo, por la ruta hacia Abancay, hasta el pueblo de Mollepata (98 km), ubicado en un cálido valle interandino. En el camino es posible visitar el sitio arqueológico de Tarawasi (’la casa de la tara’), un tambo o lugar de descanso construido durante el gobierno del Inca Pachacutec para los caminantes que se dirigían al Chinchaysuyo. Está compuesto por una amplia plataforma flanqueada por altos muros de piedras finamente labradas con formas poligonales. Completa el conjunto un ushnu o altar elevado y una serie de andenes agrícolas.\",\n        \"loimage\": \"http://mlearningservice.blob.core.windows.net/images/image-065.jpg\",\n        \"lotype\": 1\n},\n\t  {\n        \"lotitle\": \"El dato\",\n        \"loparagraph\": \"El primer día de caminata es de poca pendiente, así que muy importante para aclimatarse. Camine despacio mientras su cuerpo se adecúa a la menor cantidad de oxígeno de las alturas.\",\n        \"lotype\": 2\n},\n\n      {\n        \"lotitle\": \"Salkantay lodge to lodge\",\n        \"loparagraph\": \"Mountain Lodges of Peru ofrece una forma diferente de realizar el trekking de Salkantay-Machu Picchu, con pernocte en cómodos albergues rurales construidos de forma amigable con el entorno. De esta forma, a la aventura de la caminata se le añade el disfrute de infraestructura de primer nivel cada noche de la ruta. Cuentan con cuatro albergues de lujo con capacidad para 20 huéspedes: Salkantay Lodge  Adventure Resort (Soraypampa), Wayra Lodge (Huayraccmachay), Colpa Lodge (Collpapampa) y Lucma Lodge (Lucmapampa). El viaje desde Cusco toma siete días, incluida la visita a Machu Picchu y el viaje de ida y vuelta desde Mollepata. Una experiencia de primera en medio de las montañas.\",\n        \"lotype\": 2\n},\n      {\n        \"lotitle\": \"Otro dato\",\n\t\t\"loparagraph\": \"A pesar de su belleza, la ortiga macho (Loasa urens) posee un compuesto urticante al contacto con los petalos.\",\n     \t\"loimage\": \"http://mlearningservice.blob.core.windows.net/images/image-064.jpg\",\n         \"lotype\": 1\n}\n      \n    ]\n  }\n}" ;


	}
}