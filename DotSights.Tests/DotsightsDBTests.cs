using DotSights.Core.Common;
using DotSights.Core.Common.Types;

namespace DotSights.Tests
{
    [TestFixture]
    public class DotsightsDBTests
    {
        private DotsightsDB _dotsightsDB;

        [SetUp]
        public void Setup()
        {
            Core.DotSights.SetDataPath("test_data.json", "test_daily_data.json");
            _dotsightsDB = new DotsightsDB();
        }

        [TearDown]
        public void Cleanup()
        {
            // Clean up test data files
            File.Delete(Core.DotSights.DataFilePath);
            File.Delete(Core.DotSights.DailyDataFilePath);
        }

        [Test]
        public void LoadDataFromFile_ShouldPopulateActivitiesAndDailyDatas()
        {
            // Arrange
            var activityData1 = new ActivityData { ProcessName = "Process1", WindowTitle = "Window1" };
            var activityData2 = new ActivityData { ProcessName = "Process2", WindowTitle = "Window2" };
            var dailyData1 = new DailyData { Date = DateTime.Now.Date.AddDays(-1) };
            var dailyData2 = new DailyData { Date = DateTime.Now.Date.AddDays(-2) };
            var activities = new List<ActivityData> { activityData1, activityData2 };
            var dailyDatas = new List<DailyData> { dailyData1, dailyData2 };
            Core.DotSights.SetDataPath("test_data.json", "test_daily_data.json");
            File.WriteAllText(Core.DotSights.DataFilePath, Core.DotSights.SerializeData(activities));
            File.WriteAllText(Core.DotSights.DailyDataFilePath, Core.DotSights.SerializeData(dailyDatas));

            // Act
            _dotsightsDB.LoadDataFromFile();

            // Assert
            Assert.That(_dotsightsDB.Activities[0].WindowTitle, Is.EqualTo(activities[0].WindowTitle));
            Assert.That(_dotsightsDB.DailyDatas[0].Date, Is.EqualTo(dailyDatas[0].Date));
        }

        [Test]
        public void AddData_ShouldAddDataToActivitiesAndDailyDatas()
        {
            // Arrange
            var activityData1 = new ActivityData { ProcessName = "Process1", WindowTitle = "Window1" };
            var activityData2 = new ActivityData { ProcessName = "Process2", WindowTitle = "Window2" };
            _dotsightsDB.LoadDataFromFile();

            // Act
            _dotsightsDB.AddData(activityData1);
            _dotsightsDB.AddData(activityData2);

            // Assert
            Assert.That(_dotsightsDB.Activities.Count, Is.EqualTo(2));
            Assert.That(_dotsightsDB.DailyDatas[0].Date, Is.EqualTo(DateTime.Today));
            Assert.IsTrue(_dotsightsDB.Activities.Contains(activityData1));
            Assert.IsTrue(_dotsightsDB.Activities.Contains(activityData2));
        }

        [Test]
        public void SaveChanges_ShouldSaveActivitiesAndDailyDatasToFile()
        {
            // Arrange
            var activityData1 = new ActivityData { ProcessName = "Process1", WindowTitle = "Window1" };
            var activityData2 = new ActivityData { ProcessName = "Process2", WindowTitle = "Window2" };
            _dotsightsDB.LoadDataFromFile();
            _dotsightsDB.AddData(activityData1);
            _dotsightsDB.AddData(activityData2);

            // Act
            _dotsightsDB.SaveChanges();

            // Assert
            Core.DotSights.DeserializeData(File.ReadAllText(Core.DotSights.DataFilePath), out List<ActivityData> savedActivities);
            Core.DotSights.DeserializeData(File.ReadAllText(Core.DotSights.DailyDataFilePath), out List<DailyData> savedDailyDatas);
            Assert.That(savedActivities.Count, Is.EqualTo(2), "Did not save all activities");
            Assert.That(savedDailyDatas.Count, Is.EqualTo(1), "Did not save only todays daily data");
            Assert.IsTrue(savedActivities.Any(a => a.WindowTitle.Equals(activityData1.WindowTitle)));
            Assert.IsTrue(savedActivities.Any(a => a.WindowTitle.Equals(activityData2.WindowTitle)));
            Assert.That(savedDailyDatas[0].Date, Is.EqualTo(DateTime.Today));
        }

        [Test]
        public void AddData_WhenStorageOptimized_ShouldGroupOnlyGroupedProcessesTogether()
        {
            // Arrange
            var grouped1 = new ActivityData { ProcessName = "Process1", WindowTitle = "Window1" };
            var grouped2 = new ActivityData { ProcessName = "Process1", WindowTitle = "Window2" };
            var ungrouped1 = new ActivityData { ProcessName = "Process2", WindowTitle = "Window3" };
            var ungrouped2 = new ActivityData { ProcessName = "Process3", WindowTitle = "Window4" };
            var settings = new DotSightsSettings { GroupedProcessNames = ["process1"], OptimizeForStorageSpace = true };

            // Act
            var db = new DotsightsDB(settings);
            db.AddData(grouped1);
            db.AddData(grouped2);
            db.AddData(ungrouped1);
            db.AddData(ungrouped2);

            // Assert
            Assert.That(db.Activities.Count, Is.EqualTo(3), "Did not group any processes");
            Assert.That(db.Activities.Where(a => a.ProcessName == ungrouped1.ProcessName).Count(), Is.EqualTo(1), "Grouped process 2");
            Assert.That(db.Activities.Where(a => a.ProcessName == ungrouped2.ProcessName).Count(), Is.EqualTo(1), "Grouped process 3");
            Assert.That(db.Activities.Where(a => a.ProcessName == grouped1.ProcessName).Count(), Is.EqualTo(1), "Did not group process 1");
        }
    }

}