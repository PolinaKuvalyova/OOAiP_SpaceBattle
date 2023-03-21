namespace VectorSpaceBattle;

public class Vector
{
    private int[] array;
    public int size;

    public Vector(params int[] array)
    {
        if (array.Length == 0)
        {
            throw new ArgumentException("Objects of type Vector must contain at least one argument!");
        }
        size = array.Length;
        this.array = new int[size];
        for (int i = 0; i < size; i++)
        {
            this.array[i] = array[i];
        }
    }

    public override string ToString()
    {
        string str = "Vector (";

        for (int i = 0; i < size - 1; i++)
        {
            str += array[i] + ", ";
        }

        str += array[size - 1] + ")";

        return str;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(array);
    }


    public int this[int index]
    {
        get
        {
            return array[index];
        }
        set
        {
            this.array[index] = value;
        }
    }

    public static Vector operator +(Vector x, Vector y)
    {
        if (x.size != y.size)
        {
            throw new System.ArgumentException();
        }
        else
        {
            int[] new_array = new int[x.size];

            for (int i = 0; i < x.size; i++)
            {
                new_array[i] = x[i] + y[i];
            }
            return new Vector(new_array);
        }
    }

    public static Vector operator -(Vector x, Vector y)
    {
        if (x.size != y.size)
        {
            throw new System.ArgumentException();
        }
        else
        {
            int[] new_array = new int[x.size];

            for (int i = 0; i < x.size; i++)
            {
                new_array[i] = x[i] - y[i];
            }
            return new Vector(new_array);
        }
    }

    public static Vector operator *(int n, Vector x)
    {
        int[] new_array = new int[x.size];

        for (int i = 0; i < x.size; i++)
        {
            new_array[i] = n * x[i];
        }
        return new Vector(new_array);
    }

    public static bool operator ==(Vector x, Vector y)
    {
        if (x.size != y.size)
        {
            return false;
        }
        for (int i = 0; i < x.size; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }

    public static bool operator !=(Vector x, Vector y)
    {
        if (x == y)
        {
            return false;
        }
        return true;
    }

    public static bool operator <(Vector x, Vector y)
    {
        if (x == y)
        {
            return false;
        }

        if (x.size > y.size)
        {
            return false;
        }
        for (int i = 0; i < x.size; i++)
        {
            if (x[i] > y[i])
            {
                return false;
            }
        }
        return true;
    }

    public static bool operator >(Vector x, Vector y)
    {
        return y < x;
    }

}
