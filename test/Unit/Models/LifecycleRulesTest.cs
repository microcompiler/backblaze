using System;

using Bytewizer.Backblaze.Models;

using Xunit;

namespace Backblaze.Tests.Unit
{
    public class LifecycleRulesTest
    {
        [Fact]
        public void LifecycleRuleEquatable()
        {
            var rule1 = new LifecycleRule()
            {
                DaysFromHidingToDeleting = 6,
                DaysFromUploadingToHiding = 5,
                FileNamePrefix = "backup/",
            };
            var rule2 = new LifecycleRule()
            {
                DaysFromHidingToDeleting = 6,
                DaysFromUploadingToHiding = 5,
                FileNamePrefix = "backup/",
            };

            Assert.True(rule1.Equals(rule2));
            Assert.Equal(rule1.GetHashCode(), rule2.GetHashCode());
            Assert.True(rule1 == rule2);
            Assert.False(rule1 != rule2);

            var rule3 = new LifecycleRules()
                {   new LifecycleRule()
                    {
                        DaysFromHidingToDeleting = 6,
                        DaysFromUploadingToHiding = 5,
                        FileNamePrefix = "backup/",
                    },
                    new LifecycleRule()
                    {
                        DaysFromHidingToDeleting = 45,
                        DaysFromUploadingToHiding = 7,
                        FileNamePrefix = "files/",
                    },
                };

            var rule4 = new LifecycleRules()
                {   new LifecycleRule()
                    {
                        DaysFromHidingToDeleting = 6,
                        DaysFromUploadingToHiding = 5,
                        FileNamePrefix = "backup/",
                    },
                    new LifecycleRule()
                    {
                        DaysFromHidingToDeleting = 45,
                        DaysFromUploadingToHiding = 7,
                        FileNamePrefix = "files/",
                    },
                };

            Assert.True(rule3.Equals(rule4));
            Assert.Equal(rule3.GetHashCode(), rule4.GetHashCode());
            Assert.True(rule3 == rule4);
            Assert.False(rule3 != rule4);
        }

        [Fact]
        public void MaximumRulesAllowed()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {

                var list = new LifecycleRules();

                for (int i = 0; i < LifecycleRules.MaximumRulesAllowed + 1; i++)
                {
                    list.Add(new LifecycleRule()
                    {
                        DaysFromHidingToDeleting = 45,
                        DaysFromUploadingToHiding = 7
                    });
                }
            });
        }
    }
}
