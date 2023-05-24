namespace StrftimeParser
{
    internal class ElementContainer
    {
        public string AbbrWeekDay { get; set; }
        public string FullWeekDay { get; set; }
        public string DayOfTheMonthZeroPadded { get; set; }
        public string DayOfTheMonthSpacePadded { get; set; }
        public string MonthFull { get; set; }
        public string AbbreviatedMonth { get; set; }
        public string ShortMmDdYy { get; set; }
        public string ShortYyyyMmDd { get; set; }
        public string YearDividedBy100 { get; set; }
        public string Hour24 { get; set; }
        public string AmPm { get; set; }
        public string Hour12 { get; set; }
        public string DayOfYear { get; set; }
        public string Month { get; set; }
        public string Minute { get; set; }
        public string Second { get; set; }
        public string IsoTime { get; set; }
        public string IsoWeekDay { get; set; }
        public string WeekDaySundayBased { get; set; }
        public string YearTwoDigits { get; set; }
        public string Year { get; set; }
    }
}