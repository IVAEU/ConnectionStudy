using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;

public enum PacketType
{
    Welcome = 1,
    Default,
    ColorChange,
    MovePosition
}

public class Packet
{
    private List<byte> _buffer;
    private int _readPos = 0;
    
    public Packet()
    {
        _buffer = new List<byte>();
        _readPos = 0;
    }
    
    public Packet(int id)
    {
        _buffer = new List<byte>();
        Write(id);
        _readPos = 0;
    }
    
    public Packet(byte[] data)
    {
        _buffer = data.ToList();
        _readPos = 0;
    }
    
    public int Length()
    {
        return _buffer.Count;
    }

    public int UnreadLength()
    {
        return Length() - _readPos;
    }
    
    public byte[] UnreadBytes()
    {
        return _buffer.GetRange(_readPos, UnreadLength()).ToArray();
    }

    public byte[] ToArray()
    {
        return _buffer.ToArray();
    }
    
    public void Reset()
    {
        _buffer.Clear();
        _readPos = 0;
    }
    
    public void SetBytes(byte[] value)
    {
        Write(value);
    }

    public void Insert(int value)
    {
        _buffer.InsertRange(0, BitConverter.GetBytes(value));
    }
    
    public void Write(byte value)
    {
        _buffer.Add(value);
    }
    
    public void Write(byte[] value)
    {
        _buffer.AddRange(value);
    }
    
    public void Write(int value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value)); 
    }
    
    public void Write(float value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value)); 
    }
    
    public void Write(string value)
    {
        Write(value.Length);
        _buffer.AddRange(Encoding.UTF8.GetBytes(value)); 
    }
    
    public void Write(Vector2 value)
    {
        Write(value.x);
        Write(value.y);
    }
    
    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }
    
    public void Write(Quaternion value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
        Write(value.w);
    }
    
    public byte ReadByte()
    {
        if (_buffer.Count <= _readPos) throw new Exception("'byte'를 읽어올 수 없습니다!");
        
        byte value = _buffer[_readPos];
        _readPos += 1;
        return value;
    }
    
    public byte[] ReadBytes(int length)
    {
        if (_buffer.Count <= _readPos) throw new Exception("'byte'를 읽어올 수 없습니다!");
        
        byte[] value = _buffer.GetRange(_readPos, length).ToArray();
        _readPos += length;
        return value;
    }
    
    public int ReadInt()
    {
        if (_buffer.Count <= _readPos) throw new Exception("'int'를 읽어올 수 없습니다!");
        
        int value = BitConverter.ToInt32(_buffer.ToArray(), _readPos);
        _readPos += 4;
        return value;
    }
    
    public float ReadFloat()
    {
        if (_buffer.Count <= _readPos) throw new Exception("'float'를 읽어올 수 없습니다!");
        
        float value = BitConverter.ToSingle(_buffer.ToArray(), _readPos);
        _readPos += 4;
        return value;
    }
    
    public string ReadString()
    {
        if (_buffer.Count <= _readPos) throw new Exception("'string'를 읽어올 수 없습니다!");
        
        try
        {
            int length = ReadInt();
            string value = Encoding.UTF8.GetString(_buffer.ToArray(), _readPos, length);
            _readPos += 4 * length;
            return value;
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }
    
    public Vector2 ReadVector2()
    {
        return new Vector2(ReadFloat(), ReadFloat());
    }
    
    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    }
    
    public Quaternion ReadQuaternion()
    {
        return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
    }
}
