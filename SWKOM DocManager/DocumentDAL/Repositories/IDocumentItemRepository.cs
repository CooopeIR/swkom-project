﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDAL.Entities;

namespace DocumentDAL.Repositories
{
    public interface IDocumentItemRepository
    {
        Task<IEnumerable<DocumentItem>> GetAllAsync();
        Task<DocumentItem> GetByIdAsync(Guid id);
        Task AddAsync(DocumentItem item);
        Task UpdateAsync(DocumentItem item);
        Task DeleteAsync(Guid id);
    }
}