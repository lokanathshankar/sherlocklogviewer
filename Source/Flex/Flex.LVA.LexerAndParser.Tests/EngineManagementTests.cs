namespace Flex.LVA.LexerAndParser.Tests
{
    using Flex.LVA.Core.EngineManagement;
    using Flex.LVA.Core.Interfaces;
    using Moq;
    using System.Security.Cryptography;

    [TestClass]
    public class EngineManagementTests
    {
        [TestMethod]
        public void EngineRegistrationStartTest()
        {
            Mock<IRenderer> aMockRenderer = new Mock<IRenderer>();
            Mock<IRendererFactory> aMockFactory = new Mock<IRendererFactory>();
            aMockFactory.Setup(theX => theX.GetRenderer(It.IsAny<long>())).Returns(aMockRenderer.Object);
            using (EngineInstanceManager aManager = new EngineInstanceManager(aMockFactory.Object, new Mock<IApplicationEvents>().Object))
            {
                long aId;
                long aId2;
                string aAddress;
                Assert.IsTrue(aManager.StartLogEngine(out aId, out aAddress));
                Assert.IsTrue(aId > 0);
                Assert.IsTrue(aManager.StartLogEngine(out aId2, out aAddress));
                Assert.IsTrue(aId != aId2);
            }
        }

        [TestMethod]
        public void EngineRegistrationStartAndEndTest()
        {
            Mock<IRenderer> aMockRenderer = new Mock<IRenderer>();
            Mock<IRendererFactory> aMockFactory = new Mock<IRendererFactory>();
            aMockFactory.Setup(theX => theX.GetRenderer(It.IsAny<long>())).Returns(aMockRenderer.Object);
            using (EngineInstanceManager aManager = new EngineInstanceManager(aMockFactory.Object, new Mock<IApplicationEvents>().Object))
            {
                long aId;
                string aAddress;
                Assert.IsTrue(aManager.StartLogEngine(out aId, out aAddress));
                Assert.IsTrue(aManager.StopLogEngine(aId));
                Assert.IsFalse(aManager.StopLogEngine(2));
                Assert.IsFalse(aManager.StopLogEngine(3));
                Assert.IsFalse(aManager.StopLogEngine(4));
            }
        }

        [TestMethod]
        public void EngineRegistrationAutoDisposeTest()
        {
            Mock<IRenderer> aMockRenderer = new Mock<IRenderer>();
            Mock<IRendererFactory> aMockFactory = new Mock<IRendererFactory>();
            aMockFactory.Setup(theX => theX.GetRenderer(It.IsAny<long>())).Returns(aMockRenderer.Object);
            using (EngineInstanceManager aManager = new EngineInstanceManager(aMockFactory.Object, new Mock<IApplicationEvents>().Object))
            {
                Assert.IsTrue(aManager.StartLogEngine(out long aId, out string _));
                aMockRenderer.Raise(theX => theX.OnConnectionLost += null, this, aId);
                aMockRenderer.Verify(theX => theX.Dispose(), Times.Once);
            }
        }
    }
}
