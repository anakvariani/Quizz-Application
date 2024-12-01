using models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class UserRepository
{
    private readonly string _filePath; 
    private List<User> _users; 

    public UserRepository(string filePath)
    {
        _filePath = filePath;
        _users = LoadUsers(); 
    }

    public User Register(string username, string password)
    {
        if (_users.Any(u => u.Username == username))
            throw new Exception("Username already exists.");

        var user = new User
        {
            Username = username,
            Password = password
        };

        _users.Add(user);
        SaveUsers(); 
        return user;
    }

    public User Login(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user == null)
            throw new Exception("Invalid username or password.");

        return user;
    }

    public void SaveUsers()
    {
        var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }


    public List<User> LoadUsers()
    {
        if (!File.Exists(_filePath))
            return new List<User>();

        var json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<User>();  //////////////////////

        var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

        foreach (var user in users)
        {
            user.QuizRecords ??= new List<QuizRecord>();
        }

        return users;
    }


    public List<User> GetAllUsers()
    {
        return _users.OrderByDescending(u => u.TotalScore).ToList();
    }
}
