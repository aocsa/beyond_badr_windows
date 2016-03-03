using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLearning.Core.Entities;
using FizzWare.NBuilder;
using MLearning.Core.Services;
using Core.Repositories;
using Nito.AsyncEx;
using MLearningDB;

namespace DataGenerator
{
    class Program
    {
        private static  IMLearningService _mLearningService;

        static void Main(string[] args)
        {
            try
            {
                AsyncContext.Run(() => AsyncMain(args));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return;
            }
        }

        static async void AsyncMain(string[] args)
        {
            IRepositoryService repositoryService = new WAMSRepositoryService();
            _mLearningService = new MLearningAzureService(repositoryService);

            Console.WriteLine("Hello world!");

            //await LOcomments();
            //await CirclePosts();
            //await LOquizzes();
            await CreateLearningObjects(951);

            Console.ReadKey(true);
            
        }

        static async Task LOcomments()
        {
            List<lo_by_owner> los = await _mLearningService.GetLOsbyOwner();
            Console.WriteLine();
            foreach (var l in los)
            {
                Console.WriteLine(l.title);
                IList<LOComment> LOcoments = Builder<LOComment>.CreateListOfSize(10).All().With(x => x.lo_id = l.id).And(x => x.user_id = l.user_id).Build();
                foreach (var c in LOcoments)
                {
                    c.id = default(int);
                    c.text = LoremIpsum(5, 20, 1, 10, 1);
                    await _mLearningService.CreateObject<LOComment>(c, x => x.id);
                }
            }
        }

        static async Task LOquizzes()
        {
            List<lo_by_owner> los = await _mLearningService.GetLOsbyOwner();
            Console.WriteLine();
            foreach (var l in los)
            {
                Console.WriteLine(l.title);
                IList<Quiz> loquizzes = Builder<Quiz>.CreateListOfSize(10).All().With(x => x.LearningObject_id = l.id).Build();
                foreach (var q in loquizzes)
                {
                    q.id = default(int);
                    q.title = LoremIpsum(5, 20, 1, 1, 1);
                    q.content = LoremIpsum(5, 20, 1, 10, 1);
                    await _mLearningService.CreateObject<Quiz>(q, x => x.id);
                }
            }
        }

        static async Task CirclePosts()
        {
            List<Circle> circles = await _mLearningService.GetCircles();
            foreach (var c in circles)
            {
                Console.WriteLine(c.name);
                IList<Post> criclePosts = Builder<Post>.CreateListOfSize(10).All().With(x => x.circle_id = c.id).And(x => x.user_id = c.owner_id??default(int)).Build();
                foreach (var p in criclePosts)
                {
                    p.id = default(int);
                    p.text = LoremIpsum(5, 20, 1, 10, 1);
                    await _mLearningService.CreateObject<Post>(p, x => x.id);
                }
            }
        }

        static async Task CreateLearningObjects(int n)
        {
            IList<LearningObject> learningObjects = Builder<LearningObject>.CreateListOfSize(n).All().With(x => x.Publisher_id = 1).Build();
            int i = 0;
            foreach (var lo in learningObjects)
            {
                lo.id = default(int);
                lo.title = LoremIpsum(5, 20, 1, 1, 1);
                lo.description = LoremIpsum(5, 20, 1, 5, 1);
                await _mLearningService.CreateObject<LearningObject>(lo, x => x.id);
                drawTextProgressBar(++i, n);
            }
        }

        static string LoremIpsum(int minWords, int maxWords,
    int minSentences, int maxSentences,
    int numParagraphs)
        {

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences)
                + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            string result = string.Empty;

            for (int p = 0; p < numParagraphs; p++)
            {
                
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result += " "; }
                        result += words[rand.Next(words.Length)];
                    }
                    result += ". ";
                }
                
            }

            return result;
        }

        private static void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }

    }
}
