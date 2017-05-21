using System;
using System.Collections;
using System.Data.Entity;

namespace ChangerMints.Database {

    public class UnitOfWork : IUnitOfWork {
        private readonly DbContext _context;

        private bool _disposed;
        private Hashtable _repositories;

        public UnitOfWork(DbContext context) {
            if (context == null) {
                _context = context;
            }
        }

        public UnitOfWork() {
            _context = new ChangerMintsEntities();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing) {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();

            _disposed = true;
        }

        public IRepository<T> Repository<T>() where T : class {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type)) {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                            .MakeGenericType(typeof(T)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }

        public void Save() {
            _context.SaveChanges();
        }
    }
}