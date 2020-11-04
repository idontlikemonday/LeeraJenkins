using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Logic.Helpers;

namespace LeeraJenkins.Logic.Tests
{
    [TestClass]
    public class PlayerParsingTests
    {
        [TestMethod]
        public void PasreSingleNamedPlayerTest()
        {
            var playerName = "Имя @TgName";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasreDoubleNamedPlayerTest()
        {
            var playerName = "Имя Фамилия @TgName";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя Фамилия");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasreTripleNamedPlayerTest()
        {
            var playerName = "Имя Фамилия Отчество @TgName";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя Фамилия Отчество");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasreOnlyTgNamePlayerTest()
        {
            var playerName = "@TgName";

            var player = ParseHelper.FromString(playerName);
            Assert.IsNull(player.Name);
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasreOnlySingleNamePlayerTest()
        {
            var playerName = "Имя";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя");
            Assert.IsNull(player.TgNickname);
        }

        [TestMethod]
        public void PasreOnlyDoubleNamePlayerTest()
        {
            var playerName = "Имя Фамилия";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя Фамилия");
            Assert.IsNull(player.TgNickname);
        }

        [TestMethod]
        public void PasreDoubleNamedPlayerBetweenTest()
        {
            var playerName = "Имя @TgName Фамилия";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя Фамилия");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasrePlayerWithPrefixTest()
        {
            var playerName = "Имя Фамилия +1 @TgName";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя Фамилия +1");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasreSingleNamedPlayerWithoutSpacingTest()
        {
            var playerName = "Имя@TgName";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasreDoubleNamedPlayerWithAndWithoutSpacingTest()
        {
            var playerName = "Имя Фамилия@TgName";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя Фамилия");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasreSingleNamedPlayerWithoutSpacingWithPrefixTest()
        {
            var playerName = "Имя@TgName+1";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя +1");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }

        [TestMethod]
        public void PasreDoubleNamedPlayerWithoutSpacingWithInsidePrefixTest()
        {
            var playerName = "Имя@TgName+1Фамилия";

            var player = ParseHelper.FromString(playerName);
            Assert.AreEqual(player.Name, "Имя +1 Фамилия");
            Assert.AreEqual(player.TgNickname, "@TgName");
        }
    }
}
