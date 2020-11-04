namespace LeeraJenkins.Model.Notifications
{
    public class NotificationPlayersModel : NotificationBaseModel
    {
        public string GameName { get; set; }
        public string Host { get; set; }

        public string Range { get; set; }
        public int ColumnIndex { get; set; }
        public int ColumnLastIndex { get; set; }
        public int RowLastIndex { get; set; }
        public double ColumnIndexD { get; set; }
        public double ColumnLastIndexD { get; set; }
        public double RowIndexD { get; set; }
        public double RowLastIndexD { get; set; }

        public void InitIntValues()
        {
            ColumnIndex = (int)ColumnIndexD;
            ColumnLastIndex = (int)ColumnLastIndexD;
            RowIndex = (int)RowIndexD;
            RowLastIndex = (int)RowLastIndexD;
        }
    }
}