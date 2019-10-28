using System;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

using Xunit;

namespace Backblaze.Tests.Unit
{
    public class CorsRulesTest
    {
        [Fact]
        public void CorsRuleEquatable()
        {
            var rule1 = new CorsRule(
                     "downloadFromAnyOrigin",
                     new List<string> { "https" },
                     new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                     3600)
            {
                AllowedHeaders = new List<string> { "range" },
                ExposeHeaders = new List<string> { "x-bz-content-sha1" },
            };

            var rule2 = new CorsRule(
                 "downloadFromAnyOrigin",
                 new List<string> { "https" },
                 new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                 3600)
            {
                AllowedHeaders = new List<string> { "range" },
                ExposeHeaders = new List<string> { "x-bz-content-sha1" },
            };

            Assert.True(rule1.Equals(rule2));
            Assert.Equal(rule1.GetHashCode(), rule2.GetHashCode());
            Assert.True(rule1 == rule2);
            Assert.False(rule1 != rule2);

            var rule3 = new CorsRules
                { new CorsRule(
                     "downloadFromAnyOrigin",
                     new List<string> { "https" },
                     new List<string> { "b2_download_file_by_id" , "b2_download_file_by_name" },
                     3600 )
                    {
                        AllowedHeaders = new List<string> { "range" },
                        ExposeHeaders = new List<string> {"x-bz-content-sha1" },
                    }
                };

            var rule4 = new CorsRules
                { new CorsRule(
                     "downloadFromAnyOrigin",
                     new List<string> { "https" },
                     new List<string> { "b2_download_file_by_id" , "b2_download_file_by_name" },
                     3601 )
                    {
                        AllowedHeaders = new List<string> { "range" },
                        ExposeHeaders = new List<string> {"x-bz-content-sha1" },
                    }
                };

            Assert.False(rule3.Equals(rule4));
            Assert.NotEqual(rule3.GetHashCode(), rule4.GetHashCode());
        }

        [Fact]
        public void MaximumRulesAllowed()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var list = new CorsRules();

                for (int i = 0; i < CorsRules.MaximumRulesAllowed + 1; i++)
                {
                    list.Add(new CorsRule(
                         Guid.NewGuid().ToString(),
                         new List<string> { "https" },
                         new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                         3600)
                    {
                        AllowedHeaders = new List<string> { "range" },
                        ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                    }
                    );
                }
            });
        }

        [Fact]
        public void UniqueRuleName()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var list = new CorsRules();

                for (int i = 0; i < 2 + 1; i++)
                {
                    list.Add(new CorsRule(
                         "123456",
                         new List<string> { "https" },
                         new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                         3600)
                    {
                        AllowedHeaders = new List<string> { "range" },
                        ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                    }
                    );
                }
            });
        }

        [Fact]
        public void RuleNameLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var list = new CorsRules();
                list.Add(new CorsRule(
                        "12345",
                        new List<string> { "https" },
                        new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                        3600)
                {
                    AllowedHeaders = new List<string> { "range" },
                    ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                }
                );
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var list = new CorsRules();
                list.Add(new CorsRule(
                        "012345678901234567890123456789012345678901234567890",
                        new List<string> { "https" },
                        new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                        3600)
                {
                    AllowedHeaders = new List<string> { "range" },
                    ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                }
                );
            });
        }

        [Fact]
        public void RuleNameInvalidChar()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var list = new CorsRules();
                list.Add(new CorsRule(
                        "12345$",
                        new List<string> { "https" },
                        new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                        3600)
                {
                    AllowedHeaders = new List<string> { "range" },
                    ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                }
                );
            });
        }

        [Fact]
        public void RuleNameReservedUse()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var list = new CorsRules();
                list.Add(new CorsRule(
                        "b2-123456",
                        new List<string> { "https" },
                        new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                        3600)
                {
                    AllowedHeaders = new List<string> { "range" },
                    ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                }
                );
            });
        }

        [Fact]
        public void MaxAgeLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var list = new CorsRules();
                list.Add(new CorsRule(
                        "123456",
                        new List<string> { "https" },
                        new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                        CorsRule.MinimumAgeSeconds - 1)
                {
                    AllowedHeaders = new List<string> { "range" },
                    ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                }
                );
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var list = new CorsRules();
                list.Add(new CorsRule(
                        "123456",
                        new List<string> { "https" },
                        new List<string> { "b2_download_file_by_id", "b2_download_file_by_name" },
                        CorsRule.MaximumAgeSeconds + 1)
                {
                    AllowedHeaders = new List<string> { "range" },
                    ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                }
                );
            });
        }
    }
}
