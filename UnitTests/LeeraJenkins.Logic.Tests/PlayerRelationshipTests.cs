using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Tests
{
    [TestClass]
    public class PlayerRelationshipTests
    {
        [TestMethod]
        public void EqualityByTgNameTest1()
        {
            var player1Name = "Имя @TgName";
            var player2Name = "Имя Фамилия @TgName";

            EqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void EqualityByTgNameAndFriendsTest2()
        {
            var player1Name = "Имя @TgName +1";
            var player2Name = "Имя Фамилия @TgName +1";

            EqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void EqualityByTgNameAndFriendsTest3()
        {
            var player1Name = "Имя +1 @TgName";
            var player2Name = "Имя Фамилия @TgName +1";

            EqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void EqualityByFriendsTest1()
        {
            var player1Name = "Имя +1";
            var player2Name = "Имя +1";

            EqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void EqualityByFriendsTest2()
        {
            var player1Name = "+1 Имя";
            var player2Name = "Имя +1";

            EqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void EqualityByFriendsTest3()
        {
            var player1Name = "Имя + 1";
            var player2Name = "Имя +1";

            EqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void EqualityByFriendsTest4()
        {
            var player1Name = "Имя + 1";
            var player2Name = "+ 1 Имя";

            EqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void InEqualityByNameTest()
        {
            var player1Name = "Имя";
            var player2Name = "Имя Фамилия";

            InEqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void InEqualityByFriendsTest1()
        {
            var player1Name = "Имя";
            var player2Name = "Имя +1";

            FriendsEqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void InEqualityByFriendsTest2()
        {
            var player1Name = "Имя +1";
            var player2Name = "Имя +2";

            FriendsEqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void InEqualityByFriendsTest3()
        {
            var player1Name = "+1 Имя";
            var player2Name = "Имя +2";

            FriendsEqualityTest(player1Name, player2Name);
        }

        [TestMethod]
        public void InEqualityByNameAndFriendsTest()
        {
            var player1Name = "Имя +1";
            var player2Name = "Имя Фамилия +1";

            InEqualityTest(player1Name, player2Name);
        }

        private void EqualityTest(string player1Name, string player2Name)
        {
            var player1 = ParseHelper.FromString(player1Name);
            var player2 = ParseHelper.FromString(player2Name);
            var result = player1.GetRelationshipWith(player2);
            Assert.AreEqual(PlayerRelationship.Same, result);
        }

        private void FriendsEqualityTest(string player1Name, string player2Name)
        {
            var player1 = ParseHelper.FromString(player1Name);
            var player2 = ParseHelper.FromString(player2Name);
            var result = player1.GetRelationshipWith(player2);
            Assert.AreEqual(PlayerRelationship.Friend, result);
        }

        private void InEqualityTest(string player1Name, string player2Name)
        {
            var player1 = ParseHelper.FromString(player1Name);
            var player2 = ParseHelper.FromString(player2Name);
            var result = player1.GetRelationshipWith(player2);
            Assert.AreEqual(PlayerRelationship.Other, result);
        }
    }
}
