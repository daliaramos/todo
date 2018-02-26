using System.Collections.Generic;
using System;
using ToDoList;
using MySql.Data.MySqlClient;

namespace ToDoList.Models
{
  public class Item
  {
    private int _id;
    private string _description;
    private DateTime _dueDate;

    public Item(string Description, DateTime DueDate, int Id = 0)
    {
      _id = Id;
      _description = Description;
      _dueDate = DueDate;
    }

    public override bool Equals(System.Object otherItem)
    {
      if (!(otherItem is Item))
      {
        return false;
      }
      else
      {
        Item newItem = (Item) otherItem;
        bool idEquality = (this.GetId() == newItem.GetId());
        bool descriptionEquality = (this.GetDescription() == newItem.GetDescription());
        bool dueDateEquality = (this.GetDueDate() == newItem.GetDueDate());
        return (idEquality && descriptionEquality && dueDateEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetDescription().GetHashCode();
    }

    public int GetId()
    {
      return _id;
    }
    public string GetDescription()
    {
      return _description;
    }
    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }
    public DateTime GetDueDate()
    {
      return _dueDate;
    }
    public void SetDueDate(DateTime newDueDate)
    {
      _dueDate = newDueDate;
    }

    public static List<Item> GetAll()
    {
      List<Item> allItems = new List<Item> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM items;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        string itemDescription = rdr.GetString(1);
        DateTime itemDueDate = rdr.GetDateTime(2);
        Item newItem = new Item(itemDescription, itemDueDate, itemId);
        allItems.Add(newItem);
      }
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
      return allItems;
    }

    public static void DeleteAll()
    {
    MySqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"DELETE FROM items;";

     cmd.ExecuteNonQuery();

     conn.Close();
     if (conn != null)
     {
      conn.Dispose();
     }
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = conn.CreateCommand() as MySqlCommand;
    //  cmd.CommandText = @"INSERT INTO `items` (`description`) VALUES (@ItemDescription);";
    // names in parentheses must match column names in database exactly
    cmd.CommandText = @"INSERT INTO `items` (description, duedate) VALUES (@ItemDescription, @ItemDueDate);";

     MySqlParameter description = new MySqlParameter();
     description.ParameterName = "@ItemDescription";
     description.Value = this._description;
     cmd.Parameters.Add(description);

     MySqlParameter dueDate = new MySqlParameter();
     dueDate.ParameterName = "@ItemDueDate";
     dueDate.Value = this._dueDate;
     cmd.Parameters.Add(dueDate);

     cmd.ExecuteNonQuery();
     _id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static Item Find(int id)
    {
     MySqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"SELECT * FROM `items` WHERE id = @thisId;";

     MySqlParameter thisId = new MySqlParameter();
     thisId.ParameterName = "@thisId";
     thisId.Value = id;
     cmd.Parameters.Add(thisId);

     var rdr = cmd.ExecuteReader() as MySqlDataReader;

     int itemId = 0;
     string itemDescription = "";
     DateTime itemDueDate = DateTime.Now;

     while (rdr.Read())
     {
       itemId = rdr.GetInt32(0);
       itemDescription = rdr.GetString(1);
       itemDueDate = rdr.GetDateTime(2);
     }

     Item foundItem= new Item(itemDescription, itemDueDate, itemId);

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

     return foundItem;
    }

    public static List<Item> SortByDueDate()
    {
      List<Item> sortedItems = new List<Item>{};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM items ORDER BY (duedate) ASC;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while (rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        string itemDescription = rdr.GetString(1);
        DateTime itemDueDate = rdr.GetDateTime(2);

        Item newItem = new Item(itemDescription, itemDueDate, itemId);

        sortedItems.Add(newItem);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return sortedItems;
    }

    public static List<Item> SortById()
    {
      List<Item> sortedItems = new List<Item>{};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM items ORDER BY (id) ASC;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while (rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        string itemDescription = rdr.GetString(1);
        DateTime itemDueDate = rdr.GetDateTime(2);

        Item newItem = new Item(itemDescription, itemDueDate, itemId);

        sortedItems.Add(newItem);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return sortedItems;
    }

    public void Edit (string newDescription, DateTime newDuedate)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE items SET description = @newDescription, duedate = @newDuedate WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter description = new MySqlParameter();
      description.ParameterName = "@newDescription";
      description.Value = newDescription;
      cmd.Parameters.Add(description);

      MySqlParameter duedate = new MySqlParameter();
      duedate.ParameterName = "@newDuedate";
      duedate.Value = newDuedate;
      cmd.Parameters.Add(duedate);

      cmd.ExecuteNonQuery();
      _description = newDescription;
      _dueDate = newDuedate;

      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }

    public void Delete ()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM items WHERE id = @thisId;";

      MySqlParameter thisId = new MySqlParameter();
      thisId.ParameterName = "@thisId";
      thisId.Value = this._id;
      cmd.Parameters.Add(thisId);

      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }

    public void AddCategory(Category newCategory)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO categories_items (category_id, item_id) VALUES (@CategoryId, @ItemId);";

      MySqlParameter category_id = new MySqlParameter();
      category_id.ParameterName = "@CategoryId";
      category_id.Value = newCategory.GetId();
      cmd.Parameters.Add(category_id);

      MySqlParameter item_id = new MySqlParameter();
      item_id.ParameterName = "@ItemId";
      item_id.Value = _id;
      cmd.Parameters.Add(item_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }

    public List<Category> GetCategories()
   {
     MySqlConnection conn = DB.Connection();
     conn.Open();
     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"SELECT category_id FROM categories_items WHERE item_id = @itemId;";

     MySqlParameter itemIdParameter = new MySqlParameter();
     itemIdParameter.ParameterName = "@itemId";
     itemIdParameter.Value = _id;
     cmd.Parameters.Add(itemIdParameter);

     var rdr = cmd.ExecuteReader() as MySqlDataReader;

     List<int> categoryIds = new List<int> {};
     while(rdr.Read())
     {
         int categoryId = rdr.GetInt32(0);
         categoryIds.Add(categoryId);
     }
     rdr.Dispose();

     List<Category> categories = new List<Category> {};
     foreach (int categoryId in categoryIds)
     {
         var categoryQuery = conn.CreateCommand() as MySqlCommand;
         categoryQuery.CommandText = @"SELECT * FROM categories WHERE id = @CategoryId;";

         MySqlParameter categoryIdParameter = new MySqlParameter();
         categoryIdParameter.ParameterName = "@CategoryId";
         categoryIdParameter.Value = categoryId;
         categoryQuery.Parameters.Add(categoryIdParameter);

         var categoryQueryRdr = categoryQuery.ExecuteReader() as MySqlDataReader;
         while(categoryQueryRdr.Read())
         {
             int thisCategoryId = categoryQueryRdr.GetInt32(0);
             string categoryName = categoryQueryRdr.GetString(1);
             Category foundCategory = new Category(categoryName, thisCategoryId);
             categories.Add(foundCategory);
         }
         categoryQueryRdr.Dispose();
     }
     conn.Close();
     if (conn != null)
     {
         conn.Dispose();
     }
     return categories;
   }

  }
}
