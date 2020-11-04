using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeraJenkins.Logic.Helpers
{
    public static class WordFormHelper
    {
        public static string GetFullPluralPhrase(int count, string resourceWordForms, string valueSubstring = null)
        {
            string[] wordForms = GetWordForms(resourceWordForms);
            return GetFullPluralPhrase(count, wordForms.Select(w => w.Trim()).ToList(), valueSubstring);
        }

        public static string GetFullCasePhrase(int caseNumber, string resourceWordForms)
        {
            string[] wordForms = GetWordForms(resourceWordForms);
            return GetWordCaseForm(caseNumber, wordForms.Select(w => w.Trim()).ToList());
        }

        private static string[] GetWordForms(string resourceWordForms)
        {
            return resourceWordForms.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string GetFullPluralPhrase(int count, IList<string> wordForms, string valueSubstring = null)
        {
            var formNumber = GetFormNumber(count);
            return String.Format("{0} {1}", valueSubstring ?? count.ToString(), wordForms[formNumber - 1]);
        }

        private static string GetWordCaseForm(int caseNumber, IList<string> wordForms)
        {
            return wordForms[caseNumber - 1];
        }

        private static int GetFormNumber(int count)
        {
            if (count >= 11 && count <= 20)
            {
                return 3;
            }
            int reminder = count % 10;
            return reminder == 1 ? 1
                : reminder >= 2 && reminder <= 4 ? 2
                : 3;
        }
    }
}
