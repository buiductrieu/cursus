﻿using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
	public interface ICourseRepository : IRepository<Course>
    {
		Task<bool> AnyAsync(Expression<Func<Course, bool>> predicate); 
		Task<Course> GetAllIncludeStepsAsync(int courseId);
	}
}