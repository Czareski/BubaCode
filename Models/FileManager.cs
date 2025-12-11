using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;

namespace BubaCode.Models;

public class FileManager
{
    private string _path;
    private FileStream _stream;
    
    public bool OpenFile(string path)
    {
        _path = path;
        try
        {
            _stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            return true;
        }
        catch (Exception err)
        {
            Debug.WriteLine(err);
            return false;
        }
    }

    public void ReadFile()
    {
        if (_stream == null)
        {
            throw new Exception("File isnt opened");
            return;
        }
    }
}