using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingApp.Base.Domain;

namespace VotingApp.Tests.Base.Domain
{
    public class PaginationTests
    {
        [Fact]
        public void NewPagination_PropertiesEqualsOrLessThanZero_SetsDefaultValue()
        {
            var pagination = new Pagination()
            {
                CurrentPage = 0,
                PageSize = 0
            };

            Assert.Equal(1, pagination.CurrentPage);
            Assert.Equal(10, pagination.PageSize);
        }
    }
}
