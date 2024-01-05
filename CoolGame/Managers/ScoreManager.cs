using CoolGame.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoolGame.Managers
{

    /// <summary>
    /// Manages the scores for the game, including storing, retrieving, and ranking player scores.
    /// </summary>
    public class ScoreManager
	{
		private static string _fileName = "score.xml";
		public List<Score> Highscores { get; private set; }
		public List<Score> Scores { get; private set; }
        /// <summary>
        /// Initializes a new instance of the ScoreManager class with an empty list of scores.
        /// </summary>
        public ScoreManager() : this(new List<Score>()) { }
        /// <summary>
        /// Initializes a new instance of the ScoreManager class with a specified list of scores.
        /// </summary>
        /// <param name="scores">The initial list of scores.</param>
        public ScoreManager(List<Score> scores)
		{
			Scores = scores;
			UpdateHighscores();
		}
        /// <summary>
        /// Adds a new score to the manager and updates the high score list.
        /// </summary>
        /// <param name="score">The score to add.</param>
        public void Add(Score score)
		{
			Scores.Add(score);
			Scores = Scores.OrderByDescending(c => c.Value).ToList(); // Orders the list so that the higher scores are first
			UpdateHighscores();
		}
        /// <summary>
        /// Retrieves the top five scores based on the number of enemies destroyed.
        /// </summary>
        /// <returns>An enumerable of top scores ordered by number of enemies destroyed.</returns>
        public IEnumerable<Score> GetTopFiveByNumberDestroyedEnemy()
        {
            return Scores.OrderByDescending(score => score.NumberDestroyedEnemy).Take(5);
        }
        /// <summary>
        /// Retrieves the top five scores based on survival time.
        /// </summary>
        /// <returns>An enumerable of top scores ordered by survival time.</returns>
        public IEnumerable<Score> GetTopFiveBySurvivalTime()
        {
            return Scores.OrderByDescending(score => score.survivalTime).Take(5);
        }
        /// <summary>
        /// Loads scores from a file, or creates a new ScoreManager if the file does not exist.
        /// </summary>
        /// <returns>A ScoreManager instance with loaded scores.</returns>
        public static ScoreManager Load()
		{
			if (!File.Exists(_fileName))
				return new ScoreManager();

				using (var reader = new StreamReader(new FileStream(_fileName, FileMode.Open)))
				{
					var serializer = new XmlSerializer(typeof(List<Score>));

					var scores = (List<Score>)serializer.Deserialize(reader);
					return new ScoreManager(scores);
				}
		}
        /// <summary>
        /// Updates the high score list to include the top five scores.
        /// </summary>
        public void UpdateHighscores()
		{
			Highscores = Scores.Take(5).ToList(); // Takes the first 5 elements}
		}

        /// <summary>
        /// Saves the current scores to a file.
        /// </summary>
        /// <param name="scoreManager">The ScoreManager instance to save.</param>
        public static void Save(ScoreManager scoreManager)
		{
			using (var writer = new StreamWriter(new FileStream(_fileName, FileMode.Create)))
			{
				var serializer = new XmlSerializer(typeof(List<Score>));

				serializer.Serialize(writer, scoreManager.Scores);
			}
		}

	}
}

