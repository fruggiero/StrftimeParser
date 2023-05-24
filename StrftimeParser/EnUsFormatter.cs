using System;
using System.Globalization;

namespace StrftimeParser
{
	internal class EnUsFormatter : Formatter
	{
		private CultureInfo _culture = CultureInfo.GetCultureInfo("en-us");
		
    	public override string ConsumeAbbreviatedDayOfWeek(ref string input, ref int inputIndex)
    	{
    		var abbrWeekDay = input.Substring(inputIndex, 3);
    		inputIndex += 3;
    		return abbrWeekDay;
    	}
        
        public override string ConsumeAbbreviatedMonth(ref string input, ref int inputIndex)
        {
	        var month = input.Substring(inputIndex, 3);
	        inputIndex += 3;
	        return month;
        }

        public override int ParseMonthFull(string input)
        {
	        switch (input.ToLower())
	        {
		        case "january":
			        return 1;
		        case "february":
			        return 2;
		        case "march":
			        return 3;
		        case "april":
			        return 4;
		        case "may":
			        return 5;
		        case "june":
			        return 6;
		        case "july":
			        return 7;
		        case "august":
			        return 8;
		        case "september":
			        return 9;
		        case "october":
			        return 10;
		        case "november":
			        return 11;
		        case "december":
			        return 12;
	        }

	        throw new FormatException("Unrecognized month name for this locale");
        }

        public override int ParseMonthAbbreviated(string input)
        {
	        return input.ToLower() switch
	        {
		        "jan" => 1,
		        "feb" => 2,
		        "mar" => 3,
		        "apr" => 4,
		        "may" => 5,
		        "jun" => 6,
		        "jul" => 7,
		        "aug" => 8,
		        "sep" => 9,
		        "oct" => 10,
		        "nov" => 11,
		        "dec" => 12,
		        _ => throw new FormatException("Unrecognized month abbreviated for this locale")
	        };
        }
        
        public override string ConsumeFullMonth(ref string input, ref int inputIndex)
        {
	        switch (input[inputIndex])
	        {
		        case 'j':
		        case 'J':
			        switch (input[inputIndex + 1])
			        {
				        case 'a':
				        case 'A':
				        {
					        // Parse January
					        var fullMonth = input.Substring(inputIndex, "January".Length);
					        inputIndex += "January".Length;
					        return fullMonth;
				        }
				        case 'u':
				        case 'U':
				        {
					        switch (input[inputIndex + 2])
					        {
						        case 'l':
						        case 'L':
						        {
							        // Parse July
							        var fullMonth = input.Substring(inputIndex, "July".Length);
							        inputIndex += "July".Length;
							        return fullMonth;
						        }
						        case 'n':
						        case 'N':
						        {
							        // Parse June
							        var fullMonth = input.Substring(inputIndex, "June".Length);
							        inputIndex += "June".Length;
							        return fullMonth;
						        }
					        }

					        break;
				        }
			        }

			        break;
		        case 'f':
		        case 'F':
		        {
			        // Parse February
			        var fullMonth = input.Substring(inputIndex, "February".Length);
			        inputIndex += "January".Length;
			        return fullMonth;
		        }
		        case 'm':
		        case 'M':
		        {
			        switch (input[inputIndex + 2])
			        {
				        case 'r':
				        case 'R':
				        {
					        // Parse March:
					        var fullMonth = input.Substring(inputIndex, "March".Length);
					        inputIndex += "March".Length;
					        return fullMonth;
				        }
				        case 'Y':
				        case 'y':
				        {
					        // Parse May:
					        var fullMonth = input.Substring(inputIndex, "May".Length);
					        inputIndex += "May".Length;
					        return fullMonth;
				        }
			        }
			        break;
		        }
		        case 'A':
		        case 'a':
			        switch (input[inputIndex + 1])
			        {
				        case 'p':
				        case 'P':
				        {
					        // Parse April:
					        var fullMonth = input.Substring(inputIndex, "April".Length);
					        inputIndex += "April".Length;
					        return fullMonth;
				        }
				        case 'u':
				        case 'U':
				        {
					        // Parse August:
					        var fullMonth = input.Substring(inputIndex, "August".Length);
					        inputIndex += "August".Length;
					        return fullMonth;
				        }
			        }
			        break;
		        case 'S':
		        case 's':
		        {
			        // Parse September:
			        var fullMonth = input.Substring(inputIndex, "September".Length);
			        inputIndex += "September".Length;
			        return fullMonth;
		        }
		        case 'O':
		        case 'o':
		        {
			        // Parse October:
			        var fullMonth = input.Substring(inputIndex, "October".Length);
			        inputIndex += "October".Length;
			        return fullMonth;
		        }
		        case 'N':
		        case 'n':
		        {
			        // Parse November:
			        var fullMonth = input.Substring(inputIndex, "November".Length);
			        inputIndex += "November".Length;
			        return fullMonth;
		        }
		        case 'D':
		        case 'd':
		        {
			        // Parse December:
			        var fullMonth = input.Substring(inputIndex, "December".Length);
			        inputIndex += "December".Length;
			        return fullMonth;
		        }
	        }

	        throw new FormatException("Unrecognized full month format for this locale");
        }

