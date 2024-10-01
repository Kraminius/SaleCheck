using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SaleCheck.DataAccess.Interfaces;
using SaleCheck.Model.DataClasses;
using SaleCheck.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SaleCheck.Repositories
{
    public class WebsiteRepository
    {
        private readonly IMongoCollection<Website> _websites;
        private readonly ILogger<WebsiteRepository> _logger;


        public WebsiteRepository(IMongoDbContext context, ILogger<WebsiteRepository> logger)
        {
            _websites = context.Websites;
            _logger = logger;
        }

        #region Website Operations

        // TODO: Implement




        #endregion Website Operations












    }
}
