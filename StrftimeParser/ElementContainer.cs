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
    }
}