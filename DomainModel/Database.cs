using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DomainModel
{
    public class Database
    {
        private List<Category> _categories;

        public Database()
        {
            _categories = new List<Category>
            {
                new Category { cid = 1, name = "Beverages" },
                new Category { cid = 2, name = "Condiments" },
                new Category { cid = 3, name = "Confections" }
            };
        }

        public Category GetCategory(int id)
        {
            return _categories.Find(i => i.cid == id);
        }

        public Category AddCategory(string newName)
        {
            var newCategory = new Category { cid = GetNextID(), name = newName };
            _categories.Add(newCategory);
            return newCategory;
        }

        public int GetNextID()
        {
            /*List<int> ids = new List<int>();
            foreach (var cat in categories)
            {
                ids.Add(cat.cid);
            }
            return ids.Max() + 1;*/

            return _categories.Max(i => i.cid) + 1;

        }

        public bool DeleteCategory(int id)
        {
            var category = GetCategory(id);
            return _categories.Remove(category);
        }

        public Category UpdateCategory(Category category)
        {
            var cat = GetCategory(category.cid);
            cat.name = category.name;
            return cat;
        }

        public List<Category> GetAllCategories()
        {
            return _categories;
        }

        public bool CategoryExists(int id)
        {
            foreach (var cat in _categories)
            {
                if (cat.cid == id) return true;
            }
            return false;
        }


    }
}
