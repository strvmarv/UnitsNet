﻿// Licensed under MIT No Attribution, see LICENSE file at the root.
// Copyright 2013 Andreas Gullberg Larsen (andreas.larsen84@gmail.com). Maintained at https://github.com/angularsen/UnitsNet.

using UnitsNet.Tests.CustomQuantities;
using Xunit;

namespace UnitsNet.Tests
{
    public class QuantityParserTests
    {
        [Fact]
        public void Parse_WithSingleCaseInsensitiveMatch_ParsesWithMatchedUnit()
        {
            var unitAbbreviationsCache = new UnitAbbreviationsCache();
            unitAbbreviationsCache.MapUnitToAbbreviation(HowMuchUnit.Some, "foo");
            var quantityParser = new QuantityParser(unitAbbreviationsCache);

            HowMuch q = quantityParser.Parse<HowMuch, HowMuchUnit>("1 FOO",
                null,
                (value, unit) => new HowMuch((double) value, unit));

            Assert.Equal(HowMuchUnit.Some, q.Unit);
            Assert.Equal(1, q.Value);
        }

        [Fact]
        public void Parse_WithOneCaseInsensitiveMatchAndOneExactMatch_ParsesWithTheExactMatchUnit()
        {
            var unitAbbreviationsCache = new UnitAbbreviationsCache();
            unitAbbreviationsCache.MapUnitToAbbreviation(HowMuchUnit.Some, "foo");
            unitAbbreviationsCache.MapUnitToAbbreviation(HowMuchUnit.ATon, "FOO");
            var quantityParser = new QuantityParser(unitAbbreviationsCache);

            HowMuch q = quantityParser.Parse<HowMuch, HowMuchUnit>("1 FOO",
                null,
                (value, unit) => new HowMuch((double) value, unit));

            Assert.Equal(HowMuchUnit.ATon, q.Unit);
            Assert.Equal(1, q.Value);
        }

        [Fact]
        public void Parse_WithMultipleCaseInsensitiveMatchesButNoExactMatches_ThrowsAmbiguousUnitParseException()
        {
            var unitAbbreviationsCache = new UnitAbbreviationsCache();
            unitAbbreviationsCache.MapUnitToAbbreviation(HowMuchUnit.Some, "foo");
            unitAbbreviationsCache.MapUnitToAbbreviation(HowMuchUnit.ATon, "FOO");
            var quantityParser = new QuantityParser(unitAbbreviationsCache);

            void Act()
            {
                quantityParser.Parse<HowMuch, HowMuchUnit>("1 Foo", null, (value, unit) => new HowMuch((double) value, unit));
            }

            var ex = Assert.Throws<AmbiguousUnitParseException>(Act);
            Assert.Equal("Cannot parse \"Foo\" since it matched multiple units [Some, ATon] with case-insensitive comparison, but zero units with case-sensitive comparison. To resolve the ambiguity, pass a unit abbreviation with the correct casing.", ex.Message);
        }

        [Fact]
        public void Parse_MappedCustomUnit()
        {
            var unitAbbreviationsCache = new UnitAbbreviationsCache();
            unitAbbreviationsCache.MapUnitToAbbreviation(HowMuchUnit.Some, "fooh");
            var quantityParser = new QuantityParser(unitAbbreviationsCache);

            HowMuch q = quantityParser.Parse<HowMuch, HowMuchUnit>("1 fooh",
                null,
                (value, unit) => new HowMuch((double) value, unit));

            Assert.Equal(HowMuchUnit.Some, q.Unit);
            Assert.Equal(1, q.Value);
        }

        [Fact]
        public void TryParse_MappedCustomUnit()
        {
            var unitAbbreviationsCache = new UnitAbbreviationsCache();
            unitAbbreviationsCache.MapUnitToAbbreviation(HowMuchUnit.Some, "fooh");
            var quantityParser = new QuantityParser(unitAbbreviationsCache);

            bool success = quantityParser.TryParse<HowMuch, HowMuchUnit>("1 fooh",
                null,
                (value, unit) => new HowMuch((double) value, unit),
                out HowMuch q);

            Assert.True(success);
            Assert.Equal(HowMuchUnit.Some, q.Unit);
            Assert.Equal(1, q.Value);
        }

    }
}
