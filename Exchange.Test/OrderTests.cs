using ConsoleExchange.Model;
using Exchange.BL;

namespace Exchange.Test
{
    [TestClass]
    public class MetaExchangeTests
    {
        // File path constants
        private const string AllOrderBooksPath = "Assets/order_books_data";
        private const string SingleBookPath = "Assets/order_books_data_one_book";
        private const string InvalidFilePath = "Assets/non_existing_file";

        private MetaExchange _metaExchange;

        [TestInitialize]
        public void Setup()
        {
            _metaExchange = new MetaExchange();
        }

        [TestMethod]
        public async Task Sell_Bitcoin_ValidOrders_ShouldReturnCorrectCount()
        {
            // Arrange
            OrderType orderType = OrderType.Sell;
            decimal amount = 1m;

            // Act
            var result = await _metaExchange.FindBestPossibleOrderToExecute(AllOrderBooksPath, orderType, amount);

            // Assert
            Assert.AreEqual(20, result.Count, "The number of orders returned is incorrect.");
            Assert.AreEqual(amount, result.Sum(o => o.Order.Amount), "The total amount of orders does not match the requested amount.");
        }

        [TestMethod]
        public async Task Buy_Bitcoin_ValidOrders_ShouldReturnCorrectCount()
        {
            // Arrange
            OrderType orderType = OrderType.Buy;
            decimal amount = 1m;

            // Act
            var result = await _metaExchange.FindBestPossibleOrderToExecute(AllOrderBooksPath, orderType, amount);

            // Assert
            Assert.AreEqual(1, result.Count, "The number of orders returned is incorrect.");
            Assert.AreEqual(amount, result.Sum(o => o.Order.Amount), "The total amount of orders does not match the requested amount.");
        }

        [TestMethod]
        public async Task Buy_Bitcoin_FromSingleBook_ShouldReturnCorrectCount()
        {
            // Arrange
            OrderType orderType = OrderType.Buy;
            decimal amount = 1m;

            // Act
            var result = await _metaExchange.FindBestPossibleOrderToExecute(SingleBookPath, orderType, amount);

            // Assert
            Assert.AreEqual(2, result.Count, "The number of orders returned is incorrect.");
            Assert.AreEqual(amount, result.Sum(o => o.Order.Amount), "The total amount of orders does not match the requested amount.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Buy_Bitcoin_InsufficientLiquidity_ShouldThrowException()
        {
            // Arrange
            OrderType orderType = OrderType.Buy;
            decimal amount = 10m; // Requesting more than available

            // Act
            await _metaExchange.FindBestPossibleOrderToExecute(SingleBookPath, orderType, amount);

            // Assert: Exception is expected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task FilePath_Empty_ShouldThrowException()
        {
            // Arrange
            string filePath = string.Empty;
            OrderType orderType = OrderType.Buy;
            decimal amount = 1m;

            // Act
            await _metaExchange.FindBestPossibleOrderToExecute(filePath, orderType, amount);

            // Assert: Exception is expected
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async Task FilePath_Invalid_ShouldThrowFileNotFoundException()
        {
            // Arrange
            OrderType orderType = OrderType.Buy;
            decimal amount = 1m;

            // Act
            await _metaExchange.FindBestPossibleOrderToExecute(InvalidFilePath, orderType, amount);

            // Assert: Exception is expected
        }

        [TestMethod]
        public async Task Buy_Bitcoin_ExactMatch_ShouldReturnAllOrders()
        {
            // Arrange
            OrderType orderType = OrderType.Buy;
            decimal amount = 5.83386034m;

            // Act
            var result = await _metaExchange.FindBestPossibleOrderToExecute(SingleBookPath, orderType, amount);

            // Assert
            Assert.AreEqual(5, result.Count, "The number of orders returned is incorrect.");
            Assert.AreEqual(amount, result.Sum(o => o.Order.Amount), "The total amount of orders does not match the requested amount.");
        }

        [TestMethod]
        public async Task Sell_Bitcoin_ExactMatch_ShouldReturnAllOrders()
        {
            // Arrange
            OrderType orderType = OrderType.Sell;
            decimal amount = 4.06696998m;

            // Act
            var result = await _metaExchange.FindBestPossibleOrderToExecute(SingleBookPath, orderType, amount);

            // Assert
            Assert.AreEqual(5, result.Count, "The number of orders returned is incorrect.");
            Assert.AreEqual(amount, result.Sum(o => o.Order.Amount), "The total amount of orders does not match the requested amount.");
        }
    }
}