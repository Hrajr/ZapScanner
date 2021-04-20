using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;
using System;
using SecurityScanner;

namespace SecurityScannerTest
{
    public class SecurityScannerTest
    {
        private program Scanner;

        public SecurityScannerTest()
        {
            Scanner = new program();
            Scanner.SetTarget("http://localhost:5000");
        }

        [Fact]
        public async Task TargetHeartbeatTest()
        {
            // Act

            // Arrange

            // Assert
            Assert.True(await Scanner.UrlIsReachable());
        }

        [Fact]
        public void StartSecurityTest()
        {
            // Act

            // Arrange

            // Assert
            Assert.True(Scanner.StartZapScan());
        }

        [Fact]
        public void GenerateResultsTest()
        {
            // Act
            Scanner.StartZapScan();

            // Arrange
            List<XElement> RiskList = Scanner.SortResultXML();

            // Assert
            Assert.NotNull(RiskList);
        }

        [Fact]
        public void StartAdvancedScan()
        {
            // Act
            Scanner.StartScan();

            // Arrange
            var result = Scanner.GenerateResultHTML();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void HighVulnerabilityTest()
        {
            Scanner.StartScan();
        }

        [Fact]
        public void ResultLogginTest()
        {

        }
    }
}