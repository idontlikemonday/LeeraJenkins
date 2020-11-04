using System;
using System.Text.RegularExpressions;
using LeeraJenkins.Common.Extentions;
using LeeraJenkins.Common.Helpers;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Model.Core
{
    public class Player
    {
        private static string _playerFriendRegexp = Misc.PlayerFriendRegexp;

        public long Id { get; set; }
        public string Name { get; set; }
        public string TgNickname { get; set; }
        public string Fullname => ToString();
        public bool IsExcess { get; set; }

        public bool IsEmpty => ToString().IsLogicallyEmptyAsPlayer();

        public override string ToString()
        {
            return NameHasWithFriendsSymbols()
                ? String.Format("{0} {1}", TgNickname ?? String.Empty, Name ?? String.Empty).Trim()
                : String.Format("{0} {1}", Name ?? String.Empty, TgNickname ?? String.Empty).Trim();
        }

        public string ToListedFormatString()
        {
            return IsExcess
                ? $"🛑 { ToString() }"
                : ToString();
        }

        public bool NameHasWithFriendsSymbols()
        {
            return Name != null && Regex.IsMatch(Name.Trim(), _playerFriendRegexp);
        }

        public string GetCleanedFriendsSymbols()
        {
            if (!NameHasWithFriendsSymbols())
            {
                return String.Empty;
            }

            var match = Regex.Match(Name.Trim(), _playerFriendRegexp);
            var cleanedMatch = Regex.Replace(match.Value, @"\s+", "");
            return cleanedMatch;
        }

        public string GetCleanedName()
        {
            if (!NameHasWithFriendsSymbols())
            {
                return Name;
            }

            var cleanedName = Regex.Replace(Name, _playerFriendRegexp, "").Trim();
            return cleanedName;
        }

        public bool IsLogicallyEquals(Player other)
        {
            if (other == null)
            {
                return false;
            }
            var areTgNamesEqual = true;
            if (TgNickname != null)
            {
                areTgNamesEqual = TgNickname.Trim().Equals(other.TgNickname?.Trim(), StringComparison.InvariantCultureIgnoreCase);
            }
            if (!areTgNamesEqual)
            {
                return false;
            }
            var areFriendsSymbolsEqual = this.GetCleanedFriendsSymbols() == other.GetCleanedFriendsSymbols();
            if (!String.IsNullOrEmpty(this.TgNickname))
            {
                return areFriendsSymbolsEqual;
            }
            if (!areFriendsSymbolsEqual)
            {
                return false;
            }
            return !StringHelper.IsNotSame(this.GetCleanedName(), other.GetCleanedName());
        }

        public PlayerRelationship GetRelationshipWith(Player other)
        {
            if (other == null)
            {
                return PlayerRelationship.OtherIsEmpty;
            }
            if (other.IsEmpty)
            {
                return PlayerRelationship.OtherIsEmpty;
            }
            var areTgNamesEqual = true;
            if (TgNickname != null)
            {
                areTgNamesEqual = TgNickname.Equals(other.TgNickname, StringComparison.InvariantCultureIgnoreCase);
            }
            if (!areTgNamesEqual)
            {
                return PlayerRelationship.Other;
            }
            var areFriendsSymbolsEqual = this.GetCleanedFriendsSymbols() == other.GetCleanedFriendsSymbols();
            if (!String.IsNullOrEmpty(this.TgNickname))
            {
                return areFriendsSymbolsEqual
                    ? PlayerRelationship.Same
                    : PlayerRelationship.Friend;
            }
            if (!areFriendsSymbolsEqual)
            {
                return PlayerRelationship.Friend;
            }
            return StringHelper.IsNotSame(this.GetCleanedName(), other.GetCleanedName())
                ? PlayerRelationship.Other
                : PlayerRelationship.Same;
        }
    }
}
