﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ITT.Shared.Application.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IQueryable<T>> FindAll();
        Task<T?> FindById(int Id);
        Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(T entity);
    }

}

public async Task<List<Language>> BindAllLanguages()
{
    return await GetObjectPost<List<Language>>(Endpoints.BindAllLanguages);
}

public async Task<List<TranslatedContent>> GetTranslations(GetTranslationsRequest getTranslationsRequest)
{
    return await PostObjectAsync<List<TranslatedContent>>(Endpoints.GetTranslations, getTranslationsRequest);
}


public async Task<List<EventCategory>> GetAllEventCategories(EventCategoriesRequest eventCategoriesRequest)
{
    return await PostObjectAsync<List<EventCategory>>(Endpoints.GetAllEventCategories, eventCategoriesRequest);
}

 public async Task<List<Event>> GetEvents(EventRequest eventRequest)
 {
     return await PostObjectAsync<List<Event>>(Endpoints.GetEvents, eventRequest);
 }

 ActionResult Get(int id);
IActionResult Manage();