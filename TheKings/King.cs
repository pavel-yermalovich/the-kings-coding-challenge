using System;

namespace TheKings
{
    public class King
    {
        public King() { }

        public King(ApiKing apiKingApi)
        {
            Id = int.Parse(apiKingApi.id);
            Name = apiKingApi.nm;
            Country = apiKingApi.cty;
            House = apiKingApi.hse;
            Ruled = YearRange.Parse(apiKingApi.yrs);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string House { get; set; }

        public YearRange Ruled { get; set; }

        public string FirstName => 
            Name.IsNotNullOrEmpty() && Name.Contains(" ") 
                ? Name.Substring(0, Name.IndexOf(" ", StringComparison.Ordinal)) 
                : Name;
    }
}
