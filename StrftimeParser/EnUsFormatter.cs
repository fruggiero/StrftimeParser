using System;

namespace StrftimeParser
{
	internal class EnUsFormatter : Formatter
    {
    	public override string ConsumeAbbreviatedDayOfWeek(ref string input, ref int inputIndex)
    	{
    		var abbrWeekDay = input.Substring(inputIndex, 3);
    		inputIndex += 3;
    		return abbrWeekDay;
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