        public override DayOfWeek ParseDayOfWeekAbbreviated(string input)
        {
	        return input.ToLower() switch
	        {
		        "mon" => DayOfWeek.Monday,
		        "tue" => DayOfWeek.Tuesday,
		        "wed" => DayOfWeek.Wednesday,
		        "thu" => DayOfWeek.Thursday,
		        "fri" => DayOfWeek.Friday,
		        "sat" => DayOfWeek.Saturday,
		        "sun" => DayOfWeek.Sunday,
		        _ => throw new ArgumentException()
	        };
        }

        public override DayOfWeek ParseDayOfWeekFull(string input)
        {
	        return input.ToLower() switch
	        {
		        "monday" => DayOfWeek.Monday,
		        "tuesday" => DayOfWeek.Tuesday,
		        "wednesday" => DayOfWeek.Wednesday,
		        "thursday" => DayOfWeek.Thursday,
		        "friday" => DayOfWeek.Friday,
		        "saturday" => DayOfWeek.Saturday,
		        "sunday" => DayOfWeek.Sunday,
		        _ => throw new ArgumentException()
	        };
        }

        public override string ConsumeDayOfWeek(ref string input, ref int inputIndex)
    	{
    		string fullWeekDay;
    
    		if (input[inputIndex] == 'm' || input[inputIndex] == 'M')
    		{
    			// Parse Wednesday
    			fullWeekDay = input.Substring(inputIndex, "Monday".Length);
    			inputIndex += "Monday".Length;
    		}
    		else if (input[inputIndex] == 't' || input[inputIndex] == 'T')
    		{
    			if (input[inputIndex + 1] == 'u' || input[inputIndex + 1] == 'U')
    			{
    				fullWeekDay = input.Substring(inputIndex, "Tuesday".Length);
    				inputIndex += "Tuesday".Length;
    			}
    			else if (input[inputIndex + 1] == 'h' || input[inputIndex + 1] == 'H')
    			{
    				fullWeekDay = input.Substring(inputIndex, "Thursday".Length);
    				inputIndex += "Thursday".Length;
    			}
    			else
                {
	                throw new FormatException("Unrecognized day of week format for this locale");
                }
    		}
    		else if (input[inputIndex] == 'w' || input[inputIndex] == 'W')
    		{
    			fullWeekDay = input.Substring(inputIndex, "Wednesday".Length);
    			inputIndex += "Wednesday".Length;
    		}
    		else if (input[inputIndex] == 'f' || input[inputIndex] == 'F')
    		{
    			fullWeekDay = input.Substring(inputIndex, "Friday".Length);
    			inputIndex += "Friday".Length;
    		}
    		else if (input[inputIndex] == 's' || input[inputIndex] == 'S')
    		{
    			if (input[inputIndex + 1] == 'a' || input[inputIndex + 1] == 'A')
    			{
    				fullWeekDay = input.Substring(inputIndex, "Saturday".Length);
    				inputIndex += "Saturday".Length;
    			}
    			else if (input[inputIndex + 1] == 'u' || input[inputIndex + 1] == 'U')
    			{
    				fullWeekDay = input.Substring(inputIndex, "Sunday".Length);
    				inputIndex += "Sunday".Length;
    			}
    			else
    			{
	                throw new FormatException("Unrecognized day of week format for this locale");
    			}
    		}
    		else
    		{
	            throw new FormatException("Unrecognized day of week format for this locale");
    		}
    
    		return fullWeekDay;
    	}
    }
}