using AuctionBayWebApi.Controllers;
using AuctionBayWebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http.Dependencies;

namespace AuctionBayWebApi.Resolvers
{
    public class DbDependencyResolver : IDependencyResolver
    {
        private DbContext context;
        private DbUsersRepository usersRepository;
        private DbProductsRepository productsRepository;
        private DbOffersRepository offersRepository;
        private DropboxUploader uploader;

        public DbDependencyResolver(DbContext context)
        {
            this.context = context;
            this.usersRepository = new DbUsersRepository(context);
            this.offersRepository = new DbOffersRepository(context);
            this.productsRepository = new DbProductsRepository(context);
            this.uploader = new DropboxUploader();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(UsersController))
            {
                return new UsersController(this.usersRepository, this.uploader);
            }
            if (serviceType == typeof(ProductsController))
            {
                return new ProductsController(this.usersRepository, this.productsRepository, this.uploader, this.offersRepository);
            }
            if (serviceType == typeof(OffersController))
            {
                return new OffersController(this.offersRepository, this.usersRepository, this.productsRepository);
            }
            
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose() { }
    }
}