namespace MIS.Api.Domain.ValueObjects;

/// <summary>
/// Базовый класс для Value Objects в Domain-Driven Design.
/// Value Objects характеризуются равенством по значению, а не по идентичности.
/// </summary>
public abstract class ValueObject : IComparable<ValueObject>, IEquatable<ValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    
    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }
    
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }
    
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }
    
    
    public virtual int CompareTo(ValueObject? other)
    {
        if (other == null)
            return 1;

        if (ReferenceEquals(this, other))
            return 0;

        var thisComponents = GetEqualityComponents().ToArray();
        var otherComponents = other.GetEqualityComponents().ToArray();

        if (thisComponents.Length != otherComponents.Length)
            return thisComponents.Length.CompareTo(otherComponents.Length);

        for (var i = 0; i < thisComponents.Length; i++)
        {
            var thisComponent = thisComponents[i];
            var otherComponent = otherComponents[i];

            if (thisComponent == null && otherComponent == null)
                continue;

            if (thisComponent == null)
                return -1;

            if (otherComponent == null)
                return 1;

            if (thisComponent is IComparable comparable)
            {
                var comparison = comparable.CompareTo(otherComponent);
                if (comparison != 0)
                    return comparison;
            }
            else
            {
                var comparison = string.Compare(thisComponent.ToString(), otherComponent.ToString(), StringComparison.Ordinal);
                if (comparison != 0)
                    return comparison;
            }
        }

        return 0;
    }
    
    public override string ToString()
    {
        var components = GetEqualityComponents().ToArray();
        if (components.Length == 0)
            return GetType().Name;

        if (components.Length == 1)
            return $"{GetType().Name}({components[0]})";

        return $"{GetType().Name}({string.Join(", ", components)})";
    }
}