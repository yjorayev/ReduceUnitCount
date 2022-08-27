
using Moq;

namespace ProductManagement.Test
{
    [TestClass]
    public class OrderTest
    {
        [TestMethod]
        public async Task Can_Process_MultipleProducts()
        {
            var mockStore = new Mock<IProductStore>();
            mockStore
                .Setup(store => store.GetUnitsForProductAsync("Cola"))
                .ReturnsAsync(
                    new List<UnitOfMeasure>
                    {
                        new UnitOfMeasure("EACH", 1),
                        new UnitOfMeasure("PACK", 6),
                        new UnitOfMeasure("CASE", 24)
                    });

            mockStore.Setup(store => store.GetUnitsForProductAsync("Chocolate"))
                .ReturnsAsync(
                    new List<UnitOfMeasure>
                    {
                        new UnitOfMeasure("DOZEN", 12),
                        new UnitOfMeasure("PACK", 5),
                        new UnitOfMeasure("PAIR", 2),
                    });

            var items = new OrderItem[]
            {
                new OrderItem("Cola", "PACK", 1),
                new OrderItem("Cola", "EACH", 62),
                new OrderItem("Chocolate", "DOZEN", 1),
                new OrderItem("Chocolate", "PAIR", 4),
            };

            var result = await Order.ReduceUnitsAsync(items, mockStore.Object);
            CollectionAssert.Contains(result, new OrderItem("Cola", "CASE", 2));
            CollectionAssert.Contains(result, new OrderItem("Cola", "PACK", 3));
            CollectionAssert.Contains(result, new OrderItem("Cola", "EACH", 2));

            CollectionAssert.Contains(result, new OrderItem("Chocolate", "PACK", 4));
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var mockStore = new Mock<IProductStore>();
            mockStore
                .Setup(store => store.GetUnitsForProductAsync(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<UnitOfMeasure>
                    {
                        new UnitOfMeasure("EACH", 1),
                        new UnitOfMeasure("PACK", 6),
                        new UnitOfMeasure("CASE", 24)
                    });

            var items = new OrderItem[]
            {
                new OrderItem("Cola", "PACK", 1),
                new OrderItem("Cola", "EACH", 62)
            };

            var result = await Order.ReduceUnitsAsync(items, mockStore.Object);
            CollectionAssert.Contains(result, new OrderItem("Cola", "CASE", 2));
            CollectionAssert.Contains(result, new OrderItem("Cola", "PACK", 3));
            CollectionAssert.Contains(result, new OrderItem("Cola", "EACH", 2));
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            var mockStore = new Mock<IProductStore>();
            mockStore
                .Setup(store => store.GetUnitsForProductAsync(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<UnitOfMeasure>
                    {
                        new UnitOfMeasure("EACH", 1),
                        new UnitOfMeasure("PACK", 2),
                    });

            var items = new OrderItem[]
            {
                new OrderItem("Cola", "EACH", 3),
            };

            var result = await Order.ReduceUnitsAsync(items, mockStore.Object);
            CollectionAssert.Contains(result, new OrderItem("Cola", "PACK", 1));
            CollectionAssert.Contains(result, new OrderItem("Cola", "EACH", 1));
        }

        [TestMethod]
        public async Task TestMethod3()
        {
            var mockStore = new Mock<IProductStore>();
            mockStore
                .Setup(store => store.GetUnitsForProductAsync(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<UnitOfMeasure>
                    {
                        new UnitOfMeasure("CASE", 6),
                        new UnitOfMeasure("PACK", 3),
                        new UnitOfMeasure("EACH", 2),
                    });

            var items = new OrderItem[]
            {
                new OrderItem("Cola", "EACH", 2),
                new OrderItem("Cola", "PACK", 1),
            };

            var result = await Order.ReduceUnitsAsync(items, mockStore.Object);
            CollectionAssert.Contains(result, new OrderItem("Cola", "PACK", 1));
            CollectionAssert.Contains(result, new OrderItem("Cola", "EACH", 2));
        }
    }
}