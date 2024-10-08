﻿using Microsoft.EntityFrameworkCore;
using DocumentDAL.Entities;
using DocumentDAL.Data;

namespace DocumentDAL.Repositories
{
    public class DocumentItemRepository(DocumentContext context) : IDocumentItemRepository
    {

        public async Task<IEnumerable<DocumentItem>> GetAllAsync()
        {
            return await context.DocumentItems.ToListAsync();
        }

        public async Task<DocumentItem> GetByIdAsync(Guid id)
        {
            return await context.DocumentItems.FindAsync(id);
        }

        public async Task AddAsync(DocumentItem item)
        {
            await context.DocumentItems.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DocumentItem item)
        {
            context.DocumentItems.Update(item);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await context.DocumentItems.FindAsync(id);
            if (item != null)
            {
                context.DocumentItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
