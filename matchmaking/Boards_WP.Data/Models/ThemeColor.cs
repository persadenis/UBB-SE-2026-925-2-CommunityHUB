using System;

namespace Boards_WP.Data.Models
{
    public enum ThemeColor
    {
        Default,
        Pink,
        Orange,
        Turquoise,
        Yellow,
        Blue,
        Green,
        Red,
        Purple
    }

    public class CategoryThemeMapper
    {

        public static ThemeColor GetColorForCategoryId(int categoryId)
        {
            return categoryId switch
            {
                // Arts & Design, Music & Audio, Literature & Writing
                1 or 2 or 3 => ThemeColor.Pink,

                // Internet Culture & Humor, Food & Gastronomy, Gaming
                4 or 5 or 6 => ThemeColor.Orange,

                // Physical Sciences & Space, Technology & Computing, Engineering & Systems
                7 or 8 or 9 => ThemeColor.Turquoise,

                // Animals & Wildlife, Environment & Ecology, Travel & Geography
                10 or 11 or 12 => ThemeColor.Yellow,

                // Politics & Policy, Law & Justice, Economics & Business
                13 or 14 or 15 => ThemeColor.Blue,

                // Philosophy & Spirituality, Health & Wellness, Sports & Athletics
                16 or 17 or 18 => ThemeColor.Green,

                // Social & Relationships, Lifestyle & Home, Fashion & Grooming
                19 or 20 or 21 => ThemeColor.Red,

                // Education, History & Culture, Hobbies & Leisure
                22 or 23 or 24 => ThemeColor.Purple,

                _ => ThemeColor.Default 
            };
        }
    }
}

