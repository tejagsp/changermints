using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangerMints.Database
{
    interface IUnitOfWork
    {
        void Dispose();
        void Dispose(bool disposing);

        IRepository<T> Repository<T>() where T : class;

        void Save();
    }
}
