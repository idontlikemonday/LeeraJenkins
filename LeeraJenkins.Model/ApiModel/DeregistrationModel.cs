namespace LeeraJenkins.Model.ApiModel
{
    public class DeregistrationModel
    {
        public string PlayerName { get; set; }
        public long SheetRowId { get; set; }
        public bool WithFullGameDeletion { get; set; }
        public string TrimmedGameName { get; set; }
    }
}
