using BookReader.Core.Abstract.Repositories;
using BookReader.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Repositories
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context) 
        {  
        }
    }
}
