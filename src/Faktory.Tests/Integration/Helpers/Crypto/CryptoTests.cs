using System;
using System.IO;
using NUnit.Framework;

namespace Faktory.Tests.Integration.Helpers.Crypto
{
    [TestFixture]
    [NonParallelizable]
    [Category("Integration")]
    public class CryptoTests
    {
        [Test, Order(1)]
        [NonParallelizable]
        public void GetFileHash_NotFoundApp_ReturnsFailure()
        {
            // Act 
            var exception = Assert.Throws<FileNotFoundException>(() => Core.Helpers.Crypto.GetFileHash("nada_i_dont_exist.exe"));

            // Assert
            StringAssert.Contains("Could not find file", exception.Message);
        }

        [Test, Order(2)]
        [NonParallelizable]
        public void GetFileHash_OnExe_ReturnsHash()
        {
            // Arrange
            const string knownHash = "DE6478C25D3DEEC68ADA12CDA8D05B79C4526FB175304BE3B82557ACC1CD400B";
            // Act 
            var hash = Core.Helpers.Crypto.GetFileHash("dummy.exe");

            // Assert
            Assert.AreEqual(knownHash, hash);
        }
    }
}