using System;

namespace Models.Entities;

public class Entity
{
    public Guid Id { get; set; }


    public override bool Equals(object obj)
    {
        Entity? e = obj as Entity;
        bool v = e == null || obj == null || GetType() != obj.GetType();
        if (v)
            return false;
        return Id == e.Id;
    }

    public override int GetHashCode()
    {
        byte[] bytes = Id.ToByteArray();
        int i = BitConverter.ToInt32(bytes, 0);
        return i;
    }
}
